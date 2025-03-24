using Daric.Application.Services.Account;
using Daric.Domain.Accounts.DomainServices;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Application.DomainServices.Account
{
    internal static class AccountDomainServicesExtensions
    {
        public static IServiceCollection AddAccountDomainServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<Domain.Accounts.Services.IAccountDepositService, AccountDepositService>();
            serviceCollection.AddScoped<IAccountTransferService, Domain.Accounts.DomainServices.AccountTransferService>();
            return serviceCollection;
        }
    }
}
