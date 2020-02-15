using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Cofiguration
{
    public interface IConfigurationSerializer
    {
        string Serialize<T>(T conf) where T : IConfiguration;

        T Deserialize<T>(string buffer) where T : IConfiguration;
    }
}
