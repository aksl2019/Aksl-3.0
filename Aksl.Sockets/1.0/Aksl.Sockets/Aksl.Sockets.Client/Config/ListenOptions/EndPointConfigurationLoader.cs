using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Configuration;

namespace Aksl.Sockets.Client.Configuration
{
    public class EndPointConfigurationLoader
    {
        #region Members
        private SocketClientOptions _socketClientOptions;

        private IConfiguration _configuration;
        private IConfiguration _certConfiguration;
        private IConfiguration _endpointConfiguration;

        private List<Endpoint> _endpoints;
        private IDictionary<string, Certificate> _certificates;
        private IDictionary<string, SocketPipes> _pipeSettings;
        private IDictionary<string, BlockSettings> _blockSettings;
        #endregion

        #region Constructor
        public EndPointConfigurationLoader(SocketClientOptions socketClientOptions, IConfiguration configuration)
        {
            _socketClientOptions = socketClientOptions ?? throw new ArgumentNullException(nameof(socketClientOptions));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _certConfiguration = configuration.GetSection("Certificates");
            _endpointConfiguration = configuration.GetSection("Endpoints");

            //SocketClientSettingsSet = new List<SocketClientSettings>();

            ReadEndpoints();
            ReadCertificates();
        }

        public EndPointConfigurationLoader(SocketClientOptions socketClientOptions)
        {
            _socketClientOptions = socketClientOptions ?? throw new ArgumentNullException(nameof(socketClientOptions));

            _certificates = _socketClientOptions.Certificates;
            _pipeSettings = _socketClientOptions.Pipes;
            _blockSettings= _socketClientOptions.Blocks;

            DefaultBlockSettings = BlockSettings.Default;
            DefaultPipeSettings = new SocketPipes();
            _socketClientOptions.DefaultBlockSettings = DefaultBlockSettings;
            _socketClientOptions.DefaultPipeSettings = DefaultPipeSettings;

            ReadEndpoints();
           // ReadCertificates();
        }
        #endregion

        #region Properties
        // public List<SocketClientSettings> SocketClientSettingsSet { get; set; }
        //public IConfiguration CertConfiguration { get; set; }
       // public SocketClientOptions Options { get; }

        //public IConfiguration EndpointConfiguration { get; set; }

        public BlockSettings DefaultBlockSettings { get; set; }

        public SocketPipes DefaultPipeSettings { get; set; }

        public X509Certificate2 DefaultCertificate { get; set; }
        #endregion

        #region Read Methods
        public void ReadEndpoints()
        {
            _endpoints = new List<Endpoint>();

            foreach (var endpointPair in _socketClientOptions.EndPoints)
            {
                if (string.IsNullOrEmpty(endpointPair.Value.Url))
                {
                    throw new InvalidOperationException($"Format Endpoint Missing Url{endpointPair.Key}");
                }

                //if (endpointPair.Value.Pipe == null)
                //{
                //    endpointPair.Value.Pipe = DefaultPipeSettings;
                //    if ( _pipeSettings.TryGetValue(endpointPair.Key, out var pipeSetting))
                //    {
                //        endpointPair.Value.Pipe = pipeSetting;
                //    }
                //}

                //if (endpointPair.Value.Block == null)
                //{
                //    endpointPair.Value.Block = DefaultBlockSettings;
                //    if (_blockSettings.TryGetValue(endpointPair.Key, out var blockSetting))
                //    {
                //        endpointPair.Value.Block = blockSetting; 
                //    }
                //}

                endpointPair.Value.Name = endpointPair.Key;
            }

            _endpoints.AddRange(_socketClientOptions.EndPoints.Values.Select(e => e).ToList());
        }

        private void ReadCertificates()
        {
            //_certificates = new Dictionary<string, Certificate>();
            //_certConfiguration.Bind(_certificates);

            _certificates = _socketClientOptions.Certificates;
        }
        #endregion

        #region Load Method
        public void Load()
        {
            if (_socketClientOptions.ConfigurationLoader == null)
            {
                // The loader has already been run.
                return;
            }
            _socketClientOptions.ConfigurationLoader = null;

            LoadDefaults();

            foreach (var endpoint in _endpoints)
            {
                var socketClientSettings = new SocketClientSettings()
                {
                    Options = _socketClientOptions,
                    PipeSettings = endpoint.Pipe,
                    BlockSettings = endpoint.Block
                };

                var endPointInformation = AddressParser.Parse(endpoint.Url, out var https);
                {
                    endPointInformation.Options = _socketClientOptions;
                    endPointInformation.NoDelay = endpoint.NoDelay;
                    endPointInformation.PipeSettings = endpoint.Pipe;
                    endPointInformation.BlockSettings = endpoint.Block;
                    _socketClientOptions.ApplyEndpointDefaults(endPointInformation);
                    SetCertificate();
                }

                socketClientSettings.EndPointInformation = endPointInformation;
                SetPipeSettings();

                void SetCertificate()
                {
                    if (https)
                    {
                        endPointInformation.ClientCertificate = LoadCertificate(endpoint.Certificate, endpoint.Name);

                        if (endPointInformation.ClientCertificate == null)
                        {
                            endPointInformation.ClientCertificate = DefaultCertificate;

                            if (_certificates.TryGetValue(endpoint.Name, out var certConfig))
                            {
                                endPointInformation.ClientCertificate = LoadCertificate(certConfig, endpoint.Name);
                            }
                        }
                    }
                }

                void SetPipeSettings()
                {
                    if (endPointInformation.PipeSettings==null)
                    {
                        endPointInformation.PipeSettings = DefaultPipeSettings;
                        if (_pipeSettings.TryGetValue(endpoint.Name, out var pipeSetting))
                        {
                            endPointInformation.PipeSettings = pipeSetting;
                            socketClientSettings.PipeSettings = pipeSetting;
                        }
                    }

                    if (endPointInformation.BlockSettings == null)
                    {
                        endPointInformation.BlockSettings = DefaultBlockSettings;
                        if (_blockSettings.TryGetValue(endpoint.Name, out var blockSetting))
                        {
                            endPointInformation.BlockSettings = endpoint.Block;
                            socketClientSettings.BlockSettings = blockSetting;
                        }
                    }
                }

                _socketClientOptions.EndPointInformations.Add(endPointInformation);

                _socketClientOptions.SocketClientSettingsList.Add(socketClientSettings);
            }

            //foreach (var endpointInfo in _socketClientOptions.EndPointInformations)
            //{
            //    var socketClientSettings=new SocketClientSettings()
            //    {
            //        Options = _socketClientOptions,
            //        EndPointInformation = endpointInfo,
            //        PipeSettings = _socketClientOptions.DefaultPipeSettings,
            //        BlockSettings = _socketClientOptions.DefaultBlockSettings,
            //        CloseTime = _socketClientOptions.CloseTime
            //    };

            //    _socketClientOptions.SocketClientSettingsSet.Add(socketClientSettings);
            //}
        }
        #endregion

        #region Certificate Methods
        private void LoadDefaults()
        {
            if (_certificates.TryGetValue("Default", out var defaultCert))
            {
                var defCert = LoadCertificate(defaultCert, "Default");
                if (defCert != null)
                {
                    DefaultCertificate = defCert;
                    _socketClientOptions.DefaultCertificate = defCert;
                }
            }

            if (_pipeSettings.TryGetValue("Default", out var defaultPipe))
            {
                DefaultPipeSettings = defaultPipe;
                _socketClientOptions.DefaultPipeSettings = defaultPipe;
            }

            if (_blockSettings.TryGetValue("Default", out var defaultBlock))
            {
                DefaultBlockSettings = defaultBlock;
                _socketClientOptions.DefaultBlockSettings = defaultBlock;
            }
        }

        private X509Certificate2 LoadCertificate(Certificate certInfo, string endpointName)
        {
            if (certInfo.IsFileCert && certInfo.IsStoreCert)
            {
                throw new InvalidOperationException($"The endpoint { endpointName } specified multiple certificate sources.");
            }
            else if (certInfo.IsFileCert)
            {
                string certPath = Path.Combine(_socketClientOptions.ContentRootPath, certInfo.Path);
                return new X509Certificate2(certPath, certInfo.Password);
            }
            else if (certInfo.IsStoreCert)
            {
                return LoadFromStoreCert(certInfo);
            }
            return null;
        }

        private X509Certificate2 LoadFromStoreCert(Certificate certInfo)
        {
            var subject = certInfo.Subject;
            var storeName = certInfo.Store;
            var location = certInfo.Location;
            var storeLocation = StoreLocation.CurrentUser;

            if (!string.IsNullOrEmpty(location))
            {
                storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), location, ignoreCase: true);
            }

            if (!string.IsNullOrEmpty(location))
            {
                if (Enum.TryParse<StoreLocation>(location, out var currentLocation))
                {
                    storeLocation = currentLocation;
                }
            }

            var allowInvalid = certInfo.AllowInvalid ?? false;

            return LoadFromStoreCert(subject, storeName, storeLocation, allowInvalid);
        }

        private X509Certificate2 LoadFromStoreCert(string subject, string storeName, StoreLocation storeLocation, bool allowInvalid)
        {
            using (var store = new X509Store(storeName, storeLocation))
            {
                X509Certificate2Collection storeCertificates = null;
                X509Certificate2 foundCertificate = null;

                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    storeCertificates = store.Certificates;
                    var foundCertificates = storeCertificates.Find(X509FindType.FindBySubjectName, subject, !allowInvalid);
                    foundCertificate = foundCertificates
                                       .OfType<X509Certificate2>()
                                       .OrderByDescending(certificate => certificate.NotAfter)
                                       .FirstOrDefault();

                    if (foundCertificate == null)
                    {
                        throw new InvalidOperationException($"The requested certificate {subject} could not be found in {storeLocation}/{storeName} with AllowInvalid setting: {allowInvalid}.");
                    }

                    return foundCertificate;
                }
                finally
                {
                    DisposeCertificates(storeCertificates, except: foundCertificate);
                }
            }
        }

        private static void DisposeCertificates(X509Certificate2Collection certificates, X509Certificate2 except)
        {
            if (certificates != null)
            {
                foreach (var certificate in certificates)
                {
                    if (!certificate.Equals(except))
                    {
                        certificate.Dispose();
                    }
                }
            }
        }
        #endregion
    }
}
