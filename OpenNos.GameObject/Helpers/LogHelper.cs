using System;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.DAL;
using OpenNos.GameObject.Networking;
using System.Collections.Generic;

namespace OpenNos.GameObject.Helpers
{
    public class LogHelper
    {
        public LogHelper()
        {
            QuestLogList = new List<QuestLogDTO>();
            RaidLogList = new List<RaidLogDTO>();
            LogCommandsList = new List<LogCommandsDTO>();
            ChatLogList = new List<LogChatDTO>();
        }

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
            DAOFactory.LogCommandsDAO.InsertOrUpdate(ref command);
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