// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.InvalidDataException
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Runtime.Serialization;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 无效数据异常
    /// </summary>
    [Serializable]
    public sealed class InvalidDataException : SystemException
    {
        public InvalidDataException()
            : base(SR.GetString("Invalid data"))
        {
        }

        public InvalidDataException(string message)
            : base(message)
        {
        }

        public InvalidDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal InvalidDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
