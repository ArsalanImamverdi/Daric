using Daric.Domain.Shared;

namespace Daric.Domain.Transactions
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        public Task<Transaction[]> GetTodayTransactionsAsync(Guid accountId, CancellationToken cancellationToken);
        public Task<decimal> GetSumOfTodayTransactionsAsync(Guid accountId, CancellationToken cancellationToken);
        public Task<Transaction[]> GetPendingScheduledTransactionsAsync(CancellationToken cancellationToken);

        Task<(int Count, Transaction[] Result)> FilterTransactionsAsync(Guid accountId,
                                                                        DateTime? MinDateTime,
                                                                        DateTime? MaxDateTime,
                                                                        decimal? MinAmount,
                                                                        decimal? MaxAmount,
                                                                        TransactionType? transactionType,
                                                                        int? index,
                                                                        int? length,
                                                                        CancellationToken cancellationToken);
    }
}
