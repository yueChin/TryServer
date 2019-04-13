using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ServerNetFrame
{
    /// <summary>
    /// 序列化
    /// </summary>
    public class SerializeUtil
    {
        /// <summary>
        /// 压缩流
        /// </summary>
        /// <param name="source">源流</param>
        /// <param name="dest">目标流</param>
        private static void Compress(Stream source, Stream dest)
        {
            try
            {
                //把dest
                using (GZipStream gzipStream = new GZipStream(dest, CompressionMode.Compress, true))
                {
                    source.Position = 0L;
                    int length = (int)source.Length;
                    byte[] buffer = new byte[length];
                    int count;
                    //直到字节数组全读取源流完后，写入压缩流
                    while ((count = source.Read(buffer, 0, length)) > 0)
                        //写入压缩流
                        gzipStream.Write(buffer, 0, count);
                    gzipStream.Close();
                }
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.ToString());
            }
        }
        /// <summary>
        /// 编码流，序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Encode(object value)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms,value);
                    byte[] array = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(),0,array,0,(int)ms.Length);
                    return array;
                };
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)ex.ToString());
                return (byte[])null;
            }
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        private static void Decompress(Stream source, Stream dest)
        {
            try
            {
                using (GZipStream gzipStream = new GZipStream(source, CompressionMode.Decompress, true))
                {
                    int length = (int)source.Length;
                    //创建长度为源流的 数组
                    byte[] buffer = new byte[length];
                    int count;
                    //如果流还未读到末尾，就继续读取，直到把解压缩后的源流全写到目标流为止
                    while ((count = gzipStream.Read(buffer, 0, length)) > 0)
                        //把解压缩后的字节数组写入目标流
                        dest.Write(buffer, 0, count);
                    dest.Position = 0L;
                    gzipStream.Close();
                }
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.ToString());
            }
        }
        /// <summary>
        /// 解码流,反序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Decode(byte[] value)
        {
            try
            {
                object obj;              
                using (MemoryStream ms1 = new MemoryStream(value, false))
                using (MemoryStream ms2 = new MemoryStream())
                {
                    ////新建二进制格式
                    //BinaryFormatter binaryFormatter = new BinaryFormatter();
                    ms1.Position = 0L;
                    //把流一解压缩到流二
                    SerializeUtil.Decompress((Stream)ms1, (Stream)ms2);
                    //对内存流二反序列化到二进制格式
                    obj = new BinaryFormatter().Deserialize((Stream)ms2);
                }
                return obj;
            }
            catch (FileLoadException ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)ex.ToString());
                return (object)null;
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)ex.ToString());
                return (object)null;
            }
        }

        public static byte[] EncodeObj(object value)
        {
            try
            {
                byte[] numArray;
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize((Stream)ms, value);
                    numArray = new byte[ms.Length];
                    Buffer.BlockCopy((Array)ms.GetBuffer(), 0, (Array)numArray, 0, (int)ms.Length);
                }
                return numArray;
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)ex.ToString());
                return (byte[])null;
            }
        }

        public static object DecodeObj(byte[] value)
        {
            object obj;
            try
            {               
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    ms.Position = 0L;
                    obj = binaryFormatter.Deserialize((Stream)ms);
                    ms.Close();
                }                
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)ex.ToString());
                return (object)null;
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
            try
            {              
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    obj = Serializer.Deserialize<T>(ms);
                }
                
            }
            catch (Exception e)
            {
                if (DebugMessage.Error == null)
                    return null;
                DebugMessage.Error((object)e.Message);
            }
            return (T)obj;
        }

        /// <summary>
        /// 用protocolbuff来解码/反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object ProtoBufDecode(byte[] bytes,Type type)
        {
            object obj = null;
            try
            {             
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    obj = RuntimeTypeModel.Default.Deserialize(ms, null, type);
                }
                
            }
            catch (Exception e)
            {
                if (DebugMessage.Error == null)
                    return null;
                DebugMessage.Error((object)e.Message);
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
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (obj != null)
                    {
                        RuntimeTypeModel.Default.Serialize(ms, obj);
                    }
                    ms.Position = 0;
                    int lenth = (int)ms.Length;
                    bytes = new byte[lenth];
                    ms.Read(bytes, 0, lenth);
                }
            }
            catch (Exception e)
            {
                if (DebugMessage.Error == null)
                    return null;
                DebugMessage.Error((object)e.Message);
            }            
            return bytes;
        }
    }
}
