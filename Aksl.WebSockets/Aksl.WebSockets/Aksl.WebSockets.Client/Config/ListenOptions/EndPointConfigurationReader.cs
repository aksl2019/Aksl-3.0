using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace Aksl.WebSockets.Client.Configuration
{
    internal class EndPointConfigurationReader
    {
        #region Members
        private IConfiguration _configuration;
        private IList<EndpointConfig> _endpoints;
        private IDictionary<string, PipeConfig> _pipeLookups;
        private IDictionary<string, BlockConfig> _blockLookups;
        #endregion

        #region Constructors
        public EndPointConfigurationReader(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        #endregion

        #region Properties
        internal IDictionary<string, PipeConfig> Pipes
        {
            get
            {
                if (_pipeLookups == null)
                {
                    ReadPipeLookups();
                }

                return _pipeLookups;
            }
        }

        internal IDictionary<string, BlockConfig> Blocks
        {
            get
            {
                if (_blockLookups == null)
                {
                    ReadBlockLookups();
                }

                return _blockLookups;
            }
        }

        internal IEnumerable<EndpointConfig> Endpoints
        {
            get
            {
                if (_endpoints == null)
                {
                    ReadEndpoints();
                }

                return _endpoints;
            }
        }
        #endregion

        #region Read Methods
        private void ReadPipeLookups()
        {
            _pipeLookups = new Dictionary<string, PipeConfig>();

            var pipesConfig = _configuration.GetSection("Pipes").GetChildren();
            foreach (var pipeConfig in pipesConfig)
            {
                _pipeLookups.Add(pipeConfig.Key, new PipeConfig(pipeConfig));
            }
        }

        private void ReadBlockLookups()
        {
            _blockLookups = new Dictionary<string, BlockConfig>();

            var blocksConfig = _configuration.GetSection("Blocks").GetChildren();
            foreach (var blockConfig  in blocksConfig)
            {
                _blockLookups.Add(blockConfig.Key, new BlockConfig(blockConfig));
            }
        }

        private void ReadEndpoints()
        {
            _endpoints = new List<EndpointConfig>();

            var endpointConfigs = _configuration.GetSection("EndPoints").GetChildren();
            foreach (var endpointConfig in endpointConfigs)
            {
                // "EndpointName": {
                //    "Url": "https://*:5463"
                // }

                var url = endpointConfig["Url"];
                //var url = endpointConfig.GetValue<string>("Url");
                if (string.IsNullOrEmpty(url))
                {
                    throw new InvalidOperationException($"Format Endpoint Missing Url{endpointConfig.Key}");
                }

                var endpoint = new EndpointConfig()
                {
                    Name = endpointConfig.Key,
                    Url = url,  
                    Pipe = new PipeConfig(endpointConfig.GetSection("Pipe")),
                    Block = new BlockConfig(endpointConfig.GetSection("Block")), 
                    ConfigSection = endpointConfig
                };

                var receiveBufferSize = endpointConfig["ReceiveBufferSize"];
                if (!string.IsNullOrEmpty(receiveBufferSize))
                {
                    endpoint.ReceiveBufferSize = int.Parse(receiveBufferSize);
                }

                var sendBufferSize = endpointConfig["SendBufferSize"];
                if (!string.IsNullOrEmpty(sendBufferSize))
                {
                    endpoint.SendBufferSize = int.Parse(sendBufferSize);
                }

                var keepAliveDuration = endpointConfig["KeepAliveDuration"];
                if (!string.IsNullOrEmpty(keepAliveDuration))
                {
                    endpoint.KeepAliveDuration = double.Parse(keepAliveDuration);
                }

                var closeTime  = endpointConfig["CloseTime"];
                if (!string.IsNullOrEmpty(closeTime))
                {
                    endpoint.CloseTime =int.Parse(closeTime);
                }

                _endpoints.Add(endpoint);
            }
        }
    }
    #endregion

    // "EndpointName": {
    //    "Url": "https://*:5463",
    //    "Pipe": {
    //        "PauseWriterThreshold": "1048576",
    //        "ResumeWriterThreshold": "65536",
    //        "MinimumSegmentSize": "4096",
    //        "UseSynchronizationContext": "false",
    //        "MinAllocBufferSize": "4096"
    // }
    internal class EndpointConfig
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public int ReceiveBufferSize { get; set; }

        public int SendBufferSize { get; set; }

        public double KeepAliveDuration { get; set; }

        public int CloseTime { get; set; }

        public PipeConfig Pipe { get; set; }
        public BlockConfig Block { get; set; }

        public IConfigurationSection ConfigSection { get; set; }
    }

    internal class PipeConfig
    {
        #region Members
        #endregion

        public PipeConfig(IConfigurationSection configSection)
        {
            ConfigSection = configSection;
            ConfigSection.Bind(this);
        }

        public IConfigurationSection ConfigSection { get; }

        public bool IsNotNull => Default != null && Input != null && Input != null;

        #region Properties
        public PipeSettings Default { get; set; }

        public PipeSettings Input { get; set; }

        public PipeSettings Output { get; set; }
        #endregion
    }

    internal class BlockConfig
    {
        #region Members
        #endregion

        public BlockConfig(IConfigurationSection configSection)
        {
            ConfigSection = configSection;
            ConfigSection.Bind(this);
        }

        public IConfigurationSection ConfigSection { get; }

        public bool IsNotNull => BlockCount > 0 && MinPerBlock > 0 && MaxPerBlock > 0 && MaxAllocBlockSize > 0 && MaxDegreeOfParallelism > 0;

        #region Properties
        public int BlockCount { get; set; }

        public int MinPerBlock { get; set; }

        public int MaxPerBlock { get; set; }

        public int MaxAllocBlockSize { get; set; }

        public int MaxDegreeOfParallelism { get; set; }
        #endregion
    }
}