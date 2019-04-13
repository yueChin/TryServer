

namespace ServerNetFrame
{
    /// <summary>
    /// 抽象的处理中心接口
    /// </summary>
    public abstract class AbsHandlerCenter
    {
        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="token"></param>
        public abstract void ClientConnect(UserToken token);
        /// <summary>
        /// 单条数据接收完成的回调
        /// </summary>
        /// <param name="token">客户端</param>
        /// <param name="message">报文</param>
        public abstract void MessageReceive(UserToken token, object message);
        /// <summary>
        /// 关闭客户端
        /// </summary>
        /// <param name="token">客户端</param>
        /// <param name="error">关闭原因</param>
        public abstract void ClientClose(UserToken token, string error);
    }
}
