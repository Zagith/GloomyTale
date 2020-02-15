using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.Data
{
    [Serializable]
    public class LogCommandsDTO : MappingBaseDTO
    {
        public long CommandId { get; set; }

        public long? CharacterId { get; set; }

        public string Command { get; set; }

        public string Data { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
