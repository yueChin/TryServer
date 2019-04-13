// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.SocketModel
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

namespace ClientNetFrame
{
    /// <summary>
    /// Socket模型
    /// </summary>
    public class SocketModel
    {
        public byte type { get; set; }

        public int command { get; set; }

        public object message { get; set; }

        public SocketModel()
        {
        }

        public SocketModel(byte t, int c, object o)
        {
            this.type = t;
            this.command = c;
            this.message = o;
        }

        public T GetMessage<T>()
        {
            return (T)this.message;
        }
    }
}
