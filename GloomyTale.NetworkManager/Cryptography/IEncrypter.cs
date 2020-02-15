using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.NetworkManager.Cryptography
{
    public interface IEncrypter
    {
        ReadOnlyMemory<byte> Encode(string packet);
    }
}
