using Daric.Database.SqlServer;
using Daric.Domain.Bonuses;

using Microsoft.EntityFrameworkCore;

namespace Daric.Infrastructure.SqlServer.Repositories
{
    internal class BonusRepository(DbSet<Bonus> entity, IServiceProvider serviceProvider) : SqlServerRepository<Bonus>(entity, serviceProvider), IBonusRepository
    {
    }
}
