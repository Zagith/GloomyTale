﻿/*
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

using GloomyTale.Data;
using System.Collections.Generic;
using System.Linq;
using GloomyTale.GameObject.Networking;
using GloomyTale.Domain;

namespace GloomyTale.GameObject
{
    public class Account : AccountDTO
    {
        public Account(AccountDTO input)
        {
            AccountId = input.AccountId;
            Authority = input.Authority;
            Email = input.Email;
            Name = input.Name;
            Password = input.Password;
            ReferrerId = input.ReferrerId;
            RegistrationIP = input.RegistrationIP;
            VerificationToken = input.VerificationToken;
            Language = input.Language;
        }

        #region Properties

        public bool IsLimited => Authority >= AuthorityType.TMOD && Authority != AuthorityType.Administrator && Authority != AuthorityType.DEV;//permited by Pavvx

        public List<PenaltyLogDTO> PenaltyLogs
        {
            get
            {
                PenaltyLogDTO[] logs = new PenaltyLogDTO[ServerManager.Instance.PenaltyLogs.Count + 10];
                ServerManager.Instance.PenaltyLogs.CopyTo(logs);
                return logs.Where(s => s != null && s.AccountId == AccountId).ToList();
            }
        }

        #endregion
    }
}