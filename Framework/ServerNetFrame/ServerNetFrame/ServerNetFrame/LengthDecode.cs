
using System.Collections.Generic;

namespace ServerNetFrame
{
    /// <summary>
    /// 定长解码
    /// </summary>
    /// <param name="value">丢出数据包</param>
    /// <returns></returns>
    public delegate byte[] LengthDecode(ref List<byte> value);
}

