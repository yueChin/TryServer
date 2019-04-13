
using System.Collections.Generic;

namespace ServerNetFrame
{
    public class UserTokenPool
    {
        /// <summary>
        /// 连接池
        /// </summary>
        private Queue<UserToken> pool;

        public UserTokenPool(int max)
        {
            this.pool = new Queue<UserToken>(max);
        }

        public UserToken DeQuene()
        {
            return this.pool.Dequeue();
        }

        public void EnterQuene(UserToken token)
        {
            if (token == null)
                return;
            this.pool.Enqueue(token);
        }

        public int Size
        {
            get
            {
                return this.pool.Count;
            }
        }
    }
}
