using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.Model.Match
{
    /// <summary>
    /// 匹配信息模型
    /// </summary>
    [Serializable]
    public class MatchModel
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomID = 0;
        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxPlayer = 0;
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<int> Team = new List<int>();

        public SeverConst.GameType Type = SeverConst.GameType.WinThree;
    }
}
