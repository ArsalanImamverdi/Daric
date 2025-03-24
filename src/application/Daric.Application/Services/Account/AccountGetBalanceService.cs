using Daric.Application.Contracts;
using Daric.Caching.Abstractions;
using Daric.Domain.Accounts;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Services.Account
{
    public class AccountGetBalanceService(IDistributedCacheDatabase redisDatabase, IAccountRepository accountRepository, ILogger<AccountGetBalanceService> logger)
    {
        public async Task<ErrorOr<decimal>> ExecuteAsync(string accountNumber, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountNumber))
                    return new InvalidDataError(nameof(Domain.Accounts.Account.AccountNumber));

                var redisKey = $"account:balance:{accountNumber}";
                var balance = await redisDatabase.SecureGetAsync(redisKey, cancellationToken);
                if (balance >= 0)
                    return balance;

                balance = await accountRepository.GetBalance(accountNumber);

                _ = redisDatabase.SecureSetAsync(redisKey, balance, cancellationToken);
                return balance;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(AccountGetBalanceService));
                return ex;
            }
        }
    }
}
