/*
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

using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Packets.ClientPackets;
using System;
using System.Configuration;
using System.Linq;
using GloomyTale.GameObject.Networking;
using System.Collections.Generic;
using GloomyTale.Communication;
using System.Text;
using Newtonsoft.Json;

namespace OpenNos.Handler
{
    public class LoginPacketHandler : IPacketHandler
    {
        #region Members

        private readonly ClientSession _session;

        #endregion

        #region Instantiation

        public LoginPacketHandler(ClientSession session) => _session = session;

        #endregion

        #region Methods

        public void SendServerListPacket(int sessionId, string accountName)
        {
            IEnumerable<SerializableWorldServer> worlds = CommunicationServiceClient.Instance.RetrieveRegisteredWorldServers();

            if (worlds?.Any() == true)
            {
                string lastGroup = string.Empty;
                int worldGroupCount = 0;
                var packetBuilder = new StringBuilder();
                packetBuilder.AppendFormat("NsTeST {0} {1} ", accountName, sessionId);
                foreach (SerializableWorldServer world in worlds)
                {
                    Logger.Log.Info($"Worlds : {JsonConvert.SerializeObject(world)}");
                    if (lastGroup != world.WorldGroup)
                    {
                        worldGroupCount++;
                    }

                    lastGroup = world.WorldGroup;
                    int color = 0;
                    packetBuilder.AppendFormat("{0}:{1}:{2}:", world.EndPointIp, world.EndPointPort, color);
                    packetBuilder.AppendFormat("{0}.{1}.{2} ", worldGroupCount, world.ChannelId, world.WorldGroup);
                }

                packetBuilder.Append("-1:-1:-1:10000.10000.1");
                Logger.Log.Info("Packet : " + packetBuilder);
                _session.SendPacket(packetBuilder.ToString());
                return;
            }

            Logger.Log.Warn("Could not retrieve Worldserver groups. Please make sure they've already been registered.");
            _session.SendPacket($"failc {(byte)LoginFailType.Maintenance}");
        }

        /// <summary>
        /// login packet
        /// </summary>
        /// <param name="loginPacket"></param>
        public void VerifyLogin(LoginPacket loginPacket)
        {
            if (loginPacket == null || loginPacket.Name == null || loginPacket.Password == null)
            {
                return;
            }

            UserDTO user = new UserDTO
            {
                Name = loginPacket.Name,
                Password = ConfigurationManager.AppSettings["UseOldCrypto"] == "true"
                    ? CryptographyBase.Sha512(LoginCryptography.GetPassword(loginPacket.Password)).ToUpper()
                    : loginPacket.Password
            };
            if (user == null || user.Name == null || user.Password == null)
            {
                return;
            }
            AccountDTO loadedAccount = DAOFactory.AccountDAO.LoadByName(user.Name);
            if (loadedAccount != null && loadedAccount.Name != user.Name)
            {
                _session.SendPacket($"failc {(byte)LoginFailType.WrongCaps}");
                return;
            }
            if (loadedAccount?.Password.ToUpper().Equals(user.Password) == true)
            {
                string ipAddress = _session.IpAddress;
                DAOFactory.AccountDAO.WriteGeneralLog(loadedAccount.AccountId, ipAddress, null,
                    GeneralLogType.Connection, "LoginServer");

                if (DAOFactory.PenaltyLogDAO.LoadByIp(ipAddress).Count() > 0)
                {
                    _session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
                    return;
                }

                //check if the account is connected
                if (!CommunicationServiceClient.Instance.IsAccountConnected(loadedAccount.AccountId))
                {
                    AuthorityType type = loadedAccount.Authority;
                    PenaltyLogDTO penalty = DAOFactory.PenaltyLogDAO.LoadByAccount(loadedAccount.AccountId)
                        .FirstOrDefault(s => s.DateEnd > DateTime.Now && s.Penalty == PenaltyType.Banned);
                    if (penalty != null)
                    {
                        _session.SendPacket($"failc {(byte)LoginFailType.Banned}");
                    }
                    else
                    {
                        switch (type)
                        {
                            case AuthorityType.Unconfirmed:
                                {
                                    _session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
                                }
                                break;

                            case AuthorityType.Banned:
                                {
                                    _session.SendPacket($"failc {(byte)LoginFailType.Banned}");
                                }
                                break;

                            case AuthorityType.Closed:
                                {
                                    _session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
                                }
                                break;

                            default:
                            {
                                if (loadedAccount.Authority < AuthorityType.SMOD)
                                {
                                    MaintenanceLogDTO maintenanceLog = DAOFactory.MaintenanceLogDAO.LoadFirst();
                                    if (maintenanceLog != null && maintenanceLog.DateStart < DateTime.Now)
                                        {
                                            _session.SendPacket($"failc {(byte)LoginFailType.Maintenance}");
                                            return;
                                    }
                                }

                                int newSessionId = SessionFactory.Instance.GenerateSessionId();
                                Logger.Debug(string.Format(Language.Instance.GetMessageFromKey("CONNECTION"), user.Name,
                                    newSessionId));
                                try
                                {
                                    ipAddress = ipAddress.Substring(6, ipAddress.LastIndexOf(':') - 6);
                                    CommunicationServiceClient.Instance.RegisterAccountLogin(loadedAccount.AccountId,
                                        newSessionId, loadedAccount.Name, ipAddress);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("General Error SessionId: " + newSessionId, ex);
                                }

                                    string[] clientData = loginPacket.ClientData.Split('.');

                                    if (clientData.Length < 2)
                                    {
                                        clientData = loginPacket.ClientDataOld.Split('.');
                                    }

                                    SendServerListPacket(newSessionId, loadedAccount.Name);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    _session.SendPacket($"failc {(byte)LoginFailType.AlreadyConnected}");
                }
            }
            else
            {
                _session.SendPacket($"failc {(byte)LoginFailType.AccountOrPasswordWrong}");
            }
        }

        #endregion
    }
}