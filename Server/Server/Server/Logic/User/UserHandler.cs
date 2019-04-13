using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using Server.Business;
using Server.Model.User;
using ServerNetFrame;
using ServerNetFrame.Auto;
using ServerTools.Debug;

namespace Server.Logic.User
{
    public class UserHandler : IHandler
    {
        /// <summary>
        /// 客户端关闭连接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public void ClientClose(UserToken token, string error)
        {
            BizProxy.Login.LogOut(token);
        }

        /// <summary>
        /// 客户端消息到达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public void MessageReceive(UserToken token, SocketModel message)
        {
            switch (message.command)
            {
                case UserProtocol.GetInfo_CReq:
                    UserModel model = BizProxy.User.GetUserModel(token);
                    if (model != null)
                    {
                        token.write(TypeProtocol.User,UserProtocol.GetInfo_SRes,model);
                        DebugUtil.Instance.Log2Time(model.Id +"获取用户信息",logType.Debug);
                    }
                    else
                    {
                        DebugUtil.Instance.Log2Time(token.conn.RemoteEndPoint + "获取用户信息", logType.Debug);
                    }
                    break;
            }
        }
    }
}
