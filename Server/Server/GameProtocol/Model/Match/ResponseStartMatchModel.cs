using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.Model.Match
{
    /// <summary>
    /// 服务器返回匹配信息
    /// </summary>
    [Serializable]
    public class ResponseStartMatchModel
    {
        /// <summary>
        /// 返回状态码
        ///  0 开始匹配
        /// -1 当前余额不足
        /// -3 玩家已经在匹配队列中
        /// </summary>
        public int Status = -1;

        public SeverConst.GameType Type = SeverConst.GameType.WinThree;
        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxPlayer;
        /// <summary>
        /// 当前信息
        /// </summary>
        public int PlayerCount;
    }
}
