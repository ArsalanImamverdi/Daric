using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daric.Caching.Redis.Internals
{
    internal class Crc32
    {
        private static readonly uint[] Table;

        static Crc32()
        {
            Table = new uint[256];
            const uint polynomial = 0xEDB88320;
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                Table[i] = crc;
            }
        }

        public static byte[] Compute(byte[] bytes)
        {
            uint crc = 0xFFFFFFFF;
            foreach (byte b in bytes)
            {
                crc = (crc >> 8) ^ Table[(crc & 0xFF) ^ b];
            }
            return BitConverter.GetBytes(crc ^ 0xFFFFFFFF);
        }

        public static Span<byte> Compute(Span<byte> bytes)
        {
            uint crc = 0xFFFFFFFF;
            foreach (byte b in bytes)
            {
                crc = (crc >> 8) ^ Table[(crc & 0xFF) ^ b];
            }
            return BitConverter.GetBytes(crc ^ 0xFFFFFFFF);
        }
    }
}
