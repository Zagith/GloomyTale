using OpenNos.DAL.EF.Entities;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public static class MultiAccountExceptionMapper
    {
        #region Methods

        public static bool ToMultiAccountException(MultiAccountExceptionDTO input, MultiAccountException output)
        {
            if (input == null)
            {
                return false;
            }
            output.AccountId = input.AccountId;
            output.ExceptionId = input.ExceptionId;
            output.ExceptionLimit = input.ExceptionLimit;
            return true;
        }

        public static bool ToMultiAccountExceptionDTO(MultiAccountException input, MultiAccountExceptionDTO output)
        {
            if (input == null)
            {
                return false;
            }
            output.AccountId = input.AccountId;
            output.ExceptionId = input.ExceptionId;
            output.ExceptionLimit = input.ExceptionLimit;
            return true;
        }

        #endregion
    }
}
