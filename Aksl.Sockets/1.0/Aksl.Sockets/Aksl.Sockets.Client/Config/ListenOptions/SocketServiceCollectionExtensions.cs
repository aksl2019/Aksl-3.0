using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Aksl.Sockets.Client.Configuration
{
    public static class SocketServiceCollectionExtensions
    {
        public static IServiceCollection UseSockets(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SocketClientOptions>, SocketClientOptionsSetup>();

            return services;
        }

        public static IServiceCollection UseSockets(this IServiceCollection services, Action<SocketClientOptions> configureOptions)
        {
            return services.UseSockets()
                           .Configure(configureOptions);
        }
    }
}
