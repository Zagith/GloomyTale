using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    public class MultiAccountExceptionDAO : IMultiAccountExceptionDAO
    {
        public MultiAccountExceptionDTO Insert(MultiAccountExceptionDTO exception)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MultiAccountException entity = new MultiAccountException();
                    context.MultiAccountException.Add(entity);
                    context.SaveChanges();
                    if (Mapper.Mappers.MultiAccountExceptionMapper.ToMultiAccountExceptionDTO(entity, exception))
                    {
                        return exception;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public MultiAccountExceptionDTO LoadByAccount(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MultiAccountExceptionDTO dto = new MultiAccountExceptionDTO();
                    if (Mapper.Mappers.MultiAccountExceptionMapper.ToMultiAccountExceptionDTO(context.MultiAccountException.AsNoTracking().FirstOrDefault(s => s.AccountId.Equals(accountId)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
    }
}
