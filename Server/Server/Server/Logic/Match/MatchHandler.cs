using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.Model.Login;
using GameProtocol.Model.Match;
using Server.Business;
using Server.Cache;
using ServerNetFrame;
using ServerNetFrame.Auto;

namespace Server.Logic.Match
{
    public class MatchHandler : IHandler
    {
        public void ClientClose(UserToken token, string error)
        {
            BizProxy.Match.LevelMatch(token);
        }

        public void MessageReceive(UserToken token, SocketModel message)
        {
            if(!CacheProxy.User.IsOnline(token)) return;
            
            switch (message.command)
            {
                //请求开始匹配
                case MatchProtocol.StartMatch_CReq:
                {
                    ResponseStartMatchModel result = BizProxy.Match.StartMatch(token, (SeverConst.GameType)message.GetMessage<int>());
                    token.write(TypeProtocol.Match, MatchProtocol.StartMatch_SRes, result);
                }
                    break;
                //请求离开匹配
                case MatchProtocol.StartMatch_SRes:
                {
                    int result = BizProxy.Match.LevelMatch(token);
                    token.write(TypeProtocol.Match, MatchProtocol.LevelMatch_SRes, result);
                }
                    break;
                    ;
            }
        }

        void LeaveMatch(int id)
        {

        }
    }
}
