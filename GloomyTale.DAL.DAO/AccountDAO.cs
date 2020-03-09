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

using Microsoft.EntityFrameworkCore;
using GloomyTale.Core;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using GloomyTale.Data.Enums;
using GloomyTale.Domain;
using System;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class AccountDAO : MappingBaseDao<Account, AccountDTO>, IAccountDAO
    {
        public AccountDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public DeleteResult Delete(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(c => c.AccountId.Equals(accountId));

                    if (account != null)
                    {
                        context.Account.Remove(account);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ACCOUNT_ERROR"), accountId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref AccountDTO account)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long accountId = account.AccountId;
                    Account entity = context.Account.FirstOrDefault(c => c.AccountId.Equals(accountId));

                    if (entity == null)
                    {
                        account = insert(account, context);
                        return SaveResult.Inserted;
                    }
                    account = update(entity, account, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_ACCOUNT_ERROR"), account.AccountId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public AccountDTO LoadById(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.AccountId.Equals(accountId));
                    if (account != null)
                    {
                        return _mapper.Map<AccountDTO>(account);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public AccountDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.Name.Equals(name));
                    if (account != null)
                    {
                        return _mapper.Map<AccountDTO>(account);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return null;
        }

        public AccountDTO LoadByRefToken(string token)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.ReferToken.Equals(token));
                    if (account != null)
                    {
                        return _mapper.Map<AccountDTO>(account);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        public void WriteGeneralLog(long accountId, string ipAddress, long? characterId, GeneralLogType logType, string logData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    GeneralLog log = new GeneralLog
                    {
                        AccountId = accountId,
                        IpAddress = ipAddress,
                        Timestamp = DateTime.Now,
                        LogType = logType.ToString(),
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

        private AccountDTO insert(AccountDTO account, OpenNosContext context)
        {
            var entity = _mapper.Map<Account>(account);
            context.Account.Add(entity);
            context.SaveChanges();
            return _mapper.Map<AccountDTO>(entity);
        }

        private AccountDTO update(Account entity, AccountDTO account, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            // The _mapper breaks context.SaveChanges(), so we need to "map" the data by hand...
            // entity = _mapper.Map<Account>(account);
            entity.Authority = account.Authority;
            entity.Name = account.Name;
            entity.Password = account.Password;
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();

            return _mapper.Map<AccountDTO>(entity);
        }

        #endregion
    }
}