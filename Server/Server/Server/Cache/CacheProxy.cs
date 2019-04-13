
namespace Server.Cache
{
    /// <summary>
    /// 缓存代理，为了数据库速度
    /// </summary>
    public class CacheProxy
    {
        public readonly static UserCache User;
        public readonly static MatchCache Match;
        public readonly static FightCache Fight;
        public readonly static AliveCache Alive;
        static CacheProxy()
        {
            User = new UserCache();
            Match = new MatchCache();
            Fight = new FightCache();
            Alive = new AliveCache();
        }
    }
}
