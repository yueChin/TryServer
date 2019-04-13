using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.Model.Alive;
using Server.Business;
using Server.Cache;
using ServerNetFrame;
using ServerNetFrame.Auto;

namespace Server.Logic.Alive
{
    public class AliveHandler : IHandler
    {
        /// <summary>
        /// 用户断开连接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public void ClientClose(UserToken token, string error)
        {
            BizProxy.Alive.TimeOut(token);
        }

        public void MessageReceive(UserToken token, SocketModel message)
        {
            switch (message.command)
            {
                case ALiveProtocol.Alive_Creq:
                    AliveModel model = BizProxy.Alive.TimeCheck(token,message.GetMessage<AliveModel>());
                    token.write(TypeProtocol.Alive,ALiveProtocol.Alive_SRes,model);
                    break;
            }
        }
    }
}
