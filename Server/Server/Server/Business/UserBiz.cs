using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Cache;
using Server.DAO;
using Server.Model.User;
using ServerNetFrame;

namespace Server.Business
{
    public class UserBiz
    {
        /// <summary>
        /// 获取用户模型
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserModel GetUserModel(UserToken token)
        {
            if (token == null) return null;
            RoleInfo role = CacheProxy.User.Get(token);
            if (role == null) return null;
            UserModel model = new UserModel()
            {
                Coin = role.Coin,
                Diamond = role.Diamond,
                Icon = role.Icon,
                Id = role.Id,
                NickName = role.NickName,
                PhoneNum = role.PhoneNum,
                Sex = role.Sex,
                UserName = role.UserName,
            };
            return model;
        }
    }
}
