using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aksl.Sockets.Server.Processor
{
    public interface IProcessor
    {
        //Func<IEnumerable<byte[]>, CancellationToken, Task<IEnumerable<byte[]>>>
        //Task<IEnumerable<byte[]>> ProcessAsync(IEnumerable<byte[]> data, CancellationToken cancellationToken);

        Task<IEnumerable<byte[]>> DoAsync(IEnumerable<byte[]> buffer, CancellationToken cancellationToken);

        // Task<IEnumerable<byte[]>> DoAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);
    }

    public interface ISequenceProcessor
    {
        //Func<IEnumerable<byte[]>, CancellationToken, Task<IEnumerable<byte[]>>>
        //Task<IEnumerable<byte[]>> ProcessAsync(IEnumerable<byte[]> data, CancellationToken cancellationToken);

        // Task<IEnumerable<byte[]>> ProcessAsync(IEnumerable<byte[]> buffer, CancellationToken cancellationToken);

        Task<IEnumerable<byte[]>> DoAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);
    }

    public interface IStringProcessor
    {
        //Func<IEnumerable<string>, CancellationToken, Task<IEnumerable<byte[]>>>

        Task<IEnumerable<byte[]>> DoAsync(IEnumerable<string> buffer, CancellationToken cancellationToken);

        // Task<IEnumerable<byte[]>> DoAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken);
    }
}