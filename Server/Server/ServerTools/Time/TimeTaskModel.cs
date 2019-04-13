using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Time
{
    public class TimeTaskModel
    {
        
        /// <summary>
        /// 任务ID
        /// </summary>
        public int Id;
        /// <summary>
        /// 任务事件
        /// </summary>
        public TimeTask Event;
        /// <summary>
        /// 执行时间，ms
        /// </summary>
        public long Time;

        public TimeTaskModel()
        {
        }

        public TimeTaskModel(int id, long time, TimeTask task)
        {
            this.Id = id;
            this.Time = time;
            this.Event = task;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        public void Run()
        {
            Event();
        }
    }
}
