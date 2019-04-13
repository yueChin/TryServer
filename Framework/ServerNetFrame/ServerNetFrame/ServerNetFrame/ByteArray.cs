using System;
using System.IO;

namespace ServerNetFrame
{
    public class ByteArray
    {
        /// <summary>
        /// 内存流
        /// </summary>
        private MemoryStream ms = new MemoryStream();
        /// <summary>
        /// 写入流
        /// </summary>
        private BinaryWriter bw;
        /// <summary>
        /// 读入流
        /// </summary>
        private BinaryReader br;

        public void Close()
        {
            this.bw.Close();
            this.br.Close();
            this.ms.Close();
        }

        public ByteArray()
        {
            this.bw = new BinaryWriter((Stream)this.ms);
            this.br = new BinaryReader((Stream)this.ms);
        }

        public ByteArray(byte[] buff)
        {
            this.ms = new MemoryStream(buff);
            this.bw = new BinaryWriter((Stream)this.ms);
            this.br = new BinaryReader((Stream)this.ms);
        }

        public int Position
        {
            get
            {
                return (int)this.ms.Position;
            }
        }

        public int Length
        {
            get
            {
                return (int)this.ms.Length;
            }
        }
        //是否还是可读的
        public bool Readnable
        {
            get
            {
                return this.ms.Length > this.ms.Position;
            }
        }
        //写入整数
        public void Write(int value)
        {
            this.bw.Write(value);
        }
        //写入单字节
        public void Write(byte value)
        {
            this.bw.Write(value);
        }
        //写入bool
        public void Write(bool value)
        {
            this.bw.Write(value);
        }
        //写入字节串
        public void Write(string value)
        {
            this.bw.Write(value);
        }
        //写入字节数组
        public void Write(byte[] value)
        {
            this.bw.Write(value);
        }
        //写入双浮点数
        public void Write(double value)
        {
            this.bw.Write(value);
        }
        //写入浮点数
        public void Write(float value)
        {
            this.bw.Write(value);
        }
        //写入长整数
        public void Write(long value)
        {
            this.bw.Write(value);
        }
        //读取整数
        public void Read(out int value)
        {
            value = this.br.ReadInt32();
        }
        //读取字节
        public void Read(out byte value)
        {
            value = this.br.ReadByte();
        }
        //读取布尔
        public void Read(out bool value)
        {
            value = this.br.ReadBoolean();
        }
        //读取字节串
        public void Read(out string value)
        {
            value = this.br.ReadString();
        }
        //读取一段字节数组
        public void Read(out byte[] value, int length)
        {
            value = this.br.ReadBytes(length);
        }
        //读取双浮点数
        public void Read(out double value)
        {
            value = this.br.ReadDouble();
        }
        //读取浮点数
        public void Read(out float value)
        {
            value = this.br.ReadSingle();
        }
        //读取长整数
        public void Read(out long value)
        {
            value = this.br.ReadInt64();
        }
        //重置字节串位置
        public void reposition()
        {
            this.ms.Position = 0L;
        }
        //获取字节数组
        public byte[] getBuff()
        {
            byte[] numArray = new byte[this.ms.Length];
            Buffer.BlockCopy((Array)this.ms.GetBuffer(), 0, (Array)numArray, 0, (int)this.ms.Length);
            return numArray;
        }
    }
}
