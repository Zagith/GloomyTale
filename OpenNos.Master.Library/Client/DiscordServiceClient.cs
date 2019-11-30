using OpenNos.Core;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Client;
using System;
using System.Configuration;

namespace OpenNos.Master.Library.Client
{
    public class DiscordServiceClient : IDiscordService
    {
        #region Members

        private static DiscordServiceClient _instance;

        private readonly IScsServiceClient<IDiscordService> _client;

        #endregion

        #region Instantiation

        public DiscordServiceClient()
        {
            string ip = ConfigurationManager.AppSettings["MasterIP"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["MasterPort"]);
            _client = ScsServiceClientBuilder.CreateClient<IDiscordService>(new ScsTcpEndPoint(ip, port));
            System.Threading.Thread.Sleep(5000);
            while (_client.CommunicationState != CommunicationStates.Connected)
            {
                try
                {
                    _client.Connect();
                }
                catch
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"), memberName: "DiscordServiceClient");
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Properties

        public static DiscordServiceClient Instance => _instance ?? (_instance = new DiscordServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey) => _client.ServiceProxy.Authenticate(authKey);

        //public AccountDTO ValidateAccount(string userName, string passHash) => _client.ServiceProxy.ValidateAccount(userName, passHash);

        //public void GetAct4Stat(int angel, int demon) => _client.ServiceProxy.GetAct4Stat(angel, angel);

        //public void SendItem(long characterId, MallItem item) => _client.ServiceProxy.SendItem(characterId, item);

        //public void SendStaticBonus(long characterId, MallStaticBonus item) => _client.ServiceProxy.SendStaticBonus(characterId, item);

        #endregion
    }
}
