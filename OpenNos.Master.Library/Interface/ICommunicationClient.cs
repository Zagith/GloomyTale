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

using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;

namespace OpenNos.Master.Library.Interface
{
    public interface ICommunicationClient
    {
        #region Methods

        void CharacterConnected(long characterId);

        void CharacterDisconnected(long characterId);

        void KickSession(long? accountId, int? sessionId);

        void RunGlobalEvent(Domain.EventType eventType);

        void SendMessageToCharacter(SCSCharacterMessage message);

        void Shutdown();

        void Restart(int time = 5);

        void UpdateBazaar(long bazaarItemId);

        void UpdateFamily(long familyId, bool changeFaction);

        void UpdatePenaltyLog(int penaltyLogId);

        void UpdateRelation(long relationId);

        void UpdateStaticBonus(long characterId);

        void SendMail(MailDTO mail);

        void UpdateCharacterAchievement(long characterId, long achievementId);

        void ChangeAuthority(long accountAccountId, AuthorityType authority);

        #endregion
    }
}