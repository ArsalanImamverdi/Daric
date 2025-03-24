
using Daric.Database.SqlServer;
using Daric.Domain.Accounts;

using Microsoft.EntityFrameworkCore;

namespace Daric.Infrastructure.SqlServer.Repositories
{
    internal class AccountRepository(DbSet<Account> entity, IServiceProvider serviceProvider) : SqlServerRepository<Account>(entity, serviceProvider), IAccountRepository
    {
        public Task<decimal> GetBalance(string accountNumber)
        {
            return Entity.AsNoTracking().Where(account => account.AccountNumber == accountNumber).Select(account => account.Balance).FirstOrDefaultAsync();
        }
    }
}
