using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace GameProtocol.Model.Alive
{
    /// <summary>
    /// 心跳包模型
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class AliveModel
    {
        /// <summary>
        /// 目前状态
        /// 0 在线
        /// 1 不在线
        /// </summary>
        [ProtoMember(1)]
        public int Statue;
        /// <summary>
        /// 系统收到消息的时间
        /// </summary>
        [ProtoMember(2)]
        public long Time;
    }
}
