using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Customers;
using Daric.Domain.Shared;
using Daric.Domain.Transactions;

namespace Daric.Domain.Accounts.DomainServices
{
    public interface IAccountTransferService
    {
        Task<ErrorOr<bool>> Transfer(Account senderAccount, Customer sender, Account receiverAccount, Customer receiver, decimal amount, CancellationToken cancellationToken);
    }

    public class AccountTransferService(IAccountRepository accountRepository, IUnitOfWork unitOfWork) : IAccountTransferService
    {
        public async Task<ErrorOr<bool>> Transfer(Account senderAccount, Customer sender, Account receiverAccount, Customer receiver, decimal amount, CancellationToken cancellationToken)
        {
            try
            {
                if (amount > 100_000_000)
                    return new InvalidOperationError("The amount is not valid.");
                if (senderAccount.Id == receiver.Id)
                    return new InvalidOperationError("Sender and receiver cannot be the same.");

                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var withdraw = senderAccount.Withdraw(amount, $"Transfer to {receiver.FirstName} {receiver.LastName}", Transactions.PerformByType.System);
                if (!withdraw.Success)
                    return withdraw;

                var debitCompletedEvent = senderAccount.DomainEventOfType<AccountDebitCompleted>();
                var deposit = receiverAccount.Deposit(amount, $"Transfer From {sender.FirstName} {sender.LastName}", Transactions.PerformByType.System, scheduledTransaction: amount >= 10_000_000, debitCompletedEvent: debitCompletedEvent);
                if (!deposit.Success)
                {
                    return deposit;
                }

                await accountRepository.Update(senderAccount);
                await accountRepository.Update(receiverAccount);

                await unitOfWork.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return ex;
            }
        }
    }
}
