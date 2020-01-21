using GloomyTale.AdminTool.Shared.ChatLog;
using Hik.Communication.ScsServices.Service;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Domain.AdminTool;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GloomyTale.AdminTool.Server
{
    internal class AdminToolService : ScsService, IAdminToolService
    {
        public bool AuthenticateAdmin(string user, string passHash)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(passHash))
            {
                return false;
            }

            if (AuthentificationServiceClient.Instance.ValidateAccount(user, passHash) is AccountDTO account && account.Authority > AuthorityType.User)
            {
                AdminToolManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public bool Authenticate(string authKey)
        {
            if (string.IsNullOrWhiteSpace(authKey))
            {
                return false;
            }

            if (authKey == ConfigurationManager.AppSettings["AdminToolgKey"])
            {
                AdminToolManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public List<ChatLogEntry> GetChatLogEntries(string sender, long? senderid, string receiver, long? receiverid, string message, DateTime? start, DateTime? end, ChatLogType? logType)
        {
            Logger.Info($"Received Log Request - Sender: {sender} SenderId: {senderid} Receiver: {receiver} ReceiverId: {receiverid} Message: {message} DateStart: {start} DateEnd: {end} ChatLogType: {logType}");
            List<ChatLogEntry> tmp = AdminToolManager.Instance.AllChatLogs.GetAllItems();
            if (!string.IsNullOrWhiteSpace(sender))
            {
                tmp = tmp.Where(s => s.Sender.IndexOf(sender, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }
            if (senderid.HasValue)
            {
                tmp = tmp.Where(s => s.SenderId == senderid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(receiver))
            {
                tmp = tmp.Where(s => s.Receiver?.IndexOf(receiver, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }
            if (receiverid.HasValue)
            {
                tmp = tmp.Where(s => s.ReceiverId == receiverid).ToList();
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                tmp = tmp.Where(s => s.Message.IndexOf(message, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }
            if (start.HasValue)
            {
                tmp = tmp.Where(s => s.Timestamp >= start).ToList();
            }
            if (end.HasValue)
            {
                tmp = tmp.Where(s => s.Timestamp <= end).ToList();
            }
            if (logType.HasValue)
            {
                tmp = tmp.Where(s => s.MessageType == logType).ToList();
            }
            return tmp;
        }

        public void LogChatMessage(ChatLogEntry logEntry)
        {
            if (!AdminToolManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) || logEntry == null)
            {
                return;
            }

            logEntry.Timestamp = DateTime.Now;
            AdminToolManager.Instance.ChatLogs.Add(logEntry);
            AdminToolManager.Instance.AllChatLogs.Add(logEntry);
        }
    }
}