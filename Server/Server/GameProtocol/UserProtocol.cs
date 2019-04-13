using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    public class UserProtocol
    {
        /// <summary>
        /// 客户端发起获取信息的请求
        /// </summary>
        public const int GetInfo_CReq = 0;
        /// <summary>
        /// 服务器回应获取信息的请求
        /// </summary>
        public const int GetInfo_SRes = 1;
    }
}
