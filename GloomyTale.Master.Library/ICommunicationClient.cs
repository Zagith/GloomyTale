﻿using OpenNos.Data;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Communication
{
    /// <summary>
    ///     Master to WorldServer
    /// </summary>
    public interface ICommunicationClient
    {
        void CharacterConnected(long characterId);

        void CharacterDisconnected(long characterId);

        void UpdateFamily(long familyId, bool changeFaction);

        void SendMessageToCharacter(SCSCharacterMessage message);

        void Shutdown();

        void UpdateBazaar(long bazaarItemId);
        void UpdatePenaltyLog(int penaltyLogId);

        void UpdateRelation(long relationId);

        void KickSession(long? accountId, long? sessionId);

        void SendMail(MailDTO mail);

        void ChangeAuthority(long accountId, AuthorityType authority);
    }
}
