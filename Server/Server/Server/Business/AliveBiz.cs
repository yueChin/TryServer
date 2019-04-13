using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameProtocol.Model.Alive;
using Server.Cache;
using ServerNetFrame;

namespace Server.Business
{
    public class AliveBiz
    {
        //确认时间
        public AliveModel TimeCheck(UserToken token,AliveModel model)
        {
            if (model == null || model.Time == 0) return new AliveModel(){Statue = -1};
            if (!CacheProxy.User.IsOnline(token))
            {
                //连接非法，玩家不在线
                model.Statue = -2;              
            }
            else
            {
                //连接合法
                model.Statue = 0;
                //刷新时间
                CacheProxy.Alive.RefreshTime(token);
            }
            model.Time = DateTime.Now.Ticks;
            return model;
        }

        //超时
        public void TimeOut(UserToken token)
        {
            if(token == null) return;
            //离开战斗
            CacheProxy.Fight.CloseClient(token);
            //离开匹配
            CacheProxy.Match.LeaveMatch(token);
            //退出登录
            BizProxy.Login.LogOut(token);
        }
    }
}
