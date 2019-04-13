using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace GameProtocol.Model.Login
{
    /// <summary>
    /// 注册返回结果
    /// </summary>
    [Serializable]//传输模型需要可以序列化
    [ProtoContract]
    public class ResponseRegisterModel
    {
        /// <summary>
        /// 注册结果的状态码
        /// 0-成功
        /// 1-失败
        /// </summary>
        [ProtoMember(1)]
        public int Status = 0;
        /// <summary>
        /// 返回给客户端一个加密后的密码
        /// </summary>
        [ProtoMember(2)]
        public string Password = string.Empty;
    }
}
