using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.DAO;
using ServerNetFrame;
using ServerTools.Debug;

namespace Server.Cache
{
    /// <summary>
    /// 用户数据缓存
    /// </summary>
    public class UserCache
    {
        /// <summary>
        /// 玩家信息与玩家帐号映射
        /// </summary>
        ConcurrentDictionary<string,Lazy<RoleInfo>> accountMap = new ConcurrentDictionary<string, Lazy<RoleInfo>>();
        /// <summary>
        /// 在线玩家与用户帐号的映射
        /// </summary>
        ConcurrentDictionary<UserToken,string> onLineAccoutDict = new ConcurrentDictionary<UserToken, string>();
        /// <summary>
        /// 玩家ID与帐号映射
        /// </summary>
        ConcurrentDictionary<int,UserToken> id2TokenDict = new ConcurrentDictionary<int, UserToken>();

        private int mIndex = 0;
        /// <summary>
        /// 注册帐号
        /// </summary>
        public string Register(UserToken token)
        {
            Lazy<RoleInfo> role = new Lazy<RoleInfo>();
            role.Value.Id = ++mIndex;
            role.Value.UserName = "Lin" + (mIndex + 10000);
            role.Value.PassWord = "Password";
            role.Value.NickName = "游客" + (mIndex + 10000);
            //其余使用默认数据
            DebugUtil.Instance.Log2Time("新建人物 : 用户名称"+role.Value.UserName + "密码" +role.Value.PassWord);
            accountMap.TryAdd(role.Value.UserName,role);
            CacheProxy.User.GetOnLine(token,role.Value.UserName);
            return role.Value.PassWord;
        }

        /// <summary>
        /// 是否含有此账号
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsHasAccount(string username)
        {
            if (accountMap.ContainsKey(username))
                return true;
            return false;
        }

        /// <summary>
        /// 帐号密码是否匹配
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsMatch(string userName, string password)
        {
            if (!IsHasAccount(userName))
                return false;
            if (accountMap[userName].Value.PassWord.Equals(password))
                return true;
            return false;
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsOnline(UserToken token)
        {
            if (onLineAccoutDict.ContainsKey(token))
                return true;
            return false;
        }

        /// <summary>
        /// 在线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void GetOnLine(UserToken token,string name)
        {
            if (IsOnline(token))
            {
                DebugUtil.Instance.Log2Time(name +"帐号已在线",logType.Debug);
                return;
            }

            if (!IsHasAccount(name))
            {
                DebugUtil.Instance.Log2Time(name + "帐号不存在", logType.Debug);
                return;
            }

            if (id2TokenDict.ContainsKey(accountMap[name].Value.Id))
            {
                DebugUtil.Instance.Log2Time(name + "移除帐号", logType.Debug);
                id2TokenDict.TryRemove(accountMap[name].Value.Id,out token);
                return;
            }

            DebugUtil.Instance.Log2Time(name + "上线成功", logType.Debug);
            id2TokenDict.TryAdd(accountMap[name].Value.Id,token);
            onLineAccoutDict.TryAdd(token,name);

        }

        /// <summary>
        /// 离线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void GetOffLine(UserToken token)
        {
            if(IsHasAccount(onLineAccoutDict[token]))
                if (id2TokenDict.ContainsKey(accountMap[onLineAccoutDict[token]].Value.Id))
                {
                    id2TokenDict.TryRemove(accountMap[onLineAccoutDict[token]].Value.Id,out token);
                    string str;
                    onLineAccoutDict.TryRemove(token,out str);
                }
            DebugUtil.Instance.Log2Time("玩家下线了",logType.Debug);
        }

        /// <summary>
        /// 保存帐号信息
        /// </summary>
        /// <param name="token"></param>
        public void Save(UserToken token)
        {

        }

        /// <summary>
        /// 获取帐号信息
        /// </summary>
        /// <param name="token"></param>
        public RoleInfo Get(UserToken token)
        {
            if (!IsOnline(token) || !IsHasAccount(onLineAccoutDict[token])) return null;
            return accountMap[onLineAccoutDict[token]].Value;
        }

        /// <summary>
        /// 通过连接获取用户ID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int GetID2Token(UserToken token)
        {
            if (!IsOnline(token) || !IsHasAccount(onLineAccoutDict[token])) return -1;
            return accountMap[onLineAccoutDict[token]].Value.Id;
        }

        public UserToken GetToken(int id)
        {
            if (!id2TokenDict.ContainsKey(id)) return null;
            return id2TokenDict[id];
        }
    }
}
