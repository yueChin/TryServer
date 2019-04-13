// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.OutputBuffer
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Diagnostics;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 输出缓存
    /// </summary>
    internal class OutputBuffer
    {
        private byte[] byteBuffer;
        private int pos;
        private uint bitBuf;
        private int bitCount;

        internal void UpdateBuffer(byte[] output)
        {
            this.byteBuffer = output;
            this.pos = 0;
        }

        internal int BytesWritten
        {
            get
            {
                return this.pos;
            }
        }

        internal int FreeBytes
        {
            get
            {
                return this.byteBuffer.Length - this.pos;
            }
        }
        /// <summary>
        /// 写16位整数
        /// </summary>
        /// <param name="value"></param>
        internal void WriteUInt16(ushort value)
        {
            Debug.Assert(this.FreeBytes >= 2, "No enough space in output buffer!");
            this.byteBuffer[this.pos++] = (byte)value;
            this.byteBuffer[this.pos++] = (byte)((uint)value >> 8);
        }
        /// <summary>
        /// 写字节数
        /// </summary>
        /// <param name="n"></param>
        /// <param name="bits"></param>
        internal void WriteBits(int n, uint bits)
        {
            Debug.Assert(n <= 16, "length must be larger than 16!");
            this.bitBuf |= bits << this.bitCount;
            this.bitCount += n;
            if (this.bitCount < 16)
                return;
            Debug.Assert(this.byteBuffer.Length - this.pos >= 2, "No enough space in output buffer!");
            this.byteBuffer[this.pos++] = (byte)this.bitBuf;
            this.byteBuffer[this.pos++] = (byte)(this.bitBuf >> 8);
            this.bitCount -= 16;
            this.bitBuf >>= 16;
        }
        /// <summary>
        /// 刷掉bit
        /// </summary>
        internal void FlushBits()
        {
            while (this.bitCount >= 8)
            {
                this.byteBuffer[this.pos++] = (byte)this.bitBuf;
                this.bitCount -= 8;
                this.bitBuf >>= 8;
            }
            if (this.bitCount <= 0)
                return;
            this.byteBuffer[this.pos++] = (byte)this.bitBuf;
            this.bitBuf = 0U;
            this.bitCount = 0;
        }
        /// <summary>
        /// 写字节数组
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        internal void WriteBytes(byte[] byteArray, int offset, int count)
        {
            Debug.Assert(this.FreeBytes >= count, "Not enough space in output buffer!");
            if (this.bitCount == 0)
            {
                Array.Copy((Array)byteArray, offset, (Array)this.byteBuffer, this.pos, count);
                this.pos += count;
            }
            else
                this.WriteBytesUnaligned(byteArray, offset, count);
        }
        /// <summary>
        /// 字节数组写到无符号整数
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        private void WriteBytesUnaligned(byte[] byteArray, int offset, int count)
        {
            for (int index = 0; index < count; ++index)
                this.WriteByteUnaligned(byteArray[offset + index]);
        }
        /// <summary>
        /// 字节写到无符号
        /// </summary>
        /// <param name="b"></param>
        private void WriteByteUnaligned(byte b)
        {
            this.WriteBits(8, (uint)b);
        }

        internal int BitsInBuffer
        {
            get
            {
                return this.bitCount / 8 + 1;
            }
        }
        /// <summary>
        /// 设置buffstate状态
        /// </summary>
        /// <returns></returns>
        internal OutputBuffer.BufferState DumpState()
        {
            OutputBuffer.BufferState bufferState;
            bufferState.pos = this.pos;
            bufferState.bitBuf = this.bitBuf;
            bufferState.bitCount = this.bitCount;
            return bufferState;
        }
        /// <summary>
        /// buffstate的状态重置
        /// </summary>
        /// <param name="state"></param>
        internal void RestoreState(OutputBuffer.BufferState state)
        {
            this.pos = state.pos;
            this.bitBuf = state.bitBuf;
            this.bitCount = state.bitCount;
        }

        internal struct BufferState
        {
            internal int pos;
            internal uint bitBuf;
            internal int bitCount;
        }
    }
}
