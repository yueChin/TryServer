using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Business
{
    /// <summary>
    /// 业务逻辑代理
    /// </summary>
    public class BizProxy
    {
        public readonly static LoginBiz Login;
        public readonly static UserBiz User;
        public readonly static MatchBiz Match;
        public readonly static AliveBiz Alive;
        static BizProxy()
        {
            Login = new LoginBiz();
            User = new UserBiz();
            Match = new MatchBiz();
            Alive = new AliveBiz();
        }
    }
}
