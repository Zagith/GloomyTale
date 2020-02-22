/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class GeneralLogDAO : MappingBaseDao<GeneralLog, GeneralLogDTO>, IGeneralLogDAO
    {
        public GeneralLogDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public bool IdAlreadySet(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.GeneralLog.Any(gl => gl.LogId == id);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return false;
            }
        }
        
        public GeneralLogDTO Insert(GeneralLogDTO generalLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<GeneralLog>(generalLog);
                    context.GeneralLog.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<GeneralLogDTO>(generalLog);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public SaveResult InsertOrUpdate(ref GeneralLogDTO GeneralLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long LogId = GeneralLog.LogId;
                    GeneralLog entity = context.GeneralLog.FirstOrDefault(c => c.LogId.Equals(LogId));

                    if (entity == null)
                    {
                        GeneralLog = insert(GeneralLog, context);
                        return SaveResult.Inserted;
                    }
                    GeneralLog = update(entity, GeneralLog, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_GeneralLog_ERROR"), GeneralLog.LogId, e.Message), e);
                return SaveResult.Error;
            }
        }

        private GeneralLogDTO insert(GeneralLogDTO GeneralLog, OpenNosContext context)
        {
            var entity = _mapper.Map<GeneralLog>(GeneralLog);
            context.GeneralLog.Add(entity);
            context.SaveChanges();
            return _mapper.Map<GeneralLogDTO>(entity);
        }

        private GeneralLogDTO update(GeneralLog entity, GeneralLogDTO GeneralLog, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(GeneralLog, entity);
                context.SaveChanges();
            }

            return _mapper.Map<GeneralLogDTO>(entity);
        }
        
        public IEnumerable<GeneralLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (GeneralLog generalLog in context.GeneralLog)
                {
                    yield return _mapper.Map<GeneralLogDTO>(generalLog);
                }
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByIp(string ip)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                string cleanIp = ip.Replace("tcp://", "");
                cleanIp = cleanIp.Substring(0, cleanIp.LastIndexOf(":") > 0 ? cleanIp.LastIndexOf(":") : cleanIp.Length);
                foreach (GeneralLog log in context.GeneralLog.Where(s => s.IpAddress.Contains(cleanIp)))
                {
                    yield return _mapper.Map<GeneralLogDTO>(log);
                }
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByAccount(long? accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (GeneralLog generalLog in context.GeneralLog.Where(s => s.AccountId == accountId))
                {
                    yield return _mapper.Map<GeneralLogDTO>(generalLog);
                }
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByLogType(string logType, long? characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (GeneralLog log in context.GeneralLog.Where(c => c.LogType.Equals(logType) && c.CharacterId == characterId))
                {
                    yield return _mapper.Map<GeneralLogDTO>(log);
                }
            }
        }

        public void SetCharIdNull(long? characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (GeneralLog log in context.GeneralLog.Where(c => c.CharacterId == characterId))
                    {
                        log.CharacterId = null;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public void WriteGeneralLog(long LogId, string ipAddress, long? characterId, string logType, string logData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    GeneralLog log = new GeneralLog
                    {
                        LogId = LogId,
                        IpAddress = ipAddress,
                        Timestamp = DateTime.Now,
                        LogType = logType,
                        LogData = logData,
                        CharacterId = characterId
                    };

                    context.GeneralLog.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        #endregion
    }
}