﻿// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.IOCompression.IFileFormatReader
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

namespace ClientNetFrame.IOCompression
{
    internal interface IFileFormatReader
    {
        bool ReadHeader(InputBuffer input);

        bool ReadFooter(InputBuffer input);

        void UpdateWithBytesRead(byte[] buffer, int offset, int bytesToCopy);

        void Validate();
    }
}