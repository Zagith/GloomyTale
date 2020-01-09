using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Domain
{
    public enum ChatType : byte
    {
        General = 0,
        Whisper = 1,
        Party = 2,
        Family = 3,
        Friend = 4,
        Speaker = 5
    }
}
