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

using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject
{
    public class Account : AccountDTO
    {

        #region Members

        public bool hasVerifiedSecondPassword;

        #endregion

        public Account(AccountDTO input)
        {
            AccountId = input.AccountId;
            Authority = input.Authority;
            Email = input.Email;
            Name = input.Name;
            Password = input.Password;
            ReferrerId = input.ReferrerId;
            ReferToken = input.ReferToken;
            RegistrationIP = input.RegistrationIP;
            VerificationToken = input.VerificationToken;
            Language = input.Language;
            TotpSecret = input.TotpSecret;
        }

        #region Properties

        public List<PenaltyLogDTO> PenaltyLogs
        {
            get
            {
                PenaltyLogDTO[] logs = new PenaltyLogDTO[ServerManager.Instance.PenaltyLogs.Count + 10];
                ServerManager.Instance.PenaltyLogs.CopyTo(logs);
                return logs.Where(s => s != null && s.AccountId == AccountId).ToList();
            }
        }

        public void UnlockAccount(ClientSession Session)
        {
            Session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 11, Session.Account.AccountId, 2));
            Observable.Timer(TimeSpan.FromSeconds(60)).Subscribe(o =>
            {
                if (Session.Account.hasVerifiedSecondPassword == false)
                {
                    Session.Disconnect();
                }
            });
        }
        #endregion
    }
}