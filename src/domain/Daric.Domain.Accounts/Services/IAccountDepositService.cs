using Daric.Domain.Shared;
using Daric.Domain.Transactions;

namespace Daric.Domain.Accounts.Services
{
    public interface IAccountDepositService
    {
        Task<ErrorOr<bool>> DepositAsync(Account account, decimal amount, string description, ITransactionRepository transactionRepository, CancellationToken cancellationToken);
    }
}
