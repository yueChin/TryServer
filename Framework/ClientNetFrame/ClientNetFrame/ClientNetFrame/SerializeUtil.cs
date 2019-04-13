// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.SerializeUtil
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using ClientNetFrame.IOCompression;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ClientNetFrame
{
    /// <summary>
    /// 序列化功能
    /// </summary>
    public class SerializeUtil
    {
        public static byte[] encodeObj(object value)
        {
            byte[] numArray;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize((Stream) memoryStream, value);
                numArray = new byte[memoryStream.Length];
                Buffer.BlockCopy((Array) memoryStream.GetBuffer(), 0, (Array) numArray, 0, (int) memoryStream.Length);
            }
            return numArray;
        }

        public static object decodeObj(byte[] value)
        {
            object obj = null;
            using (MemoryStream memoryStream = new MemoryStream(value))
            {
                obj = new BinaryFormatter().Deserialize((Stream)memoryStream);
            }
            return obj;
        }
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        private static void Compress(Stream source, Stream dest)
        {
            using (GZipStream gzipStream = new GZipStream(dest, CompressionMode.Compress, true))
            {
                source.Position = 0L;
                byte[] buffer = new byte[1024];
                int count;
                while ((count = source.Read(buffer, 0, 1024)) > 0)
                    gzipStream.Write(buffer, 0, count);
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] encode(object value)
        {
            byte[] array = null;
            using (MemoryStream memoryStream1 = new MemoryStream())
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
                //序列化流一
                new BinaryFormatter().Serialize((Stream)memoryStream1, value);
                //流一压缩到流二
                SerializeUtil.Compress((Stream)memoryStream1, (Stream)memoryStream2);
                array = memoryStream2.ToArray();
            }
            return array;
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        private static void Decompress(Stream source, Stream dest)
        {
            using (GZipStream gzipStream = new GZipStream(source, CompressionMode.Decompress, true))
            {
                int count1 = 1024;
                byte[] buffer = new byte[count1];
                int count2;
                while ((count2 = gzipStream.Read(buffer, 0, count1)) > 0)
                    dest.Write(buffer, 0, count2);
                dest.Position = 0L;
            }          
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object decode(byte[] value)
        {
            object obj = null;
            using (MemoryStream memoryStream1 = new MemoryStream(value, false))
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
                //流一解压缩到流二
                SerializeUtil.Decompress((Stream)memoryStream1, (Stream)memoryStream2);
                //流二序列化到obj
                obj = new BinaryFormatter().Deserialize((Stream)memoryStream2);
            }
            return obj;
        }

        /// <summary>
        /// 用protocolbuff来解码/反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object ProtoBufDecode<T>(byte[] bytes)
        {
            object obj = null;
            using (MemoryStream memoryStream1 = new MemoryStream(bytes, false))
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
                SerializeUtil.Decompress((Stream)memoryStream1, (Stream)memoryStream2);
                obj = Serializer.Deserialize<T>(memoryStream2);
            }
            return (T)obj;
        }

        /// <summary>
        /// 用protocolbuff来解码/反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object ProtoBufDecode(byte[] bytes, Type type)
        {
            object obj = null;
            using (MemoryStream memoryStream1 = new MemoryStream(bytes, false))
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
                SerializeUtil.Decompress((Stream)memoryStream1, (Stream)memoryStream2);
                obj = RuntimeTypeModel.Default.Deserialize(memoryStream2, null, type);
            }
            return obj;
        }

        /// <summary>
        /// 用protocolbuff来编码/序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ProteBufEncode(object obj)
        {
            byte[] bytes = null;
            using (MemoryStream memoryStream1 = new MemoryStream())
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
                RuntimeTypeModel.Default.Serialize(memoryStream1, obj);                             
                SerializeUtil.Compress((Stream)memoryStream1, (Stream)memoryStream2);
                memoryStream2.Position = 0;
                int lenth = (int)memoryStream2.Length;
                bytes = new byte[lenth];
                memoryStream2.Read(bytes, 0, lenth);
            }
            return bytes;
        }
    }
}
