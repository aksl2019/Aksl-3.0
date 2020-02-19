using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.Features;

using Aksl.Sockets.Server.Configure;

namespace Aksl.Sockets.Server
{
    public interface ISocketServer
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        IFeatureCollection Features { get; }

        SocketServerOptions Options { get;  }

        ServiceContext ServiceContext { get;  }
    }
}
