// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.Match
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 匹配
    /// </summary>
    internal class Match
    {
        private MatchState state;
        private int pos;
        private int len;
        private byte symbol;

        internal MatchState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        internal int Position
        {
            get
            {
                return this.pos;
            }
            set
            {
                this.pos = value;
            }
        }

        internal int Length
        {
            get
            {
                return this.len;
            }
            set
            {
                this.len = value;
            }
        }

        internal byte Symbol
        {
            get
            {
                return this.symbol;
            }
            set
            {
                this.symbol = value;
            }
        }
    }
}
