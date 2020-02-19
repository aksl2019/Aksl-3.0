using System;
using System.Linq;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aksl.Sockets.Client
{
    public static class BytesExtensions
    {
        public static char[] ConvertToChar(this byte[] bytes)
        {
            //char[] cs = new char[bytes.Count()];
            //for (int i = 0; i < bytes.Count(); i++)
            //{
            //    cs[i] = (char)bytes[i];
            //}
            //return cs;

            return bytes.Select(b => (char)b).ToArray();
        }
    }
}
