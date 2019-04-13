using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 线程安全的整数
    /// </summary>
    public class ConcurrentInt
    {
        private int mValue;

        public int Value
        {
            get { return mValue; }
        }

        public ConcurrentInt(int value)
        {
            this.mValue = value;
        }
        /// <summary>
        /// Int加
        /// </summary>
        /// <param name="value"></param>
        public void AddInt(int value)
        {
            lock (this)
            {
                mValue += value;
            }
        }
        /// <summary>
        /// Int减
        /// </summary>
        /// <param name="value"></param>
        public void ReduceInt(int value)
        {
            lock (this)
            {
                mValue -= value;
            }
        }

    }
}
