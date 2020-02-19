using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Configuration;

namespace Aksl.WebSockets.Client.Configuration
{
    public class EndPointConfigurationLoader
    {
        #region Members
        private WebSocketClientOptions _webSocketClientOptions;
        private List<Endpoint> _endpoints;
        private IDictionary<string, BlockSettings> _blockSettingsLookups;
        private IDictionary<string, WebSocketPipes> _pipeSettingsLookups;
        #endregion

        #region Constructor
        public EndPointConfigurationLoader(WebSocketClientOptions webSocketClientOptions, IConfiguration configuration)
        {
            _webSocketClientOptions = webSocketClientOptions ?? throw new ArgumentNullException(nameof(webSocketClientOptions));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            LoadFromConfig();
        }

        public EndPointConfigurationLoader(WebSocketClientOptions webSocketClientOptions)
        {
            Options = webSocketClientOptions ?? throw new ArgumentNullException(nameof(webSocketClientOptions));

            _blockSettingsLookups = _webSocketClientOptions.Blocks;
            _pipeSettingsLookups = _webSocketClientOptions.Pipes;

            DefaultBlockSettings = BlockSettings.Default;
            DefaultPipeSettings = new WebSocketPipes();
            _webSocketClientOptions.DefaultBlockSettings = DefaultBlockSettings;
            _webSocketClientOptions.DefaultPipeSettings = DefaultPipeSettings;

            ReadEndpoints();
        }
        #endregion

        #region Properties
        public WebSocketClientOptions Options { get; }

        public IConfiguration Configuration { get; }

        public BlockSettings DefaultBlockSettings { get; set; }

        public WebSocketPipes DefaultPipeSettings { get; set; }
        #endregion

        #region Load From Configuration Method
        private void LoadFromConfig()
        {
            var configReader = new EndPointConfigurationReader(Configuration);

            var endpointConfigs = configReader.Endpoints;
            _endpoints = new List<Endpoint>(endpointConfigs.Count());
            foreach (var endpointConfig in endpointConfigs)
            {
                Endpoint endpoint = new Endpoint()
                {
                    Name = endpointConfig.Name,
                    Url = endpointConfig.Url,
                    ReceiveBufferSize = endpointConfig.ReceiveBufferSize,
                    SendBufferSize = endpointConfig.SendBufferSize,
                    KeepAliveDuration = endpointConfig.KeepAliveDuration,
                    CloseTime = endpointConfig.CloseTime,
                    Block = ConvertToBlockSettings(endpointConfig.Block),
                    Pipe = ConvertToPipeSettings(endpointConfig.Pipe)
                };

                _endpoints.Add(endpoint);
            }

            var blockConfigs = configReader.Blocks;
            _blockSettingsLookups = new Dictionary<string, BlockSettings>(blockConfigs.Count());
            foreach (var blockConfig in blockConfigs)
            {
                _blockSettingsLookups.Add(blockConfig.Key, ConvertToBlockSettings(blockConfig.Value));
            }

            var pipeConfigs = configReader.Pipes;
            _pipeSettingsLookups = new Dictionary<string, WebSocketPipes>(pipeConfigs.Count());
            foreach (var pipeConfig in pipeConfigs)
            {
                _pipeSettingsLookups.Add(pipeConfig.Key, ConvertToPipeSettings(pipeConfig.Value));
            }

            BlockSettings ConvertToBlockSettings(BlockConfig blockConfig)
            {
                if (blockConfig.IsNotNull)
                {
                    return new BlockSettings()
                    {
                        BlockCount = blockConfig.BlockCount,
                        MaxPerBlock = blockConfig.MaxPerBlock,
                        MinPerBlock = blockConfig.MinPerBlock,
                        MaxAllocBlockSize = blockConfig.MaxAllocBlockSize,
                        MaxDegreeOfParallelism = blockConfig.MaxDegreeOfParallelism
                    };
                }

                return null;
            }

            WebSocketPipes ConvertToPipeSettings(PipeConfig pipeConfig)
            {
                if (pipeConfig.IsNotNull)
                {
                    return new WebSocketPipes()
                    {
                        Default = pipeConfig.Default,
                        Input = pipeConfig.Input,
                        Output = pipeConfig.Output
                    };
                }

                return null;
            }
        }
        #endregion

        #region Read Methods
        public void ReadEndpoints()
        {
            _endpoints = new List<Endpoint>();

            foreach (var endpointPair in _webSocketClientOptions.EndPoints)
            {
                if (string.IsNullOrEmpty(endpointPair.Value.Url))
                {
                    throw new InvalidOperationException($"Format Endpoint Missing Url{endpointPair.Key}");
                }

                endpointPair.Value.Name = endpointPair.Key;
            }

            _endpoints.AddRange(_webSocketClientOptions.EndPoints.Values.Select(e => e).ToList());
        }
        #endregion

        #region Load Method
        public void Load()
        {
            if (_webSocketClientOptions.ConfigurationLoader == null)
            {
                // The loader has already been run.
                return;
            }
            _webSocketClientOptions.ConfigurationLoader = null;

            LoadDefaults();

            foreach (var endpoint in _endpoints)
            {
                var endPointInformation = AddressParser.Parse(endpoint.Url, out var https);
                {
                    endPointInformation.Options = _webSocketClientOptions;
                    endPointInformation.PipeSettings = endpoint.Pipe;
                    endPointInformation.BlockSettings = endpoint.Block;
                    endPointInformation.ReceiveBufferSize = endpoint.ReceiveBufferSize>0? endpoint.ReceiveBufferSize: endPointInformation.ReceiveBufferSize;
                    endPointInformation.SendBufferSize = endpoint.SendBufferSize > 0 ? endpoint.SendBufferSize : endPointInformation.SendBufferSize;
                    endPointInformation.KeepAliveDuration = endpoint.KeepAliveDuration > 0 ? endpoint.KeepAliveDuration : endPointInformation.KeepAliveDuration;
                    endPointInformation.CloseTime = endpoint.CloseTime > 0 ? endpoint.CloseTime : endPointInformation.CloseTime;
                    _webSocketClientOptions.ApplyEndpointDefaults(endPointInformation);
                }

                SetPipeSettings();

                void SetPipeSettings()
                {
                    if (endPointInformation.PipeSettings==null)
                    {
                        endPointInformation.PipeSettings = DefaultPipeSettings;
                        if (_pipeSettingsLookups.TryGetValue(endpoint.Name, out var pipeSetting))
                        {
                            endPointInformation.PipeSettings = pipeSetting;
                        }
                    }

                    if (endPointInformation.BlockSettings == null)
                    {
                        endPointInformation.BlockSettings = DefaultBlockSettings;
                        if (_blockSettingsLookups.TryGetValue(endpoint.Name, out var blockSetting))
                        {
                            endPointInformation.BlockSettings = endpoint.Block;
                        }
                    }
                }

                _webSocketClientOptions.EndPointInformations.Add(endPointInformation);
            }
        }
        #endregion

        #region Load Default Method
        private void LoadDefaults()
        {
            if (_pipeSettingsLookups.TryGetValue("Default", out var defaultPipe))
            {
                DefaultPipeSettings = defaultPipe;
                _webSocketClientOptions.DefaultPipeSettings = defaultPipe;
            }

            if (_blockSettingsLookups.TryGetValue("Default", out var defaultBlock))
            {
                DefaultBlockSettings = defaultBlock;
                _webSocketClientOptions.DefaultBlockSettings = defaultBlock;
            }
        }
        #endregion
    }
}
