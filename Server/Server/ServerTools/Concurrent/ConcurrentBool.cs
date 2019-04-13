using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 线程安全的bool类
    /// </summary>
    public class ConcurrentBool
    {
        private bool mBoolean;

        public bool Value
        {
            get { return mBoolean; }
            set
            {
                lock (this)
                {
                    mBoolean = value;
                }
            }
        }

        public ConcurrentBool(bool boolean = false)
        {
            this.mBoolean = boolean;
        }
    }
}
