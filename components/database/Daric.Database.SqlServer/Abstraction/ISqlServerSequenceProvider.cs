namespace Daric.Database.SqlServer
{
    public interface ISqlServerSequenceProvider<TContext>
        where TContext : ISqlDbContext<TContext>
    {
        Task<TResult?> GetNextValue<TResult>(string schema, string sequenceName);
        Task<TResult?> GetNextValue<TResult>(string sequenceName);
    }
}
