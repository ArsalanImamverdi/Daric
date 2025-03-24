namespace Daric.Application.Contracts.Account
{
    public record AccountReportResponseContract(long TrackingCode, string TransactionType, decimal Amount, DateTime CreatedAt, string Status, string Description);
}
