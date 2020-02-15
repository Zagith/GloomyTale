using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.NetworkManager.Cryptography
{
    public interface IDecrypter
    {
        string Decode(ReadOnlySpan<byte> bytesBuffer);
    }
}
