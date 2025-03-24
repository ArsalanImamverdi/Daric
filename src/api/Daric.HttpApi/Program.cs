
using Daric.Application;
using Daric.Application.Jobs;
using Daric.Caching.Redis;
using Daric.Configurations;
using Daric.HttpApi.Services.Account;
using Daric.HttpApi.Utilities;
using Daric.Infrastructure.SqlServer;
using Daric.Locking.Abstraction;
using Daric.Locking.MedallionRedis;
using Daric.Logging.Abstractions;
using Daric.Logging.Console;
using Daric.Scheduling.Quartz;
using Daric.Shared;

using Quartz;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddConfig();
        builder.Services.AddMicroserviceInfo(m => m.WithName(nameof(Daric)));
        builder.Services.AddAppInfo();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddLogging(logging => logging.AddConsoleLogging());
        builder.Services.AddDaricDbContext();
        builder.AddRedis();
        builder.Services.AddDistributedLock(opt => opt.AddMedallionRedisLockMechanism());
        builder.Services.AddScoped<ScheduledTransferJob>();

        builder.Services.AddQuartz(opt => opt.AddJobs((config, serviceProvider) =>
        {
            config.AddJob<ScheduledTransferJob>(builder =>
            {
                builder.WithIdentity(nameof(ScheduledTransferJob)).StoreDurably().DisallowConcurrentExecution().PersistJobDataAfterExecution(true);
            });

            config.AddTrigger(trigger =>
            {

                trigger.WithIdentity(nameof(ScheduledTransferJob) + "-trigger").ForJob(nameof(ScheduledTransferJob)).WithCronSchedule("0 0 */4 * * ?").WithPriority(1);
            });

        }), builder.Configuration);

        builder.Services.AddApplicationService();
        builder.Services.AddAccountServices();

        var app = builder.Build();
        // Configure the HTTP request pipeline.

        app.MapAccountServices();
        app.UseSwagger();
        app.UseSwaggerUI();

        await app.RunAppAsync();
    }

}