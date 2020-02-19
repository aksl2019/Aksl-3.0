//using System;
//using System.Net;

//namespace Aksl.Sockets.Client
//{
//    public class AddressParser
//    {
//        #region ParseAddress Methods
//        internal static EndPointInformation Parse(string address)
//        {
//            EndPointInformation endPointInformation = new EndPointInformation ();

//            var parsedAddress = ServerAddress.FromUrl(address);
//            bool  https = false;

//            if (parsedAddress.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
//            {
//                https = true;
//            }
//            else if (!parsedAddress.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
//            {
//                throw new InvalidOperationException($"Format Unsupported Address Scheme:{address}");
//            }

//            endPointInformation.IsHttps = https;

//            if (!string.IsNullOrEmpty(parsedAddress.PathBase))
//            {
//                throw new InvalidOperationException($"Format Configure PathBase From MethodCall({nameof(AddressParser)}.UsePathBase()");
//            }

          
//            if (parsedAddress.IsUnixPipe)
//            {
//                endPointInformation.Type = ListenType.SocketPath;
//                endPointInformation.SocketPath =parsedAddress.UnixPipePath;
//            }
//            else if (string.Equals(parsedAddress.Host, "localhost", StringComparison.OrdinalIgnoreCase))
//            {
//                // "localhost" for both IPv4 and IPv6 can't be represented as an IPEndPoint.
//                endPointInformation.Type =  ListenType.IPEndPoint;
//                endPointInformation.IPEndPoint = new IPEndPoint(IPAddress.Loopback, parsedAddress.Port);
//            }
//            else if (TryCreateIPEndPoint(parsedAddress, out var endpoint))
//            {
//                endPointInformation.Type = ListenType.IPEndPoint;
//                endPointInformation.IPEndPoint = new IPEndPoint(IPAddress.IPv6Any, parsedAddress.Port);
//            }
//            else
//            {
//                // when address is 'http://hostname:port', 'http://*:port', or 'http://+:port'
//                endPointInformation.Type = ListenType.IPEndPoint;
//                endPointInformation.IPEndPoint = new IPEndPoint(IPAddress.Loopback, parsedAddress.Port);
//            }
          
//            return endPointInformation;
//        }

//        /// <summary>
//        /// Returns an <see cref="IPEndPoint"/> for the given host an port.
//        /// If the host parameter isn't "localhost" or an IP address, use IPAddress.Any.
//        /// </summary>
//        protected internal static bool TryCreateIPEndPoint(ServerAddress address, out IPEndPoint endpoint)
//        {
//            if (!IPAddress.TryParse(address.Host, out var ip))
//            {
//                endpoint = null;
//                return false;
//            }

//            endpoint = new IPEndPoint(ip, address.Port);
//            return true;
//        }
//        #endregion
//    }
//}
