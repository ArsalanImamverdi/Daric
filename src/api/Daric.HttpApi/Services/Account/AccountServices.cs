using Daric.Application.Contracts;
using Daric.Application.Contracts.Account;
using Daric.Application.Services.Account;
using Daric.HttpApi.Contracts.Account;

using Microsoft.AspNetCore.Mvc;

namespace Daric.HttpApi.Services.Account
{
    public static class AccountServices
    {
        public static IServiceCollection AddAccountServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<AddAccountService>();
            serviceCollection.AddScoped<AccountWithdrawService>();
            serviceCollection.AddScoped<AccountDepositService>();
            serviceCollection.AddScoped<AccountGetBalanceService>();
            serviceCollection.AddScoped<AccountTransferService>();
            serviceCollection.AddScoped<AccountReportService>();
            return serviceCollection;
        }

        public static IHost MapAccountServices(this IHost host)
        {
            if (host is WebApplication webApplication)
            {
                var groupPrefix = nameof(Account).ToLower();
                var group = webApplication.MapGroup(groupPrefix).WithTags(groupPrefix);
                group.MapPost("/add", (AddAccountService accountService,
                                       [FromBody] AddAccountRequestContract contract,
                                       IHttpContextAccessor httpContextAccessor,
                                       CancellationToken cancellationToken) =>
                {
                    return accountService.ExecuteAsync(contract, cancellationToken)
                                          .ContinueWith((result, state) =>
                                          {
                                              if (!result.Result.Success)
                                              {
                                                  return Results.NotFound(string.Join(',', result.Result.Errors.Select(err => err.GetErrorMessage())));
                                              }

                                              return Results.Json(result.Result);
                                          }, httpContextAccessor, cancellationToken);
                }).Produces(201, typeof(ErrorOr<bool>)).Produces(400, typeof(string));

                group.MapPost("{accountNumber}/deposit/{amount:decimal}", (AccountDepositService accountDeposit,
                                       [FromRoute] string accountNumber,
                                       [FromRoute] decimal amount,
                                       IHttpContextAccessor httpContextAccessor,
                                       CancellationToken cancellationToken) =>
                {
                    return accountDeposit.ExecuteAsync(accountNumber, amount, cancellationToken)
                                          .ContinueWith((result, state) =>
                                          {
                                              if (!result.Result.Success)
                                              {
                                                  return Results.NotFound(string.Join(',', result.Result.Errors.Select(err => err.GetErrorMessage())));
                                              }

                                              return Results.Json(result.Result);
                                          }, httpContextAccessor, cancellationToken);
                }).Produces(201, typeof(ErrorOr<bool>)).Produces(400, typeof(string));

                group.MapPost("/{accountNumber}/withdraw/{amount:decimal}", (AccountWithdrawService accountWithdrawService,
                                       [FromRoute] string accountNumber,
                                       [FromRoute] decimal amount,
                                       IHttpContextAccessor httpContextAccessor,
                                       CancellationToken cancellationToken) =>
                {
                    return accountWithdrawService.ExecuteAsync(accountNumber, amount, cancellationToken)
                                          .ContinueWith((result, state) =>
                                          {
                                              if (!result.Result.Success)
                                              {
                                                  return Results.NotFound(string.Join(',', result.Result.Errors.Select(err => err.GetErrorMessage())));
                                              }

                                              return Results.Json(result.Result);
                                          }, httpContextAccessor, cancellationToken);
                }).Produces(201, typeof(ErrorOr<bool>)).Produces(400, typeof(string));

                group.MapPost("/{accountNumber}/balance", (AccountGetBalanceService accountGetBalance,
                                       [FromRoute] string accountNumber,
                                       IHttpContextAccessor httpContextAccessor,
                                       CancellationToken cancellationToken) =>
                {
                    return accountGetBalance.ExecuteAsync(accountNumber, cancellationToken)
                                          .ContinueWith((result, state) =>
                                          {
                                              if (!result.Result.Success)
                                              {
                                                  return Results.NotFound(string.Join(',', result.Result.Errors.Select(err => err.GetErrorMessage())));
                                              }

                                              return Results.Json(result.Result);
                                          }, httpContextAccessor, cancellationToken);
                }).Produces(200, typeof(ErrorOr<decimal>)).Produces(400, typeof(string));


                group.MapPost("/{accountNumber}/transfer", (AccountTransferService accountTransferService,
                                      [FromRoute] string accountNumber,
                                      [FromBody] TransferRequestContract contract,
                                      IHttpContextAccessor httpContextAccessor,
                                      CancellationToken cancellationToken) =>
                {
                    return accountTransferService.ExecuteAsync(accountNumber, contract, cancellationToken)
                                          .ContinueWith((result, state) =>
                                          {
                                              if (!result.Result.Success)
                                              {
                                                  return Results.NotFound(string.Join(',', result.Result.Errors.Select(err => err.GetErrorMessage())));
                                              }

                                              return Results.Json(result.Result);
                                          }, httpContextAccessor, cancellationToken);
                }).Produces(200, typeof(ErrorOr<string>)).Produces(400, typeof(string));
                group.MapPost("/{accountNumber}/report", (AccountReportService accountReportService,
                                      [FromRoute] string accountNumber,
                                      [FromBody] AccountReportRequestContract contract,
                                      [FromQuery] int? index,
                                      [FromQuery] int? length,
                                      CancellationToken cancellationToken) =>
                {
                    return accountReportService.ExecuteAsync(accountNumber, contract, index, length, cancellationToken);
                });
            }
            return host;
        }
    }
}
