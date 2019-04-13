using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace GameProtocol.Model.Login
{
    /// <summary>
    /// 请求登录模型
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class RequestLoginModel
    {
        /// <summary>
        /// 渠道
        /// 0普通
        /// 1微信
        /// 2手机
        /// </summary>
        [ProtoMember(1)]
        public int Ditch = 0;
        /// <summary>
        /// 帐号
        /// </summary>
        [ProtoMember(2)]
        public string UserName = string.Empty;
        /// <summary>
        /// 密码
        /// </summary>
        [ProtoMember(3)]
        public string password = string.Empty;
    }
}
