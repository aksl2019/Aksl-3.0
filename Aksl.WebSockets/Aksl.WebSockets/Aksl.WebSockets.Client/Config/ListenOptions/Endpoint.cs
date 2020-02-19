
namespace Aksl.WebSockets.Client.Configuration
{
    // "EndpointName": {
    //    "Url": "https://*:5463",
    //    "Pipe": {
    //       "PauseWriterThreshold": "1048576",
    //"ResumeWriterThreshold": "65536",
    //"MinimumSegmentSize": "4096",
    //"UseSynchronizationContext": "false",
    //"MinAllocBufferSize": "4096"
    //    }
    // }
    public class Endpoint
    {
        public Endpoint()
        {
          
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public WebSocketPipes Pipe { get; set; }

        public BlockSettings Block{ get; set; }

        /// <summary>
        /// Gets or sets the size of the protocol buffer used to receive and parse frames.
        /// The default is 4kb.
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        public int SendBufferSize { get; set; }

        public double KeepAliveDuration { get; set; }

        public int CloseTime { get; set; }


    }
}
