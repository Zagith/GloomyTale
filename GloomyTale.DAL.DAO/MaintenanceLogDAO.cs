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
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class MaintenanceLogDAO : MappingBaseDao<MaintenanceLog, MaintenanceLogDTO>, IMaintenanceLogDAO
    {
        public MaintenanceLogDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public MaintenanceLogDTO Insert(MaintenanceLogDTO maintenanceLog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MaintenanceLog>(maintenanceLog);
                    context.MaintenanceLog.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MaintenanceLogDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MaintenanceLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MaintenanceLog maintenanceLog in context.MaintenanceLog)
                {
                    yield return _mapper.Map<MaintenanceLogDTO>(maintenanceLog);
                }
            }
        }

        public MaintenanceLogDTO LoadFirst()
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MaintenanceLog log = context.MaintenanceLog.FirstOrDefault(m => m.DateEnd > DateTime.Now);
                    if (log != null)
                    {
                        return _mapper.Map<MaintenanceLogDTO>(log);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);                
            }
            return null;
        }

        #endregion
    }
}