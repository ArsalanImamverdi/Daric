using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StackExchange.Redis;

namespace Daric.Caching.Redis.Internals
{
    internal class ConnectionMultiplexerConnect
    {
        public Task<ConnectionMultiplexer>? ConnectTask { get; set; }
    }
}
