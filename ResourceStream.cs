using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        public Stream ShortStream;
        public byte[] KeyBytes;
        public bool KeyFounded;
        public bool ValueReaded;

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException(); }

        public ResourceReaderStream(Stream stream, string key)
        {
            ShortStream = new BufferedStream(stream, 1024);
            KeyBytes = Encoding.ASCII.GetBytes(key);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var value = 0;
            if (!KeyFounded) SeekValue();
            KeyFounded = true;
            if (!ValueReaded) value = ReadFieldValue(buffer, offset, count);
            return value;
        }

        public int ReadFieldValue(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                var currentValue = ShortStream.ReadByte();
                if (currentValue == 0) currentValue = ShortStream.ReadByte(); 
                if (currentValue < 0 || currentValue == 1)
                {
                    ValueReaded = true;
                    return i;
                }
                buffer[offset + i] = (byte)currentValue;
            }
            return buffer.Length;
        }

        private void SeekValue()
        {
            while (true)
            {
                var index = 0;
                var same = true;
                while (true)
                {
                    var currentValue = ShortStream.ReadByte();
                    if (currentValue == 0) currentValue = ShortStream.ReadByte();
                    if (currentValue < 0 || currentValue == 1)
                        break;
                    if (index < KeyBytes.Length)
                        if (KeyBytes[index] != currentValue) same = false;
                    index++;
                }
                if (same) return;
            }
        }

        public override void Flush()
        {
            // nothing to do 
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
