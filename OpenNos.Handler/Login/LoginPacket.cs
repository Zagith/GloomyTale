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
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.Master.Library.Client;
using System;
using System.Configuration;
using System.Linq;

namespace OpenNos.Handler
{
    [PacketHeader("NoS0575", CharacterRequired = false)]
    public class LoginPacket
    {
        #region Properties

        public int Number { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string ClientDataOld { get; set; }

        public string ClientData { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 6)
            {
                return;
            }
            LoginPacket loginPacket = new LoginPacket();
            if (int.TryParse(packetSplit[1], out int number))
            {
                loginPacket.Number = number;
                loginPacket.Name = packetSplit[2];
                loginPacket.Password = packetSplit[3];
                loginPacket.ClientDataOld = packetSplit[4];
                loginPacket.ClientData = packetSplit[5];
                loginPacket.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(LoginPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            string BuildServersPacket(string username, int sessionId, bool ignoreUserName)
            {
                string channelpacket =
                    CommunicationServiceClient.Instance.RetrieveRegisteredWorldServers(username, sessionId, ignoreUserName);

                if (channelpacket == null || !channelpacket.Contains(':'))
                {
                    Logger.Debug(
                        "Could not retrieve Worldserver groups. Please make sure they've already been registered.");
                    session.SendPacket($"failc {Language.Instance.GetMessageFromKey("NO_WORLDSERVERS")}");
                }

                return channelpacket;
            }

            UserDTO user = new UserDTO
            {
                Name = Name,
                Password = ConfigurationManager.AppSettings["UseOldCrypto"] == "true"
                    ? CryptographyBase.Sha512(LoginCryptography.GetPassword(Password)).ToUpper()
                    : Password
            };
            if (user == null || user.Name == null || user.Password == null)
            {
                return;
            }
            AccountDTO loadedAccount = DAOFactory.AccountDAO.LoadByName(user.Name);
            if (loadedAccount != null && loadedAccount.Name != user.Name)
            {
                session.SendPacket($"failc {(byte)LoginFailType.WrongCaps}");
                return;
            }
            if (loadedAccount?.Password.ToUpper().Equals(user.Password) == true)
            {
                string ipAddress = session.IpAddress;
                DAOFactory.AccountDAO.WriteGeneralLog(loadedAccount.AccountId, ipAddress, null,
                    GeneralLogType.Connection, "LoginServer");

                if (DAOFactory.PenaltyLogDAO.LoadByIp(ipAddress).Count() > 0)
                {
                    session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
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
                        session.SendPacket($"failc {(byte)LoginFailType.Banned}");
                    }
                    else
                    {
                        switch (type)
                        {
                            case AuthorityType.Unconfirmed:
                                {
                                    session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
                                }
                                break;

                            case AuthorityType.Banned:
                                {
                                    session.SendPacket($"failc {(byte)LoginFailType.Banned}");
                                }
                                break;

                            case AuthorityType.Closed:
                                {
                                    session.SendPacket($"failc {(byte)LoginFailType.CantConnect}");
                                }
                                break;

                            default:
                                {
                                    if (loadedAccount.Authority < AuthorityType.GM)
                                    {
                                        MaintenanceLogDTO maintenanceLog = DAOFactory.MaintenanceLogDAO.LoadFirst();
                                        if (maintenanceLog != null && maintenanceLog.DateStart < DateTime.Now)
                                        {
                                            session.SendPacket($"failc {(byte)LoginFailType.Maintenance}");
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
                                            newSessionId, ipAddress);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("General Error SessionId: " + newSessionId, ex);
                                    }

                                    string[] clientData = ClientData.Split('.');

                                    if (clientData.Length < 2)
                                    {
                                        clientData = ClientDataOld.Split('.');
                                    }

                                    bool ignoreUserName = short.TryParse(clientData[3], out short clientVersion)
                                                          && (clientVersion < 3075
                                                           || ConfigurationManager.AppSettings["UseOldCrypto"] == "true");
                                    session.SendPacket(BuildServersPacket(user.Name, newSessionId, ignoreUserName));
                                }
                                break;
                        }
                    }
                }
                else
                {
                    session.SendPacket($"failc {(byte)LoginFailType.AlreadyConnected}");
                }
            }
            else
            {
                session.SendPacket($"failc {(byte)LoginFailType.AccountOrPasswordWrong}");
            }
        }
        #endregion
    }
}