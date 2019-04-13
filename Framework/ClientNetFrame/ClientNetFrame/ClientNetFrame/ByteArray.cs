// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.ByteArray
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.IO;

namespace ClientNetFrame
{
    public class ByteArray
    {
        private MemoryStream ms = new MemoryStream();
        private BinaryWriter bw;
        private BinaryReader br;

        public void Close()
        {
            this.bw.Close();
            this.br.Close();
            this.ms.Close();
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

        public bool Readnable
        {
            get
            {
                return this.ms.Length > this.ms.Position;
            }
        }

        public ByteArray()
        {
            this.bw = new BinaryWriter((Stream)this.ms);
            this.br = new BinaryReader((Stream)this.ms);
        }

        public void write(int value)
        {
            this.bw.Write(value);
        }

        public void write(byte value)
        {
            this.bw.Write(value);
        }

        public void write(bool value)
        {
            this.bw.Write(value);
        }

        public void write(string value)
        {
            this.bw.Write(value);
        }

        public void write(byte[] value)
        {
            this.bw.Write(value);
        }

        public void write(double value)
        {
            this.bw.Write(value);
        }

        public void write(float value)
        {
            this.bw.Write(value);
        }

        public void write(long value)
        {
            this.bw.Write(value);
        }

        public void read(out int value)
        {
            value = this.br.ReadInt32();
        }

        public void read(out byte value)
        {
            value = this.br.ReadByte();
        }

        public void read(out bool value)
        {
            value = this.br.ReadBoolean();
        }

        public void read(out string value)
        {
            value = this.br.ReadString();
        }

        public void read(out byte[] value, int length)
        {
            value = this.br.ReadBytes(length);
        }

        public void read(out double value)
        {
            value = this.br.ReadDouble();
        }

        public void read(out float value)
        {
            value = this.br.ReadSingle();
        }

        public void read(out long value)
        {
            value = this.br.ReadInt64();
        }

        public void reposition()
        {
            this.ms.Position = 0L;
        }

        public byte[] getBuff()
        {
            byte[] numArray = new byte[this.ms.Length];
            Buffer.BlockCopy((Array)this.ms.GetBuffer(), 0, (Array)numArray, 0, (int)this.ms.Length);
            return numArray;
        }
    }
}
