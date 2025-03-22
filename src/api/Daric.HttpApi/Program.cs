using Daric.Configurations;
using Daric.HttpApi.Utilities;
using Daric.Locking.Abstraction;
using Daric.Locking.MedallionRedis;
using Daric.Logging.Abstractions;
using Daric.Logging.Console;
using Daric.Shared;

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
        builder.Services.AddDistributedLock(opt => opt.AddMedallionRedisLockMechanism());


        var app = builder.Build();
        // Configure the HTTP request pipeline.

        app.UseSwagger();
        app.UseSwaggerUI();

        await app.RunAppAsync();
    }

}