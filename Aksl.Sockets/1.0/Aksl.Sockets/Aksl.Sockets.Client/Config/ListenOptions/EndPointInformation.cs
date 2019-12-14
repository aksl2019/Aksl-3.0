using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Aksl.Sockets.Client.Configuration
{
    public class EndPointInformation: IEndPointInformation
    {
        #region Members
        private FileHandleType _handleType;
        #endregion

        #region Constructors
        public EndPointInformation()
        {
        }

        internal EndPointInformation(IPEndPoint endPoint)
        {
            Type = ListenType.IPEndPoint;
            IPEndPoint = endPoint;
        }

        internal EndPointInformation(string socketPath)
        {
            Type = ListenType.SocketPath;
            SocketPath = socketPath;
        }

        internal EndPointInformation(ulong fileHandle)
            : this(fileHandle, FileHandleType.Auto)
        {
        }

        internal EndPointInformation(ulong fileHandle, FileHandleType handleType)
        {
            Type = ListenType.FileHandle;
            FileHandle = fileHandle;
            switch (handleType)
            {
                case FileHandleType.Auto:
                case FileHandleType.Tcp:
                case FileHandleType.Pipe:
                    _handleType = handleType;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        #endregion

        #region Properties
        public SocketClientOptions Options { get; set; }

        /// <summary>
        /// The type of interface being described: either an <see cref="IPEndPoint"/>, Unix domain socket path, or a file descriptor.
        /// </summary>
        public ListenType Type { get; set; }

        /// <summary>
        /// A file descriptor for the socket to open.
        /// Only set if the <see cref="ListenOptions"/> <see cref="Type"/> is <see cref="ListenType.FileHandle"/>.
        /// </summary>
        public ulong FileHandle { get; }

        public FileHandleType HandleType
        {
            get => _handleType;
            set
            {
                if (value == _handleType)
                {
                    return;
                }
                if (Type != ListenType.FileHandle || _handleType != FileHandleType.Auto)
                {
                    throw new InvalidOperationException();
                }

                switch (value)
                {
                    case FileHandleType.Tcp:
                    case FileHandleType.Pipe:
                        _handleType = value;
                        break;
                    default:
                        throw new ArgumentException(nameof(HandleType));
                }
            }
        }

        /// <summary>
        /// The absolute path to a Unix domain socket to bind to.
        /// Only set if the <see cref="ListenOptions"/> <see cref="Type"/> is <see cref="ListenType.SocketPath"/>.
        /// </summary>
        public string SocketPath { get; set; }

        // IPEndPoint is mutable so port 0 can be updated to the bound port.
        /// <summary>
        /// The <see cref="IPEndPoint"/> to bind to.
        /// Only set if the <see cref="ListenOptions"/> <see cref="Type"/> is <see cref="ListenType.IPEndPoint"/>.
        /// </summary>
        public IPEndPoint IPEndPoint { get; set; }

        /// <summary>
        /// Set to false to enable Nagle's algorithm for all connections.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool NoDelay { get; set; } = true;

        public bool IsHttps { get; set; }

        public X509Certificate2 ClientCertificate { get; set; }

        public string Scheme { get;set; }

        public string DisplayName => GetDisplayName();

        public string GetDisplayName()
        {
           // var scheme = "http";

            switch (Type)
            {
                case ListenType.IPEndPoint:
                    return $"{Scheme}://{IPEndPoint}";
                case ListenType.SocketPath:
                    return $"{Scheme}://unix:{SocketPath}";
                case ListenType.FileHandle:
                    return $"{Scheme}://<file handle>";
                default:
                    throw new InvalidOperationException();
            }
        }
        #endregion
    }
}
