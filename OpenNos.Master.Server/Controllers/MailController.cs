﻿using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Server.Controllers.ControllersParameters;
using System;
using System.Web.Http;

namespace OpenNos.Master.Server.Controllers
{
    public class MailController : ApiController
    {
        // POST /mail 
        public void Post([FromBody] MailPostParameter mail)
        {
            var mail2 = new MailDTO
            {
                AttachmentAmount = mail.Amount,
                IsOpened = false,
                Date = DateTime.Now,
                ReceiverId = mail.CharacterId,
                SenderId = mail.CharacterId,
                AttachmentRarity = (byte)mail.Rare,
                AttachmentUpgrade = mail.Upgrade,
                IsSenderCopy = false,
                Title = mail.IsNosmall ? "NOSMALL" : "ACHIEVEMENT",
                AttachmentVNum = mail.VNum
            };
            if (mail.IsAchievement && mail.AchievementId != null)
            {
                CommunicationServiceClient.Instance.UpdateCharacterAchievement(mail.WorldGroup, mail.CharacterId, mail.AchievementId.Value);
            }
            Logger.Log.Info($"[{(mail.IsNosmall ? "NOSMALL" : "ACHIEVEMENT")}] Receiver ID : {mail2.ReceiverId}");
            CommunicationServiceClient.Instance.SendMail(mail.WorldGroup, mail2);
        }
    }
}
