using System.Data;
using System.Reflection;

using Daric.Database.Abstraction;
using Daric.Database.SqlServer;
using Daric.Database.SqlServer.Internals;
using Daric.Domain.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Daric.Database;

public class SqlServerDbContext<TContext>(ISqlServerDataConfig config,
                                            IServiceProvider scopedServiceProvider,
                                            ILogger<TContext> logger) : DbContext, ISqlDbContext<TContext> where TContext : ISqlDbContext<TContext>
{
    public ISqlServerDataConfig Config { get; } = config;
    public ILogger<TContext> Logger { get; } = logger;
    protected IServiceProvider ServiceProvider { get; } = scopedServiceProvider;

    protected bool IsDatabaseInDesignMode { get; private set; }

    public void DatabaseIsInDesignMode(bool databaseIsInDesignMode = true)
    {
        IsDatabaseInDesignMode = databaseIsInDesignMode;
    }

    public virtual string GetConnectionString()
    {
        return Config.SqlServerConfig?.ConnectionString ?? throw new InvalidOperationException("Can not find connection string");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var item in GetDbSets())
        {
            if (item.Value.IsAssignableTo(typeof(IDomainEventEntity)))
            {
                modelBuilder.Entity(item.Value).Ignore(nameof(IDomainEventEntity.DomainEvents));
            }
            if (item.Value.IsAssignableTo(typeof(IPersistenceEventEntity)))
            {
                modelBuilder.Entity(item.Value).Ignore(nameof(IPersistenceEventEntity.PersistenceDomainEvents));
            }
            if (item.Value.IsAssignableTo(typeof(IPersistenceTransactionalEventEntity)))
            {
                modelBuilder.Entity(item.Value).Ignore(nameof(IPersistenceTransactionalEventEntity.PersistenceTransactionalEvents));
            }
            if (item.Value.IsAssignableTo(typeof(IEntity)))
            {
                modelBuilder.Entity(item.Value).HasIndex(nameof(IEntity.Id)).IsUnique();
                modelBuilder.Entity(item.Value).Property(nameof(IEntity.Id)).HasDefaultValueSql("NEWSEQUENTIALID()");
            }
            if (item.Value.IsAssignableTo(typeof(IDateTime)))
            {
                modelBuilder.Entity(item.Value).HasIndex(nameof(IDateTime.CreatedAt));
                modelBuilder.Entity(item.Value).HasIndex(nameof(IDateTime.ModifiedAt));
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = GetConnectionString();
        if (connectionString.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            optionsBuilder.UseInMemoryDatabase(GetType()?.FullName ?? Guid.NewGuid().ToString());
        else
            optionsBuilder.UseSqlServer(connectionString).AddInterceptors(new TracingInterceptor(ServiceProvider));

        var contextInterceptors = ServiceProvider.GetKeyedServices<DbCommandInterceptor>(typeof(TContext).FullName!);
        if (contextInterceptors?.Any() == true)
            optionsBuilder.AddInterceptors(contextInterceptors.Where(interceptor => interceptor is not null)!);

        var connectionInterceptors = ServiceProvider.GetServices<DbConnectionInterceptor>();
        if (connectionInterceptors?.Any() == true)
            optionsBuilder.AddInterceptors(connectionInterceptors.Where(interceptor => interceptor is not null)!);

        base.OnConfiguring(optionsBuilder);
    }
    /// <summary>
    /// get list of all DbSets they are tables of context
    /// </summary>
    /// <param name="contextType"></param>
    /// <returns></returns>
    private IEnumerable<KeyValuePair<string, Type>> GetDbSets()
    {
        var dbSets = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.PropertyType.GetGenericArguments().Length > 0 && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToList();
        foreach (var dbSetProperty in dbSets)
        {
            var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
            yield return new KeyValuePair<string, Type>(dbSetProperty.Name, entityType);
        }
    }

    public (string ContextName, List<string> TableNames) GetTableNames()
    {
        return (GetType().Name, GetDbSets().Select(dbSet => dbSet.Value.Name).ToList());
    }

}

