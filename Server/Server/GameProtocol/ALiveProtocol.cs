using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    public class ALiveProtocol
    {
        /// <summary>
        /// 客户端保活请求
        /// </summary>
        public const int Alive_Creq = 5001;
        /// <summary>
        /// 服务器保活回应
        /// 0 在线
        /// -1 时间数据丢失，请求重发
        /// -2 非法或者离线
        /// </summary>
        public const int Alive_SRes = 5002;
    }
}
