
namespace Aksl.Sockets.Client.Configuration
{
    // "EndpointName": {
    //    "Url": "https://*:5463",
    //    "Certificate": {
    //        "Path": "testCert.pfx",
    //        "Password": "testPassword"
    //    }
    // }
    public class Endpoint
    {
        public Endpoint()
        {
            Certificate = new Certificate();
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool NoDelay { get; set; } = true;

        public Certificate Certificate { get; set; }

        public SocketPipes Pipe { get; set; }

        public BlockSettings Block{ get; set; }
    }
}
