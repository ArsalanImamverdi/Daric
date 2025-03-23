using Daric.Domain.Shared;
using Daric.Domain.Transactions;
using Daric.Domain.Transactions.DomainServices;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Dirac.Infrastructure.Tests.Customer
{
    [Collection("TransactionTests")]
    public class TransactionRepositoryTests(TransactionTestFixture transactionTestFixture)
    {
        private readonly Mock<ITrackingCodeGenerator> _trackingCodeGeneratorMock = new();

        [Fact]
        public async Task AddTransaction_ValidData_ShouldSucceed()
        {
            var transactionRepository = transactionTestFixture.ServiceProvider.GetRequiredService<ITransactionRepository>();
            var unitOfWork = transactionTestFixture.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var accountId = Guid.NewGuid();
            var trackingCode = 123456789L;
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(trackingCode);

            // Act
            var transaction = await Transaction.Create(
                accountId: accountId,
                transactionType: TransactionType.Debit,
                amount: 1_000_000m,
                description: "Test transaction",
                performBy: PerformByType.User,
                parentId: null,
                isScheduled: false,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            );
            Assert.True(transaction);
            Assert.NotNull(transaction.Result);

            await transactionRepository.Insert(transaction!);
            var saveResult = await unitOfWork.SaveChangesAsync();
            Assert.True(saveResult >= 1);

            Assert.True(transaction.Result!.Id != Guid.Empty);
        }
    }
}