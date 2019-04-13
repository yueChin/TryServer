// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.GZipConstants
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

namespace ClientNetFrame.IOCompression
{
    /// <summary>
    /// 压缩构成
    /// </summary>
    internal static class GZipConstants
    {
        internal const int CompressionLevel_3 = 3;
        internal const int CompressionLevel_10 = 10;
        internal const long FileLengthModulo = 4294967296;
        internal const byte ID1 = 31;
        internal const byte ID2 = 139;
        internal const byte Deflate = 8;
        internal const int Xfl_HeaderPos = 8;
        internal const byte Xfl_FastestAlgorithm = 4;
        internal const byte Xfl_MaxCompressionSlowestAlgorithm = 2;
    }
}
