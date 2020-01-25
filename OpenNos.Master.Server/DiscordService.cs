using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
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

        public void SendItem(string characterName, DiscordItem item)
        {
            ItemDTO dto = DAOFactory.ItemDAO.LoadById(item.ItemVNum);
            if (dto != null)
            {
                CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(characterName);
                if (character != null)
                {
                    do
                    {
                        short limit = 99;

                        if (dto.Type == InventoryType.Equipment || dto.Type == InventoryType.Miniland)
                        {
                            limit = 1;
                        }
                        MailDTO mailDTO = new MailDTO
                        {
                            AttachmentAmount = (item.Amount > limit ? limit : item.Amount),
                            AttachmentRarity = 0,
                            AttachmentUpgrade = 0,
                            AttachmentVNum = item.ItemVNum,
                            Date = DateTime.Now,
                            EqPacket = string.Empty,
                            IsOpened = false,
                            IsSenderCopy = false,
                            Message = string.Empty,
                            ReceiverId = character.CharacterId,
                            SenderId = character.CharacterId,
                            Title = "Discord bot"
                        };

                        DAOFactory.MailDAO.InsertOrUpdate(ref mailDTO);

                        AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(mailDTO.ReceiverId));
                        if (account?.ConnectedWorld != null)
                        {
                            account.ConnectedWorld.MailServiceClient.GetClientProxy<IMailClient>().MailSent(mailDTO);
                        }

                        item.Amount -= limit;
                    } while (item.Amount > 0);
                }
            }
        }

        public void RestartAll()
        {
            string worldGroup = "*";

            CommunicationServiceClient.Instance.Restart(worldGroup, 1);
        }

        public void Home(string characterName)
        {
            long id =DAOFactory.CharacterDAO.LoadByName(characterName).CharacterId;
            ServerManager.Instance.ChangeMap(id, 129, 127, 73);
        }
    }
}
