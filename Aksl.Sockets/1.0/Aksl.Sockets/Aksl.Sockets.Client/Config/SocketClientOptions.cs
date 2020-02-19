using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Configuration;


namespace Aksl.Sockets.Client.Configuration
{
    public class SocketClientOptions
    {
        #region Constructor
        public SocketClientOptions()
        {
            SocketClientSettingsList = new List<SocketClientSettings>();

            EndPointInformations = new List<IEndPointInformation>();
            Certificates = new Dictionary<string, Certificate>();
            Pipes = new Dictionary<string, SocketPipes>();
            Blocks = new Dictionary<string, BlockSettings>();
        }
        #endregion

        #region Properties
        public IServiceProvider ApplicationServices { get; set; }

        public string ContentRootPath { get; set; }

        public IDictionary<string, Endpoint> EndPoints { get; set; }

        public IDictionary<string, Certificate> Certificates { get; set; }

        public X509Certificate2 DefaultCertificate { get; set; }

        public IDictionary<string, SocketPipes> Pipes { get; set; }

        public SocketPipes DefaultPipeSettings { get; set; }

        public IDictionary<string, BlockSettings> Blocks { get; set; }

        public BlockSettings DefaultBlockSettings { get; set; }

        // public EndPointConfigurationReader ConfigurationReader { get; set; }

        public EndPointConfigurationLoader ConfigurationLoader { get; set; }

        private Action<EndPointInformation> EndpointDefaults { get; set; }= _ => { };

        public List<IEndPointInformation> EndPointInformations { get; }

        public List<SocketClientSettings> SocketClientSettingsList { get; }
        #endregion

        #region EndpointDefault Method
        /// <summary>
        /// Specifies a configuration Action to run for each newly created endpoint. Calling this again will replace
        /// the prior action.
        /// </summary>
        public void ConfigureEndpointDefaults(Action<EndPointInformation> configureOptions)
        {
            EndpointDefaults = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        }

        internal void ApplyEndpointDefaults(EndPointInformation endPointInformation)
        {
            endPointInformation.Options = this;
            EndpointDefaults(endPointInformation);
        }
        #endregion

        #region Configure Method
        public EndPointConfigurationLoader Configure()
        {
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
    }
}