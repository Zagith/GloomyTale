using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Data
{
    [Serializable]
    public class MultiAccountExceptionDTO
    {
        public long AccountId { get; set; }

        public long ExceptionId { get; set; }

        public byte ExceptionLimit { get; set; }
    }
}
