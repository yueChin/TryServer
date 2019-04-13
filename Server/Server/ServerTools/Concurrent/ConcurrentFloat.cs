using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 线程安全浮点数
    /// </summary>
    public class ConcurrentFloat
    {
        private float mValue;

        public float Value
        {
            get { return mValue; }
        }

        public ConcurrentFloat(float value)
        {
            this.mValue = value;
        }
        /// <summary>
        /// Float加
        /// </summary>
        /// <param name="value"></param>
        public void Addfloat(float value)
        {
            lock (this)
            {
                mValue += value;
            }
        }
        /// <summary>
        /// Float加减
        /// </summary>
        /// <param name="value"></param>
        public void Reducefloat(float value)
        {
            lock (this)
            {
                mValue -= value;
            }
        }
    }
}
