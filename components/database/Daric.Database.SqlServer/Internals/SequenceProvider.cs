using System.Data;
using System.Runtime.CompilerServices;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Daric.Database.SqlServer.Internals
{
    internal class SequenceProvider<TContext>(TContext context) : ISqlServerSequenceProvider<TContext>
        where TContext : ISqlDbContext<TContext>
    {
        public async Task<TResult?> GetNextValue<TResult>(string schema, string sequenceName)
        {
            try
            {
                var sequenceOutputParameter = new SqlParameter()
                {
                    ParameterName = "range_first_value",
                    SqlDbType = SqlDbType.Variant,
                    Direction = ParameterDirection.Output
                };

                object[] sequenceParameters =
                {
                new SqlParameter("sequence_name", string.Format("[{1}]",schema,sequenceName)),
                new SqlParameter("range_size", 1),
                sequenceOutputParameter
            };
                await context.Database.ExecuteSqlRawAsync(
                    "EXEC sys.sp_sequence_get_range @sequence_name,@range_size,@range_first_value output",
                    sequenceParameters);


                return (TResult)sequenceOutputParameter.Value;
            }
            catch
            {
                return default;
            }
        }
        public Task<TResult?> GetNextValue<TResult>(string sequenceName)
        {
            return GetNextValue<TResult>("dbo", sequenceName);
        }
    }
}
