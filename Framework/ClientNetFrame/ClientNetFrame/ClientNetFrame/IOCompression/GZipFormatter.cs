// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.GZipFormatter
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 压缩格式
    /// </summary>
    internal class GZipFormatter : IFileFormatWriter
    {
        private byte[] headerBytes = new byte[10]
        {
            (byte) 31,
            (byte) 139,
            (byte) 8,
            (byte) 0,
            (byte) 0,
            (byte) 0,
            (byte) 0,
            (byte) 0,
            (byte) 4,
            (byte) 0
        };
        private uint _crc32;
        private long _inputStreamSizeModulo;

        internal GZipFormatter()
            : this(3)
        {
        }

        internal GZipFormatter(int compressionLevel)
        {
            if (compressionLevel != 10)
                return;
            this.headerBytes[8] = (byte)2;
        }

        public byte[] GetHeader()
        {
            return this.headerBytes;
        }

        public void UpdateWithBytesRead(byte[] buffer, int offset, int bytesToCopy)
        {
            this._crc32 = Crc32Helper.UpdateCrc32(this._crc32, buffer, offset, bytesToCopy);
            long num = this._inputStreamSizeModulo + (long)(uint)bytesToCopy;
            if (num >= 4294967296L)
                num %= 4294967296L;
            this._inputStreamSizeModulo = num;
        }

        public byte[] GetFooter()
        {
            byte[] b = new byte[8];
            this.WriteUInt32(b, this._crc32, 0);
            this.WriteUInt32(b, (uint)this._inputStreamSizeModulo, 4);
            return b;
        }

        internal void WriteUInt32(byte[] b, uint value, int startIndex)
        {
            b[startIndex] = (byte)value;
            b[startIndex + 1] = (byte)(value >> 8);
            b[startIndex + 2] = (byte)(value >> 16);
            b[startIndex + 3] = (byte)(value >> 24);
        }
    }
}
