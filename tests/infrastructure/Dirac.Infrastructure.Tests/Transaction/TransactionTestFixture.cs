using Daric.Configurations;
using Daric.Infrastructure.SqlServer;
using Daric.Logging.Abstractions;
using Daric.Logging.Console;
using Daric.Shared;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dirac.Infrastructure.Tests.Customer
{

    [CollectionDefinition("TransactionTests")]
    public class TransactionTestCollection : ICollectionFixture<TransactionTestFixture> { }
    public class TransactionTestFixture : IAsyncLifetime
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConfig(srv => srv.AddJsonStream(GetConfigurationStream()));
            serviceCollection.AddAppInfo();
            serviceCollection.AddMicroserviceInfo(mi => mi.WithName("TransactionTest"));
            serviceCollection.AddLogging(x => x.AddConsoleLogging());
            serviceCollection.AddDaricDbContext();

            ServiceProvider = serviceCollection.BuildServiceProvider();
            return Task.CompletedTask;
        }

        private Stream GetConfigurationStream()
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes("""
                {
                    "Config":{
                        "DatabaseConfig":{
                            "SqlServerConfig": {
                                "ConnectionString" :"InMemory"
                            }
                        }
                    }
                }
                """));
        }
    }
}
