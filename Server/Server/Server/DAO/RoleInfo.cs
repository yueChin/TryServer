

using ServerTools.Debug;

namespace Server.DAO
{
    public class RoleInfo
    {
        public int Id;
        public string UserName;
        public string PassWord;
        public string NickName;
        //头像
        public string Icon = "Default";
        //游戏币
        public int Coin = 10000;
        //钻石
        public int Diamond = 100;
        /// <summary>
        /// 性别
        /// 0男 1女 -1未知
        /// </summary>
        public int Sex = -1;
        /// <summary>
        /// 排名
        /// </summary>
        public int Rank = 0;
        /// <summary>
        /// 电话
        /// </summary>
        public string PhoneNum = string.Empty;
    }
}
