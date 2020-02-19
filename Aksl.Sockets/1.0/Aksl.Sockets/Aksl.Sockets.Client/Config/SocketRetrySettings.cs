using System;

//Microsoft.AspNetCore.WebSockets

namespace Aksl.Sockets.Client.Configuration
{
    public class SocketRetrySettings
    {
        public SocketRetrySettings()
        {
            Count = 5; 
            Attempt = 1000;
        }

        //public string ServerUri { get; set; }

        //public bool NoDelay { get; set; }

        public int Count { get; set; }

        public int Attempt { get; set; }
    }
}