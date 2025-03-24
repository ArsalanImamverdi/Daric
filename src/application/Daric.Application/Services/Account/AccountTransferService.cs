using Daric.Application.Contracts;
using Daric.Application.Contracts.Account;
using Daric.Domain.Accounts;
using Daric.Domain.Accounts.DomainServices;
using Daric.Domain.Customers;
using Daric.Locking.Abstraction;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Services.Account
{
    public class AccountTransferService(IAccountRepository accountRepository,
                                          IAccountTransferService accountTransferService,
                                          ICustomerRepository customerRepository,
                                          IDistributedLockMechanism distributedLockMechanism,
                                          ILogger<AccountTransferService> logger)
    {
        public async Task<Contracts.ErrorOr<bool>> ExecuteAsync(string accountNumber, TransferRequestContract transferRequestContract, CancellationToken cancellationToken)
        {
            var trackingCode = string.Empty;
            try
            {
                if (distributedLockMechanism is null)
                {
                    logger.LogCritical("Can not get the LockMechanism instance, {service}", nameof(AccountTransferService));
                    return new InternalError("Can not perform the operation!");
                }

                await using (var @lock = await distributedLockMechanism.AcquireAsync($"lock:{accountNumber}", TimeSpan.FromSeconds(5), cancellationToken: cancellationToken))
                {
                    if (!@lock.IsAcquired())
                        return new LockError();

                    await using (var receiverLock = await distributedLockMechanism.AcquireAsync($"lock:{transferRequestContract.ReceiverAccountNumber}", TimeSpan.FromSeconds(5), cancellationToken: cancellationToken))
                    {
                        if (!receiverLock.IsAcquired())
                            return new LockError();

                        var accounts = (await accountRepository.FilterAsync(account => account.AccountNumber == accountNumber || account.AccountNumber == transferRequestContract.ReceiverAccountNumber))?.ToList();

                        if (accounts is not { Count: 2 })
                            return new Contracts.NullOrEmptyError(nameof(Account));

                        var account = accounts.First(account => account.AccountNumber == accountNumber);
                        var receiverAccount = accounts.First(account => account.AccountNumber == transferRequestContract.ReceiverAccountNumber);

                        var customers = await customerRepository.GetTransferCustomersAsync(account.CustomerId, receiverAccount.CustomerId, cancellationToken);

                        var customer = customers.First(customer => customer.Id == account.CustomerId);
                        var receiverCustomer = customers.First(customer => customer.Id == receiverAccount.CustomerId);

                        var transferResult = await accountTransferService.Transfer(account, customer, receiverAccount, receiverCustomer, transferRequestContract.Amount, cancellationToken);

                        if (!transferResult.Success)
                            return transferResult.Errors;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(AccountTransferService));
                return ex;
            }
        }
    }
}
