using Daric.Scheduling.Abstraction;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using IJob = Quartz.IJob;

namespace Daric.Scheduling.Quartz
{
    public class QuartzOptionsBuilder(IServiceCollection serviceCollection)
    {
        public QuartzOptionsBuilder AddJobs(Action<QuartzOptions, IServiceProvider> options)
        {
            serviceCollection.AddOptions<QuartzOptions>().PostConfigure<IServiceProvider>((opt, serviceProvider) =>
            {
                options?.Invoke(opt, serviceProvider);
            });
            return this;
        }

        public QuartzOptionsBuilder AddJobs(Action<QuartzOptions> options)
        {
            serviceCollection.AddOptions<QuartzOptions>().PostConfigure(opt =>
            {
                options?.Invoke(opt);
            });
            return this;
        }

    }

    public static class QuartzExtensions
    {
        public static QuartzOptions AddJob<TJob>(this QuartzOptions options, Action<JobBuilder> jobBuilder) where TJob : Abstraction.IJob
        {
            return options.AddJob<QuartzJob<TJob>>(jobBuilder);
        }
        public static IServiceCollection AddQuartz(this IServiceCollection serviceCollection, Func<QuartzOptionsBuilder, QuartzOptionsBuilder> options, IConfiguration configuration)
        {
            options?.Invoke(new QuartzOptionsBuilder(serviceCollection));
            serviceCollection.AddQuartz(config =>
            {
                config.UsePersistentStore(opt =>
                {
                    opt.UseSqlServer(configuration.GetSection("Config:Scheduling:ConnectionString").Value!);
                    opt.UseProperties = true;
                    opt.UseNewtonsoftJsonSerializer();
                    opt.PerformSchemaValidation = true;
                });
            });

            serviceCollection.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            return serviceCollection;
        }
    }

    internal class QuartzJob<TJob>(TJob job) : IJob
        where TJob : Abstraction.IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return job.ExecuteAsync(context.JobDetail.Key.ToString(), GetJobArgument(context.JobDetail.JobDataMap));
        }

        private IJobArguments GetJobArgument(JobDataMap jobDataMap)
        {
            var method = typeof(TJob).GetMethod(nameof(Abstraction.IJob.ExecuteAsync));
            var type = method?.GetParameters().FirstOrDefault(param => param.ParameterType.IsAssignableTo(typeof(IJobArguments)))?.ParameterType;
            if (type is null || type.IsInterface || type.IsAbstract)
                return new DefaultJobArguments();

            if (type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0))
            {
                var properties = type.GetProperties().Where(prop => prop.CanWrite);
                var instance = Activator.CreateInstance(type) as IJobArguments;
                foreach (var param in jobDataMap)
                {
                    properties.FirstOrDefault(prop => prop.Name == param.Key)?.SetValue(instance, param.Value);
                }
                return instance!;
            }

            var ctor = type.GetConstructors().FirstOrDefault();
            if (ctor is null)
                return new DefaultJobArguments();

            var @params = ctor.GetParameters();
            var values = new object[@params.Length];
            for (var i = 0; i < @params.Length; i++)
            {
                values[i] = jobDataMap[@params[i].Name!];
            }
            return (Activator.CreateInstance(type, values) as IJobArguments)!;
        }
    }
    internal class DefaultJobArguments : IJobArguments
    {

    }
}
