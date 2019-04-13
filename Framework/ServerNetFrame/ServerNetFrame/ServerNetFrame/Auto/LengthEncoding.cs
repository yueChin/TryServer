
using System;
using System.Collections.Generic;
using System.IO;

namespace ServerNetFrame.Auto
{
    /// <summary>
    /// 定长编码的实现
    /// </summary>
    public class LengthEncoding
    {
        public static byte[] Encode(byte[] buff)
        {
            byte[] numArray;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //把内存流置入二进制写入流
                BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
                //写入整数，有多个标识则需要写入多次,以下是例子
                //binaryWriter.Write(buff.bool);
                binaryWriter.Write(buff.Length);
                binaryWriter.Write(buff);
                //新建缓冲流
                numArray = new byte[memoryStream.Length];
                Buffer.BlockCopy((Array) memoryStream.GetBuffer(), 0, (Array) numArray, 0, (int) memoryStream.Length);
                //关闭流
                binaryWriter.Close();
            }
            return numArray;
        }

        public static byte[] Decode(ref List<byte> cache)
        {
            //根据目前报文的格式来调整数组
            if (cache.Count < 3)
                return (byte[])null;
            byte[] numArray;
            using (MemoryStream memoryStream = new MemoryStream(cache.ToArray()))
            {
                //把内存流置入二进制读取流
                BinaryReader binaryReader = new BinaryReader((Stream)memoryStream);
                //读取整数，有多个标识则需要读取多次,读取顺序和写入顺序相同
                //bool exp = binaryReader.ReadBoolean();
                //读取后会把位置往后推
                int count = binaryReader.ReadInt32();
                //如果读出来的长度（目前是这个）大于剩下的长度，则返回null
                if ((long)count > memoryStream.Length - memoryStream.Position)
                    return (byte[])null;
                //从二进制流中 读取字节数组
                numArray = binaryReader.ReadBytes(count);
                //完成后则清空缓存
                cache.Clear();
                cache.AddRange((IEnumerable<byte>)binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position)));
                binaryReader.Close();
            }
            return numArray;
        }
    }
}

