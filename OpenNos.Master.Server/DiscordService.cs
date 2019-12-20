using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;


namespace OpenNos.Master.Server
{
    internal class DiscordService : ScsService, IDiscordService
    {
        public bool Authenticate(string authKey)
        {
            if (string.IsNullOrWhiteSpace(authKey))
            {
                return false;
            }

            if (authKey == ConfigurationManager.AppSettings["MasterAuthKey"])
            {
                MSManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }
    }
}
