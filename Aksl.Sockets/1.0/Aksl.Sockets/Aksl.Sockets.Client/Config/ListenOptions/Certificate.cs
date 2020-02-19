
namespace Aksl.Sockets.Client.Configuration
{
    // "CertificateName": {
    //      "Path": "testCert.pfx",
    //      "Password": "testPassword"
    // }
    public class Certificate
    {
        public Certificate()
        {
        }

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
