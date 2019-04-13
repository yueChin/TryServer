using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 双浮点安全类型
    /// </summary>
    public class ConcurrentDouble
    {
        private double mValue;

        public double Value
        {
            get { return mValue; }
        }

        public ConcurrentDouble(double value)
        {
            this.mValue = value;
        }
        /// <summary>
        /// double加
        /// </summary>
        /// <param name="value"></param>
        public void AddDouble(double value)
        {
            lock (this)
            {
                mValue += value;
            }
        }
        /// <summary>
        /// double减
        /// </summary>
        /// <param name="value"></param>
        public void ReduceDouble(double value)
        {
            lock (this)
            {
                mValue -= value;
            }
        }
    }
}
