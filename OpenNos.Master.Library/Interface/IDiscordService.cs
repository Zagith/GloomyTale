﻿using OpenNos.SCS.Communication.ScsServices.Service;

namespace OpenNos.Master.Library.Interface
{
    [ScsService(Version = "1.1.0.0")]
    public interface IDiscordService
    {
        /// <summary>
        /// Authenticates a Client to the Service
        /// </summary>
        /// <param name="authKey">The private Authentication key</param>
        /// <returns>true if successful, else false</returns>
        bool Authenticate(string authKey);

        //void GetAct4Stat(int angel, int demon);
    }
}