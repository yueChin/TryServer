// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.HuffmanTree
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System.Diagnostics;

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 最优二叉树
    /// </summary>
    internal class HuffmanTree
    {
        private static HuffmanTree staticLiteralLengthTree = new HuffmanTree(HuffmanTree.GetStaticLiteralTreeLength());
        private static HuffmanTree staticDistanceTree = new HuffmanTree(HuffmanTree.GetStaticDistanceTreeLength());
        internal const int MaxLiteralTreeElements = 288;
        internal const int MaxDistTreeElements = 32;
        internal const int EndOfBlockCode = 256;
        internal const int NumberOfCodeLengthTreeElements = 19;
        private int tableBits;
        private short[] table;
        private short[] left;
        private short[] right;
        private byte[] codeLengthArray;
        private uint[] codeArrayDebug;
        private int tableMask;

        public static HuffmanTree StaticLiteralLengthTree
        {
            get
            {
                return HuffmanTree.staticLiteralLengthTree;
            }
        }

        public static HuffmanTree StaticDistanceTree
        {
            get
            {
                return HuffmanTree.staticDistanceTree;
            }
        }

        public HuffmanTree(byte[] codeLengths)
        {
            Debug.Assert(codeLengths.Length == 288 || codeLengths.Length == 32 || codeLengths.Length == 19, "we only expect three kinds of Length here");
            this.codeLengthArray = codeLengths;
            this.tableBits = this.codeLengthArray.Length != 288 ? 7 : 9;
            this.tableMask = (1 << this.tableBits) - 1;
            this.CreateTable();
        }

        private static byte[] GetStaticLiteralTreeLength()
        {
            byte[] numArray = new byte[288];
            for (int index = 0; index <= 143; ++index)
                numArray[index] = (byte)8;
            for (int index = 144; index <= (int)byte.MaxValue; ++index)
                numArray[index] = (byte)9;
            for (int index = 256; index <= 279; ++index)
                numArray[index] = (byte)7;
            for (int index = 280; index <= 287; ++index)
                numArray[index] = (byte)8;
            return numArray;
        }

        private static byte[] GetStaticDistanceTreeLength()
        {
            byte[] numArray = new byte[32];
            for (int index = 0; index < 32; ++index)
                numArray[index] = (byte)5;
            return numArray;
        }

        private uint[] CalculateHuffmanCode()
        {
            uint[] numArray1 = new uint[17];
            foreach (int codeLength in this.codeLengthArray)
                ++numArray1[codeLength];
            numArray1[0] = 0U;
            uint[] numArray2 = new uint[17];
            uint num = 0;
            for (int index = 1; index <= 16; ++index)
            {
                num = (uint)((int)num + (int)numArray1[index - 1] << 1);
                numArray2[index] = num;
            }
            uint[] numArray3 = new uint[288];
            for (int index = 0; index < this.codeLengthArray.Length; ++index)
            {
                int codeLength = (int)this.codeLengthArray[index];
                if (codeLength > 0)
                {
                    numArray3[index] = FastEncoderStatics.BitReverse(numArray2[codeLength], codeLength);
                    ++numArray2[codeLength];
                }
            }
            return numArray3;
        }

        private void CreateTable()
        {
            uint[] huffmanCode = this.CalculateHuffmanCode();
            this.table = new short[1 << this.tableBits];
            this.codeArrayDebug = huffmanCode;
            this.left = new short[2 * this.codeLengthArray.Length];
            this.right = new short[2 * this.codeLengthArray.Length];
            short length = (short)this.codeLengthArray.Length;
            for (int index1 = 0; index1 < this.codeLengthArray.Length; ++index1)
            {
                int codeLength = (int)this.codeLengthArray[index1];
                if (codeLength > 0)
                {
                    int index2 = (int)huffmanCode[index1];
                    if (codeLength <= this.tableBits)
                    {
                        int num1 = 1 << codeLength;
                        if (index2 >= num1)
                            throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
                        int num2 = 1 << this.tableBits - codeLength;
                        for (int index3 = 0; index3 < num2; ++index3)
                        {
                            this.table[index2] = (short)index1;
                            index2 += num1;
                        }
                    }
                    else
                    {
                        int num1 = codeLength - this.tableBits;
                        int num2 = 1 << this.tableBits;
                        int index3 = index2 & (1 << this.tableBits) - 1;
                        short[] numArray = this.table;
                        do
                        {
                            short num3 = numArray[index3];
                            if (num3 == (short)0)
                            {
                                numArray[index3] = (short)-length;
                                num3 = (short)-length;
                                ++length;
                            }
                            if (num3 > (short)0)
                                throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
                            Debug.Assert(num3 < (short)0, "CreateTable: Only negative numbers are used for tree pointers!");
                            numArray = (index2 & num2) != 0 ? this.right : this.left;
                            index3 = (int)-num3;
                            num2 <<= 1;
                            --num1;
                        }
                        while ((uint)num1 > 0U);
                        numArray[index3] = (short)index1;
                    }
                }
            }
        }

        public int GetNextSymbol(InputBuffer input)
        {
            uint num1 = input.TryLoad16Bits();
            if (input.AvailableBits == 0)
                return -1;
            int index1 = (int)this.table[(long)num1 & (long)this.tableMask];
            if (index1 < 0)
            {
                uint num2 = (uint)(1 << this.tableBits);
                do
                {
                    int index2 = -index1;
                    index1 = ((int)num1 & (int)num2) != 0 ? (int)this.right[index2] : (int)this.left[index2];
                    num2 <<= 1;
                }
                while (index1 < 0);
            }
            int codeLength = (int)this.codeLengthArray[index1];
            if (codeLength <= 0)
                throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
            if (codeLength > input.AvailableBits)
                return -1;
            input.SkipBits(codeLength);
            return index1;
        }
    }
}
