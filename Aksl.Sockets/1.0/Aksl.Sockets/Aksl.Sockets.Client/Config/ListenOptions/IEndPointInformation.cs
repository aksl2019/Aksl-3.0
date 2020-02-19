using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Aksl.Sockets.Client.Configuration
{
    public interface IEndPointInformation
    {
        /// <summary>
        /// The type of interface being described: either an <see cref="IPEndPoint"/>, Unix domain socket path, or a file descriptor.
        /// </summary>
        ListenType Type { get; }

        /// <summary>
        /// A file descriptor for the socket to open.
        /// Only set if <see cref="Type"/> is <see cref="ListenType.FileHandle"/>.
        /// </summary>
        ulong FileHandle { get; }

        //  HandleType is mutable so it can be re-specified later.
        /// <summary>
        /// The type of file descriptor being used.
        /// Only set if <see cref="Type"/> is <see cref="ListenType.FileHandle"/>.
        /// </summary>
        FileHandleType HandleType { get; set; }

        /// <summary>
        /// The absolute path to a Unix domain socket to bind to.
        /// Only set if <see cref="Type"/> is <see cref="ListenType.SocketPath"/>.
        /// </summary>
        string SocketPath { get; }

        // IPEndPoint is mutable so port 0 can be updated to the bound port.
        /// <summary>
        /// The <see cref="IPEndPoint"/> to bind to.
        /// Only set if <see cref="Type"/> is <see cref="ListenType.IPEndPoint"/>.
        /// </summary>
        IPEndPoint IPEndPoint { get; set; }

        /// <summary>
        /// Set to false to enable Nagle's algorithm for all connections.
        /// </summary>
        bool NoDelay { get; }

        //增加
        X509Certificate2 ClientCertificate { get; set; }

        SocketClientOptions Options { get; set; }

        string Scheme { get; set; }

        string DisplayName { get; }

        SocketPipes PipeSettings { get; set; }

        BlockSettings BlockSettings { get; set; }
    }
}
