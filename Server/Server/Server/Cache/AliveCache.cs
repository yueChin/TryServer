using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameProtocol;
using GameProtocol.Model.Alive;
using Server.Business;
using ServerNetFrame;
using ServerTools.Time;

namespace Server.Cache
{
    public class AliveCache
    {
        ConcurrentDictionary<int,long> safeTimeDict = new ConcurrentDictionary<int, long>();

        ConcurrentDictionary<int,long> dangerTimeDict = new ConcurrentDictionary<int, long>();

        ConcurrentQueue<int> removeQueue = new ConcurrentQueue<int>();
        private long mPreFreshOne;
        private long mPreFreshFive;
        public AliveCache()
        {
            //线程方式
            //Thread thread = new Thread(TimeClick);
            //thread.Start();
            mPreFreshFive = 0;
            mPreFreshOne = 0;
            TimeClick();
        }

        public void RefreshTime(UserToken token)
        {
            int id = CacheProxy.User.GetID2Token(token);
            //连接的用户不存在
            if (id == -1) return;
            //如果在安全缓存中存在，就更新数值
            if (safeTimeDict.ContainsKey(id))
            {
                safeTimeDict[id] = DateTime.Now.Ticks;
            }
            //如果在危险缓存中存在，就取出并放入安全缓存，并更新
            else if (dangerTimeDict.ContainsKey(id))
            {
                long err;
                safeTimeDict.TryAdd(id, DateTime.Now.Ticks);
                dangerTimeDict.TryRemove(id, out err);                
            }
            //都不存在则加入安全缓存中
            else
            {
                safeTimeDict.TryAdd(id, DateTime.Now.Ticks);
            }
        }
        /// <summary>
        /// 心跳确认
        /// </summary>
        void TimeClick()
        {                
            TimeCheck3S();
            TimeCheck1S();
        }

        /// <summary>
        /// 时间检测，3秒一次
        /// </summary>
        /// <param name="time"></param>
        void TimeCheck3S()
        {
            //如果安全缓存里数量为0，或者计时不到5秒
            if (safeTimeDict.Count == 0 || DateTime.Now.Ticks - mPreFreshFive < 3000) return;
            mPreFreshFive = DateTime.Now.Ticks;
            long err;
            foreach (KeyValuePair<int,long> v in safeTimeDict)
            {
                //如果超过25秒没有消息来，就置入高危连接中
                if (mPreFreshFive - v.Value > 25000)
                {
                    //危险连接置入危险缓存中
                    dangerTimeDict.TryAdd(v.Key, v.Value);
                    // 移除已经放入危险队列中的安全连接
                    safeTimeDict.TryRemove(v.Key, out err);
                }
                //不然就什么都不做                
            }
            //伪循环
            ScheduleUtil.Instance.AddSchedule(TimeCheck3S, 3000);
        }
        /// <summary>
        /// 危险连接
        /// </summary>
        /// <param name="dict"></param>
        void TimeCheck1S()
        {
            //如果危险缓存里数量为0，或者计时不到1秒
            if (dangerTimeDict.Count == 0 || DateTime.Now.Ticks - mPreFreshOne < 1000) return;
            mPreFreshOne = DateTime.Now.Ticks;
            long err;
            foreach (KeyValuePair<int, long> v in dangerTimeDict)
            {
                //如果大于30秒没有信息来，就直接关闭连接
                if (mPreFreshOne - v.Value > 30000)
                {
                    //所有超时的加入到移除队列中
                    removeQueue.Enqueue(v.Key);
                    // 移除已经放入断连队列的危险连接
                    dangerTimeDict.TryRemove(v.Key, out err);
                }
                //请求客户端自救
                UserToken token = CacheProxy.User.GetToken(v.Key);
                if(token != null)
                    token.write(TypeProtocol.Alive,ALiveProtocol.Alive_SRes,new AliveModel(){Statue = -1});
            }
            TimeOut();
            //伪循环
            ScheduleUtil.Instance.AddSchedule(TimeCheck1S, 1000);
        }

        /// <summary>
        /// 断连的队列执行断连
        /// </summary>
        void TimeOut()
        {
            if (removeQueue.Count == 0) return;
            int err;
            //清理危险连接
            for (int i = 0; i < removeQueue.Count; i++)
            {
                //如果移出队列成功的话，就发送关闭客户的消息
                if (removeQueue.TryDequeue(out err))
                {
                    UserToken token = CacheProxy.User.GetToken(err);
                    if(token !=null)
                        BizProxy.Alive.TimeOut(token);
                    
                }
            }
        }
    }
}
