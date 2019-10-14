using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.DAO
{
    class LogCommandsDAO
    {
        #region Methods


        public SaveResult InsertOrUpdate(ref LogCommandsDTO mail)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long mailId = mail.CommandId;
                    LogCommands entity = context.LogCommands.FirstOrDefault(c => c.CommandId.Equals(mailId));

                    if (entity == null)
                    {
                        mail = insert(mail, context);
                        return SaveResult.Inserted;
                    }

                    mail.CommandId = entity.CommandId;
                    mail = update(entity, mail, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<LogCommandsDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<LogCommandsDTO> result = new List<LogCommandsDTO>();
                foreach (LogCommands mail in context.LogCommands)
                {
                    LogCommandsDTO dto = new LogCommandsDTO();
                    Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(mail, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public LogCommandsDTO LoadById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogCommandsDTO dto = new LogCommandsDTO();
                    if (Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(context.LogCommands.FirstOrDefault(i => i.CommandId.Equals(mailId)), dto))
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

        public IEnumerable<MailDTO> LoadSentByCharacter(long characterId)
        {
            //Where(s => s.SenderId == CharacterId && s.IsSenderCopy && MailList.All(m => m.Value.MailId != s.MailId))
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MailDTO> result = new List<MailDTO>();
                foreach (Mail mail in context.Mail.Where(s => s.SenderId == characterId && s.IsSenderCopy).Take(40))
                {
                    MailDTO dto = new MailDTO();
                    Mapper.Mappers.MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<MailDTO> LoadSentToCharacter(long characterId)
        {
            //s => s.ReceiverId == CharacterId && !s.IsSenderCopy && MailList.All(m => m.Value.MailId != s.MailId)).Take(50)
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MailDTO> result = new List<MailDTO>();
                foreach (Mail mail in context.Mail.Where(s => s.ReceiverId == characterId && !s.IsSenderCopy).Take(40))
                {
                    MailDTO dto = new MailDTO();
                    Mapper.Mappers.MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private static LogCommandsDTO insert(LogCommandsDTO mail, OpenNosContext context)
        {
            try
            {
                LogCommands entity = new LogCommands();
                Mapper.Mappers.LogCommandsMapper.ToLogCommands(mail, entity);
                context.LogCommands.Add(entity);
                context.SaveChanges();
                if (Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(entity, mail))
                {
                    return mail;
                }

                return null;
            }
            catch (DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                    {
                        // raise a new exception nesting the current instance as InnerException
                        Logger.Error(new InvalidOperationException($"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}", raise));
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static LogCommandsDTO update(LogCommands entity, LogCommandsDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.LogCommandsMapper.ToLogCommands(respawn, entity);
                context.SaveChanges();
            }
            if (Mapper.Mappers.LogCommandsMapper.ToLogCommandsDTO(entity, respawn))
            {
                return respawn;
            }

            return null;
        }

        #endregion
    }
}
