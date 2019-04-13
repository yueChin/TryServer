// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.InputBuffer
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Diagnostics;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 输入缓存
    /// </summary>
    internal class InputBuffer
    {
        private uint bitBuffer = 0;
        private int bitsInBuffer = 0;
        private byte[] buffer;
        private int start;
        private int end;

        public int AvailableBits
        {
            get
            {
                return this.bitsInBuffer;
            }
        }

        public int AvailableBytes
        {
            get
            {
                return this.end - this.start + this.bitsInBuffer / 8;
            }
        }

        public bool EnsureBitsAvailable(int count)
        {
            Debug.Assert(0 < count && count <= 16, "count is invalid.");
            if (this.bitsInBuffer < count)
            {
                if (this.NeedsInput())
                    return false;
                this.bitBuffer |= (uint)this.buffer[this.start++] << this.bitsInBuffer;
                this.bitsInBuffer += 8;
                if (this.bitsInBuffer < count)
                {
                    if (this.NeedsInput())
                        return false;
                    this.bitBuffer |= (uint)this.buffer[this.start++] << this.bitsInBuffer;
                    this.bitsInBuffer += 8;
                }
            }
            return true;
        }

        public uint TryLoad16Bits()
        {
            if (this.bitsInBuffer < 8)
            {
                if (this.start < this.end)
                {
                    this.bitBuffer |= (uint)this.buffer[this.start++] << this.bitsInBuffer;
                    this.bitsInBuffer += 8;
                }
                if (this.start < this.end)
                {
                    this.bitBuffer |= (uint)this.buffer[this.start++] << this.bitsInBuffer;
                    this.bitsInBuffer += 8;
                }
            }
            else if (this.bitsInBuffer < 16 && this.start < this.end)
            {
                this.bitBuffer |= (uint)this.buffer[this.start++] << this.bitsInBuffer;
                this.bitsInBuffer += 8;
            }
            return this.bitBuffer;
        }

        private uint GetBitMask(int count)
        {
            return (uint)((1 << count) - 1);
        }

        public int GetBits(int count)
        {
            Debug.Assert(0 < count && count <= 16, "count is invalid.");
            if (!this.EnsureBitsAvailable(count))
                return -1;
            int num = (int)this.bitBuffer & (int)this.GetBitMask(count);
            this.bitBuffer >>= count;
            this.bitsInBuffer -= count;
            return num;
        }

        public int CopyTo(byte[] output, int offset, int length)
        {
            Debug.Assert(output != null, "");
            Debug.Assert(offset >= 0, "");
            Debug.Assert(length >= 0, "");
            Debug.Assert(offset <= output.Length - length, "");
            Debug.Assert(this.bitsInBuffer % 8 == 0, "");
            int num1 = 0;
            while (this.bitsInBuffer > 0 && length > 0)
            {
                output[offset++] = (byte)this.bitBuffer;
                this.bitBuffer >>= 8;
                this.bitsInBuffer -= 8;
                --length;
                ++num1;
            }
            if (length == 0)
                return num1;
            int num2 = this.end - this.start;
            if (length > num2)
                length = num2;
            Array.Copy((Array)this.buffer, this.start, (Array)output, offset, length);
            this.start += length;
            return num1 + length;
        }

        public bool NeedsInput()
        {
            return this.start == this.end;
        }

        public void SetInput(byte[] buffer, int offset, int length)
        {
            Debug.Assert(buffer != null, "");
            Debug.Assert(offset >= 0, "");
            Debug.Assert(length >= 0, "");
            Debug.Assert(offset <= buffer.Length - length, "");
            Debug.Assert(this.start == this.end, "");
            this.buffer = buffer;
            this.start = offset;
            this.end = offset + length;
        }

        public void SkipBits(int n)
        {
            Debug.Assert(this.bitsInBuffer >= n, "No enough bits in the buffer, Did you call EnsureBitsAvailable?");
            this.bitBuffer >>= n;
            this.bitsInBuffer -= n;
        }

        public void SkipToByteBoundary()
        {
            this.bitBuffer >>= this.bitsInBuffer % 8;
            this.bitsInBuffer -= this.bitsInBuffer % 8;
        }
    }
}
