// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.IDeflater
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;

namespace ClientNetFrame.IOCompression
{
    internal interface IDeflater : IDisposable
    {
        bool NeedsInput();

        void SetInput(byte[] inputBuffer, int startIndex, int count);

        int GetDeflateOutput(byte[] outputBuffer);

        bool Finish(byte[] outputBuffer, out int bytesRead);
    }
}
