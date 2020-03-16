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
    public class TrueOrFalseDAO : ITrueOrFalseDAO
    {
        #region Methods

        public IEnumerable<TrueOrFalseDTO> LoadByType(short questionType)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<TrueOrFalseDTO> result = new List<TrueOrFalseDTO>();
                foreach (TrueOrFalse trueOrFalse in context.TrueOrFalse.Where(s => s.QuestionType == questionType))
                {
                    TrueOrFalseDTO trueOrFalseDto = new TrueOrFalseDTO();
                    Mapper.Mappers.TrueOrFalseMapping.ToTrueOrFalseDTO(trueOrFalse, trueOrFalseDto);
                    result.Add(trueOrFalseDto);
                }
                return result;
            }
        }
        #endregion
    }
}
