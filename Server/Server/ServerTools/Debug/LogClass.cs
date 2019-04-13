using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.Debug
{
    class LogClass
    {
        /// <summary>
        /// 待输出的日志消息
        /// </summary>
        public object msg = "";

        /// <summary>
        /// 待输出日志的级别
        /// </summary>
        public logType Type = logType.Debug;

        public LogClass()
        {

        }

        public LogClass(object obj)
        {
            this.msg = obj;
        }

        public LogClass(object obj,logType type)
        {
            this.msg = obj;
            this.Type = type;
        }
    }
}
