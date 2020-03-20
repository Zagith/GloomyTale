using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF.Entities
{
    public class MultiAccountException
    {
        #region Properties

        public virtual Account Account { get; set; }

        public long AccountId { get; set; }

        [Key]
        public long ExceptionId { get; set; }

        public byte ExceptionLimit { get; set; }

        #endregion
    }
}
