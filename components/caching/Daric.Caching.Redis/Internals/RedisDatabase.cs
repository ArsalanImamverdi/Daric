
using Daric.Caching.Abstractions;

namespace Daric.Caching.Redis.Internals
{
    internal sealed class RedisDatabase(IRedisDatabaseProvider redisDatabaseProvider) : IDistributedCacheDatabase
    {
        public Task SecureSetAsync(string key, decimal value, CancellationToken cancellationToken)
        {
            return redisDatabaseProvider.GetDatabaseAsync(cancellationToken).ContinueWith(res =>
            {
                var database = res.Result;
                return database.StringSetAsync(key, SecureValue(value));
            }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current).Unwrap();
        }
        public Task<decimal> SecureGetAsync(string key, CancellationToken cancellationToken)
        {
            return redisDatabaseProvider.GetDatabaseAsync(cancellationToken).ContinueWith(res =>
            {
                var database = res.Result;
                return database.StringGetAsync(key);
            }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current)
                .Unwrap().ContinueWith(redisValue =>
            {
                if (redisValue?.Result == null || redisValue.Result.IsNull)
                    return -1;
                _ = TryGetSecureValue(redisValue.Result!, out var value);
                return value;
            }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);

        }

        private static bool TryGetSecureValue(byte[] bytes, out decimal value)
        {
            if (bytes.Length != 20)
            {
                value = -1;
                return false;
            }
            var span = bytes.AsSpan();
            var valueBytes = span[0..16];
            var checksum = Crc32.Compute(valueBytes);
            if (checksum.SequenceEqual(span[16..]))
            {
                value = BytesToDecimal(valueBytes);
                return true;
            }

            value = -1;
            return false;
        }

        private static decimal BytesToDecimal(ReadOnlySpan<byte> bytes)
        {
            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                bits[i] = BitConverter.ToInt32(bytes.Slice(i * 4, 4));
            }
            return new decimal(bits);
        }
        private static byte[] SecureValue(decimal value)
        {
            var bytes = GetBytes(value);
            var checksum = Crc32.Compute(bytes);

            return Concat(bytes, checksum);
        }

        private static byte[] Concat(byte[] first, byte[] second)
        {
            var bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
        private static byte[] GetBytes(uint value)
        {
            return BitConverter.GetBytes(value);
        }
        private static byte[] GetBytes(decimal value)
        {
            var valueBits = Decimal.GetBits(value).AsSpan();
            var bytes = new byte[16];
            for (var i = 0; i < 4; i++)
            {
                var valueBytes = BitConverter.GetBytes(valueBits[i]);
                Buffer.BlockCopy(valueBytes, 0, bytes, i * 4, 4);
            }
            return bytes;
        }
    }
}
