using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.Model.Login;
using Server.Business;
using ServerNetFrame;
using ServerNetFrame.Auto;
using ServerTools.Debug;

namespace Server.Logic.Login
{
    /// <summary>
    /// 处理客户端二级业务分发
    /// </summary>
    public class LoginHandler :IHandler
    {
        
        /// <summary>
        /// 用户断开连接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public void ClientClose(UserToken token, string error)
        {
            BizProxy.Login.LogOut(token);
        }

        /// <summary>
        /// 用户消息到达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public void MessageReceive(UserToken token, SocketModel message)
        {
            //处理客户端请求
            switch (message.command)
            {
                //请求登录
                case GameProtocol.LoginProtocol.Login_CReq:
                    DebugUtil.Instance.Log2Time("用户请求登录消息到达");
                    //对三级消息体的转换
                    RequestLoginModel rlm = message.GetMessage<RequestLoginModel>();
                    int result = BizProxy.Login.LoginIn(token, rlm);
                    token.write(TypeProtocol.Login,LoginProtocol.Login_SRes, result);
                    break;
                //处理客户端注册请求
                case GameProtocol.LoginProtocol.QuickReg_CReq:
                    DebugUtil.Instance.Log2Time("用户请求快速注册登录消息到达");
                    ResponseRegisterModel rrm = BizProxy.Login.Reg(token);
                    token.write(TypeProtocol.Login,LoginProtocol.QuickReg_SRes,rrm);
                    break;;
            }
        }

    }
}
