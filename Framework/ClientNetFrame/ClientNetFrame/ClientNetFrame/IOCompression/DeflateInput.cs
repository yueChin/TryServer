// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.DeflateInput
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System.Diagnostics;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 解压缩输入
    /// </summary>
    internal class DeflateInput
    {
        private byte[] buffer;
        private int count;
        private int startIndex;

        internal byte[] Buffer
        {
            get
            {
                return this.buffer;
            }
            set
            {
                this.buffer = value;
            }
        }

        internal int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
            }
        }

        internal int StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                this.startIndex = value;
            }
        }

        internal void ConsumeBytes(int n)
        {
            Debug.Assert(n <= this.count, "Should use more bytes than what we have in the buffer");
            this.startIndex += n;
            this.count -= n;
            Debug.Assert(this.startIndex + this.count <= this.buffer.Length, "Input buffer is in invalid state!");
        }

        internal DeflateInput.InputState DumpState()
        {
            DeflateInput.InputState inputState;
            inputState.count = this.count;
            inputState.startIndex = this.startIndex;
            return inputState;
        }

        internal void RestoreState(DeflateInput.InputState state)
        {
            this.count = state.count;
            this.startIndex = state.startIndex;
        }

        internal struct InputState
        {
            internal int count;
            internal int startIndex;
        }
    }
}
