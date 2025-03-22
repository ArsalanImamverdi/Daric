using System.Data;
using System.Data.Common;
using System.Diagnostics;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Daric.Database.SqlServer.Internals;

internal class TracingInterceptor(IServiceProvider serviceProvider) : DbCommandInterceptor
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        AddSqlTextTraces(command);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        AddSqlTextTraces(command);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        AddSqlTextTraces(command);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }
    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        AddSqlTextTraces(command);
        return base.ScalarExecuted(command, eventData, result);
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        AddSqlTextTraces(command);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        AddSqlTextTraces(command);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void AddSqlTextTraces(DbCommand command)
    {
        var tracingConfig = _serviceProvider.GetService<ISqlTracingConfig>();
        if (tracingConfig is not null && tracingConfig.Enabled && tracingConfig.IncludeCommandText)
        {
            Activity.Current?.AddTag("sql.Text", command.CommandText);
            foreach (var @param in command.Parameters.Cast<SqlParameter>())
                Activity.Current?.AddTag($"sql.Text.{param.ParameterName}", param.Value);
        }

    }

}

