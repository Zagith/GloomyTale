using Newtonsoft.Json;
using OpenNos.Core;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace OpenNos.Master.Server.Controllers
{
    public class StatController : ApiController
    {
        // GET /stat
        public Dictionary<int, List<AccountConnection.CharacterSession>> Get()
        {
            try
            {
                string tmp = CommunicationServiceClient.Instance.RetrieveServerStatistic();
                Dictionary<int, List<AccountConnection.CharacterSession>> newDictionary = JsonConvert.DeserializeObject<Dictionary<int, List<AccountConnection.CharacterSession>>>(tmp);

                return newDictionary;
            }
            catch (Exception e)
            {
                Logger.Log.Error("[WEBAPI]", e);
                return new Dictionary<int, List<AccountConnection.CharacterSession>>();
            }
        }
    }
}
