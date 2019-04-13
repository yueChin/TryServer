using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerTools.Debug;

namespace ServerTools.Time
{

    /// <summary>
    /// 任务委托
    /// </summary>
    public delegate void TimeTask();
    /// <summary>
    /// 计时器模块
    /// </summary>
    public class ScheduleUtil
    {
        //单例模式
        private static ScheduleUtil mInstance;
        public static ScheduleUtil Instance
        {
            get { return mInstance ==null ? mInstance = new ScheduleUtil() : mInstance; }
        }

        private Timer timer;

        /// <summary>
        /// 待执行的任务
        /// </summary>
        private ConcurrentDictionary<int,TimeTaskModel> mTaskDict = new ConcurrentDictionary<int, TimeTaskModel>();
        /// <summary>
        /// 待移除的任务
        /// </summary>
        private List<int> mRemoveList = new List<int>();

        private int Index;

        private Thread TimeThread;

        private ScheduleUtil()
        {
            //开始计时器线程
            TimeThread = new Thread(TimeStart);
            TimeThread.Start();
        }

        private void TimeStart()
        {
            while (true)
            {
                Thread.Sleep(20);
                //获取现行刻度
                //本次刻度数 - 上次刻度 & 10000 = 毫秒数
                //long time = DateTime.Now.Ticks;
                //int i = 0;
                //while (i < 10)
                //{
                //    i = (int)(DateTime.Now.Ticks - time) / 10000;
                //}
                CallBack();
            }
        }

        /// <summary>
        /// 执行回调
        /// </summary>
        private void CallBack()
        {
            //线程锁，防止数据竞争
            lock (mRemoveList)
            {
                lock (mTaskDict)
                {
                    TimeTaskModel model = null;
                    //执行前将待移除的任务移除
                    for (int i = 0; i < mRemoveList.Count; i++)
                    {
                        mTaskDict.TryRemove(mRemoveList[i],out model);
                    }
                    mRemoveList.Clear();
                    long endTime = DateTime.Now.Ticks;
                    List<int> dkeyList = new List<int>(mTaskDict.Keys.ToList());
                    for (int i = 0; i < dkeyList.Count; i++)
                    {
                        //如果待执行的时间小于等于当前时间
                        if (mTaskDict[dkeyList[i]].Time <= endTime)
                        {
                            //将本人无添加至移除列表
                            mRemoveList.Add(mTaskDict[dkeyList[i]].Id);
                            try
                            {
                                mTaskDict[dkeyList[i]].Run();
                            }
                            catch (Exception e)
                            {
                                DebugUtil.Instance.Log2Time(e, logType.Error);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加一个计时器任务
        /// </summary>
        /// <param name="task">将执行的任务</param>
        /// <param name="time">任务执行时间 ms</param>
        public int AddSchedule(TimeTask task, long time)
        {
            lock (task)
            {
                Index++;
                long nowTime = DateTime.Now.Ticks;
                //当前时间+延时间隔ms
                nowTime += time / 10000;
                TimeTaskModel model = new TimeTaskModel(Index, nowTime, task);
                //将任务添加到任务字典
                mTaskDict.TryAdd(Index, model);
                return Index;
            }
        }

        public bool RemoveSchedule(int taskid)
        {
            //如果该任务已经存在在待移除列表，就返回
            if (mRemoveList.Contains(taskid))
                return true;
            //如果该任务存在于待执行字典中，就加入到待移除列表，返回
            if (mTaskDict.ContainsKey(taskid))
            {
                mRemoveList.Add(taskid);
                return true;
            }

            return false;
        }
    }
}
