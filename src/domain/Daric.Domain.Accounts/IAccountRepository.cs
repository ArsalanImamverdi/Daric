using Daric.Domain.Shared;

namespace Daric.Domain.Accounts
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<decimal> GetBalance(string accountNumber);
    }
}
