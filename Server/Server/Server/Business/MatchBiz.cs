using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.Model.Match;
using Server.Cache;
using Server.DAO;
using ServerNetFrame;
using ServerTools.Debug;

namespace Server.Business
{
    /// <summary>
    /// 用来处理匹配逻辑
    /// </summary>
    public class MatchBiz
    {
        /// <summary>
        /// 请求开始匹配
        /// </summary>
        /// <param name="token">用户连接对象</param>
        /// <param name="type">房间类型</param>
        /// <returns> 0 请求开始匹配成功 </returns>
        /// <returns>-1 当前金币余额不足 </returns>
        /// <returns>-2 当前玩家已在匹配列表 </returns>
        public ResponseStartMatchModel StartMatch(UserToken token,SeverConst.GameType type)
        {
            ResponseStartMatchModel rsmm = new ResponseStartMatchModel();
            RoleInfo user = CacheProxy.User.Get(token);          
            if (user == null)
            {
                rsmm.Status = -3;
                DebugUtil.Instance.Log2Time(string.Format("{0} 请求链接匹配失败，连接失败",token.conn.RemoteEndPoint));
                return rsmm;
            }
            int uid = user.Id;
            int uCoin = user.Coin;
            //获取进去房间需要的最新金币
            int coin = CacheProxy.Match.GetRoomCoinAtType(type);           
            if (uCoin < coin)
            {
                rsmm.Status = -1;
                DebugUtil.Instance.Log2Time(string.Format("{0} 请求链接匹配失败，余额不足", token.conn.RemoteEndPoint));
                return rsmm;
            }

            //获取是否在匹配队列中
            int matchid = CacheProxy.Match.IsInMatchLine(user.Id);
            if (matchid > 0)
            {
                rsmm.Status = -2;
                DebugUtil.Instance.Log2Time(string.Format("{0} 请求链接匹配失败，当前已在队列中", token.conn.RemoteEndPoint));
                return rsmm;
            }          
            rsmm.Status = 0;
            CacheProxy.Match.AddMatch(uid, type, ref rsmm);
            DebugUtil.Instance.Log2Time(string.Format("{0} 匹配成功", token.conn.RemoteEndPoint));
            return rsmm;
        }

        /// <summary>
        /// 请求离开队列
        /// </summary>
        /// <param name="token">用户连接对象</param>
        /// <returns>0 请求离开成功</returns>
        /// <returns>-1 游戏已经开始</returns>
        /// <returns>-2 玩家不在队列中</returns>
        public int LevelMatch(UserToken token)
        {
            return CacheProxy.Match.LeaveMatch(token);           
        }
    }
}
