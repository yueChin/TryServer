// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.CopyEncoder
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Diagnostics;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 复制
    /// </summary>
    internal class CopyEncoder
    {
        private const int PaddingSize = 5;
        private const int MaxUncompressedBlockSize = 65536;

        public void GetBlock(DeflateInput input, OutputBuffer output, bool isFinal)
        {
            Debug.Assert(output != null);
            Debug.Assert(output.FreeBytes >= 5);
            int num = 0;
            if (input != null)
            {
                num = Math.Min(input.Count, output.FreeBytes - 5 - output.BitsInBuffer);
                if (num > 65531)
                    num = 65531;
            }
            if (isFinal)
                output.WriteBits(3, 1U);
            else
                output.WriteBits(3, 0U);
            output.FlushBits();
            this.WriteLenNLen((ushort)num, output);
            if (input == null || num <= 0)
                return;
            output.WriteBytes(input.Buffer, input.StartIndex, num);
            input.ConsumeBytes(num);
        }

        private void WriteLenNLen(ushort len, OutputBuffer output)
        {
            output.WriteUInt16(len);
            ushort num = (ushort)~len;
            output.WriteUInt16(num);
        }
    }
}
