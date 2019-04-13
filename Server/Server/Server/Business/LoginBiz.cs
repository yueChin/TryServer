using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol.Model.Login;
using Server.Cache;
using ServerNetFrame;
using ServerTools.Debug;

namespace Server.Business
{
    public class LoginBiz
    {
        /// <summary>
        /// 登入
        /// 返回登录结果
        /// 0登录成功
        /// -1请求错误
        /// -2请求不合法
        /// -3没有此帐号
        /// -4密码错误
        /// -5帐号已登录
        /// </summary>
        /// <param name="token"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int LoginIn(UserToken token, RequestLoginModel model)
        {
            //判断请求是否正确
            if (model == null || string.IsNullOrEmpty(model.UserName)||string.IsNullOrEmpty(model.password))
            {
                DebugUtil.Instance.Log2Time("token =" + token.conn.RemoteEndPoint + "请求登录失败，请求错误",logType.Warring);
                return -1;
            }
            //判断帐号密码是否合法
            if (model.UserName.Length < 6 || (model.password.Length < 6 && model.Ditch == 0))
            {
                DebugUtil.Instance.Log2Time("token =" + token.conn.RemoteEndPoint + "请求登录失败，帐号密码不合法", logType.Warring);
                return -2;
            }
            //判断是否含有此账号
            if (!CacheProxy.User.IsHasAccount(model.UserName))
            {
                DebugUtil.Instance.Log2Time("token =" + token.conn.RemoteEndPoint + "请求登录失败，没有此账号", logType.Warring);
                return -3;
            }
            //密码是否正确
            if (model.Ditch == 0 && !CacheProxy.User.IsMatch(model.UserName,model.password) )
            {
                DebugUtil.Instance.Log2Time("token =" + token.conn.RemoteEndPoint + "请求登录失败，帐号密码不匹配", logType.Warring);
                return -4;
            }
            //帐号是否正在登录中
            if (!CacheProxy.User.IsOnline(token))
            {
                DebugUtil.Instance.Log2Time("token =" + token.conn.RemoteEndPoint + "请求登录失败，帐号在线", logType.Warring);
                return -5;
            }
            //满足条件，登录
            DebugUtil.Instance.Log2Time("username = " + model.UserName + "请求登录验证成功");
            CacheProxy.User.GetOnLine(token,model.UserName);
            return 0;
        }

        /// <summary>
        /// 返回快速注册结果
        /// </summary>
        /// <returns></returns>
        public ResponseRegisterModel Reg(UserToken token)
        {
            DebugUtil.Instance.Log2Time("ip = " + token + "快速注册");
            ResponseRegisterModel model = new ResponseRegisterModel();
            model.Password = CacheProxy.User.Register(token);
            model.Status = 0;
            
            return model;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token"></param>
        public void LogOut(UserToken token)
        {
            CacheProxy.User.GetOffLine(token);
            CacheProxy.User.Save(token);
        }
    }
}
