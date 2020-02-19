using System;
using System.Collections.Generic;

namespace Aksl.WebSockets.Client
{
    /// <summary>
    /// MessageContext
    /// </summary>
    public class MessageContext
    {
        public byte[] Data { get; set; }

        public IEnumerable<byte[]> Datas { get; set; }

        /// <summary>
        /// The exception that occured in Load.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// If true, the exception will not be rethrown.
        /// </summary>
        public bool Ignore { get; set; } = true;

        //Ö´ÐÐÊ±¼ä
        public TimeSpan ExecutionTime { get; set; }

        public int MessageConunt { get; set; }
    }
}