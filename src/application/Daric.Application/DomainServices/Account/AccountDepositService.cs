using Daric.Domain.Transactions;

namespace Daric.Application.DomainServices.Account
{
    internal class AccountDepositService : Domain.Accounts.Services.IAccountDepositService
    {
        public async Task<Domain.Shared.ErrorOr<bool>> DepositAsync(Domain.Accounts.Account account,
                                                                   decimal amount,
                                                                   string description,
                                                                   ITransactionRepository transactionRepository,
                                                                   CancellationToken cancellationToken)
        {
            try
            {
                var transactions = await transactionRepository.GetSumOfTodayTransactionsAsync(account.Id, cancellationToken);
                if (transactions + amount > 100_000_000)
                    return new Domain.Shared.InvalidOperationError("Maximum credit for today reached!");

                account.Deposit(amount, description, PerformByType.User);

                return true;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
