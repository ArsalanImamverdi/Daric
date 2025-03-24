using Daric.Application.Contracts;
using Daric.Application.Contracts.Account;
using Daric.Domain.Accounts;
using Daric.Domain.Transactions;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Services.Account
{

    public class AccountReportService(IAccountRepository accountRepository,
                                      ITransactionRepository transactionRepository,
                                      ILogger<AccountReportService> logger)
    {
        public async Task<ErrorOr<PaginationResult<AccountReportResponseContract>>> ExecuteAsync(string accountNumber, AccountReportRequestContract request, int? index, int? length, CancellationToken cancellationToken)
        {
            try
            {
                request ??= new AccountReportRequestContract();
                var account = await accountRepository.FirstOrDefaultAsync(account => account.AccountNumber == accountNumber);
                if (account is null)
                    return new ResourceNotFoundError(nameof(Account));

                var transactions = await transactionRepository.FilterTransactionsAsync(account.Id, request.DateTime?.Min,
                                                                                        request.DateTime?.Max,
                                                                                        request.Amount?.Min,
                                                                                        request.Amount?.Max,
                                                                                        request?.TransactionType == Contracts.Account.TransactionType.Credit ? Domain.Transactions.TransactionType.Credit :
                                                                                                 request?.TransactionType == Contracts.Account.TransactionType.Debit ? Domain.Transactions.TransactionType.Debit : null,
                                                                                        index,
                                                                                        length,
                                                                                        cancellationToken);

                if (transactions.Result is null)
                    return new ResourceNotFoundError(nameof(Transaction));

                var result = transactions.Result.Select(transaction => new AccountReportResponseContract(transaction.TrackingCode, transaction.TransactionType.ToString(), transaction.Amount, transaction.CreatedAt, transaction.Status.ToString(), transaction.Description)).ToArray();
                return new PaginationResult<AccountReportResponseContract>(transactions.Count, result);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(AccountReportService));
                return ex;
            }
        }
    }
}
