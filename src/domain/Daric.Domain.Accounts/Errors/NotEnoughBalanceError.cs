using Daric.Domain.Shared;

namespace Daric.Domain.Accounts.DomainErrors
{
    internal record NotEnoughBalanceError() : Error(ErrorCode.InvalidData, "Not enough balance")
    {
    }
}
