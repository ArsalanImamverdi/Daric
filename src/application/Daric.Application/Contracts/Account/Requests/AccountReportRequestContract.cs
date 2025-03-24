namespace Daric.Application.Contracts.Account
{
    public record AccountReportRequestContract(RangeContract<DateTime>? DateTime, RangeContract<decimal>? Amount, TransactionType? TransactionType)
    {
        public AccountReportRequestContract() : this(null, null, null)
        {

        }
    }
}
