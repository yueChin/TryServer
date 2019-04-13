// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.GZipStream
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.IO;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// Gzip压缩流
    /// </summary>
    public class GZipStream : Stream
    {
        private DeflateStream deflateStream;

        public GZipStream(Stream stream, CompressionMode mode)
          : this(stream, mode, false)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            this.deflateStream = new DeflateStream(stream, mode, leaveOpen);
            this.SetDeflateStreamFileFormatter(mode);
        }

        private void SetDeflateStreamFileFormatter(CompressionMode mode)
        {
            if (mode == CompressionMode.Compress)
                this.deflateStream.SetFileFormatWriter((IFileFormatWriter)new GZipFormatter());
            else
                this.deflateStream.SetFileFormatReader((IFileFormatReader)new GZipDecoder());
        }

        public override bool CanRead
        {
            get
            {
                if (this.deflateStream == null)
                    return false;
                return this.deflateStream.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (this.deflateStream == null)
                    return false;
                return this.deflateStream.CanWrite;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (this.deflateStream == null)
                    return false;
                return this.deflateStream.CanSeek;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException(SR.GetString("Not supported"));
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException(SR.GetString("Not supported"));
            }
            set
            {
                throw new NotSupportedException(SR.GetString("Not supported"));
            }
        }

        public override void Flush()
        {
            if (this.deflateStream == null)
                throw new ObjectDisposedException((string)null, SR.GetString("Object disposed"));
            this.deflateStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.GetString("Not supported"));
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.GetString("Not supported"));
        }

        public override IAsyncResult BeginRead(
          byte[] array,
          int offset,
          int count,
          AsyncCallback asyncCallback,
          object asyncState)
        {
            if (this.deflateStream == null)
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            return this.deflateStream.BeginRead(array, offset, count, asyncCallback, asyncState);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (this.deflateStream == null)
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            return this.deflateStream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(
          byte[] array,
          int offset,
          int count,
          AsyncCallback asyncCallback,
          object asyncState)
        {
            if (this.deflateStream == null)
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            return this.deflateStream.BeginWrite(array, offset, count, asyncCallback, asyncState);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (this.deflateStream == null)
                throw new InvalidOperationException(SR.GetString("Object disposed"));
            this.deflateStream.EndWrite(asyncResult);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (this.deflateStream == null)
                throw new ObjectDisposedException((string)null, SR.GetString("Object disposed"));
            return this.deflateStream.Read(array, offset, count);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (this.deflateStream == null)
                throw new ObjectDisposedException((string)null, SR.GetString("Object disposed"));
            this.deflateStream.Write(array, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.deflateStream != null)
                    this.deflateStream.Dispose();
                this.deflateStream = (DeflateStream)null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public Stream BaseStream
        {
            get
            {
                if (this.deflateStream != null)
                    return this.deflateStream.BaseStream;
                return (Stream)null;
            }
        }
    }
}
