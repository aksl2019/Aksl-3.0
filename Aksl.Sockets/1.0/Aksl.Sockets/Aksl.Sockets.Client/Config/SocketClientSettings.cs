using System;

namespace Aksl.Sockets.Client.Configuration
{
    //public class SocketConnectSettings
    //{
    //    public SocketConnectSettings()
    //    {
    //        NoDelay = true;
    //        RetryCount = 5;
    //    }

    //    public string ServerUri { get; set; }

    //    public bool NoDelay { get; set; }

    //    public int RetryCount { get; set; }
    //}

    public class SocketClientSettings
    {
        public SocketClientSettings()
        {
        }

        public SocketClientOptions Options { get; set; }

        public IEndPointInformation EndPointInformation { get; set; }

        public SocketPipes PipeSettings { get; set; }

        public BlockSettings BlockSettings { get; set; }
    }
}