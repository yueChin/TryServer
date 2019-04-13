using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 匹配的二级协议
    /// </summary>
    public class MatchProtocol
    {
        /// <summary>
        /// 开始匹配客户端请求
        /// </summary>
        public const int StartMatch_CReq = 3001;
        /// <summary>
        /// 开始匹配服务器回应
        /// 0 开始匹配
        /// -1 当前余额不足
        /// -3 玩家已经在匹配队列中
        /// </summary>
        public const int StartMatch_SRes = 3002;
        /// <summary>
        /// 离开匹配客户端请求
        /// </summary>
        public const int LevelMatch_CReq = 3003;
        /// <summary>
        /// 离开匹配服务器回应
        /// 0 离开成功
        /// -1 游戏已经开始
        /// -2 不再匹配队列中
        /// </summary>
        public const int LevelMatch_SRes = 3004;
        /// <summary>
        /// 同步匹配队列信息
        /// </summary>
        public const int MatchInfo_BRQ = 3005;
        /// <summary>
        /// 匹配完成
        /// </summary>
        public const int MacchInfoFinsh_BRQ = 3006;
        /// <summary>
        /// 匹配被关闭
        /// </summary>
        public const int MatchBeClosed = 3007;
    }
}
