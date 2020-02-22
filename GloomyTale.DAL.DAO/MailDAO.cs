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
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class MailDAO : MappingBaseDao<Mail, MailDTO>, IMailDAO
    {
        public MailDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult DeleteById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Mail mail = context.Mail.First(i => i.MailId.Equals(mailId));

                    if (mail != null)
                    {
                        context.Mail.Remove(mail);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MailDTO mail)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long mailId = mail.MailId;
                    Mail entity = context.Mail.FirstOrDefault(c => c.MailId.Equals(mailId));

                    if (entity == null)
                    {
                        mail = insert(mail, context);
                        return SaveResult.Inserted;
                    }

                    mail.MailId = entity.MailId;
                    mail = update(entity, mail, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MailDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Mail mail in context.Mail)
                {
                    yield return _mapper.Map<MailDTO>(mail);
                }
            }
        }

        public MailDTO LoadById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MailDTO>(context.Mail.FirstOrDefault(i => i.MailId.Equals(mailId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<MailDTO> LoadSentByCharacter(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Mail mail in context.Mail.Where(s => s.SenderId == characterId && s.IsSenderCopy).Take(40))
                {
                    yield return _mapper.Map<MailDTO>(mail);
                }
            }
        }

        public IEnumerable<MailDTO> LoadSentToCharacter(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Mail mail in context.Mail.Where(s => s.ReceiverId == characterId && !s.IsSenderCopy).Take(40))
                {
                    yield return _mapper.Map<MailDTO>(mail);
                }
            }
        }

        private MailDTO insert(MailDTO mail, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<Mail>(mail);
                context.Mail.Add(entity);
                context.SaveChanges();
                return _mapper.Map<MailDTO>(entity);
            }
            catch (Exception dbEx)
            {
                Logger.Log.Error(dbEx);
                return null;
            }
        }

        private MailDTO update(Mail entity, MailDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawn, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MailDTO>(entity);
        }

        #endregion
    }
}