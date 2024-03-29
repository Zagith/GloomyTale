﻿/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using Newtonsoft.Json;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Data.Achievements;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace OpenNos.Master.Server
{
    internal class CommunicationService : ScsService, ICommunicationService
    {
        #region Methods

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

        public void Cleanup()
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            MSManager.Instance.ConnectedAccounts.Clear();
            MSManager.Instance.WorldServers.Clear();
        }

        public void CleanupOutdatedSession()
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            AccountConnection[] tmp = new AccountConnection[MSManager.Instance.ConnectedAccounts.Count + 20];
            lock (MSManager.Instance.ConnectedAccounts)
            {
                MSManager.Instance.ConnectedAccounts.CopyTo(tmp);
            }
            foreach (AccountConnection account in tmp.Where(a => a?.LastPulse.AddMinutes(5) <= DateTime.Now))
            {
                KickSession(account.AccountId, null);
            }
        }

        public bool ChangeAuthority(string worldGroup, string characterName, AuthorityType authority)
        {
            CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(characterName);
            if (character == null)
            {
                return false;
            }

            if (!IsAccountConnected(character.AccountId))
            {
                AccountDTO account = DAOFactory.AccountDAO.LoadById(character.AccountId);
                account.Authority = authority;
                DAOFactory.AccountDAO.InsertOrUpdate(ref account);
            }
            else
            {
                AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(s => s.AccountId == character.AccountId);
                account?.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().ChangeAuthority(account.AccountId, authority);
            }

            return true;
        }

        public bool ConnectAccount(Guid worldId, long accountId, int sessionId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }

            AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.AccountId.Equals(accountId) && a.SessionId.Equals(sessionId));
            if (account != null)
            {
                account.ConnectedWorld = MSManager.Instance.WorldServers.Find(w => w.Id.Equals(worldId));
            }
            return account.ConnectedWorld != null;
        }

        public bool ConnectAccountCrossServer(Guid worldId, long accountId, int sessionId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }
            AccountConnection account = MSManager.Instance.ConnectedAccounts.Where(a => a.AccountId.Equals(accountId) && a.SessionId.Equals(sessionId)).FirstOrDefault();
            if (account != null)
            {
                account.CanLoginCrossServer = false;
                account.OriginWorld = account.ConnectedWorld;
                account.ConnectedWorld = MSManager.Instance.WorldServers.Find(s => s.Id.Equals(worldId));
                if (account.ConnectedWorld != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ConnectCharacter(Guid worldId, long characterId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }

            //Multiple WorldGroups not yet supported by DAOFactory
            long accountId = DAOFactory.CharacterDAO.LoadById(characterId)?.AccountId ?? 0;

            AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.AccountId.Equals(accountId) && a.ConnectedWorld.Id.Equals(worldId));
            if (account != null)
            {
                account.CharacterId = characterId;
                foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(account.ConnectedWorld.WorldGroup)))
                {
                    world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().CharacterConnected(characterId);
                }
                return true;
            }
            return false;
        }

        public void DisconnectAccount(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }
            if (MSManager.Instance.ConnectedAccounts.Any(s => s.AccountId.Equals(accountId) && s.CanLoginCrossServer))
            {
            }
            else
            {
                MSManager.Instance.ConnectedAccounts.RemoveAll(c => c.AccountId.Equals(accountId));
            }
        }

        public void DisconnectCharacter(Guid worldId, long characterId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (AccountConnection account in MSManager.Instance.ConnectedAccounts.Where(c => c.CharacterId.Equals(characterId) && c.ConnectedWorld.Id.Equals(worldId)))
            {
                foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(account.ConnectedWorld.WorldGroup)))
                {
                    world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().CharacterDisconnected(characterId);
                }
                if (!account.CanLoginCrossServer)
                {
                    account.CharacterId = 0;
                    account.ConnectedWorld = null;
                }
            }
        }

        public int? GetChannelIdByWorldId(Guid worldId) => MSManager.Instance.WorldServers.Find(w => w.Id == worldId)?.ChannelId;

        public long[][] GetOnlineCharacters()
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            List<AccountConnection> connections = MSManager.Instance.ConnectedAccounts.Where(s => s.CharacterId != 0);

            long[][] result = new long[connections.Count][];

            int i = 0;
            foreach (AccountConnection acc in connections)
            {
                result[i] = new long[3];
                result[i][0] = acc.CharacterId;
                result[i][1] = acc.ConnectedWorld?.ChannelId ?? 0;
                result[i][2] = acc.SessionId;
                i++;
            }
            return result;
        }

        public bool IsAccountConnected(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }

            return MSManager.Instance.ConnectedAccounts.Any(c => c.AccountId == accountId && c.ConnectedWorld != null && c.LastPulse.AddSeconds(90) >= DateTime.Now);
        }

        public bool IsAct4Online(string worldGroup) => MSManager.Instance.WorldServers.Any(w => w.WorldGroup.Equals(worldGroup)
            && w.Endpoint.IpAddress == MSManager.Instance.ConfigurationObject.Act4IP && w.Endpoint.TcpPort == MSManager.Instance.ConfigurationObject.Act4Port);

        public bool IsCharacterConnected(string worldGroup, long characterId)
        {
           /* if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }*/

            return MSManager.Instance.ConnectedAccounts.Any(c => c.ConnectedWorld != null && c.ConnectedWorld.WorldGroup == worldGroup && c.CharacterId == characterId);
        }

        public bool IsCrossServerLoginPermitted(long accountId, int sessionId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }

            return MSManager.Instance.ConnectedAccounts.Any(s => s.AccountId.Equals(accountId) && s.SessionId.Equals(sessionId) && s.CanLoginCrossServer);
        }

        public bool IsLoginPermitted(long accountId, int sessionId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return false;
            }

            return MSManager.Instance.ConnectedAccounts.Any(s => s.AccountId.Equals(accountId) && s.SessionId.Equals(sessionId) && s.ConnectedWorld == null);
        }

        public void KickSession(long? accountId, int? sessionId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (WorldServer world in MSManager.Instance.WorldServers)
            {
                world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().KickSession(accountId, sessionId);
            }
            if (accountId.HasValue)
            {
                MSManager.Instance.ConnectedAccounts.RemoveAll(s => s.AccountId.Equals(accountId.Value));
            }
            else if (sessionId.HasValue)
            {
                MSManager.Instance.ConnectedAccounts.RemoveAll(s => s.SessionId.Equals(sessionId.Value));
            }
        }

        public void PulseAccount(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }
            AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.AccountId.Equals(accountId));
            if (account != null)
            {
                account.LastPulse = DateTime.Now;
            }
        }

        public void RefreshPenalty(int penaltyId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (WorldServer world in MSManager.Instance.WorldServers)
            {
                world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().UpdatePenaltyLog(penaltyId);
            }
            foreach (IScsServiceClient login in MSManager.Instance.LoginServers)
            {
                login.GetClientProxy<ICommunicationClient>().UpdatePenaltyLog(penaltyId);
            }
        }

        public void RegisterAccountLogin(long accountId, int sessionId, string ipAddress)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }
            MSManager.Instance.ConnectedAccounts.RemoveAll(a => a.AccountId.Equals(accountId));
            MSManager.Instance.ConnectedAccounts.Add(new AccountConnection(accountId, sessionId, ipAddress));
        }

        public void RegisterCrossServerAccountLogin(long accountId, int sessionId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }
            AccountConnection account = MSManager.Instance.ConnectedAccounts.Where(a => a.AccountId.Equals(accountId) && a.SessionId.Equals(sessionId)).FirstOrDefault();

            if (account != null)
            {
                account.CanLoginCrossServer = true;
            }
        }

        public int? RegisterWorldServer(SerializableWorldServer worldServer)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }
            WorldServer ws = new WorldServer(worldServer.Id, new ScsTcpEndPoint(worldServer.EndPointIP, worldServer.EndPointPort), worldServer.AccountLimit, worldServer.WorldGroup)
            {
                CommunicationServiceClient = CurrentClient,
                ChannelId = Enumerable.Range(1, 30).Except(MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(worldServer.WorldGroup)).OrderBy(w => w.ChannelId).Select(w => w.ChannelId)).First()
            };
            if (worldServer.EndPointPort == MSManager.Instance.ConfigurationObject.Act4Port)
            {
                ws.ChannelId = 51;
            }
            MSManager.Instance.WorldServers.Add(ws);
            return ws.ChannelId;
        }

        public void Restart(string worldGroup, int time = 5)
        {
            /*if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }*/

            if (worldGroup == "*")
            {
                foreach (WorldServer world in MSManager.Instance.WorldServers)
                {
                    world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().Restart(time);
                }
            }
            else
            {
                foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(worldGroup)))
                {
                    world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().Restart(time);
                }
            }
        }

        public long[][] RetrieveOnlineCharacters(long characterId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            List<AccountConnection> connections = MSManager.Instance.ConnectedAccounts.Where(s => s.IpAddress == MSManager.Instance.ConnectedAccounts.Find(f => f.CharacterId == characterId)?.IpAddress && s.CharacterId != 0);

            long[][] result = new long[connections.Count][];

            int i = 0;
            foreach (AccountConnection acc in connections)
            {
                result[i] = new long[2];
                result[i][0] = acc.CharacterId;
                result[i][1] = acc.ConnectedWorld?.ChannelId ?? 0;
                i++;
            }
            return result;
        }

        public string RetrieveOriginWorld(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(s => s.AccountId.Equals(accountId));
            if (account?.OriginWorld != null)
            {
                return $"{account.OriginWorld.Endpoint.IpAddress}:{account.OriginWorld.Endpoint.TcpPort}";
            }
            return null;
        }

        public string RetrieveWorld(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(s => s.AccountId.Equals(accountId));
            if (account?.ConnectedWorld != null)
            {
                return $"{account.ConnectedWorld.Endpoint.IpAddress}:{account.ConnectedWorld.Endpoint.TcpPort}";
            }
            return null;
        }

        public string RetrieveRegisteredWorldServers(string username, int sessionId, bool ignoreUserName = false)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            string lastGroup = "";
            byte worldCount = 0;
            /*
             * Should be:
             * "NsTeST", "", "countryId", "username", "2", "sessionId", "serverInfosList", "-1:-1:-1:10000.10000.1"
             */
            string channelPacket = "NsTeST" + " " + username + $" {sessionId} ";

            foreach (WorldServer world in MSManager.Instance.WorldServers.OrderBy(w => w.WorldGroup))
            {
                if (lastGroup != world.WorldGroup)
                {
                    worldCount++;
                }
                lastGroup = world.WorldGroup;

                int currentlyConnectedAccounts = MSManager.Instance.ConnectedAccounts.CountLinq(a => a.ConnectedWorld?.ChannelId == world.ChannelId);
                int channelcolor = (int)Math.Round(((double)currentlyConnectedAccounts / world.AccountLimit) * 20) + 1;

                if (world.ChannelId == 51)
                {
                    continue;
                }

                channelPacket += $"{world.Endpoint.IpAddress}:{world.Endpoint.TcpPort}:{channelcolor}:{worldCount}.{world.ChannelId}.{world.WorldGroup} ";
            }
            return channelPacket;
        }

        public string RetrieveServerStatistic(bool onlinePlayers = false)
        {
            if (onlinePlayers)
            {
                return $"{MSManager.Instance.ConnectedAccounts.Count}";
            }

            Dictionary<int, List<AccountConnection.CharacterSession>> dictionary =
                MSManager.Instance.WorldServers.ToDictionary(world => world.ChannelId, world => new List<AccountConnection.CharacterSession>());

            foreach (IGrouping<int, AccountConnection> accountConnections in MSManager.Instance.ConnectedAccounts.Where(s => s.Character != null).GroupBy(s => s.ConnectedWorld.ChannelId))
            {
                foreach (AccountConnection i in accountConnections)
                {
                    dictionary[accountConnections.Key].Add(i.Character);
                }
            }

            return JsonConvert.SerializeObject(dictionary);

        }

        public IEnumerable<string> RetrieveServerStatistics()
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            List<string> result = new List<string>();

            try
            {
                List<string> groups = new List<string>();
                foreach (string s in MSManager.Instance.WorldServers.Select(s => s.WorldGroup))
                {
                    if (!groups.Contains(s))
                    {
                        groups.Add(s);
                    }
                }
                int totalsessions = 0;
                foreach (string message in groups)
                {
                    result.Add($"==={message}===");
                    int groupsessions = 0;
                    foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(message)))
                    {
                        int sessions = MSManager.Instance.ConnectedAccounts.CountLinq(a => a.ConnectedWorld?.Id.Equals(world.Id) == true);
                        result.Add($"Channel {world.ChannelId}: {sessions} Sessions");
                        groupsessions += sessions;
                    }
                    result.Add($"Group Total: {groupsessions} Sessions");
                    totalsessions += groupsessions;
                }
                result.Add($"Environment Total: {totalsessions} Sessions");
            }
            catch (Exception ex)
            {
                Logger.LogEventError("RETRIEVE_EXCEPTION", "Error while retreiving server Statistics:", ex);
            }

            return result;
        }

        public void RunGlobalEvent(EventType eventType)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (WorldServer world in MSManager.Instance.WorldServers)
            {
                world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().RunGlobalEvent(eventType);
            }
        }

        public int? SendMessageToCharacter(SCSCharacterMessage message)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return null;
            }

            WorldServer sourceWorld = MSManager.Instance.WorldServers.Find(s => s.Id.Equals(message.SourceWorldId));
            if (message == null || message.Message == null || sourceWorld == null)
            {
                return null;
            }

            switch (message.Type)
            {
                case MessageType.Family:
                case MessageType.FamilyChat:
                case MessageType.Broadcast:
                    foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(sourceWorld.WorldGroup)))
                    {
                        world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                    }
                    return -1;

                case MessageType.PrivateChat:
                    if (message.DestinationCharacterId.HasValue)
                    {
                        AccountConnection receiverAccount = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(message.DestinationCharacterId.Value));
                        if (receiverAccount?.ConnectedWorld != null)
                        {
                            if (sourceWorld.ChannelId == 51 && receiverAccount.ConnectedWorld.ChannelId == 51
                                && DAOFactory.CharacterDAO.LoadById(message.SourceCharacterId).Faction
                                != DAOFactory.CharacterDAO.LoadById((long)message.DestinationCharacterId).Faction)
                            {
                                AccountConnection SenderAccount = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(message.SourceCharacterId));
                                message.Message = $"talk {message.DestinationCharacterId} " + Language.Instance.GetMessageFromKey("CANT_TALK_OPPOSITE_FACTION");
                                message.DestinationCharacterId = message.SourceCharacterId;
                                SenderAccount.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                                return -1;
                            }
                            else
                            {
                                receiverAccount.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                                return receiverAccount.ConnectedWorld.ChannelId;
                            }
                        }
                    }
                    break;
                case MessageType.Whisper:
                    if (message.DestinationCharacterId.HasValue)
                    {
                        AccountConnection receiverAccount = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(message.DestinationCharacterId.Value));
                        if (receiverAccount?.ConnectedWorld != null)
                        {
                            if (sourceWorld.ChannelId == 51 && receiverAccount.ConnectedWorld.ChannelId == 51
                                && DAOFactory.CharacterDAO.LoadById(message.SourceCharacterId).Faction
                                != DAOFactory.CharacterDAO.LoadById((long)message.DestinationCharacterId).Faction)
                            {
                                AccountConnection SenderAccount = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(message.SourceCharacterId));
                                message.Message = $"say 1 {message.SourceCharacterId} 11 {Language.Instance.GetMessageFromKey("CANT_TALK_OPPOSITE_FACTION")}";
                                message.DestinationCharacterId = message.SourceCharacterId;
                                message.Type = MessageType.Other;
                                receiverAccount.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                                return -1;
                            }
                            else
                            {
                                receiverAccount.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                                return receiverAccount.ConnectedWorld.ChannelId;
                            }
                        }
                    }
                    break;
                case MessageType.WhisperSupport:
                case MessageType.WhisperGM:
                    if (message.DestinationCharacterId.HasValue)
                    {
                        AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(message.DestinationCharacterId.Value));
                        if (account?.ConnectedWorld != null)
                        {
                            account.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                            return account.ConnectedWorld.ChannelId;
                        }
                    }
                    break;

                case MessageType.Shout:
                    foreach (WorldServer world in MSManager.Instance.WorldServers)
                    {
                        world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                    }
                    return -1;

                case MessageType.Other:
                    AccountConnection receiverAcc = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(message.DestinationCharacterId.Value));
                    if (receiverAcc?.ConnectedWorld != null)
                    {
                        receiverAcc.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(message);
                        return receiverAcc.ConnectedWorld.ChannelId;
                    }
                    break;
            }
            return null;
        }

        public void Shutdown(string worldGroup)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            if (worldGroup == "*")
            {
                foreach (WorldServer world in MSManager.Instance.WorldServers)
                {
                    world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().Shutdown();
                }
            }
            else
            {
                foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(worldGroup)))
                {
                    world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().Shutdown();
                }
            }
        }

        public void UnregisterWorldServer(Guid worldId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            MSManager.Instance.ConnectedAccounts.RemoveAll(a => a?.ConnectedWorld?.Id.Equals(worldId) == true);
            MSManager.Instance.WorldServers.RemoveAll(w => w.Id.Equals(worldId));
        }

        public void UpdateBazaar(string worldGroup, long bazaarItemId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(worldGroup)))
            {
                world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().UpdateBazaar(bazaarItemId);
            }
        }

        public void UpdateFamily(string worldGroup, long familyId, bool changeFaction)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(worldGroup)))
            {
                world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().UpdateFamily(familyId, changeFaction);
            }
        }

        public void UpdateRelation(string worldGroup, long relationId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
            {
                return;
            }

            foreach (WorldServer world in MSManager.Instance.WorldServers.Where(w => w.WorldGroup.Equals(worldGroup)))
            {
                world.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().UpdateRelation(relationId);
            }
        }

        public void SendMail(string worldGroup, MailDTO mail)
        {
            if (!IsCharacterConnected(worldGroup, mail.ReceiverId))
            {
                DAOFactory.MailDAO.InsertOrUpdate(ref mail);
            }
            else
            {
                AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(mail.ReceiverId));
                if (account?.ConnectedWorld == null)
                {
                    DAOFactory.MailDAO.InsertOrUpdate(ref mail);
                    return;
                }
                DAOFactory.MailDAO.InsertOrUpdate(ref mail);
                account.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMail(mail);
            }
        }

        public void UpdateCharacterAchievement(string worldGroup, long characterId, long achievementId)
        {
            if (!IsCharacterConnected(worldGroup, characterId))
            {
                CharacterAchievementDTO load = DAOFactory.CharacterAchievementDAO.LoadByCharacterId(characterId).Where(s => s.AchievementId == achievementId).FirstOrDefault();
                if (load != null)
                {
                    DAOFactory.CharacterAchievementDAO.Delete(characterId, achievementId);
                    CharacterAchievementDTO ach = new CharacterAchievementDTO
                    {
                        CharacterId = characterId,
                        AchievementId = load.AchievementId,
                        FirstObjective = load.FirstObjective,
                        IsMainAchievement = true
                    };
                    DAOFactory.CharacterAchievementDAO.InsertOrUpdate(ach);
                }
            }
            else
            {
                AccountConnection account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(characterId));
                if (account?.ConnectedWorld == null)
                {
                    CharacterAchievementDTO load = DAOFactory.CharacterAchievementDAO.LoadByCharacterId(characterId).Where(s => s.AchievementId == achievementId).FirstOrDefault();
                    if (load != null)
                    {
                        DAOFactory.CharacterAchievementDAO.Delete(characterId, achievementId);
                        CharacterAchievementDTO ach = new CharacterAchievementDTO
                        {
                            CharacterId = characterId,
                            AchievementId = load.AchievementId,
                            FirstObjective = load.FirstObjective,
                            IsMainAchievement = true
                        };
                        DAOFactory.CharacterAchievementDAO.InsertOrUpdate(ach);
                    }
                    return;
                }
                account.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().UpdateCharacterAchievement(characterId, achievementId);
            }
        }
        #endregion
    }
}