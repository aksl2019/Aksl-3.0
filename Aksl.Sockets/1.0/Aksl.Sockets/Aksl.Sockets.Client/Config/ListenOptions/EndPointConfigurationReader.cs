using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Configuration;

namespace Aksl.Sockets.Client.Configuration
{
    public class EndPointConfigurationReader
    {
        private SocketClientOptions _socketClientOptions;
        private IConfiguration _configuration;

        private IList<EndpointConfig> _endpoints;
        private IDictionary<string, CertificateConfig> _certificates;

        public EndPointConfigurationReader(SocketClientOptions socketClientOptions,IConfiguration configuration)
        {
            _socketClientOptions = socketClientOptions ?? throw new ArgumentNullException(nameof(socketClientOptions));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            ReadCertificates();
            ReadEndpoints();
        }

        #region Properties
        public X509Certificate2 DefaultCertificate { get; set; }
        #endregion

        #region Load Method
        public void Load()
        {
            //if (_socketClientOptions.ConfigurationReader == null)
            //{
            //    // The loader has already been run.
            //    return;
            //}
            //_socketClientOptions.ConfigurationReader = null;

            LoadDefaultCert();

            foreach (var endpoint in _endpoints)
            {
                var endPointInformation = AddressParser.Parse(endpoint.Url, out var https);
                _socketClientOptions.ApplyEndpointDefaults(endPointInformation);

                if (https)
                {
                    endPointInformation.ClientCertificate = LoadCertificate(endpoint.Certificate, endpoint.Name);

                    if (endPointInformation.ClientCertificate==null)
                    {
                        endPointInformation.ClientCertificate = DefaultCertificate;

                        if (_certificates.TryGetValue(endpoint.Name, out var certConfig))
                        {
                            endPointInformation.ClientCertificate = LoadCertificate(certConfig, endpoint.Name);
                        }
                    }
                }

                _socketClientOptions.EndPointInformations.Add(endPointInformation);
            }
        }
        #endregion

        #region Read Methods
        private void ReadCertificates()
        {
            _certificates = new Dictionary<string, CertificateConfig>(0);

            var certificatesConfig = _configuration.GetSection("Certificates").GetChildren();
            foreach (var certificateConfig in certificatesConfig)
            {
                _certificates.Add(certificateConfig.Key, new CertificateConfig(certificateConfig));
            }
        }

        private void ReadEndpoints()
        {
            _endpoints = new List<EndpointConfig>();

            var endpointsConfig = _configuration.GetSection("Endpoints").GetChildren();
            foreach (var endpointConfig in endpointsConfig)
            {
                // "EndpointName": {
                //    "Url": "https://*:5463"
                // }

                var url = endpointConfig["Url"];
                var noDelay = endpointConfig["NoDelay"].SafeBool();
                if (string.IsNullOrEmpty(url))
                {
                    throw new InvalidOperationException($"Format Endpoint Missing Url{endpointConfig.Key}");
                }

                var endpoint = new EndpointConfig()
                {
                    Name = endpointConfig.Key,
                    Url = url,
                    NoDelay = noDelay,
                    ConfigSection = endpointConfig,
                    Certificate = new CertificateConfig(endpointConfig.GetSection("Certificate"))
                };
                _endpoints.Add(endpoint);
            }
        }
        #endregion

        #region Certificate Methods
        private void LoadDefaultCert()
        {
            if (_certificates.TryGetValue("Default", out var defaultCertConfig))
            {
                var defaultCert = LoadCertificate(defaultCertConfig, "Default");
                if (defaultCert != null)
                {
                    DefaultCertificate = defaultCert;
                }
            }
        }

        private X509Certificate2 LoadCertificate(CertificateConfig certInfo, string endpointName)
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

        private  X509Certificate2 LoadFromStoreCert(CertificateConfig certInfo)
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

    // "EndpointName": {
    //    "Url": "https://*:5463",
    //    "Certificate": {
    //        "Path": "testCert.pfx",
    //        "Password": "testPassword"
    //    }
    // }
    internal class EndpointConfig
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool NoDelay { get; set; }
        public IConfigurationSection ConfigSection { get; set; }
        public CertificateConfig Certificate { get; set; }
    }

    // "CertificateName": {
    //      "Path": "testCert.pfx",
    //      "Password": "testPassword"
    // }
    internal class CertificateConfig
    {
        public CertificateConfig(IConfigurationSection configSection)
        {
            ConfigSection = configSection;
            ConfigSection.Bind(this);
        }

        public IConfigurationSection ConfigSection { get; }

        // File
        public bool IsFileCert => !string.IsNullOrEmpty(Path);

        public string Path { get; set; }

        public string Password { get; set; }

        // Cert store

        public bool IsStoreCert => !string.IsNullOrEmpty(Subject);

        public string Subject { get; set; }

        public string Store { get; set; }

        public string Location { get; set; }

        public bool? AllowInvalid { get; set; }
    }
}
