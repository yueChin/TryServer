// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.FastEncoderWindow
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Diagnostics;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 快速编码窗口
    /// </summary>
    internal class FastEncoderWindow
    {
        private byte[] window;
        private int bufPos;
        private int bufEnd;
        private const int FastEncoderHashShift = 4;
        private const int FastEncoderHashtableSize = 2048;
        private const int FastEncoderHashMask = 2047;
        private const int FastEncoderWindowSize = 8192;
        private const int FastEncoderWindowMask = 8191;
        private const int FastEncoderMatch3DistThreshold = 16384;
        internal const int MaxMatch = 258;
        internal const int MinMatch = 3;
        private const int SearchDepth = 32;
        private const int GoodLength = 4;
        private const int NiceLength = 32;
        private const int LazyMatchThreshold = 6;
        private ushort[] prev;
        private ushort[] lookup;

        public FastEncoderWindow()
        {
            this.ResetWindow();
        }

        public int BytesAvailable
        {
            get
            {
                Debug.Assert(this.bufEnd - this.bufPos >= 0, "Ending pointer can't be in front of starting pointer!");
                return this.bufEnd - this.bufPos;
            }
        }

        public DeflateInput UnprocessedInput
        {
            get
            {
                return new DeflateInput()
                {
                    Buffer = this.window,
                    StartIndex = this.bufPos,
                    Count = this.bufEnd - this.bufPos
                };
            }
        }

        public void FlushWindow()
        {
            this.ResetWindow();
        }

        private void ResetWindow()
        {
            this.window = new byte[16646];
            this.prev = new ushort[8450];
            this.lookup = new ushort[2048];
            this.bufPos = 8192;
            this.bufEnd = this.bufPos;
        }

        public int FreeWindowSpace
        {
            get
            {
                return 16384 - this.bufEnd;
            }
        }

        public void CopyBytes(byte[] inputBuffer, int startIndex, int count)
        {
            Array.Copy((Array)inputBuffer, startIndex, (Array)this.window, this.bufEnd, count);
            this.bufEnd += count;
        }

        public void MoveWindows()
        {
            Debug.Assert(this.bufPos == 16384, "only call this at the end of the window");
            this.VerifyHashes();
            Array.Copy((Array)this.window, this.bufPos - 8192, (Array)this.window, 0, 8192);
            for (int index = 0; index < 2048; ++index)
            {
                int num = (int)this.lookup[index] - 8192;
                this.lookup[index] = num > 0 ? (ushort)num : (ushort)0;
            }
            for (int index = 0; index < 8192; ++index)
            {
                long num = (long)this.prev[index] - 8192L;
                this.prev[index] = num > 0L ? (ushort)num : (ushort)0;
            }
            Array.Clear((Array)this.window, 8192, this.window.Length - 8192);
            this.VerifyHashes();
            this.bufPos = 8192;
            this.bufEnd = this.bufPos;
        }

        private uint HashValue(uint hash, byte b)
        {
            return hash << 4 ^ (uint)b;
        }

        private uint InsertString(ref uint hash)
        {
            hash = this.HashValue(hash, this.window[this.bufPos + 2]);
            uint num = (uint)this.lookup[(int)hash & 2047];
            this.lookup[(int)hash & 2047] = (ushort)this.bufPos;
            this.prev[this.bufPos & 8191] = (ushort)num;
            return num;
        }

        private void InsertStrings(ref uint hash, int matchLen)
        {
            Debug.Assert(matchLen > 0, "Invalid match Len!");
            if (this.bufEnd - this.bufPos <= matchLen)
            {
                this.bufPos += matchLen - 1;
            }
            else
            {
                while (--matchLen > 0)
                {
                    int num = (int)this.InsertString(ref hash);
                    ++this.bufPos;
                }
            }
        }

        internal bool GetNextSymbolOrMatch(Match match)
        {
            Debug.Assert(this.bufPos >= 8192 && this.bufPos < 16384, "Invalid Buffer Position!");
            uint hash = this.HashValue(this.HashValue(0U, this.window[this.bufPos]), this.window[this.bufPos + 1]);
            int matchPos1 = 0;
            this.VerifyHashes();
            int matchLen1;
            if (this.bufEnd - this.bufPos <= 3)
            {
                matchLen1 = 0;
            }
            else
            {
                int search = (int)this.InsertString(ref hash);
                if ((uint)search > 0U)
                {
                    matchLen1 = this.FindMatch(search, out matchPos1, 32, 32);
                    if (this.bufPos + matchLen1 > this.bufEnd)
                        matchLen1 = this.bufEnd - this.bufPos;
                }
                else
                    matchLen1 = 0;
            }
            if (matchLen1 < 3)
            {
                match.State = MatchState.HasSymbol;
                match.Symbol = this.window[this.bufPos];
                ++this.bufPos;
            }
            else
            {
                ++this.bufPos;
                if (matchLen1 <= 6)
                {
                    int matchPos2 = 0;
                    int search = (int)this.InsertString(ref hash);
                    int num;
                    if ((uint)search > 0U)
                    {
                        num = this.FindMatch(search, out matchPos2, matchLen1 < 4 ? 32 : 8, 32);
                        if (this.bufPos + num > this.bufEnd)
                            num = this.bufEnd - this.bufPos;
                    }
                    else
                        num = 0;
                    if (num > matchLen1)
                    {
                        match.State = MatchState.HasSymbolAndMatch;
                        match.Symbol = this.window[this.bufPos - 1];
                        match.Position = matchPos2;
                        match.Length = num;
                        ++this.bufPos;
                        int matchLen2 = num;
                        this.InsertStrings(ref hash, matchLen2);
                    }
                    else
                    {
                        match.State = MatchState.HasMatch;
                        match.Position = matchPos1;
                        match.Length = matchLen1;
                        int matchLen2 = matchLen1 - 1;
                        ++this.bufPos;
                        this.InsertStrings(ref hash, matchLen2);
                    }
                }
                else
                {
                    match.State = MatchState.HasMatch;
                    match.Position = matchPos1;
                    match.Length = matchLen1;
                    this.InsertStrings(ref hash, matchLen1);
                }
            }
            if (this.bufPos == 16384)
                this.MoveWindows();
            return true;
        }

        private int FindMatch(int search, out int matchPos, int searchDepth, int niceLength)
        {
            Debug.Assert(this.bufPos >= 0 && this.bufPos < 16384, "Invalid Buffer position!");
            Debug.Assert(search < this.bufPos, "Invalid starting search point!");
            Debug.Assert((int)this.RecalculateHash(search) == (int)this.RecalculateHash(this.bufPos));
            int num1 = 0;
            int num2 = 0;
            int num3 = this.bufPos - 8192;
            Debug.Assert(num3 >= 0, "bufPos is less than FastEncoderWindowSize!");
            byte num4 = this.window[this.bufPos];
            for (; search > num3; search = (int)this.prev[search & 8191])
            {
                Debug.Assert((int)this.RecalculateHash(search) == (int)this.RecalculateHash(this.bufPos), "Corrupted hash link!");
                if ((int)this.window[search + num1] == (int)num4)
                {
                    int num5 = 0;
                    while (num5 < 258 && (int)this.window[this.bufPos + num5] == (int)this.window[search + num5])
                        ++num5;
                    if (num5 > num1)
                    {
                        num1 = num5;
                        num2 = search;
                        if (num5 <= 32)
                            num4 = this.window[this.bufPos + num5];
                        else
                            break;
                    }
                }
                if (--searchDepth != 0)
                    Debug.Assert((int)this.prev[search & 8191] < search, "we should always go backwards!");
                else
                    break;
            }
            matchPos = this.bufPos - num2 - 1;
            if (num1 == 3 && matchPos >= 16384)
                return 0;
            Debug.Assert(num1 < 3 || matchPos < 8192, "Only find match inside FastEncoderWindowSize");
            return num1;
        }

        [Conditional("DEBUG")]
        private void VerifyHashes()
        {
            ushort num;
            for (int index1 = 0; index1 < 2048; ++index1)
            {
                for (ushort index2 = this.lookup[index1]; index2 != (ushort)0 && this.bufPos - (int)index2 < 8192; index2 = num)
                {
                    Debug.Assert((long)this.RecalculateHash((int)index2) == (long)index1, "Incorrect Hashcode!");
                    num = this.prev[(int)index2 & 8191];
                    if (this.bufPos - (int)num < 8192)
                        Debug.Assert((int)num < (int)index2, "pointer is messed up!");
                    else
                        break;
                }
            }
        }

        private uint RecalculateHash(int position)
        {
            return (uint)(((int)this.window[position] << 8 ^ (int)this.window[position + 1] << 4 ^ (int)this.window[position + 2]) & 2047);
        }
    }
}
