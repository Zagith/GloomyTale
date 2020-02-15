using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.NetworkManager.Cryptography
{
    public interface INetworkClientInformation
    {
        int SessionId { get; }
        Encoding Encoding { get; }
    }
}
