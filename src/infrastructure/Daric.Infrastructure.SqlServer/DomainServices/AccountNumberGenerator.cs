using Daric.Database.SqlServer;
using Daric.Domain.Accounts;
using Daric.Domain.Shared;

namespace Daric.Infrastructure.SqlServer.DomainServices
{
    internal class AccountNumberGenerator(ISqlServerSequenceProvider<DaricDbContext> sequenceProvider) : IAccountNumberGenerator
    {
        public async Task<ErrorOr<string>> GetNextAccountNumberAsync()
        {
            try
            {
                var accountNumber = await sequenceProvider.GetNextValue<long>("Account.AccountNumber");
                if (accountNumber < 0)
                    return new Error(ErrorCode.InvalidData, "Can not get a account number");

                return accountNumber.ToString();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
