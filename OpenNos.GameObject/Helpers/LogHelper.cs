﻿using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using System;

namespace OpenNos.GameObject.Helpers
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
            DAOFactory.QuestLogDAO.InsertOrUpdate(ref log);
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
            DAOFactory.LogCommandsDAO.Insert(command);
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
            DAOFactory.LogChatDAO.Insert(log);
        }

        public void InsertDropLog(ItemInstance inv, ClientSession character, short amount, string ipAddress)
        {
            var log = new LogDropDTO
            {
                CharacterId = character.Character.CharacterId,
                ItemVNum = inv.ItemVNum,
                ItemName = inv.Item.Name[character.Account.Language],
                Amount = amount,
                Map = character.Character.MapId,
                X = character.Character.MapX,
                Y = character.Character.MapY,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DAOFactory.LogDropDAO.Insert(log);
        }

        public void InsertPutItemLog(MapItem inv, ClientSession character, string ipAddress)
        {
            var log = new LogPutItemDTO
            {
                CharacterId = character.Character.CharacterId,
                ItemVNum = inv.ItemVNum,
                Amount = inv.Amount,
                Map = character.Character.MapId,
                X = character.Character.MapX,
                Y = character.Character.MapY,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DAOFactory.LogPutItemDAO.Insert(log);
        }

        public void InsertTimeSpacesLog(long characterId, long timespaceId, DateTime time)
        {
            var log = new TimeSpacesLogDTO
            {
                CharacterId = characterId,
                TimeSpaceId = timespaceId,
                Timestamp = time
            };
            DAOFactory.TimeSpaceLogDAO.Insert(log);
        }

        public void InsertPVPLog(long characterId, long targetId, string ipAddress)
        {
            var log = new PvPLogDTO
            {
                CharacterId = characterId,
                TargetId = targetId,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DAOFactory.PvPLogDAO.Insert(log);
        }

        public void InsertRaidLog(long characterId, long timespaceId, DateTime time)
        {
            var log = new RaidLogDTO
            {
                CharacterId = characterId,
                RaidId = timespaceId,
                Timestamp = time
            };
            DAOFactory.RaidLogDAO.Insert(log);
        }

        public void InsertLevelLog(long characterId, byte level, DateTime time)
        {
            var log = new LevelLogDTO
            {
                CharacterId = characterId,
                Level = level,
                Timestamp = time
            };
            DAOFactory.LevelLogDAO.Insert(log);
        }

        public void InsertUpgradeLog(long characterId, short upgrade, Guid serialId, DateTime time)
        {
            var log = new UpgradeLogDTO
            {
                CharacterId = characterId,
                Upgrade = upgrade,
                EquipmentSerialized = serialId,
                Timestamp = time
            };
            DAOFactory.UpgradeLogDAO.Insert(log);
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