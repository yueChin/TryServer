using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.Model.Match;
using ServerNetFrame;
using ServerTools.Debug;

namespace Server.Cache
{
    public class MatchCache
    {
        /// <summary>
        /// 房间列表
        /// </summary>
        ConcurrentDictionary<int,Lazy<MatchModel>> matchDict = new ConcurrentDictionary<int, Lazy<MatchModel>>();
        /// <summary>
        /// 玩家到房间列表的映射
        /// </summary>
        ConcurrentDictionary<int,int> user2MatchDict = new ConcurrentDictionary<int, int>();
        /// <summary>
        /// 匹配队列和玩家的映射，是否开始
        /// </summary>
        ConcurrentDictionary<int,bool> isStartGameDict = new ConcurrentDictionary<int, bool>();
        /// <summary>
        /// 随机数种子
        /// </summary>
        Random ran = new Random((int)DateTime.Now.Ticks);
        /// <summary>
        /// 是否在队列中
        /// </summary>
        /// <returns></returns>
        public int IsInMatchLine(int userid)
        {
            if (user2MatchDict.ContainsKey(userid) && matchDict.ContainsKey(user2MatchDict[userid]))
            {
                return user2MatchDict[userid];
            }

            return -1;
        }

        /// <summary>
        /// 获取进入房间的最小货币数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetRoomCoinAtType(SeverConst.GameType type)
        {
            switch (type)
            {
                case SeverConst.GameType.WinThree:
                    break;
                case SeverConst.GameType.FightLine:
                    break;
                default: return 1000000000;
            }

            return 0;
        }

        /// <summary>
        /// 添加一个匹配，添加到匹配队列
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        public void AddMatch(int userid, SeverConst.GameType type,ref ResponseStartMatchModel info)
        {
            //创建一个新的匹配模型
            Lazy<MatchModel> model = null;
            if (matchDict.Count > 0)
            {
                //创建一条匹配队列的房间号列表
                List<int> roomID = new List<int>(matchDict.Keys);
                for (int i = 0; i < roomID.Count; i++)
                {
                    //当目前游戏类型和匹配的类型相同
                    if (matchDict[roomID[i]].Value.Type == type)
                    {
                        //当房间人数未满
                        if (matchDict[roomID[i]].Value.Team.Count < matchDict[roomID[i]].Value.MaxPlayer)
                        {
                            //添加返回给客户端的信息
                            matchDict[roomID[i]].Value.Team.Add(userid);
                            info.PlayerCount = model.Value.Team.Count;
                            info.MaxPlayer = model.Value.MaxPlayer;
                            info.Type = model.Value.Type;
                            //添加玩家到房间的映射
                            user2MatchDict.TryAdd(userid, model.Value.RoomID);
                            DebugUtil.Instance.Log2Time(string.Format("{0} 请求链接匹配失败，队列号", model.Value.RoomID));
                            IsFnish(roomID[i]);
                            return;
                        }
                    }
                }
            }
            else//创建一个新的队列
            {
                model = new Lazy<MatchModel>();
                //设定当前开局的数量
                model.Value.MaxPlayer = 2;
                model.Value.RoomID = GetMatchId();
                model.Value.Type = type;
                model.Value.Team.Add(userid);
                //添加返回给客户端的信息
                info.PlayerCount = model.Value.Team.Count;
                info.MaxPlayer = model.Value.MaxPlayer;
                info.Type = model.Value.Type;
                //添加房间号和房间的映射
                matchDict.TryAdd(model.Value.RoomID,model);
                //添加玩家和房间号的映射
                user2MatchDict.TryAdd(userid,model.Value.RoomID);
                //游戏是否开始的映射
                if (!isStartGameDict.ContainsKey(model.Value.RoomID))
                    isStartGameDict.TryAdd(model.Value.RoomID,false);
                IsFnish(model.Value.RoomID);
            }
        }

        int GetMatchId()
        {
            //生成一个六位数的房间号
            int id = ran.Next(100000, 999999);
            while (matchDict.ContainsKey(id))
            {
                id = ran.Next(100000, 999999);
            }

            return id;
        }

        /// <summary>
        /// 是否匹配完成
        /// </summary>
        /// <param name="roomid"></param>
        void IsFnish(int roomid)
        {
            //将队伍成员信息广播给所欲成员
            for (int i = 0; i < matchDict[roomid].Value.Team.Count; i++)
            {
                UserToken token = CacheProxy.User.GetToken(matchDict[roomid].Value.Team[i]);
                token.write(TypeProtocol.Match,MatchProtocol.MatchInfo_BRQ, matchDict[roomid]);
            }

            if (matchDict[roomid].Value.Team.Count == matchDict[roomid].Value.MaxPlayer)
            {
                DebugUtil.Instance.Log2Time(roomid + "当前匹配成功");
                //将队伍成员信息广播给所欲成员
                for (int i = 0; i < matchDict[roomid].Value.Team.Count; i++)
                {
                    UserToken token = CacheProxy.User.GetToken(matchDict[roomid].Value.Team[i]);
                    token.write(TypeProtocol.Match, MatchProtocol.MatchInfo_BRQ, matchDict[roomid]);
                }
                SetStartGame(roomid);
            }
        }

        /// <summary>
        /// 获取游戏是否已经开始
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public bool GetStartGame(int roomid)
        {
            if (!isStartGameDict.ContainsKey(roomid))
            {
                return false;
            }
            return isStartGameDict[roomid];
        }

        /// <summary>
        /// 获取游戏是否已经开始
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public bool GetStartGame(UserToken token)
        {
            int user = CacheProxy.User.GetID2Token(token);
            int roomid = user2MatchDict[user];
            if (!isStartGameDict.ContainsKey(roomid))
            {
                return false;
            }
            return isStartGameDict[roomid];
        }
        /// <summary>
        /// 判断是否在匹配队列
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool GetHaveMatch(UserToken token)
        {
            //获取玩家id
            int userid = CacheProxy.User.GetID2Token(token);
            //判断是否含有匹配队列的映射关系
            if (!user2MatchDict.ContainsKey(userid)) return false;
            if (!matchDict.ContainsKey(user2MatchDict[userid])) return false;
            if (!matchDict[user2MatchDict[userid]].Value.Team.Contains(userid)) return false;
            return true;
        }
        /// <summary>
        /// 将游戏设置为已经开始
        /// </summary>
        /// <param name="roomid"></param>
        public void SetStartGame(int roomid)
        {
            if (isStartGameDict.ContainsKey(roomid)) isStartGameDict[roomid] = true;
        }
        /// <summary>
        ///  0 离开成功
        /// -1 游戏已经开始
        /// -2 不再匹配队列中
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int LeaveMatch(UserToken token)
        {
            int userid = CacheProxy.User.GetID2Token(token);
            //是否已经含有匹配队列
            if (GetHaveMatch(token))
            {
                DebugUtil.Instance.Log2Time(string.Format("{0} 请求离开队列失败,玩家不在队列中", userid));
                return -2;
            }

            if (GetStartGame(token))
            {
                DebugUtil.Instance.Log2Time(string.Format("{0} 请求离开队列失败,游戏已经开始", userid));
                return -1;
            }           
            int roomid = user2MatchDict[CacheProxy.User.GetID2Token(token)];                     
            Lazy<MatchModel> model = matchDict[roomid];
            model.Value.Team.Remove(userid);
            user2MatchDict.TryRemove(userid,out roomid);
            //如果队列中没有玩家就移除队列
            if (model.Value.Team.Count == 0)
            {
                bool isErr;
                matchDict.TryRemove(roomid, out model);
                isStartGameDict.TryRemove(roomid,out isErr);
                DebugUtil.Instance.Log2Time(string.Format("{0} 请求移除匹配队列成功", userid));
            }
            else
            {
                //将队伍成员信息广播给所欲成员
                for (int i = 0; i < matchDict[roomid].Value.Team.Count; i++)
                {
                    UserToken usertoken = CacheProxy.User.GetToken(matchDict[roomid].Value.Team[i]);
                    usertoken.write(TypeProtocol.Match, MatchProtocol.MatchInfo_BRQ, matchDict[roomid]);
                }
            }
            return 0;
        }
        /// <summary>
        /// 关闭匹配队列
        /// </summary>
        /// <param name="roomid"></param>
        public void CloseMatch(int roomid)
        {
            if (!matchDict.ContainsKey(roomid)) return;
            //将队伍成员信息广播给所欲成员
            int error;
            for (int i = 0; i < matchDict[roomid].Value.Team.Count; i++)
            {
                int userid = matchDict[roomid].Value.Team[i];
                UserToken usertoken = CacheProxy.User.GetToken(matchDict[roomid].Value.Team[i]);
                usertoken.write(TypeProtocol.Match, MatchProtocol.MatchBeClosed, null);
                if (user2MatchDict.ContainsKey(userid))
                    user2MatchDict.TryRemove(userid,out error);
            }
            Lazy<MatchModel> model = null;
            matchDict.TryRemove(roomid,out model);
            DebugUtil.Instance.Log2Time(roomid + "匹配队列被移除");
        }
    }
}
