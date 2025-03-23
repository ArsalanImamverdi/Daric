using Daric.Database.SqlServer;
using Daric.Domain.Transactions;

using Microsoft.EntityFrameworkCore;

namespace Daric.Infrastructure.SqlServer.Repositories
{
    internal class TransactionRepository(DbSet<Transaction> entity, IServiceProvider serviceProvider) : SqlServerRepository<Transaction>(entity, serviceProvider), ITransactionRepository
    {
        private IQueryable<Transaction> GetTodayTransactions(Guid accountId)
        {
            return Entity.AsNoTracking().Where(transaction => transaction.AccountId == accountId &&
                                                              transaction.TransactionType == TransactionType.Credit &&
                                                              transaction.CreatedAt >= DateTime.Today.Date &&
                                                              transaction.PerformBy == PerformByType.User);
        }
        public Task<decimal> GetSumOfTodayTransactionsAsync(Guid accountId, CancellationToken cancellationToken)
        {
            return GetTodayTransactions(accountId).SumAsync(transaction => transaction.Amount, cancellationToken);
        }

        public async Task<Transaction[]> GetTodayTransactionsAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var transactions = await GetTodayTransactions(accountId).ToArrayAsync(cancellationToken: cancellationToken);
            return transactions;
        }

        public Task<Transaction[]> GetPendingScheduledTransactionsAsync(CancellationToken cancellationToken)
        {
            return Entity.Where(transaction => transaction.IsScheduled && transaction.Status == TransactionStatus.Pending).ToArrayAsync(cancellationToken);
        }

        public async Task<(int, Transaction[])> FilterTransactionsAsync(Guid accountId,
                                                                        DateTime? MinDateTime,
                                                                        DateTime? MaxDateTime,
                                                                        decimal? MinAmount,
                                                                        decimal? MaxAmount,
                                                                        TransactionType? transactionType,
                                                                        int? index,
                                                                        int? length,
                                                                        CancellationToken cancellationToken)
        {
            var query = Entity.Where(transaction => transaction.AccountId == accountId).AsNoTracking();
            if (MinDateTime is not null)
                query = query.Where(transaction => transaction.CreatedAt >= MinDateTime);
            if (MaxDateTime is not null)
                query = query.Where(transaction => transaction.CreatedAt <= MaxDateTime);

            if (MinAmount is not null)
                query = query.Where(transaction => transaction.Amount >= MinAmount);
            if (MaxAmount is not null)
                query = query.Where(transaction => transaction.Amount <= MaxAmount);

            if (transactionType is not null)
                query = query.Where(transaction => transaction.TransactionType == transactionType);

            return (await query.CountAsync(cancellationToken), await query.OrderByDescending(transaction => transaction.CreatedAt).Skip(index ?? 0 * length ?? 10).Take(length ?? 10).ToArrayAsync(cancellationToken));
        }
    }
}
