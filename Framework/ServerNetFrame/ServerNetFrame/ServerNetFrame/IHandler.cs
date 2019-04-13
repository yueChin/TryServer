using ServerNetFrame.Auto;

namespace ServerNetFrame
{
    /// <summary>
    /// 二级处理接口
    /// </summary>
    public interface IHandler
    {
        void ClientClose(UserToken token, string error);

        void MessageReceive(UserToken token, SocketModel message);
    }
}
