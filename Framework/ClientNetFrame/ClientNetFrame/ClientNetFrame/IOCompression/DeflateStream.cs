// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.DeflateStream
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 压缩流
    /// </summary>
    public class DeflateStream : Stream
    {
        internal const int DefaultBufferSize = 8192;
        private Stream _stream;
        private CompressionMode _mode;
        private bool _leaveOpen;
        private Inflater inflater;
        private IDeflater deflater;
        private byte[] buffer;
        private int asyncOperations;
        private readonly AsyncCallback m_CallBack;
        private readonly DeflateStream.AsyncWriteDelegate m_AsyncWriterDelegate;
        private IFileFormatWriter formatWriter;
        private bool wroteHeader;
        private bool wroteBytes;

        public DeflateStream(Stream stream, CompressionMode mode)
          : this(stream, mode, false)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (CompressionMode.Compress != mode && (uint)mode > 0U)
                throw new ArgumentException(SR.GetString("Argument out of range"), nameof(mode));
            this._stream = stream;
            this._mode = mode;
            this._leaveOpen = leaveOpen;
            switch (this._mode)
            {
                case CompressionMode.Decompress:
                    if (!this._stream.CanRead)
                        throw new ArgumentException(SR.GetString("Not a readable stream"), nameof(stream));
                    this.inflater = new Inflater();
                    this.m_CallBack = new AsyncCallback(this.ReadCallback);
                    break;
                case CompressionMode.Compress:
                    if (!this._stream.CanWrite)
                        throw new ArgumentException(SR.GetString("Not a writeable stream"), nameof(stream));
                    this.deflater = DeflateStream.CreateDeflater();
                    this.m_AsyncWriterDelegate = new DeflateStream.AsyncWriteDelegate(this.InternalWrite);
                    this.m_CallBack = new AsyncCallback(this.WriteCallback);
                    break;
            }
            this.buffer = new byte[8192];
        }

        private static IDeflater CreateDeflater()
        {
            if (DeflateStream.GetDeflaterType() == DeflateStream.WorkerType.Managed)
                return (IDeflater)new DeflaterManaged();
            throw new SystemException("Program entered an unexpected state.");
        }

        [SecuritySafeCritical]
        private static DeflateStream.WorkerType GetDeflaterType()
        {
            return DeflateStream.WorkerType.Managed;
        }

        internal void SetFileFormatReader(IFileFormatReader reader)
        {
            if (reader == null)
                return;
            this.inflater.SetFileFormatReader(reader);
        }

        internal void SetFileFormatWriter(IFileFormatWriter writer)
        {
            if (writer == null)
                return;
            this.formatWriter = writer;
        }

        public Stream BaseStream
        {
            get
            {
                return this._stream;
            }
        }

        public override bool CanRead
        {
            get
            {
                if (this._stream == null)
                    return false;
                return this._mode == CompressionMode.Decompress && this._stream.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (this._stream == null)
                    return false;
                return this._mode == CompressionMode.Compress && this._stream.CanWrite;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
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
            this.EnsureNotDisposed();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.GetString("Not supported"));
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.GetString("Not supported"));
        }

        public override int Read(byte[] array, int offset, int count)
        {
            this.EnsureDecompressionMode();
            this.ValidateParameters(array, offset, count);
            this.EnsureNotDisposed();
            int offset1 = offset;
            int length1 = count;
            while (true)
            {
                int num = this.inflater.Inflate(array, offset1, length1);
                offset1 += num;
                length1 -= num;
                if (length1 != 0)
                {
                    if (!this.inflater.Finished())
                    {
                        Debug.Assert(this.inflater.NeedsInput(), "We can only run into this case if we are short of input");
                        int length2 = this._stream.Read(this.buffer, 0, this.buffer.Length);
                        if (length2 != 0)
                            this.inflater.SetInput(this.buffer, 0, length2);
                        else
                            goto label_6;
                    }
                    else
                        break;
                }
                else
                    goto label_6;
            }
            Debug.Assert(this.inflater.AvailableOutput == 0, "We should have copied all stuff out!");
            label_6:
            return count - length1;
        }

        private void ValidateParameters(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (array.Length - offset < count)
                throw new ArgumentException(SR.GetString("Invalid argument offset count"));
        }

        private void EnsureNotDisposed()
        {
            if (this._stream == null)
                throw new ObjectDisposedException((string)null, SR.GetString("Object disposed"));
        }

        private void EnsureDecompressionMode()
        {
            if ((uint)this._mode > 0U)
                throw new InvalidOperationException(SR.GetString("Cannot read from deflate stream"));
        }

        private void EnsureCompressionMode()
        {
            if (this._mode != CompressionMode.Compress)
                throw new InvalidOperationException(SR.GetString("Cannot write to deflate stream"));
        }

        public override IAsyncResult BeginRead(
          byte[] array,
          int offset,
          int count,
          AsyncCallback asyncCallback,
          object asyncState)
        {
            this.EnsureDecompressionMode();
            if ((uint)this.asyncOperations > 0U)
                throw new InvalidOperationException(SR.GetString("Invalid begin call"));
            this.ValidateParameters(array, offset, count);
            this.EnsureNotDisposed();
            Interlocked.Increment(ref this.asyncOperations);
            try
            {
                DeflateStreamAsyncResult streamAsyncResult = new DeflateStreamAsyncResult((object)this, asyncState, asyncCallback, array, offset, count);
                streamAsyncResult.isWrite = false;
                int num = this.inflater.Inflate(array, offset, count);
                if ((uint)num > 0U)
                {
                    streamAsyncResult.InvokeCallback(true, (object)num);
                    return (IAsyncResult)streamAsyncResult;
                }
                if (this.inflater.Finished())
                {
                    streamAsyncResult.InvokeCallback(true, (object)0);
                    return (IAsyncResult)streamAsyncResult;
                }
                this._stream.BeginRead(this.buffer, 0, this.buffer.Length, this.m_CallBack, (object)streamAsyncResult);
                streamAsyncResult.m_CompletedSynchronously &= streamAsyncResult.IsCompleted;
                return (IAsyncResult)streamAsyncResult;
            }
            catch
            {
                Interlocked.Decrement(ref this.asyncOperations);
                throw;
            }
        }

        private void ReadCallback(IAsyncResult baseStreamResult)
        {
            DeflateStreamAsyncResult asyncState = (DeflateStreamAsyncResult)baseStreamResult.AsyncState;
            asyncState.m_CompletedSynchronously &= baseStreamResult.CompletedSynchronously;
            try
            {
                this.EnsureNotDisposed();
                int length = this._stream.EndRead(baseStreamResult);
                if (length <= 0)
                {
                    asyncState.InvokeCallback((object)0);
                }
                else
                {
                    this.inflater.SetInput(this.buffer, 0, length);
                    int num = this.inflater.Inflate(asyncState.buffer, asyncState.offset, asyncState.count);
                    if (num == 0 && !this.inflater.Finished())
                        this._stream.BeginRead(this.buffer, 0, this.buffer.Length, this.m_CallBack, (object)asyncState);
                    else
                        asyncState.InvokeCallback((object)num);
                }
            }
            catch (Exception ex)
            {
                asyncState.InvokeCallback((object)ex);
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            this.EnsureDecompressionMode();
            this.CheckEndXxxxLegalStateAndParams(asyncResult);
            DeflateStreamAsyncResult asyncResult1 = (DeflateStreamAsyncResult)asyncResult;
            this.AwaitAsyncResultCompletion(asyncResult1);
            Exception result = asyncResult1.Result as Exception;
            if (result != null)
                throw result;
            return (int)asyncResult1.Result;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            this.EnsureCompressionMode();
            this.ValidateParameters(array, offset, count);
            this.EnsureNotDisposed();
            this.InternalWrite(array, offset, count, false);
        }

        internal void InternalWrite(byte[] array, int offset, int count, bool isAsync)
        {
            this.DoMaintenance(array, offset, count);
            this.WriteDeflaterOutput(isAsync);
            this.deflater.SetInput(array, offset, count);
            this.WriteDeflaterOutput(isAsync);
        }

        private void WriteDeflaterOutput(bool isAsync)
        {
            while (!this.deflater.NeedsInput())
            {
                int deflateOutput = this.deflater.GetDeflateOutput(this.buffer);
                if (deflateOutput > 0)
                    this.DoWrite(this.buffer, 0, deflateOutput, isAsync);
            }
        }

        private void DoWrite(byte[] array, int offset, int count, bool isAsync)
        {
            Debug.Assert(array != null);
            Debug.Assert((uint)count > 0U);
            if (isAsync)
                this._stream.EndWrite(this._stream.BeginWrite(array, offset, count, (AsyncCallback)null, (object)null));
            else
                this._stream.Write(array, offset, count);
        }

        private void DoMaintenance(byte[] array, int offset, int count)
        {
            if (count <= 0)
                return;
            this.wroteBytes = true;
            if (this.formatWriter == null)
                return;
            if (!this.wroteHeader)
            {
                byte[] header = this.formatWriter.GetHeader();
                this._stream.Write(header, 0, header.Length);
                this.wroteHeader = true;
            }
            this.formatWriter.UpdateWithBytesRead(array, offset, count);
        }

        private void PurgeBuffers(bool disposing)
        {
            if (!disposing || this._stream == null)
                return;
            this.Flush();
            if (this._mode != CompressionMode.Compress)
                return;
            if (this.wroteBytes)
            {
                this.WriteDeflaterOutput(false);
                bool flag;
                do
                {
                    int bytesRead;
                    flag = this.deflater.Finish(this.buffer, out bytesRead);
                    if (bytesRead > 0)
                        this.DoWrite(this.buffer, 0, bytesRead, false);
                }
                while (!flag);
            }
            if (this.formatWriter == null || !this.wroteHeader)
                return;
            byte[] footer = this.formatWriter.GetFooter();
            this._stream.Write(footer, 0, footer.Length);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.PurgeBuffers(disposing);
            }
            finally
            {
                try
                {
                    if (disposing && !this._leaveOpen && this._stream != null)
                        this._stream.Dispose();
                }
                finally
                {
                    this._stream = (Stream)null;
                    try
                    {
                        if (this.deflater != null)
                            this.deflater.Dispose();
                    }
                    finally
                    {
                        this.deflater = (IDeflater)null;
                        base.Dispose(disposing);
                    }
                }
            }
        }

        public override IAsyncResult BeginWrite(
          byte[] array,
          int offset,
          int count,
          AsyncCallback asyncCallback,
          object asyncState)
        {
            this.EnsureCompressionMode();
            if ((uint)this.asyncOperations > 0U)
                throw new InvalidOperationException(SR.GetString("Invalid begin call"));
            this.ValidateParameters(array, offset, count);
            this.EnsureNotDisposed();
            Interlocked.Increment(ref this.asyncOperations);
            try
            {
                DeflateStreamAsyncResult streamAsyncResult = new DeflateStreamAsyncResult((object)this, asyncState, asyncCallback, array, offset, count);
                streamAsyncResult.isWrite = true;
                this.m_AsyncWriterDelegate.BeginInvoke(array, offset, count, true, this.m_CallBack, (object)streamAsyncResult);
                streamAsyncResult.m_CompletedSynchronously &= streamAsyncResult.IsCompleted;
                return (IAsyncResult)streamAsyncResult;
            }
            catch
            {
                Interlocked.Decrement(ref this.asyncOperations);
                throw;
            }
        }

        private void WriteCallback(IAsyncResult asyncResult)
        {
            DeflateStreamAsyncResult asyncState = (DeflateStreamAsyncResult)asyncResult.AsyncState;
            asyncState.m_CompletedSynchronously &= asyncResult.CompletedSynchronously;
            try
            {
                this.m_AsyncWriterDelegate.EndInvoke(asyncResult);
            }
            catch (Exception ex)
            {
                asyncState.InvokeCallback((object)ex);
                return;
            }
            asyncState.InvokeCallback((object)null);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.EnsureCompressionMode();
            this.CheckEndXxxxLegalStateAndParams(asyncResult);
            DeflateStreamAsyncResult asyncResult1 = (DeflateStreamAsyncResult)asyncResult;
            this.AwaitAsyncResultCompletion(asyncResult1);
            Exception result = asyncResult1.Result as Exception;
            if (result != null)
                throw result;
        }

        private void CheckEndXxxxLegalStateAndParams(IAsyncResult asyncResult)
        {
            if (this.asyncOperations != 1)
                throw new InvalidOperationException(SR.GetString("Invalid end call"));
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));
            this.EnsureNotDisposed();
            if (!(asyncResult is DeflateStreamAsyncResult))
                throw new ArgumentNullException(nameof(asyncResult));
        }

        private void AwaitAsyncResultCompletion(DeflateStreamAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.IsCompleted)
                    return;
                asyncResult.AsyncWaitHandle.WaitOne();
            }
            finally
            {
                Interlocked.Decrement(ref this.asyncOperations);
                asyncResult.Close();
            }
        }

        internal delegate void AsyncWriteDelegate(byte[] array, int offset, int count, bool isAsync);

        private enum WorkerType : byte
        {
            Managed,
            Unknown,
        }
    }
}
