﻿using Newtonsoft.Json;
using OpenNos.Domain;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Server.Controllers.ControllersParameters;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace OpenNos.Master.Server.controllers
{
    public class AuthorityController : ApiController
    {
        // POST /Authority 
        [AuthorizeRole(AuthorityType.Administrator)]
        public bool Post([FromBody] ChangeAuthorityParameter authorityParameter) =>
            CommunicationServiceClient.Instance.ChangeAuthority(authorityParameter.WorldGroup, authorityParameter.CharacterName, (AuthorityType)authorityParameter.Authority);

        public string Get()
        {
            Dictionary<string, int> authorities = new Dictionary<string, int>();
            foreach (object i in Enum.GetValues(typeof(AuthorityType)))
            {
                if ((int)(AuthorityType)i <= (int)AuthorityType.EventMaster)
                {
                    authorities[i.ToString()] = (int)(AuthorityType)i;
                }
            }

            return JsonConvert.SerializeObject(authorities);
        }
    }
}
