using System;
using System.Reflection;

using Daric.Database.Abstraction;
using Daric.Database.SqlServer.Internals;
using Daric.Domain.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Daric.Database.SqlServer;

public class SqlServerRepositoryOptionsBuilder(IServiceCollection serviceCollection, IServiceProvider serviceProvider, Type entityType)
{
    internal IServiceCollection ServiceCollection { get; } = serviceCollection;
    internal IServiceProvider ServiceProvider { get; } = serviceProvider;

    internal Type EntityType { get; } = entityType;
    internal bool UseCacheValue { get; set; }
    public SqlServerRepositoryOptionsBuilder AddEventHandler<TEventHandler>()
        where TEventHandler : class, ISqlServerEventHandler
    {
        var eventManager = ServiceProvider.GetService<ISqlServerEventManager>();
        if (eventManager is null) return this;
        var eventHandler = ServiceProvider.GetService<TEventHandler>();
        if (eventHandler is null) return this;
        if (!string.IsNullOrEmpty(EntityType?.FullName))
            eventManager.AddHandler(EntityType.FullName, eventHandler);
        return this;
    }
    public SqlServerRepositoryOptionsBuilder AddEventHandler<TEventHandler>(TEventHandler eh)
        where TEventHandler : class, ISqlServerEventHandler
    {
        var eventManager = ServiceProvider.GetService<ISqlServerEventManager>();
        if (eventManager is null) return this;
        if (!string.IsNullOrEmpty(EntityType?.FullName))
            eventManager.AddHandler(EntityType.FullName, eh);
        return this;
    }
}
public abstract class SqlServerRepositoriesOptionsBuilder(IServiceCollection serviceCollection)
{
    public IServiceCollection ServiceCollection { get; } = serviceCollection;
    protected static Type GetEntityType<T>()
    {
        var implementationType = typeof(T);
        var interfaceType = implementationType.GetInterfaces().FirstOrDefault(w => w.GetGenericTypeDefinition() == typeof(ISqlServerRepository<>))
                ?? throw new InvalidOperationException($"{implementationType.Name} Not Implemented ISqlServerRepository<>");
        var entityType = interfaceType.GetGenericArguments().FirstOrDefault()!;
        return entityType;
    }

    protected static T CreateInstance<T>(IServiceProvider serviceProvider, params object[] parameters)
    {
        var type = typeof(T);
        var constructor = type.GetConstructors().FirstOrDefault() ?? throw new InvalidOperationException($"Can not find any constructor on type {typeof(T).Name}");
        var constructorParameters = constructor.GetParameters();

        if (parameters.Length > constructorParameters.Length)
        {
            throw new InvalidOperationException($"{typeof(T).Name} accepts {constructorParameters.Length} arguments");
        }

        var @params = parameters.ToList();

        for (var i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            if (param?.GetType() != constructorParameters[i].ParameterType && param?.GetType().IsAssignableFrom(constructorParameters[i].ParameterType) == true)
                throw new InvalidOperationException($"The parameter is not match the constructor parameter, {constructorParameters[i].Name}");
        }

        if (parameters.Length < constructorParameters.Length)
        {
            for (var i = parameters.Length; i < constructorParameters.Length; i++)
            {
                var pr = serviceProvider.GetService(constructorParameters[i].ParameterType);
                @params.Add(pr!);
            }
        }
        return (T)Activator.CreateInstance(typeof(T), [.. @params])!;
    }
    internal abstract Type ContextType { get; }

}
public class SqlServerRepositoriesOptionsBuilder<TContext>(IServiceCollection serviceCollection) : SqlServerRepositoriesOptionsBuilder(serviceCollection)
    where TContext : class, ISqlDbContext<TContext>
{

    internal override Type ContextType => typeof(TContext);
    public SqlServerRepositoriesOptionsBuilder<TContext> FailOnPendingMigrations()
    {
        var serviceProvider = ServiceCollection.BuildServiceProvider();
        var context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<TContext>();
        if (context.Database.GetPendingMigrations() is { } migrations && migrations.Any())
            throw new InvalidOperationException($"The context {typeof(TContext).Name} has pending migrations {string.Join(',', migrations)}");
        return this;
    }
    public SqlServerRepositoriesOptionsBuilder<TContext> AddInterceptor<TInterceptor>()
        where TInterceptor : DbCommandInterceptor
    {
        ServiceCollection.AddKeyedScoped<DbCommandInterceptor, TInterceptor>(typeof(TContext).FullName);
        return this;
    }

    public SqlServerRepositoriesOptionsBuilder<TContext> AddRepository<TRepository, TImplementation>()
        where TRepository : class, IRepository
        where TImplementation : class, TRepository
    {
        return InternalAddRepository<TRepository, TImplementation>();
    }

    public SqlServerRepositoriesOptionsBuilder<TContext> AddRepository<TRepository, TImplementation>(Func<SqlServerRepositoryOptionsBuilder, SqlServerRepositoryOptionsBuilder> options)
                where TRepository : class, IRepository
                where TImplementation : class, TRepository
    {
        return InternalAddRepository<TRepository, TImplementation>(options);
    }

    public SqlServerRepositoriesOptionsBuilder<TContext> WithUnitOfWork<TUnitOfWork>()
        where TUnitOfWork : SqlServerUnitOfWork<TContext>
    {
        ServiceCollection.AddScoped<IUnitOfWork>(serviceProvider =>
        {
            var context = serviceProvider.GetService<TContext>()
                    ?? throw new InvalidOperationException($"Can not find the {typeof(TContext).Name} instance");
            var instance = CreateInstance<TUnitOfWork>(serviceProvider, context);
            return instance;
        });
        return this;
    }

    public SqlServerRepositoriesOptionsBuilder<TContext> WithUnitOfWork<TUnitOfWork, TKey>(TKey key)
        where TUnitOfWork : SqlServerUnitOfWork<TContext>
        where TKey : Enum
    {

        ServiceCollection.AddKeyedScoped<IUnitOfWork>(key, (serviceProvider, theKey) =>
        {
            var context = serviceProvider.GetService<TContext>()
                    ?? throw new InvalidOperationException($"Can not find the {typeof(TContext).Name} instance");
            var instance = CreateInstance<TUnitOfWork>(serviceProvider, context);
            return instance;
        });
        return this;
    }
    private static readonly MethodInfo _setMethod = typeof(TContext).GetMethods().FirstOrDefault(f => f.Name == nameof(DbContext.Set) && f.GetParameters().Length == 0)!;
    internal SqlServerRepositoriesOptionsBuilder<TContext> InternalAddRepository<TRepository, TImplementation>(Func<SqlServerRepositoryOptionsBuilder, SqlServerRepositoryOptionsBuilder>? options = null)
       where TRepository : class, IRepository
       where TImplementation : class, TRepository
    {
        ServiceCollection.AddScoped<TRepository>(serviceProvider =>
        {
            var context = serviceProvider.GetService<TContext>()
                    ?? throw new InvalidOperationException($"Can not find the {typeof(TContext).Name} instance");

            var entityType = GetEntityType<TImplementation>();
            var dbSet = _setMethod.MakeGenericMethod(entityType).Invoke(context, null)!;
            options?.Invoke(new SqlServerRepositoryOptionsBuilder(ServiceCollection, serviceProvider, entityType));
            var instance = CreateInstance<TImplementation>(serviceProvider, dbSet);

            return instance;
        });
        return this;
    }
    internal SqlServerRepositoriesOptionsBuilder<TContext> InternalAddRepository<TRepository, TImplementation, TEventHandler>(Func<SqlServerRepositoryOptionsBuilder, SqlServerRepositoryOptionsBuilder>? options = null)
       where TRepository : class, IRepository
       where TImplementation : class, TRepository
        where TEventHandler : class, ISqlServerEventHandler
    {
        ServiceCollection.AddScoped<TRepository>(serviceProvider =>
        {
            var context = serviceProvider.GetService<TContext>()
                    ?? throw new InvalidOperationException($"Can not find the {typeof(TContext).Name} instance");

            var entityType = GetEntityType<TImplementation>();
            var dbSet = _setMethod.MakeGenericMethod(entityType).Invoke(context, null)!;
            var instance = CreateInstance<TImplementation>(serviceProvider, dbSet);
            var optionsParam = new SqlServerRepositoryOptionsBuilder(ServiceCollection, serviceProvider, entityType);
            optionsParam.AddEventHandler<TEventHandler>();
            options?.Invoke(optionsParam);
            return instance;
        });

        return this;
    }
}
