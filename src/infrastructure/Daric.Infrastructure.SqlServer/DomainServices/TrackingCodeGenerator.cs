using Daric.Database.SqlServer;
using Daric.Domain.Shared;
using Daric.Domain.Transactions.DomainServices;

namespace Daric.Infrastructure.SqlServer.DomainServices
{
    internal class TrackingCodeGenerator(ISqlServerSequenceProvider<DaricDbContext> sequenceProvider) : ITrackingCodeGenerator
    {
        public async Task<ErrorOr<long>> GetNextTrackingCodeAsync()
        {
            try
            {
                var accountNumber = await sequenceProvider.GetNextValue<long>("Account.TrackingCode");
                if (accountNumber < 0)
                    return new Error(ErrorCode.InvalidData, "Can not get a tracking code");

                return accountNumber;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
