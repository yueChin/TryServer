using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    public class FightProtocol
    {
        #region 公共协议
        /// <summary>
        /// 准备的客户端请求
        /// </summary>
        public const int Ready_CReq = 4001;
        /// <summary>
        /// 准备的服务器回应
        /// </summary>
        public const int Ready_SRes = 4002;
        /// <summary>
        /// 离开的客户端请求
        /// </summary>
        public const int Level_Creq = 4003;
        /// <summary>
        /// 离开的服务端回应
        /// </summary>
        public const int Level_SRes = 4004;
        /// <summary>
        /// 游戏解散广播
        /// </summary>
        public const int GameDissolve_SDisp = 4005;
        /// <summary>
        /// 游戏开始广播
        /// </summary>
        public const int GameStart_SDisp = 4006;
        /// <summary>
        /// 游戏结束广播
        /// </summary>
        public const int GameOver_SDisp = 4007;
        /// <summary>
        /// 房间玩家信息广播
        /// </summary>
        public const int PlayerInfo_SDisp = 4008;

        #endregion

        #region 赢三张

        

        #endregion
    }
}
