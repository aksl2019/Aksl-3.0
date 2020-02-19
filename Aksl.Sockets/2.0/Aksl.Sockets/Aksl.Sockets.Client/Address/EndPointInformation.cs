//using System;
//using System.Net;

//namespace Aksl.Sockets.Client
//{
//    public class EndPointInformation
//    {
//        #region IEndPointInformation
//        /// <summary>
//        /// The type of interface being described: either an <see cref="IPEndPoint"/>, Unix domain socket path, or a file descriptor.
//        /// </summary>
//        public ListenType Type { get; set; }

//        private FileHandleType _handleType;
//        public FileHandleType HandleType
//        {
//            get => _handleType;
//            set
//            {
//                if (value == _handleType)
//                {
//                    return;
//                }
//                if (Type != ListenType.FileHandle || _handleType != FileHandleType.Auto)
//                {
//                    throw new InvalidOperationException();
//                }

//                switch (value)
//                {
//                    case FileHandleType.Tcp:
//                    case FileHandleType.Pipe:
//                        _handleType = value;
//                        break;
//                    default:
//                        throw new ArgumentException(nameof(HandleType));
//                }
//            }
//        }

//        // IPEndPoint is mutable so port 0 can be updated to the bound port.
//        /// <summary>
//        /// The <see cref="IPEndPoint"/> to bind to.
//        /// Only set if the <see cref="ListenOptions"/> <see cref="Type"/> is <see cref="ListenType.IPEndPoint"/>.
//        /// </summary>
//        public IPEndPoint IPEndPoint { get; set; }

//        /// <summary>
//        /// The absolute path to a Unix domain socket to bind to.
//        /// Only set if the <see cref="ListenOptions"/> <see cref="Type"/> is <see cref="ListenType.SocketPath"/>.
//        /// </summary>
//        public string SocketPath { get; set; }

//        /// <summary>
//        /// A file descriptor for the socket to open.
//        /// Only set if the <see cref="ListenOptions"/> <see cref="Type"/> is <see cref="ListenType.FileHandle"/>.
//        /// </summary>
//        public ulong FileHandle { get; }

//        /// <summary>
//        /// Set to false to enable Nagle's algorithm for all connections.
//        /// </summary>
//        /// <remarks>
//        /// Defaults to true.
//        /// </remarks>
//        public bool NoDelay { get; set; } = true;

//        public bool IsHttps { get; set; }

//        public string DisplayName => GetDisplayName();

//        public string GetDisplayName()
//        {
//            var scheme = "http";

//            switch (Type)
//            {
//                case ListenType.IPEndPoint:
//                    return $"{scheme}://{IPEndPoint}";
//                case ListenType.SocketPath:
//                    return $"{scheme}://unix:{SocketPath}";
//                case ListenType.FileHandle:
//                    return $"{scheme}://<file handle>";
//                default:
//                    throw new InvalidOperationException();
//            }
//        }
//        #endregion
//    }

//    public enum ListenType
//    {
//        IPEndPoint,
//        SocketPath,
//        FileHandle
//    }

//    public enum FileHandleType
//    {
//        Auto,
//        Tcp,
//        Pipe
//    }
//}
