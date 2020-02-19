using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aksl.Pipeline
{
    public static class BufferExtensions
    {
        public static ArraySegment<byte> GetArray(this Memory<byte> memory)
        {
            return ((ReadOnlyMemory<byte>)memory).GetArray();
        }

        public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
            {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }
            return result;
        }

        public static IList<byte[]> ToBytes(this ReadOnlySequence<byte> buffer, byte lineFeed = (byte)'\n')
        {
            SequencePosition? position = default;
            SequencePosition start = buffer.Start;
            string lineString = null;

            IList<byte[]> byteList = new List<byte[]>();

            do
            {
                position = buffer.PositionOf((byte)'\n');

                if (position != null)
                {
                    var sequence = buffer.Slice(0, position.Value);

                    var bytes = sequence.ToArray();
                    lineString = Encoding.UTF8.GetString(bytes);

                    byteList.Add(bytes);

                    start = buffer.GetPosition(1, position.Value);
                    buffer = buffer.Slice(start);
                }
            } while (position != null);

            return byteList;
        }

        public static IList<TMessage> ToObjects<TMessage>(this ReadOnlySequence<byte> buffer, byte lineFeed = (byte)'\n')
        {
            SequencePosition? position = default;
            SequencePosition start = buffer.Start;
            string lineString = null;

            IList<TMessage> messageList = new List<TMessage>();

            do
            {
                position = buffer.PositionOf((byte)'\n');

                if (position != null)
                {
                    var sequence = buffer.Slice(0, position.Value);

                    var bytes = sequence.ToArray();

                    lineString = Encoding.UTF8.GetString(bytes);
                    //var msg = JsonConvert.DeserializeObject<TMessage>(lineString);
                    //var msg =JsonSerializer.Deserialize<TMessage>(lineString, new JsonSerializerOptions
                    //{
                    //    PropertyNameCaseInsensitive = true
                    //});
                    var deserializeMsg = JsonSerializer.Deserialize<TMessage>(lineString);

                    messageList.Add(deserializeMsg);

                    start = buffer.GetPosition(1, position.Value);
                    buffer = buffer.Slice(start);
                }
            } while (position != null);

            return messageList;
        }

        public static IList<byte[]> GetByteList(this ReadOnlySequence<byte> buffer, byte lineFeed = (byte)'\n')
        {
            SequencePosition? position = null;
            SequencePosition start = buffer.Start;
            string lineString = default;

            IList<byte[]> byteList = new List<byte[]>();
            IList<string> readLines = new List<string>();

            do
            {
                position = buffer.PositionOf((byte)'\n');

                if (position != null)
                {
                    var sequence = buffer.Slice(0, position.Value);

                    lineString = PipeTextReader.ReadString(sequence, Encoding.UTF8);
                    readLines.Add(lineString);
                    byteList.Add(Encoding.UTF8.GetBytes(lineString));

                    start = buffer.GetPosition(1, position.Value);
                    buffer = buffer.Slice(start);
                }
            } while (position != null);

            return byteList;
        }

        public static IList<string> GetStringList(this ReadOnlySequence<byte> buffer, byte lineFeed = (byte)'\n')
        {
            SequencePosition? position = null;
            SequencePosition start = buffer.Start;
            string lineString = default;

            List<string> readLines = new List<string>();

            do
            {
                position = buffer.PositionOf((byte)'\n');

                if (position != null)
                {
                    var sequence = buffer.Slice(0, position.Value);

                    lineString = PipeTextReader.ReadString(sequence, Encoding.UTF8);
                    readLines.Add(lineString);

                    start = buffer.GetPosition(1, position.Value);
                    buffer = buffer.Slice(start);
                }
            } while (position != null);

            return readLines;
        }
    }
}
