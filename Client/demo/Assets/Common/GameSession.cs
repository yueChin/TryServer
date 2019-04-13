using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Model.User;

namespace Assets.Common
{
    /// <summary>
    /// 程序运行时网络会话的存储
    /// </summary>
    public class GameSession
    {
        private static GameSession mInstance;
        public static GameSession Instance
        {
            get { return mInstance == null ? mInstance = new GameSession() : mInstance; }
        }
        /// <summary>
        /// 信息修改监听
        /// </summary>
        public Action UserInfoChangeHandler;

        private UserModel mUserModel;

        public UserModel UserModel
        {
            get { return mUserModel; }
            set
            {
                mUserModel = value;
                UserInfoChangeHandler?.Invoke();
            }
        }
    }
}
