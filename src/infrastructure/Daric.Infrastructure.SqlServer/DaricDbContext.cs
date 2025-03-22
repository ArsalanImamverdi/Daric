using Daric.Database;
using Daric.Database.SqlServer;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Daric.Infrastructure.SqlServer
{
    internal class DaricDbContext(ISqlServerDataConfig config, IServiceProvider scopedServiceProvider, ILogger<DaricDbContext> logger)
            : SqlServerDbContext<DaricDbContext>(config, scopedServiceProvider, logger)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
