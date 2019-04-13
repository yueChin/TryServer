using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Server.Model.User
{
    [Serializable]
    [ProtoContract]
    public class UserModel
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public string UserName;
        [ProtoMember(3)]
        public string NickName;
        /// <summary>
        /// 头像
        /// </summary>
        [ProtoMember(4)]
        public string Icon = "Default";
        /// <summary>
        /// 金币
        /// </summary>
        [ProtoMember(5)]
        public int Coin = 10000;
        /// <summary>
        /// 钻石
        /// </summary>
        [ProtoMember(6)]
        public int Diamond = 100;
        /// <summary>
        /// 性别
        /// 0男 1女 -1未知
        /// </summary>
        [ProtoMember(7)]
        public int Sex = -1;
        /// <summary>
        /// 电话
        /// </summary>
        [ProtoMember(8)]
        public string PhoneNum = string.Empty;
    }
}
