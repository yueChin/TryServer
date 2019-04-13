using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 线程安全的Object类
    /// </summary>
    public class ConcurrentObject
    {
        private object mValue;

        public object Value
        {
            get { return mValue; }
            set
            {
                lock (this)
                {
                    mValue = value;
                }
            }
        }

        public ConcurrentObject(object value)
        {
            this.mValue = value;
        }
    }
}
