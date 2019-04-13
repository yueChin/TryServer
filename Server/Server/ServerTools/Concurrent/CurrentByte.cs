using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Concurrent
{
    /// <summary>
    /// 线程安全的Type类
    /// </summary>
    public class CurrentByte
    {
        private byte mValue;

        public byte Value
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

        public CurrentByte(byte value)
        {
            this.mValue = value;
        }
    }
}
