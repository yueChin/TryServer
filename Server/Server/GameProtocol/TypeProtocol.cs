using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol
{
    /// <summary>
    /// 一级协议
    /// </summary>
    public class TypeProtocol
    {
        public const byte Login = 1;
        public const byte User = 2;
        public const byte Match = 3;
        public const byte Fight = 4;
        public const byte Alive = 5;
    }
}
