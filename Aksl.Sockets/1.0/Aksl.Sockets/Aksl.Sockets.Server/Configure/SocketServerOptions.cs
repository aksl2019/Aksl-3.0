using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Configuration;

using Aksl.Pipeline;

namespace Aksl.Sockets.Server.Configure
{
    public class SocketServerOptions
    {
        #region Constructor
        public SocketServerOptions()
        {
            ApplicationSchedulingMode = SchedulingMode.Default;
            ListenOptions = new List<ListenOptions>();
            //Limits = new SocketServerLimits();
            EndpointDefaults = _ => { };

            //PauseWriterThreshold = 4 * 1024 * 8;
            //ResumeWriterThreshold = 4 * 1024 *2;
            //MinimumSegmentSize = 4 * 1024 ;
            //UseSynchronizationContext = false;
            //MinAllocBufferSize = 2 * 1024;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Enables the Listen options callback to resolve and use services registered by the application during startup.
        /// Typically initialized by UseKestrel()"/>.
        /// </summary>
        public IServiceProvider ApplicationServices { get; set; }

        public string ContentRootPath { get; set; }

        /// <summary>
        /// Configures the endpoints that Kestrel should listen to.
        /// </summary>
        /// <remarks>
        /// If this list is empty, the server.urls setting (e.g. UseUrls) is used.
        /// </remarks>
        public List<ListenOptions> ListenOptions { get; }

        /// <summary>
        /// Gets or sets a value that determines how Kestrel should schedule user callbacks.
        /// </summary>
        /// <remarks>The default mode is <see cref="SchedulingMode.Default"/></remarks>
#pragma warning disable PUB0001 // Pubternal type in public API
        public SchedulingMode ApplicationSchedulingMode { get; set; }
#pragma warning restore PUB0001 // Pubternal type in public API

        /// <summary>
        /// Gets or sets a value that controls whether synchronous IO is allowed for the <see cref="HttpContext.Request"/> and <see cref="HttpContext.Response"/>
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        //public bool AllowSynchronousIO { get; set; } = true;

        /// <summary>
        /// Provides access to request limit options.
        /// </summary>
       // public SocketServerLimits Limits { get; }

        public IDictionary<string, Endpoint> EndPoints { get; set; }

        public IDictionary<string, Certificate> Certificates { get; set; }

        public X509Certificate2 DefaultCertificate { get; set; }

        public IDictionary<string, SocketPipeSettings> Pipes { get; set; }

        public SocketPipeSettings DefaultPipe { get; set; }

        /// <summary>
        /// Provides a configuration source where endpoints will be loaded from on server start.
        /// The default is null.
        /// </summary>
        public EndPointConfigurationLoader ConfigurationLoader { get; set; }

        /// <summary>
        /// A default configuration action for all endpoints. Use for Listen, configuration, the default url, and URLs.
        /// </summary>
        private Action<ListenOptions> EndpointDefaults { get; set; }

        /// <summary>
        /// The default server certificate for https endpoints. This is applied lazily after HttpsDefaults and user options.
        /// </summary>
          // internal X509Certificate2 DefaultCertificate { get; set; }

        /// <summary>
        /// Has the default dev certificate load been attempted?
        /// </summary>
        //    internal bool IsDevCertLoaded { get; set; }
        #endregion

        #region PipeOptions Properties
        //public long? PauseWriterThreshold { get; set; }

        //public long? ResumeWriterThreshold { get; set; }

        //public int MinimumSegmentSize { get; set; }

        //public int MinAllocBufferSize { get; set; }

        //public bool UseSynchronizationContext { get; set; }
        #endregion

        #region EndpointDefault Method
        /// <summary>
        /// Specifies a configuration Action to run for each newly created endpoint. Calling this again will replace
        /// the prior action.
        /// </summary>
        public void ConfigureEndpointDefaults(Action<ListenOptions> configureOptions)
        {
            EndpointDefaults = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        internal void ApplyEndpointDefaults(ListenOptions listenOptions)
        {
            listenOptions.SocketServerOptions = this;
            EndpointDefaults(listenOptions);
        }
        #endregion

        #region HttpsDefault Method


        #endregion

        #region DefaultCert Method

        #endregion

        #region Configure Method
        /// <summary>
        /// Creates a configuration loader for setting up Kestrel.
        /// </summary>
        public EndPointConfigurationLoader Configure()
        {
            //var loader = new EndPointConfigurationLoader(this, new ConfigurationBuilder().Build());
            var loader = new EndPointConfigurationLoader(this);
            ConfigurationLoader = loader;
            return loader;
        }

        /// <summary>
        /// Creates a configuration loader for setting up Kestrel that takes an IConfiguration as input.
        /// This configuration must be scoped to the configuration section for Kestrel.
        /// </summary>
        public EndPointConfigurationLoader Configure(IConfiguration config)
        {
            var loader = new EndPointConfigurationLoader(this, config);
            ConfigurationLoader = loader;
            return loader;
        }
        #endregion

        #region Listen Methods
        /// <summary>
        /// Bind to given IP address and port.
        /// </summary>
        public void Listen(IPAddress address, int port)
        {
            Listen(address, port, _ => { });
        }

        /// <summary>
        /// Bind to given IP address and port.
        /// The callback configures endpoint-specific settings.
        /// </summary>
        public void Listen(IPAddress address, int port, Action<ListenOptions> configure)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Listen(new IPEndPoint(address, port), configure);
        }

        /// <summary>
        /// Bind to given IP endpoint.
        /// </summary>
        public void Listen(IPEndPoint endPoint)
        {
            Listen(endPoint, _ => { });
        }

        /// <summary>
        /// Bind to given IP address and port.
        /// The callback configures endpoint-specific settings.
        /// </summary>
        public void Listen(IPEndPoint endPoint, Action<ListenOptions> configure)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new ListenOptions(endPoint);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        /// <summary>
        /// Listens on ::1 and 127.0.0.1 with the given port. Requesting a dynamic port by specifying 0 is not supported
        /// for this type of endpoint.
        /// </summary>
        public void ListenLocalhost(int port) => ListenLocalhost(port, options => { });

        /// <summary>
        /// Listens on ::1 and 127.0.0.1 with the given port. Requesting a dynamic port by specifying 0 is not supported
        /// for this type of endpoint.
        /// </summary>
        public void ListenLocalhost(int port, Action<ListenOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new LocalhostListenOptions(port);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        /// <summary>
        /// Listens on all IPs using IPv6 [::], or IPv4 0.0.0.0 if IPv6 is not supported.
        /// </summary>
        public void ListenAnyIP(int port) => ListenAnyIP(port, options => { });

        /// <summary>
        /// Listens on all IPs using IPv6 [::], or IPv4 0.0.0.0 if IPv6 is not supported.
        /// </summary>
        public void ListenAnyIP(int port, Action<ListenOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new AnyIPListenOptions(port);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        /// <summary>
        /// Bind to given Unix domain socket path.
        /// </summary>
        public void ListenUnixSocket(string socketPath)
        {
            ListenUnixSocket(socketPath, _ => { });
        }

        /// <summary>
        /// Bind to given Unix domain socket path.
        /// Specify callback to configure endpoint-specific settings.
        /// </summary>
        public void ListenUnixSocket(string socketPath, Action<ListenOptions> configure)
        {
            if (socketPath == null)
            {
                throw new ArgumentNullException(nameof(socketPath));
            }
            if (socketPath.Length == 0 || socketPath[0] != '/')
            {
                throw new ArgumentException(CoreStrings.UnixSocketPathMustBeAbsolute, nameof(socketPath));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new ListenOptions(socketPath);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }

        /// <summary>
        /// Open a socket file descriptor.
        /// </summary>
        public void ListenHandle(ulong handle)
        {
            ListenHandle(handle, _ => { });
        }

        /// <summary>
        /// Open a socket file descriptor.
        /// The callback configures endpoint-specific settings.
        /// </summary>
        public void ListenHandle(ulong handle, Action<ListenOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var listenOptions = new ListenOptions(handle);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            ListenOptions.Add(listenOptions);
        }
        #endregion
    }
}