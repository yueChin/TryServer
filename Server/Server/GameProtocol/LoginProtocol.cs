using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 登录模块行为协议
    /// </summary>
    public class LoginProtocol
    {
        /// <summary>
        /// 客户端请求快速注册
        /// </summary>
        public const int QuickReg_CReq = 1001;

        /// <summary>
        /// 服务器返回注册结果
        /// </summary>
        public const int QuickReg_SRes = 1002;

        /// <summary>
        /// 请求登录
        /// </summary>
        public const int Login_CReq = 1003;

        /// <summary>
        /// 返回登录结果
        /// 0登录成功
        /// -1请求错误
        /// -2请求不合法
        /// -3没有此帐号
        /// -4密码错误
        /// -5帐号已登录
        /// </summary>
        public const int Login_SRes = 1004;
    }
}
