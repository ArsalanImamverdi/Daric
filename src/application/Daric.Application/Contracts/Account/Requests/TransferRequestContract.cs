namespace Daric.Application.Contracts.Account
{
    public class TransferRequestContract
    {
        public required string ReceiverAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
