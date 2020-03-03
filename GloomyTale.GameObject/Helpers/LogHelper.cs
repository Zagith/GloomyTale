using System;
using GloomyTale.Core;
using GloomyTale.Data;
using GloomyTale.DAL;
using GloomyTale.GameObject.Networking;
using System.Collections.Generic;
using GloomyTale.Domain;
using GloomyTale.GameObject.Items.Instance;

namespace GloomyTale.GameObject.Helpers
{
    public class LogHelper
    {
        public void InsertQuestLog(long characterId, string ipAddress, long questId, DateTime lastDaily)
        {
            var log = new QuestLogDTO
            {
                CharacterId = characterId,
                IpAddress = ipAddress,
                QuestId = questId,
                LastDaily = lastDaily
            };
            DAOFactory.Instance.QuestLogDAO.InsertOrUpdate(ref log);
        }

        public void InsertCommandLog(long characterId, PacketDefinition commandPacket, string ipAddress)
        {
            string withoutHeaderpacket = string.Empty;
            string[] packet = commandPacket.OriginalContent.Split(' ');
            for (int i = 1; i < packet.Length; i++)
            {
                withoutHeaderpacket += $" {packet[i]}";
            }

            var command = new LogCommandsDTO
            {
                CharacterId = characterId,
                Command = commandPacket.OriginalHeader,
                Data = withoutHeaderpacket,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DAOFactory.Instance.LogCommandsDAO.Insert(command);
        }

        public void InsertChatLog(ChatType type, long characterId, string message, string ipAddress)
        {
            var log = new LogChatDTO
            {
                CharacterId = characterId,
                ChatMessage = message,
                IpAddress = ipAddress,
                ChatType = type,
                Timestamp = DateTime.Now
            };
            DAOFactory.Instance.LogChatDAO.Insert(log);
        }

        public void InsertDropLog(ItemInstance inv, ClientSession character, short amount, string ipAddress)
        {
            var log = new LogDropDTO
            {
                CharacterId = character.Character.VisualId,
                ItemVNum = inv.ItemVNum,
                ItemName = inv.Item.Name,
                Amount = amount,
                Map = character.Character.MapId,
                X = character.Character.MapX,
                Y = character.Character.MapY,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DAOFactory.Instance.LogDropDAO.Insert(log);
        }

        public void InsertPutItemLog(MapItem inv, ClientSession character, string ipAddress)
        {
            var log = new LogPutItemDTO
            {
                CharacterId = character.Character.VisualId,
                ItemVNum = inv.ItemVNum,
                Amount = inv.Amount,
                Map = character.Character.MapId,
                X = character.Character.MapX,
                Y = character.Character.MapY,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DAOFactory.Instance.LogPutItemDAO.Insert(log);
        }

        public void InsertTimeSpacesLog(long characterId, long timespaceId, DateTime time)
        {
            var log = new TimeSpacesLogDTO
            {
                CharacterId = characterId,
                TimeSpaceId = timespaceId,
                Timestamp = time
            };
            DAOFactory.Instance.TimeSpaceLogDAO.Insert(log);
        }

        #region Singleton

        private static LogHelper _instance;

        public static LogHelper Instance
        {
            get { return _instance ?? (_instance = new LogHelper()); }
        }

        #endregion
    }
}