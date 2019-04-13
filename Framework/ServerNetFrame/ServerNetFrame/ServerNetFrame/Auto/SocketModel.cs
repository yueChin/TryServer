using System;

namespace ServerNetFrame.Auto
{
    /// <summary>
    /// 嵌套字模型，确定包头包尾
    /// </summary>
    public class SocketModel
    {
        public byte type { get; set; }

        public int command { get; set; }

        public object message { get; set; }

        public SocketModel()
        {
        }
        /// <summary>
        /// 目前报文所需的格式
        /// </summary>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <param name="o"></param>
        public SocketModel(byte t, int c, object o)
        {
            this.type = t;
            this.command = c;
            this.message = o;
        }
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <typeparam name="T">byte int object</typeparam>
        /// <returns></returns>
        public T GetMessage<T>()
        {
            try
            {
                return (T)this.message;
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)("错误请求:" + ex.ToString()));
                return default(T);
            }
        }
    }
}
