using System;
using System.Collections.Generic;

namespace Aksl.Sockets.Server
{
    public interface IServerAddressesFeature
    {
        ICollection<string> Addresses { get; }

        bool PreferHostingUrls { get; set; }
    }
}
