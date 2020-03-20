using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface IMultiAccountExceptionDAO
    {
        #region Methods

        MultiAccountExceptionDTO Insert(MultiAccountExceptionDTO exception);

        MultiAccountExceptionDTO LoadByAccount(long accountId);

        #endregion
    }
}
