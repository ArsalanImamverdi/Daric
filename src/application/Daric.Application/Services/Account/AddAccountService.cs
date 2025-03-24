using Daric.Domain.Accounts;
using Daric.Domain.Customers;
using Daric.Domain.Shared;
using Daric.HttpApi.Contracts.Account;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Services.Account
{
    public class AddAccountService(IAccountRepository accountRepository,
                                   ICustomerRepository customerRepository,
                                   IUnitOfWork unitOfWork,
                                   IAccountNumberGenerator accountNumberGenerator,
                                   ILogger<AddAccountService> logger)
    {
        public async Task<Contracts.ErrorOr<bool>> ExecuteAsync(AddAccountRequestContract contract, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await customerRepository.FirstOrDefaultAsync(customer => customer.Id == contract.CustomerId);
                if (customer == null)
                    return new Contracts.ResourceNotFoundError(nameof(Customer));

                var account = await Domain.Accounts.Account.Create(contract.CustomerId, accountNumberGenerator);
                if (!account)
                    return account.Errors;

                await unitOfWork.BeginTransactionAsync(cancellationToken);

                await accountRepository.Insert(account!);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                logger.LogError(ex, nameof(AddAccountService));
                return ex;
            }
        }
    }
}
