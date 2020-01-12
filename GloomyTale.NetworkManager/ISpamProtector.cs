using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.NetworkManager
{
    public interface ISpamProtector
    {
        bool CanConnect(string ipAddress);
    }
}
