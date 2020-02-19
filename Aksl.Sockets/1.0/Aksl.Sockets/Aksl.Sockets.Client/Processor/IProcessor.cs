using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aksl.Sockets.Client
{
    public interface IProcessor
    {
        ValueTask DoAsync(ReadOnlySequence<byte> buffers, CancellationToken cancellationToken);
    }

    public interface IByteProcessor
    {
        ValueTask DoAsync(byte[] buffers, CancellationToken cancellationToken);
    }
}