
namespace ServerNetFrame.Auto
{
    /// <summary>
    /// 消息编码
    /// </summary>
    public class MessageEncoding
    {
        /// <summary>
        /// 报文编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Encode(object value)
        {
            SocketModel socketModel = value as SocketModel;
            ByteArray byteArray = new ByteArray();
            byteArray.Write(socketModel.type);
            byteArray.Write(socketModel.command);
            //如果socketMsg 不为null，把它序列化后写入字节数组中
            if (socketModel.message != null)
                //byteArray.Write(SerializeUtil.Encode(socketModel.message));
                byteArray.Write(SerializeUtil.ProteBufEncode(socketModel.message));
            byte[] buff = byteArray.getBuff();
            byteArray.Close();
            return buff;
        }
        /// <summary>
        /// 报文解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Decode(byte[] value)
        {
            //从缓存中提取完整的包
            ByteArray byteArray = new ByteArray(value);
            SocketModel socketModel = new SocketModel();
            //拆包1
            byte num1;
            byteArray.Read(out num1);
            //拆包2
            int num2;
            byteArray.Read(out num2);
            //设置socket模型
            socketModel.type = num1;
            socketModel.command = num2;
            //判断字节数组还不可以读，如果不行，就关闭，并返回socket模型
            if (byteArray.Readnable)
            {
                byte[] numArray;
                //把余下的字符数组读出来
                byteArray.Read(out numArray, byteArray.Length - byteArray.Position);
                //通过序列化功能把socket报文解码出来，并设置到socket模型中
                //socketModel.message = SerializeUtil.Decode(numArray);
                socketModel.message = SerializeUtil.ProtoBufDecode<SocketModel>(numArray);
            }
            byteArray.Close();
            return (object)socketModel;
        }
    }
}
