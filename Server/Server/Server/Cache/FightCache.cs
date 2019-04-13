using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Logic.Fight;
using ServerNetFrame;

namespace Server.Cache
{
    public class FightCache
    {
        /// <summary>
        /// 房间号到房间的映射
        /// </summary>
        ConcurrentDictionary<int,Lazy<FightRoom>> RoomDict = new ConcurrentDictionary<int,Lazy<FightRoom>>();
        /// <summary>
        /// 玩家ID到房间号的映射
        /// </summary>
        ConcurrentDictionary<int,int> User2RoomDict = new ConcurrentDictionary<int, int>();
        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom()
        {

        }

        /// <summary>
        /// 增加房间
        /// </summary>
        /// <returns></returns>
        public bool AddRoom()
        {
            return true;
        }

        /// <summary>
        /// 关闭房间
        /// </summary>
        public void CloseRoom()
        {

        }

        public void CloseClient(UserToken token)
        {

        }
    }
}
