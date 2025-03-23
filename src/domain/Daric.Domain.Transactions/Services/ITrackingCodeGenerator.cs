using Daric.Domain.Shared;

namespace Daric.Domain.Transactions.DomainServices
{
    public interface ITrackingCodeGenerator
    {
        Task<ErrorOr<long>> GetNextTrackingCodeAsync();
    }
}
