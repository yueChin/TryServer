using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 线程安全的长整形
    /// </summary>
    public class ConcurrentLong
    {
        private long mValue;

        public long Value
        {
            get { return mValue; }
        }

        public ConcurrentLong(long value)
        {
            this.mValue = value;
        }
        /// <summary>
        /// Long加
        /// </summary>
        /// <param name="value"></param>
        public void AddInt(long value)
        {
            lock (this)
            {
                mValue += value;
            }
        }
        /// <summary>
        /// Long减
        /// </summary>
        /// <param name="value"></param>
        public void ReduceInt(long value)
        {
            lock (this)
            {
                mValue -= value;
            }
        }
    }
}
