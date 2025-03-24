using Daric.Domain.Shared;

namespace Daric.Domain.Accounts
{
    public interface IAccountNumberGenerator
    {
        Task<ErrorOr<string>> GetNextAccountNumberAsync();
    }
}
