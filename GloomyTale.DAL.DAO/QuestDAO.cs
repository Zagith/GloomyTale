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
using AutoMapper;
using System.Linq;

namespace GloomyTale.DAL.DAO
{
    public class QuestDAO : MappingBaseDao<Quest, QuestDTO>, IQuestDAO
    {
        public QuestDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Quest deleteEntity = context.Quest.Find(id);
                    if (deleteEntity != null)
                    {
                        context.Quest.Remove(deleteEntity);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), id, e.Message), e);
                return DeleteResult.Error;
            }
        }        

        public void Insert(List<QuestDTO> questList)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    foreach (QuestDTO quest in questList)
                    {
                        var entity = _mapper.Map<Quest>(quest);
                        context.Quest.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public IEnumerable<QuestDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Quest.ToList().Select(d => _mapper.Map<QuestDTO>(d)).ToList();
            }
        }

        public QuestDTO LoadById(long id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return _mapper.Map<QuestDTO>(context.Quest.FirstOrDefault(s => s.QuestId.Equals(id)));
            }
        }

        #endregion
    }
}