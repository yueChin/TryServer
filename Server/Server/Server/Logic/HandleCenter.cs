using System;
using Server.Logic.Login;
using GameProtocol;
using Server.Logic.Alive;
using Server.Logic.Fight;
using Server.Logic.Match;
using Server.Logic.User;
using ServerNetFrame;
using ServerNetFrame.Auto;

namespace Server.Logic
{
    public class HandleCenter : AbsHandlerCenter
    {
        private IHandler mLoginHandler;
        private IHandler mUserHandler;
        private IHandler mMatchHandler;
        private IHandler mFightHandler;
        private IHandler mAliveHandler;
        public HandleCenter()
        {
            mLoginHandler = new LoginHandler();
            mUserHandler = new UserHandler();
            mMatchHandler = new MatchHandler();
            mFightHandler = new FightHandler();
            mAliveHandler = new AliveHandler();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public override void ClientClose(UserToken token, string error)
        {
            mFightHandler.ClientClose(token, error);
            mMatchHandler.ClientClose(token,error);
            mUserHandler.ClientClose(token,error);
            mLoginHandler.ClientClose(token, error);           
            mAliveHandler.ClientClose(token,error);
            Console.WriteLine("客户端断开连接"+ token.conn.RemoteEndPoint);
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="token"></param>
        public override void ClientConnect(UserToken token)
        {
            Console.WriteLine("客户端开始连接服务器" + token.conn.RemoteEndPoint);
        }

        /// <summary>
        /// 消息到达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public override void MessageReceive(UserToken token, object message)
        {
            
            //讲类型转为socket
            SocketModel model = message as SocketModel;;
            Console.WriteLine("客户端消息到达" + token.conn.RemoteEndPoint + " 类型：" + model.type);
            //分发消息
            switch (model.type)
            {
                //处理登录模块的业务请求
                case TypeProtocol.Login:
                    mLoginHandler.MessageReceive(token, model);
                    break;
                //处理用户模块的请求
                case TypeProtocol.User:
                    mUserHandler.MessageReceive(token,model);
                    break;
                //匹配模块
                case TypeProtocol.Match:
                    mMatchHandler.MessageReceive(token,model);
                    break;
                //战斗模块
                case TypeProtocol.Fight:
                    mFightHandler.MessageReceive(token,model);
                    break;
                //心跳模块
                case TypeProtocol.Alive:
                    mFightHandler.MessageReceive(token,model);
                    break;
            }
        }
    }
}
