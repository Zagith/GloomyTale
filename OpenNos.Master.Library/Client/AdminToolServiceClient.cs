using Hik.Communication.ScsServices.Client;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.Master.Library.Interface;
using System;
using System.Collections.Generic;
using OpenNos.Core;
using GloomyTale.AdminTool.Shared.ChatLog;
using OpenNos.Domain.AdminTool;
using System.Configuration;

namespace OpenNos.Master.Library.Client
{
    public class AdminToolServiceClient : IAdminToolService
    {
        #region Members

        private static AdminToolServiceClient _instance;

        private readonly IScsServiceClient<IAdminToolService> _client;

        #endregion

        #region Instantiation

        public AdminToolServiceClient()
        {
            string ip = ConfigurationManager.AppSettings["AdminToolIP"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["AdminToolPort"]);
            _client = ScsServiceClientBuilder.CreateClient<IAdminToolService>(new ScsTcpEndPoint(ip, port));
            System.Threading.Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
            {
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"), memberName: nameof(AdminToolServiceClient));
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Properties

        public static AdminToolServiceClient Instance => _instance ?? (_instance = new AdminToolServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        //public void SendPacketToMap(Guid channelId, string packet, Guid mapInstanceId) => _client.ServiceProxy.SendPacketToMap(channelId, packet, mapInstanceId);

        //public void SendItemToMap(Guid channelId, string senderName, Guid mapInstanceId, short vnum, short amount = 1, byte rare = 0, byte Upgrade = 0, bool isNosmall = false) => _client.ServiceProxy.SendItemToMap(channelId, senderName, mapInstanceId, vnum, amount, rare, Upgrade, isNosmall);

        //public void SummonNPCMonster(Guid channelId, short npcMonsterVnum, short amount, Guid mapInstanceId, short mapX = 0, short mapY = 0) => _client.ServiceProxy.SummonNPCMonster(channelId, npcMonsterVnum, amount, mapInstanceId, mapX, mapY);

        public void LogChatMessage(ChatLogEntry logEntry) => _client.ServiceProxy.LogChatMessage(logEntry);

        public bool AuthenticateAdmin(string user, string passHash) => _client.ServiceProxy.AuthenticateAdmin(user, passHash);

        public List<ChatLogEntry> GetChatLogEntries(string sender, long? senderid, string receiver, long? receiverid, string message, DateTime? start, DateTime? end, ChatLogType? logType) => _client.ServiceProxy.GetChatLogEntries(sender, senderid, receiver, receiverid, message, start, end, logType);

        public bool Authenticate(string authKey) => _client.ServiceProxy.Authenticate(authKey);

        #endregion
    }
}
