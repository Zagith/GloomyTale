using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;
using System;
using System.Collections.Generic;
using System.Configuration;


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

        public void RefreshAct4Stat(int angel, int demon)
        {
            MSManager.Instance.Act4Stat = new Tuple<int,int>(angel,demon);
        }

        public Tuple<int, int> GetAct4Stat()
        {
            Tuple<int, int> stats = MSManager.Instance.Act4Stat;
            return stats;
        }
    }
}
