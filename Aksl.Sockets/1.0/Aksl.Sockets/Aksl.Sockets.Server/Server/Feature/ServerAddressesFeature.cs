using System;
using System.Collections.Generic;

namespace Aksl.Sockets.Server
{
    internal class ServerAddressesFeature : IServerAddressesFeature
    {
        public ICollection<string> Addresses { get; } = new List<string>();
        public bool PreferHostingUrls { get; set; }
    }

}
