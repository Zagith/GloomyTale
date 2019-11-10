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
using OpenNos.GameObject.CommandPackets;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenNos.Handler
{
    public class CommandPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CommandPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddUserLog(AddUserLogPacket packet)
        {
            if (packet == null
                || string.IsNullOrEmpty(packet.Username))
            {
                return;
            }
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            ClientSession.UserLog.Add(packet.Username);

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        public void UserLog(UserLogPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            int n = 1;
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            foreach (string username in ClientSession.UserLog)
            {
                Session.SendPacket(Session.Character.GenerateSay($"{n++}- {username}", 12));
            }

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        public void RemoveUserLog(RemoveUserLogPacket packet)
        {
            if (packet == null
                || string.IsNullOrEmpty(packet.Username))
            {
                return;
            }
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            if (ClientSession.UserLog.Contains(packet.Username))
            {
                ClientSession.UserLog.RemoveAll(username => username == packet.Username);
            }

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        public void PartnerSpXp(PartnerSpXpPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            Mate mate = Session.Character.Mates?.ToList().FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            if (mate?.Sp != null)
            {
                mate.Sp.FullXp();
                Session.SendPacket(mate.GenerateScPacket());
            }
        }

        public void Act4Stat(Act4StatPacket packet)
        {
            if (packet != null && ServerManager.Instance.ChannelId == 51)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Act4Stat]Faction: {packet.Faction} Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                switch (packet.Faction)
                {
                    case 1:
                        ServerManager.Instance.Act4AngelStat.Percentage = packet.Value;
                        break;

                    case 2:
                        ServerManager.Instance.Act4DemonStat.Percentage = packet.Value;
                        break;
                }
                Parallel.ForEach(ServerManager.Instance.Sessions, sess => sess.SendPacket(sess.Character.GenerateFc()));
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Act4StatPacket.ReturnHelp(), 10));
            }
        }

        public void SetPin(SetLockPacket p)
        {
            if (p != null && p.Message != null)
            {
                if (Session.Character.SecondPassword == null)
                {
                    if (p.Message.Length >= 8)
                    {
                        Session.Character.SecondPassword = CryptographyBase.Sha512(p.Message);
                        Session.Character.Save();
                        Session.Character.hasVerifiedSecondPassword = true;
                        Session.SendPacket(Session.Character.GenerateSay($"Done! Your second password (or pin) is now: {p.Message}. Do not forget it.", 10));
                        Session.Character.HasGodMode = false;
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay($"Your pin lenght cannot be less than 8 characters.", 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You already have a pin. Please, if you have forgotten it, contact a staff member.", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SetLockPacket.ReturnHelp(), 10));
            }
        }

        public void ConfirmPass(UnlockPacket p)
        {
            if (p != null && p.Message != null)
            {
                if (Session.Character.SecondPassword != null)
                {
                    if (CryptographyBase.Sha512(p.Message) == Session.Character.SecondPassword)
                    {
                        Session.Character.hasVerifiedSecondPassword = true;
                        Session.SendPacket(Session.Character.GenerateSay($"You have successfully verified your identity!", 10));
                        Session.Character.HasGodMode = false;
                        Session.Character.InvisibleGm = false;
                        Session.Character.Invisible = false;
                        Session.SendPacket(Session.Character.GenerateInvisible());
                        Session.SendPacket(Session.Character.GenerateEq());
                        Session.SendPacket(Session.Character.GenerateCMode());
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay($"Wrong pin.", 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You didn't set a pin yet. Use $SetPin to set a pin.", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UnlockPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddMonster Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddMonster(AddMonsterPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddMonster]NpcMonsterVNum: {packet.MonsterVNum} IsMoving: {packet.IsMoving}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (!Session.HasCurrentMapInstance)
                {
                    return;
                }

                NpcMonster npcmonster = ServerManager.GetNpcMonster(packet.MonsterVNum);
                if (npcmonster == null)
                {
                    return;
                }

                MapMonsterDTO monst = new MapMonsterDTO
                {
                    MonsterVNum = packet.MonsterVNum,
                    MapY = Session.Character.PositionY,
                    MapX = Session.Character.PositionX,
                    MapId = Session.Character.MapInstance.Map.MapId,
                    Position = Session.Character.Direction,
                    IsMoving = packet.IsMoving,
                    MapMonsterId = ServerManager.Instance.GetNextMobId()
                };
                if (!DAOFactory.MapMonsterDAO.DoesMonsterExist(monst.MapMonsterId))
                {
                    DAOFactory.MapMonsterDAO.Insert(monst);
                    if (DAOFactory.MapMonsterDAO.LoadById(monst.MapMonsterId) is MapMonsterDTO monsterDTO)
                    {
                        MapMonster monster = new MapMonster(monsterDTO);
                        monster.Initialize(Session.CurrentMapInstance);
                        Session.CurrentMapInstance.AddMonster(monster);
                        Session.CurrentMapInstance?.Broadcast(monster.GenerateIn());
                    }
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddMonsterPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddNpc Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddNpc(AddNpcPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddNpc]NpcMonsterVNum: {packet.NpcVNum} IsMoving: {packet.IsMoving}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (!Session.HasCurrentMapInstance)
                {
                    return;
                }

                NpcMonster npcmonster = ServerManager.GetNpcMonster(packet.NpcVNum);
                if (npcmonster == null)
                {
                    return;
                }

                MapNpcDTO newNpc = new MapNpcDTO
                {
                    NpcVNum = packet.NpcVNum,
                    MapY = Session.Character.PositionY,
                    MapX = Session.Character.PositionX,
                    MapId = Session.Character.MapInstance.Map.MapId,
                    Position = Session.Character.Direction,
                    IsMoving = packet.IsMoving,
                    MapNpcId = ServerManager.Instance.GetNextNpcId()
                };
                if (!DAOFactory.MapNpcDAO.DoesNpcExist(newNpc.MapNpcId))
                {
                    DAOFactory.MapNpcDAO.Insert(newNpc);
                    if (DAOFactory.MapNpcDAO.LoadById(newNpc.MapNpcId) is MapNpcDTO npcDTO)
                    {
                        MapNpc npc = new MapNpc(npcDTO);
                        npc.Initialize(Session.CurrentMapInstance);
                        Session.CurrentMapInstance.AddNPC(npc);
                        Session.CurrentMapInstance?.Broadcast(npc.GenerateIn());
                    }
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddNpcPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddPartner Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddPartner(AddPartnerPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddPartner]NpcMonsterVNum: {packet.MonsterVNum} Level: {packet.Level}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                AddMate(packet.MonsterVNum, packet.Level, MateType.Partner);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddPartnerPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddPet Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddPet(AddPetPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddPet]NpcMonsterVNum: {packet.MonsterVNum} Level: {packet.Level}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                AddMate(packet.MonsterVNum, packet.Level, MateType.Pet);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddPartnerPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddPortal Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddPortal(AddPortalPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddPortal]DestinationMapId: {packet.DestinationMapId} DestinationMapX: {packet.DestinationX} DestinationY: {packet.DestinationY}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                AddPortal(packet.DestinationMapId, packet.DestinationX, packet.DestinationY,
                    packet.PortalType == null ? (short)-1 : (short)packet.PortalType, true);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddPortalPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddShellEffect Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddShellEffect(AddShellEffectPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddShellEffect]Slot: {packet.Slot} EffectLevel: {packet.EffectLevel} Effect: {packet.Effect} Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                try
                {
                    ItemInstance instance =
                        Session.Character.Inventory.LoadBySlotAndType(packet.Slot,
                            InventoryType.Equipment);
                    if (instance != null)
                    {
                        instance.ShellEffects.Add(new ShellEffectDTO
                        {
                            EffectLevel = (ShellEffectLevelType)packet.EffectLevel,
                            Effect = packet.Effect,
                            Value = packet.Value,
                            EquipmentSerialId = instance.EquipmentSerialId
                        });
                    }
                }
                catch (Exception)
                {
                    Session.SendPacket(Session.Character.GenerateSay(AddShellEffectPacket.ReturnHelp(), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddShellEffectPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddSkill Command
        /// </summary>
        /// <param name="packet"></param>
        public void AddSkill(AddSkillPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddSkill]SkillVNum: {packet.SkillVNum}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.Character.AddSkill(packet.SkillVNum);
                Session.SendPacket(Session.Character.GenerateSki());
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddSkillPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ArenaWinner Command
        /// </summary>
        /// <param name="packet"></param>
        public void ArenaWinner(ArenaWinnerPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[ArenaWinner]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            Session.Character.ArenaWinner = Session.Character.ArenaWinner == 0 ? 1 : 0;
            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateCMode());
            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        /// <summary>
        /// $Ban Command
        /// </summary>
        /// <param name="packet"></param>
        public void Ban(BanPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Ban]CharacterName: {packet.CharacterName} Reason: {packet.Reason} Until: {(packet.Duration == 0 ? DateTime.Now.AddYears(15) : DateTime.Now.AddDays(packet.Duration))}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                BanMethod(packet.CharacterName, packet.Duration, packet.Reason);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BanPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Bank Command
        /// </summary>
        /// <param name="packet"></param>
        public void BankManagement(BankPacket packet)
        {
            if (Session.Account.IsLimited)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("LIMITED_ACCOUNT")));
                return;
            }

            if (packet != null)
            {
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                switch (packet.Mode?.ToLower())
                {
                    case "balance":
                        {
                            Logger.LogEvent("BANK",
                                $"[{Session.GenerateIdentity()}][Balance]Balance: {Session.Character.GoldBank}");

                            Session.SendPacket(
                                Session.Character.GenerateSay($"Current Balance: {Session.Character.GoldBank} Gold.", 10));
                            return;
                        }
                    case "deposit":
                        {
                            if (packet.Param1 != null
                                && (long.TryParse(packet.Param1, out long amount) || string.Equals(packet.Param1,
                                     "all", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (string.Equals(packet.Param1, "all", StringComparison.OrdinalIgnoreCase)
                                    && Session.Character.Gold > 0)
                                {
                                    Logger.LogEvent("BANK",
                                        $"[{Session.GenerateIdentity()}][Deposit]Amount: {Session.Character.Gold} OldBank: {Session.Character.GoldBank} NewBank: {Session.Character.GoldBank + Session.Character.Gold}");

                                    Session.SendPacket(
                                        Session.Character.GenerateSay($"Deposited ALL({Session.Character.Gold}) Gold.",
                                            10));
                                    Session.Character.GoldBank += Session.Character.Gold;
                                    Session.Character.Gold = 0;
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.SendPacket(
                                        Session.Character.GenerateSay($"New Balance: {Session.Character.GoldBank} Gold.",
                                            10));
                                }
                                else if (amount <= Session.Character.Gold && Session.Character.Gold > 0)
                                {
                                    if (amount < 1)
                                    {
                                        Logger.LogEvent("BANK",
                                            $"[{Session.GenerateIdentity()}][Illegal]Mode: {packet.Mode} Param1: {packet.Param1} Param2: {packet.Param2}");

                                        Session.SendPacket(Session.Character.GenerateSay(
                                            "I'm afraid I can't let you do that. This incident has been logged.", 10));
                                    }
                                    else
                                    {
                                        Logger.LogEvent("BANK",
                                            $"[{Session.GenerateIdentity()}][Deposit]Amount: {amount} OldBank: {Session.Character.GoldBank} NewBank: {Session.Character.GoldBank + amount}");

                                        Session.SendPacket(Session.Character.GenerateSay($"Deposited {amount} Gold.", 10));
                                        Session.Character.GoldBank += amount;
                                        Session.Character.Gold -= amount;
                                        Session.SendPacket(Session.Character.GenerateGold());
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                $"New Balance: {Session.Character.GoldBank} Gold.", 10));
                                    }
                                }
                            }

                            return;
                        }
                    case "withdraw":
                        {
                            if (packet.Param1 != null && long.TryParse(packet.Param1, out long amount)
                                && amount <= Session.Character.GoldBank && Session.Character.GoldBank > 0
                                && (Session.Character.Gold + amount) <= ServerManager.Instance.Configuration.MaxGold)
                            {
                                if (amount < 1)
                                {
                                    Logger.LogEvent("BANK",
                                        $"[{Session.GenerateIdentity()}][Illegal]Mode: {packet.Mode} Param1: {packet.Param1} Param2: {packet.Param2}");

                                    Session.SendPacket(Session.Character.GenerateSay(
                                        "I'm afraid I can't let you do that. This incident has been logged.", 10));
                                }
                                else
                                {
                                    Logger.LogEvent("BANK",
                                        $"[{Session.GenerateIdentity()}][Withdraw]Amount: {amount} OldBank: {Session.Character.GoldBank} NewBank: {Session.Character.GoldBank - amount}");

                                    Session.SendPacket(Session.Character.GenerateSay($"Withdrawn {amount} Gold.", 10));
                                    Session.Character.GoldBank -= amount;
                                    Session.Character.Gold += amount;
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.SendPacket(
                                        Session.Character.GenerateSay($"New Balance: {Session.Character.GoldBank} Gold.",
                                            10));
                                }
                            }

                            return;
                        }
                    case "send":
                        {
                            if (packet.Param1 != null)
                            {
                                long amount = packet.Param2;
                                ClientSession receiver =
                                    ServerManager.Instance.GetSessionByCharacterName(packet.Param1);
                                if (amount <= Session.Character.GoldBank && Session.Character.GoldBank > 0
                                    && receiver != null)
                                {
                                    if (amount < 1)
                                    {
                                        Logger.LogEvent("BANK",
                                            $"[{Session.GenerateIdentity()}][Illegal]Mode: {packet.Mode} Param1: {packet.Param1} Param2: {packet.Param2}");

                                        Session.SendPacket(Session.Character.GenerateSay(
                                            "I'm afraid I can't let you do that. This incident has been logged.", 10));
                                    }
                                    else
                                    {
                                        Logger.LogEvent("BANK",
                                            $"[{Session.GenerateIdentity()}][Send]Amount: {amount} OldBankSender: {Session.Character.GoldBank} NewBankSender: {Session.Character.GoldBank - amount} OldBankReceiver: {receiver.Character.GoldBank} NewBankReceiver: {receiver.Character.GoldBank + amount}");

                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                $"Sent {amount} Gold to {receiver.Character.Name}", 10));
                                        receiver.SendPacket(
                                            Session.Character.GenerateSay(
                                                $"Received {amount} Gold from {Session.Character.Name}", 10));
                                        Session.Character.GoldBank -= amount;
                                        receiver.Character.GoldBank += amount;
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                $"New Balance: {Session.Character.GoldBank} Gold.", 10));
                                        receiver.SendPacket(
                                            Session.Character.GenerateSay(
                                                $"New Balance: {receiver.Character.GoldBank} Gold.", 10));
                                    }
                                }
                            }

                            return;
                        }
                    default:
                        {
                            Session.SendPacket(Session.Character.GenerateSay(BankPacket.ReturnHelp(), 10));
                            return;
                        }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BankPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $BlockExp Command
        /// </summary>
        /// <param name="packet"></param>
        public void BlockExp(BlockExpPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[BlockExp]CharacterName: {packet.CharacterName} Reason: {packet.Reason} Until: {DateTime.Now.AddMinutes(packet.Duration)}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Duration == 0)
                {
                    packet.Duration = 60;
                }

                packet.Reason = packet.Reason?.Trim();
                CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(packet.CharacterName);
                if (character != null)
                {
                    ClientSession session =
                        ServerManager.Instance.Sessions.FirstOrDefault(s =>
                            s.Character?.Name == packet.CharacterName);
                    session?.SendPacket(packet.Duration == 1
                        ? UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_SINGULAR"), packet.Reason))
                        : UserInterfaceHelper.GenerateInfo(string.Format(
                            Language.Instance.GetMessageFromKey("MUTED_PLURAL"), packet.Reason,
                            packet.Duration)));
                    PenaltyLogDTO log = new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = packet.Reason,
                        Penalty = PenaltyType.BlockExp,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(packet.Duration),
                        AdminName = Session.Character.Name
                    };
                    Character.InsertOrUpdatePenalty(log);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BlockExpPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $BlockFExp Command
        /// </summary>
        /// <param name="packet"></param>
        public void BlockFExp(BlockFExpPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[BlockFExp]CharacterName: {packet.CharacterName} Reason: {packet.Reason} Until: {DateTime.Now.AddMinutes(packet.Duration)}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Duration == 0)
                {
                    packet.Duration = 60;
                }

                packet.Reason = packet.Reason?.Trim();
                CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(packet.CharacterName);
                if (character != null)
                {
                    ClientSession session =
                        ServerManager.Instance.Sessions.FirstOrDefault(s =>
                            s.Character?.Name == packet.CharacterName);
                    session?.SendPacket(packet.Duration == 1
                        ? UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_SINGULAR"),
                                packet.Reason))
                        : UserInterfaceHelper.GenerateInfo(string.Format(
                            Language.Instance.GetMessageFromKey("MUTED_PLURAL"), packet.Reason,
                            packet.Duration)));
                    PenaltyLogDTO log = new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = packet.Reason,
                        Penalty = PenaltyType.BlockFExp,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(packet.Duration),
                        AdminName = Session.Character.Name
                    };
                    Character.InsertOrUpdatePenalty(log);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BlockFExpPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $BlockPM Command
        /// </summary>
        /// <param name="packet"></param>
        public void BlockPm(BlockPMPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[BlockPM]");

            if (!Session.Character.GmPvtBlock)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GM_BLOCK_ENABLE"),
                    10));
                Session.Character.GmPvtBlock = true;
            }
            else
            {
                Session.SendPacket(
                    Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GM_BLOCK_DISABLE"), 10));
                Session.Character.GmPvtBlock = false;
            }
        }

        /// <summary>
        /// $BlockRep Command
        /// </summary>
        /// <param name="packet"></param>
        public void BlockRep(BlockRepPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[BlockRep]CharacterName: {packet.CharacterName} Reason: {packet.Reason} Until: {DateTime.Now.AddMinutes(packet.Duration)}");

                if (packet.Duration == 0)
                {
                    packet.Duration = 60;
                }

                packet.Reason = packet.Reason?.Trim();
                CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(packet.CharacterName);
                if (character != null)
                {
                    ClientSession session =
                        ServerManager.Instance.Sessions.FirstOrDefault(s =>
                            s.Character?.Name == packet.CharacterName);
                    session?.SendPacket(packet.Duration == 1
                        ? UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_SINGULAR"), packet.Reason))
                        : UserInterfaceHelper.GenerateInfo(string.Format(
                            Language.Instance.GetMessageFromKey("MUTED_PLURAL"), packet.Reason,
                            packet.Duration)));
                    PenaltyLogDTO log = new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = packet.Reason,
                        Penalty = PenaltyType.BlockRep,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(packet.Duration),
                        AdminName = Session.Character.Name
                    };
                    Character.InsertOrUpdatePenalty(log);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BlockRepPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Buff packet
        /// </summary>
        /// <param name="packet"></param>
        public void Buff(BuffPacket packet)
        {
            if (packet != null)
            {
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Buff buff = new Buff(packet.CardId, packet.Level ?? (byte)1);
                Session.Character.AddBuff(buff, Session.Character.BattleEntity);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BuffPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ChangeClass Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeClass(ChangeClassPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[ChangeClass]Class: {packet.ClassType}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.Character.ChangeClass(packet.ClassType, true);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeClassPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ChangeDignity Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeDignity(ChangeDignityPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[ChangeDignity]Dignity: {packet.Dignity}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Dignity >= -1000 && packet.Dignity <= 100)
                {
                    Session.Character.Dignity = packet.Dignity;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("DIGNITY_CHANGED"), 12));
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("BAD_DIGNITY"), 11));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeDignityPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $FLvl Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeFairyLevel(ChangeFairyLevelPacket packet)
        {
            ItemInstance fairy =
                Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[FLvl]FairyLevel: {packet.FairyLevel}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (fairy != null)
                {
                    short fairylevel = packet.FairyLevel;
                    fairylevel -= fairy.Item.ElementRate;
                    fairy.ElementRate = fairylevel;
                    fairy.XP = 0;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("FAIRY_LEVEL_CHANGED"), fairy.Item.Name),
                        10));
                    Session.SendPacket(Session.Character.GeneratePairy());
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAIRY"),
                        10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeFairyLevelPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ChangeSex Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeGender(ChangeSexPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[ChangeSex]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            Session.Character.ChangeSex();
        }

        /// <summary>
        /// $HeroLvl Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeHeroLevel(ChangeHeroLevelPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[HeroLvl]HeroLevel: {packet.HeroLevel}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.HeroLevel <= 255)
                {
                    Session.Character.HeroLevel = packet.HeroLevel;
                    Session.Character.HeroXp = 0;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HEROLEVEL_CHANGED"), 0));
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.SendPackets(Session.Character.GenerateStatChar());
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 6),
                        Session.Character.PositionX, Session.Character.PositionY);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 198),
                        Session.Character.PositionX, Session.Character.PositionY);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeHeroLevelPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $JLvl Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeJobLevel(ChangeJobLevelPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[JLvl]JobLevel: {packet.JobLevel}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (((Session.Character.Class == 0 && packet.JobLevel <= 20)
                    || (Session.Character.Class != 0 && packet.JobLevel <= 255))
                    && packet.JobLevel > 0)
                {
                    Session.Character.JobLevel = packet.JobLevel;
                    Session.Character.JobLevelXp = 0;
                    Session.Character.ResetSkills();
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOBLEVEL_CHANGED"), 0));
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 8), Session.Character.PositionX, Session.Character.PositionY);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeJobLevelPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Lvl Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeLevel(ChangeLevelPacket packet)
        {
            if (packet != null && !Session.Character.IsSeal)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Lvl]Level: {packet.Level}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Level > 0)
                {
                    Session.Character.Level = Math.Min(packet.Level,
                        ServerManager.Instance.Configuration.MaxLevel);
                    Session.Character.LevelXp = 0;
                    Session.Character.Hp = (int)Session.Character.HPLoad();
                    Session.Character.Mp = (int)Session.Character.MPLoad();
                    Session.SendPacket(Session.Character.GenerateStat());
                    Session.SendPackets(Session.Character.GenerateStatChar());
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LEVEL_CHANGED"), 0));
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 6),
                        Session.Character.PositionX, Session.Character.PositionY);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 198),
                        Session.Character.PositionX, Session.Character.PositionY);
                    ServerManager.Instance.UpdateGroup(Session.Character.CharacterId);
                    if (Session.Character.Family != null)
                    {
                        ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                        CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                        {
                            DestinationCharacterId = Session.Character.Family.FamilyId,
                            SourceCharacterId = Session.Character.CharacterId,
                            SourceWorldId = ServerManager.Instance.WorldId,
                            Message = "fhis_stc",
                            Type = MessageType.Family
                        });
                    }
                    Session.Character.LevelRewards(Session.Character.Level);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeLevelPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ChangeRep Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeReputation(ChangeReputationPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[ChangeRep]Reputation: {packet.Reputation}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Reputation > 0)
                {
                    Session.Character.Reputation = packet.Reputation;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REP_CHANGED"), 0));
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeReputationPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $SPLvl Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeSpecialistLevel(ChangeSpecialistLevelPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[SPLvl]SpecialistLevel: {packet.SpecialistLevel}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                ItemInstance sp =
                    Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                if (sp != null && Session.Character.UseSp)
                {
                    if (packet.SpecialistLevel <= 255
                        && packet.SpecialistLevel > 0)
                    {
                        sp.SpLevel = packet.SpecialistLevel;
                        sp.XP = 0;
                        Session.SendPacket(Session.Character.GenerateLev());
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SPLEVEL_CHANGED"), 0));
                        Session.Character.LearnSPSkill();
                        Session.SendPacket(Session.Character.GenerateSki());
                        Session.SendPackets(Session.Character.GenerateQuicklist());
                        Session.Character.SkillsSp.ForEach(s => s.LastUse = DateTime.Now.AddDays(-1));
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(InEffect: 1),
                            ReceiverType.AllExceptMe);
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                        Session.CurrentMapInstance?.Broadcast(
                            StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 8),
                            Session.Character.PositionX, Session.Character.PositionY);
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                    }
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_SP"),
                        0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeSpecialistLevelPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ChannelInfo Command
        /// </summary>
        /// <param name="packet"></param>
        public void ChannelInfo(ChannelInfoPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[ChannelInfo]");

            Session.SendPacket(Session.Character.GenerateSay(
                $"-----------Channel Info-----------\n-------------Channel:{ServerManager.Instance.ChannelId}-------------",
                11));
            foreach (ClientSession session in ServerManager.Instance.Sessions)
            {
                Session.SendPacket(
                    Session.Character.GenerateSay(
                        $"CharacterName: {session.Character.Name} | CharacterId: {session.Character.CharacterId} | SessionId: {session.SessionId}", 12));
            }

            Session.SendPacket(Session.Character.GenerateSay("----------------------------------------", 11));
        }

        /// <summary>
        /// $ServerInfo Command
        /// </summary>
        /// <param name="packet"></param>
        public void ServerInfo(ServerInfoPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[ServerInfo]");

            Session.SendPacket(Session.Character.GenerateSay($"------------Server Info------------", 11));

            long ActualChannelId = 0;

            CommunicationServiceClient.Instance.GetOnlineCharacters().Where(s => packet.ChannelId == null || s[1] == packet.ChannelId).OrderBy(s => s[1]).ToList().ForEach(s =>
            {
                if (s[1] > ActualChannelId)
                {
                    if (ActualChannelId > 0)
                    {
                        Session.SendPacket(Session.Character.GenerateSay("----------------------------------------", 11));
                    }
                    ActualChannelId = s[1];
                    Session.SendPacket(Session.Character.GenerateSay($"-------------Channel:{ActualChannelId}-------------", 11));
                }
                CharacterDTO Character = DAOFactory.CharacterDAO.LoadById(s[0]);
                Session.SendPacket(
                    Session.Character.GenerateSay(
                        $"CharacterName: {Character.Name} | CharacterId: {Character.CharacterId} | SessionId: {s[2]}", 12));
            });

            Session.SendPacket(Session.Character.GenerateSay("----------------------------------------", 11));
        }

        /// <summary>
        /// $CharEdit Command
        /// </summary>
        /// <param name="packet"></param>
        public void CharacterEdit(CharacterEditPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[CharEdit]Property: {packet.Property} Value: {packet.Data}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Property != null && !string.IsNullOrEmpty(packet.Data))
                {
                    PropertyInfo propertyInfo = Session.Character.GetType().GetProperty(packet.Property);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(Session.Character,
                            Convert.ChangeType(packet.Data, propertyInfo.PropertyType));
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
                        Session.Character.Save();
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"),
                            10));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(CharacterEditPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $CharStat Command
        /// </summary>
        /// <param name="packet"></param>
        public void CharStat(CharacterStatsPacket packet)
        {
            string returnHelp = CharacterStatsPacket.ReturnHelp();
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[CharStat]CharacterName: {packet.CharacterName}");

                string name = packet.CharacterName;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (ServerManager.Instance.GetSessionByCharacterName(name) != null)
                    {
                        Character character = ServerManager.Instance.GetSessionByCharacterName(name).Character;
                        SendStats(character);
                    }
                    else if (DAOFactory.CharacterDAO.LoadByName(name) != null)
                    {
                        CharacterDTO characterDto = DAOFactory.CharacterDAO.LoadByName(name);
                        SendStats(characterDto);
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(returnHelp, 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(returnHelp, 10));
            }
        }

        /// <summary>
        /// $Clear Command
        /// </summary>
        /// <param name="packet"></param>
        public void ClearInventory(ClearInventoryPacket packet)
        {
            if (packet != null && packet.InventoryType != InventoryType.Wear)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Clear]InventoryType: {packet.InventoryType}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Parallel.ForEach(Session.Character.Inventory.Where(s => s.Type == packet.InventoryType),
                    inv =>
                    {
                        Session.Character.Inventory.DeleteById(inv.Id);
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(inv.Type, inv.Slot));
                    });
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ClearInventoryPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ClearMap packet
        /// </summary>
        /// <param name="packet"></param>
        public void ClearMap(ClearMapPacket packet)
        {
            if (packet != null && Session.HasCurrentMapInstance)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[ClearMap]MapId: {Session.CurrentMapInstance.MapInstanceId}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Parallel.ForEach(Session.CurrentMapInstance.Monsters.Where(s => s.ShouldRespawn != true), monster =>
                {
                    Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster,
                        monster.MapMonsterId));
                    monster.SetDeathStatement();
                    Session.CurrentMapInstance.RemoveMonster(monster);
                });
                Parallel.ForEach(Session.CurrentMapInstance.DroppedList.GetAllItems(), drop =>
                {
                    Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Object, drop.TransportId));
                    Session.CurrentMapInstance.DroppedList.Remove(drop.TransportId);
                });
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ClearMapPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Clone Command
        /// </summary>
        /// <param name="packet"></param>
        public void CloneItem(CloneItemPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Clone]Slot: {packet.Slot}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                ItemInstance item =
                    Session.Character.Inventory.LoadBySlotAndType(packet.Slot, InventoryType.Equipment);
                if (item != null)
                {
                    item = item.DeepCopy();
                    item.Id = Guid.NewGuid();
                    Session.Character.Inventory.AddToInventory(item);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(CloneItemPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Help Command
        /// </summary>
        /// <param name="packet"></param>
        public void Command(HelpPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[Help]");

            // get commands
            List<Type> classes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t =>
                t.IsClass && t.Namespace == "OpenNos.GameObject.CommandPackets"
                && (((PacketHeaderAttribute)Array.Find(t.GetCustomAttributes(true),
                     ca => ca.GetType().Equals(typeof(PacketHeaderAttribute))))?.Authorities)
                     .Any(c => Session.Account.Authority.Equals(c)
                     || Session.Account.Authority.Equals(AuthorityType.Administrator)
                     || c.Equals(AuthorityType.User) && Session.Account.Authority >= AuthorityType.User
                     || c.Equals(AuthorityType.TMOD) && Session.Account.Authority >= AuthorityType.TMOD && Session.Account.Authority <= AuthorityType.BA
                     || c.Equals(AuthorityType.MOD) && Session.Account.Authority >= AuthorityType.MOD && Session.Account.Authority <= AuthorityType.BA
                     || c.Equals(AuthorityType.SMOD) && Session.Account.Authority >= AuthorityType.SMOD && Session.Account.Authority <= AuthorityType.BA
                     || c.Equals(AuthorityType.TGM) && Session.Account.Authority >= AuthorityType.TGM
                     || c.Equals(AuthorityType.GM) && Session.Account.Authority >= AuthorityType.GM
                     || c.Equals(AuthorityType.SGM) && Session.Account.Authority >= AuthorityType.SGM
                     || c.Equals(AuthorityType.GA) && Session.Account.Authority >= AuthorityType.GA
                     || c.Equals(AuthorityType.TM) && Session.Account.Authority >= AuthorityType.TM
                     || c.Equals(AuthorityType.CM) && Session.Account.Authority >= AuthorityType.CM
                     || c.Equals(AuthorityType.BitchNiggerFaggot) && Session.Account.Authority.Equals(AuthorityType.BitchNiggerFaggot)
                     )).ToList();
            List<string> messages = new List<string>();
            foreach (Type type in classes)
            {
                object classInstance = Activator.CreateInstance(type);
                Type classType = classInstance.GetType();
                MethodInfo method = classType.GetMethod("ReturnHelp");
                if (method != null)
                {
                    messages.Add(method.Invoke(classInstance, null).ToString());
                }
            }

            // send messages
            messages.Sort();
            if (packet.Contents == "*" || string.IsNullOrEmpty(packet.Contents))
            {
                Session.SendPacket(Session.Character.GenerateSay("-------------Commands Info-------------", 11));
                foreach (string message in messages)
                {
                    Session.SendPacket(Session.Character.GenerateSay(message, 12));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay("-------------Command Info-------------", 11));
                foreach (string message in messages.Where(s =>
                    s.IndexOf(packet.Contents, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    Session.SendPacket(Session.Character.GenerateSay(message, 12));
                }
            }

            Session.SendPacket(Session.Character.GenerateSay("-----------------------------------------------", 11));
        }

        /// <summary>
        /// $CreateItem Packet
        /// </summary>
        /// <param name="packet"></param>
        public void CreateItem(CreateItemPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[CreateItem]ItemVNum: {packet.VNum} Amount/Design: {packet.Design} Upgrade: {packet.Upgrade}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                short vnum = packet.VNum;
                short amount = 1;
                sbyte rare = 0;
                byte upgrade = 0, design = 0;
                if (vnum == 1046)
                {
                    return; // cannot create gold as item, use $Gold instead
                }

                Item iteminfo = ServerManager.GetItem(vnum);
                if (iteminfo != null)
                {
                    if (iteminfo.IsColored || (iteminfo.ItemType == ItemType.Box && iteminfo.ItemSubType == 3))
                    {
                        if (packet.Design.HasValue)
                        {
                            rare = (sbyte)ServerManager.RandomNumber();
                            if (rare > 90)
                            {
                                rare = 7;
                            }
                            else if (rare > 80)
                            {
                                rare = 6;
                            }
                            else
                            {
                                rare = (sbyte)ServerManager.RandomNumber(1, 6);
                            }
                            design = (byte)packet.Design.Value;
                        }
                    }
                    else if (iteminfo.Type == 0)
                    {
                        if (packet.Upgrade.HasValue)
                        {
                            if (iteminfo.EquipmentSlot != EquipmentType.Sp)
                            {
                                upgrade = packet.Upgrade.Value;
                            }
                            else
                            {
                                design = packet.Upgrade.Value;
                            }

                            if (iteminfo.EquipmentSlot != EquipmentType.Sp && upgrade == 0
                                && iteminfo.BasicUpgrade != 0)
                            {
                                upgrade = iteminfo.BasicUpgrade;
                            }
                        }

                        if (packet.Design.HasValue)
                        {
                            if (iteminfo.EquipmentSlot == EquipmentType.Sp)
                            {
                                upgrade = (byte)packet.Design.Value;
                            }
                            else
                            {
                                rare = (sbyte)packet.Design.Value;
                            }
                        }
                    }

                    if (packet.Design.HasValue && !packet.Upgrade.HasValue)
                    {
                        amount = packet.Design.Value > 999 ? (short)999 : packet.Design.Value;
                    }

                    ItemInstance inv = Session.Character.Inventory
                        .AddNewToInventory(vnum, amount, Rare: rare, Upgrade: upgrade, Design: design).FirstOrDefault();
                    if (inv != null)
                    {
                        ItemInstance wearable = Session.Character.Inventory.LoadBySlotAndType(inv.Slot, inv.Type);
                        if (wearable != null)
                        {
                            switch (wearable.Item.EquipmentSlot)
                            {
                                case EquipmentType.Armor:
                                case EquipmentType.MainWeapon:
                                case EquipmentType.SecondaryWeapon:
                                    wearable.SetRarityPoint();
                                    break;

                                case EquipmentType.Boots:
                                case EquipmentType.Gloves:
                                    wearable.FireResistance = (short)(wearable.Item.FireResistance * upgrade);
                                    wearable.DarkResistance = (short)(wearable.Item.DarkResistance * upgrade);
                                    wearable.LightResistance = (short)(wearable.Item.LightResistance * upgrade);
                                    wearable.WaterResistance = (short)(wearable.Item.WaterResistance * upgrade);
                                    break;
                            }
                        }

                        Session.SendPacket(Session.Character.GenerateSay(
                            $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {iteminfo.Name[Session.Account.Language]} x {amount}", 12));
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                0));
                    }
                }
                else
                {
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_ITEM"), 0);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(CreateItemPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Demote Command
        /// </summary>
        /// <param name="packet"></param>
        public void Demote(DemotePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Demote]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = packet.CharacterName;
                try
                {
                    AccountDTO account = DAOFactory.AccountDAO.LoadById(DAOFactory.CharacterDAO.LoadByName(name).AccountId);
                    if (account?.Authority > AuthorityType.User)
                    {
                        if (Session.Account.Authority >= account?.Authority)
                        {
                            AuthorityType newAuthority = AuthorityType.User;
                            switch (account.Authority)
                            {
                                case AuthorityType.DEV:
                                    newAuthority = AuthorityType.CM;
                                    break;
                                case AuthorityType.CM:
                                    newAuthority = AuthorityType.TM;
                                    break;
                                case AuthorityType.TM:
                                    newAuthority = AuthorityType.GA;
                                    break;
                                case AuthorityType.GA:
                                    newAuthority = AuthorityType.SGM;
                                    break;
                                case AuthorityType.SGM:
                                    newAuthority = AuthorityType.GM;
                                    break;
                                case AuthorityType.GM:
                                    newAuthority = AuthorityType.GS;
                                    break;
                                case AuthorityType.GS:
                                    newAuthority = AuthorityType.User;
                                    break;
                                default:
                                    newAuthority = AuthorityType.User;
                                    break;
                            }
                            account.Authority = newAuthority;
                            DAOFactory.AccountDAO.InsertOrUpdate(ref account);
                            ClientSession session =
                                ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.Name == name);
                            if (session != null)
                            {
                                session.Account.Authority = newAuthority;
                                session.Character.Authority = newAuthority;
                                if (session.Character.InvisibleGm)
                                {
                                    session.Character.Invisible = false;
                                    session.Character.InvisibleGm = false;
                                    Session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                                        Session.CurrentMapInstance?.Broadcast(m.GenerateIn(), ReceiverType.AllExceptMe));
                                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                                        ReceiverType.AllExceptMe);
                                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                                        ReceiverType.AllExceptMe);
                                }
                                ServerManager.Instance.ChangeMap(session.Character.CharacterId);
                                DAOFactory.AccountDAO.WriteGeneralLog(session.Account.AccountId, session.IpAddress,
                                    session.Character.CharacterId, GeneralLogType.Demotion, $"by: {Session.Character.Name}");
                            }
                            else
                            {
                                DAOFactory.AccountDAO.WriteGeneralLog(account.AccountId, "25.52.104.84", null,
                                    GeneralLogType.Demotion, $"by: {Session.Character.Name}");
                            }

                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THAT"), 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                    }
                }
                catch
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(DemotePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $DropRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void DropRate(DropRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[DropRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateDrop = packet.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("DROP_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(DropRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Effect Command
        /// </summary>
        /// <param name="packet"></param>
        public void Effect(EffectCommandPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Effect]EffectId: {packet.EffectId}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.CurrentMapInstance?.Broadcast(
                    StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId,
                        packet.EffectId), Session.Character.PositionX, Session.Character.PositionY);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(EffectCommandPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Faction Command
        /// </summary>
        /// <param name="packet"></param>
        public void Faction(FactionPacket packet)
        {

            if (ServerManager.Instance.ChannelId == 51)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED_ACT4"),
                        0));
                return;
            }
            if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4ShipAngel
                || Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4ShipDemon)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED_ACT4SHIP"),
                        0));
                return;
            }
            if (packet != null)
            {
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.SendPacket("scr 0 0 0 0 0 0 0");
                if (Session.Character.Faction == FactionType.Angel)
                {
                    Session.Character.Faction = FactionType.Demon;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_2"),
                            0));
                }
                else
                {
                    Session.Character.Faction = FactionType.Angel;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_1"),
                            0));
                }
                Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player,
                    Session.Character.CharacterId, 4799 + (byte)Session.Character.Faction));
                Session.SendPacket(Session.Character.GenerateFaction());
                if (ServerManager.Instance.ChannelId == 51)
                {
                    Session.SendPacket(Session.Character.GenerateFc());
                }
            }
        }

        /// <summary>
        /// $FamilyFaction Command
        /// </summary>
        /// <param name="packet"></param>
        public void FamilyFaction(FamilyFactionPacket packet)
        {

            if (ServerManager.Instance.ChannelId == 51)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED_ACT4"),
                        0));
                return;
            }
            if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4ShipAngel
                || Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4ShipDemon)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED_ACT4SHIP"),
                        0));
                return;
            }
            if (packet != null)
            {
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (String.IsNullOrEmpty(packet.FamilyName) && Session.Character.Family != null)
                {
                    Session.Character.Family.ChangeFaction(Session.Character.Family.FamilyFaction == 1 ? (byte)2 : (byte)1, Session);
                    return;
                }
                Family family = ServerManager.Instance.FamilyList.FirstOrDefault(s => s.Name == packet.FamilyName);
                if (family != null)
                {
                    family.ChangeFaction(family.FamilyFaction == 1 ? (byte)2 : (byte)1, Session);
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay("Family not found.", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(FamilyFactionPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $FairyXPRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void FairyXpRate(FairyXpRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[FairyXPRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateFairyXP = packet.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("FAIRYXP_RATE_CHANGED"),
                            0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(FairyXpRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Gift Command
        /// </summary>
        /// <param name="packet"></param>
        public void Gift(GiftPacket packet)
        {
            if (packet != null)
            {
                short Amount = packet.Amount;

                if (Amount <= 0)
                {
                    Amount = 1;
                }

                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Gift]CharacterName: {packet.CharacterName} ItemVNum: {packet.VNum} Amount: {Amount} Rare: {packet.Rare} Upgrade: {packet.Upgrade}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.CharacterName == "*")
                {
                    if (Session.HasCurrentMapInstance)
                    {
                        Parallel.ForEach(Session.CurrentMapInstance.Sessions,
                            session => Session.Character.SendGift(session.Character.CharacterId, packet.VNum,
                                Amount, packet.Rare, packet.Upgrade, packet.Design, false));
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GIFT_SENT"), 10));
                    }
                }
                else if (packet.CharacterName == "ALL")
                {
                    int levelMin = packet.ReceiverLevelMin;
                    int levelMax = packet.ReceiverLevelMax == 0 ? 99 : packet.ReceiverLevelMax;

                    DAOFactory.CharacterDAO.LoadAll().ToList().ForEach(chara =>
                    {
                        if (chara.Level >= levelMin && chara.Level <= levelMax)
                        {
                            Session.Character.SendGift(chara.CharacterId, packet.VNum, Amount,
                                packet.Rare, packet.Upgrade, packet.Design, false);
                        }
                    });
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GIFT_SENT"), 10));
                }
                else
                {
                    CharacterDTO chara = DAOFactory.CharacterDAO.LoadByName(packet.CharacterName);
                    if (chara != null)
                    {
                        Session.Character.SendGift(chara.CharacterId, packet.VNum, Amount,
                            packet.Rare, packet.Upgrade, packet.Design, false);
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GIFT_SENT"), 10));
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"),
                                0));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GiftPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $GodMode Command
        /// </summary>
        /// <param name="packet"></param>
        public void GodMode(GodModePacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[GodMode]");

            Session.Character.HasGodMode = !Session.Character.HasGodMode;
            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        /// <summary>
        /// $Gold Command
        /// </summary>
        /// <param name="packet"></param>
        public void Gold(GoldPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Gold]Amount: {packet.Amount}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                long gold = packet.Amount;
                long maxGold = ServerManager.Instance.Configuration.MaxGold;
                gold = gold > maxGold ? maxGold : gold;
                if (gold >= 0)
                {
                    Session.Character.Gold = gold;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOLD_SET"),
                        0));
                    Session.SendPacket(Session.Character.GenerateGold());
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GoldPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $GoldDropRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void GoldDropRate(GoldDropRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[GoldDropRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateGoldDrop = packet.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOLD_DROP_RATE_CHANGED"),
                            0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GoldDropRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $GoldRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void GoldRate(GoldRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[GoldRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateGold = packet.Value;

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOLD_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GoldRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ReputationRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void ReputationRate(ReputationRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[ReputationRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateReputation = packet.Value;

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REPUTATION_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GoldRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Guri Command
        /// </summary>
        /// <param name="packet"></param>
        public void Guri(GuriCommandPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Guri]Type: {packet.Type} Value: {packet.Value} Arguments: {packet.Argument}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.SendPacket(UserInterfaceHelper.GenerateGuri(packet.Type, packet.Argument,
                    Session.Character.CharacterId, packet.Value));
            }

            Session.Character.GenerateSay(GuriCommandPacket.ReturnHelp(), 10);
        }

        /// <summary>
        /// $HairColor Command
        /// </summary>
        /// <param name="packet"></param>
        public void Haircolor(HairColorPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[HairColor]HairColor: {packet.HairColor}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.Character.HairColor = packet.HairColor;
                Session.SendPacket(Session.Character.GenerateEq());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateIn());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HairColorPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $HairStyle Command
        /// </summary>
        /// <param name="packet"></param>
        public void Hairstyle(HairStylePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[HairStyle]HairStyle: {packet.HairStyle}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.Character.HairStyle = packet.HairStyle;
                Session.SendPacket(Session.Character.GenerateEq());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateIn());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HairStylePacket.ReturnHelp(), 10));
            }
        }

        public void HelpMe(HelpMePacket packet)
        {
            if (packet != null && !string.IsNullOrWhiteSpace(packet.Message))
            {
                int count = 0;
                foreach (ClientSession team in ServerManager.Instance.Sessions.Where(s =>
                    s.Account.Authority >= AuthorityType.GM))
                {
                    if (team.HasSelectedCharacter)
                    {
                        count++;

                        // TODO: move that to resx soo we follow i18n
                        team.SendPacket(team.Character.GenerateSay($"User {Session.Character.Name} needs your help!",
                            12));
                        team.SendPacket(team.Character.GenerateSay($"Reason: {packet.Message}", 12));
                        team.SendPacket(
                            team.Character.GenerateSay("Please inform the family chat when you take care of!", 12));
                        team.SendPacket(Session.Character.GenerateSpk("Click this message to start chatting.", 5));
                        team.SendPacket(
                            UserInterfaceHelper.GenerateMsg($"User {Session.Character.Name} needs your help!", 0));
                    }
                }

                if (count != 0)
                {
                    Session.SendPacket(Session.Character.GenerateSay(
                        $"{count} Team members were informed! You should get a message shortly.", 10));
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(
                        "Sadly, there are no online team member right now. Please ask for help on our Discord Server at:",
                        10));
                    Session.SendPacket(Session.Character.GenerateSay("https://discord.gg/aujJkHN", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HelpMePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $HeroXPRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void HeroXpRate(HeroXpRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[HeroXPRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateHeroicXP = packet.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HEROXP_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HeroXpRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Invisible Command
        /// </summary>
        /// <param name="packet"></param>
        public void Invisible(InvisiblePacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[Invisible]");

            Session.Character.Invisible = !Session.Character.Invisible;
            Session.Character.InvisibleGm = !Session.Character.InvisibleGm;
            Session.SendPacket(Session.Character.GenerateInvisible());
            Session.SendPacket(Session.Character.GenerateEq());
            Session.SendPacket(Session.Character.GenerateCMode());

            if (Session.Character.InvisibleGm)
            {
                Session.Character.Mates.Where(s => s.IsTeamMember).ToList()
                    .ForEach(s => Session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                Session.CurrentMapInstance?.Broadcast(Session,
                    StaticPacketHelper.Out(UserType.Player, Session.Character.CharacterId), ReceiverType.AllExceptMe);
            }
            else
            {
                foreach (Mate teamMate in Session.Character.Mates.Where(m => m.IsTeamMember))
                {
                    teamMate.PositionX = Session.Character.PositionX;
                    teamMate.PositionY = Session.Character.PositionY;
                    teamMate.UpdateBushFire();
                    Parallel.ForEach(Session.CurrentMapInstance.Sessions.Where(s => s.Character != null), s =>
                    {
                        if (ServerManager.Instance.ChannelId != 51 || Session.Character.Faction == s.Character.Faction)
                        {
                            s.SendPacket(teamMate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                        }
                        else
                        {
                            s.SendPacket(teamMate.GenerateIn(true, ServerManager.Instance.ChannelId == 51, s.Account.Authority));
                        }
                    });
                    Session.SendPacket(Session.Character.GeneratePinit());
                    Session.Character.Mates.ForEach(s => Session.SendPacket(s.GenerateScPacket()));
                    Session.SendPackets(Session.Character.GeneratePst());
                }
                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                    ReceiverType.AllExceptMe);
                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                    ReceiverType.AllExceptMe);
            }
        }

        /// <summary>
        /// $ItemRain Command
        /// </summary>
        /// <param name="packet"></param>
        public void ItemRain(ItemRainPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                       $"[ItemRain]ItemVNum: {packet.VNum} Amount: {packet.Amount} Count: {packet.Count} Time: {packet.Time}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                short vnum = packet.VNum;
                short amount = packet.Amount;
                if (amount > 999) { amount = 999; }
                int count = packet.Count;
                int time = packet.Time;
                
                GameObject.MapInstance instance = Session.CurrentMapInstance;

                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(observer =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        MapCell cell = instance.Map.GetRandomPosition();
                        MonsterMapItem droppedItem = new MonsterMapItem(cell.X, cell.Y, vnum, amount);
                        instance.DroppedList[droppedItem.TransportId] = droppedItem;
                        instance.Broadcast(
                            $"drop {droppedItem.ItemVNum} {droppedItem.TransportId} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)} 0 -1");

                        System.Threading.Thread.Sleep(time * 1000 / count);
                    }
                });
            }
        }

        /// <summary>
        /// $Kick Command
        /// </summary>
        /// <param name="packet"></param>
        public void Kick(KickPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Kick]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.CharacterName == "*")
                {
                    Parallel.ForEach(ServerManager.Instance.Sessions, session => session.Disconnect());
                }

                ServerManager.Instance.Kick(packet.CharacterName);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(KickPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $KickSession Command
        /// </summary>
        /// <param name="packet"></param>
        public void KickSession(KickSessionPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Kick]AccountName: {packet.AccountName} SessionId: {packet.SessionId}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.SessionId.HasValue) //if you set the sessionId, remove account verification
                {
                    packet.AccountName = "";
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                AccountDTO account = DAOFactory.AccountDAO.LoadByName(packet.AccountName);
                CommunicationServiceClient.Instance.KickSession(account?.AccountId, packet.SessionId);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(KickSessionPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Kill Command
        /// </summary>
        /// <param name="packet"></param>
        public void Kill(KillPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Kill]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                ClientSession sess = ServerManager.Instance.GetSessionByCharacterName(packet.CharacterName);
                if (sess != null)
                {
                    if (sess.Character.HasGodMode)
                    {
                        return;
                    }

                    if (sess.Character.Hp < 1)
                    {
                        return;
                    }

                    sess.Character.Hp = 0;
                    sess.Character.LastDefence = DateTime.Now;
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                        Session.Character.CharacterId, 1, sess.Character.CharacterId, 1114, 4, 11, 4260, 0, 0, false, 0, 60000, 3, 0));
                    sess.SendPacket(sess.Character.GenerateStat());
                    if (sess.Character.IsVehicled)
                    {
                        sess.Character.RemoveVehicle();
                    }
                    ServerManager.Instance.AskRevive(sess.Character.CharacterId);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(KillPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $PenaltyLog Command
        /// </summary>
        /// <param name="packet"></param>
        public void ListPenalties(PenaltyLogPacket packet)
        {
            string returnHelp = CharacterStatsPacket.ReturnHelp();
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[PenaltyLog]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = packet.CharacterName;
                if (!string.IsNullOrEmpty(name))
                {
                    CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(name);
                    if (character != null)
                    {
                        bool separatorSent = false;

                        void WritePenalty(PenaltyLogDTO penalty)
                        {
                            Session.SendPacket(Session.Character.GenerateSay($"Type: {penalty.Penalty}", 13));
                            Session.SendPacket(Session.Character.GenerateSay($"AdminName: {penalty.AdminName}", 13));
                            Session.SendPacket(Session.Character.GenerateSay($"Reason: {penalty.Reason}", 13));
                            Session.SendPacket(Session.Character.GenerateSay($"DateStart: {penalty.DateStart}", 13));
                            Session.SendPacket(Session.Character.GenerateSay($"DateEnd: {penalty.DateEnd}", 13));
                            Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
                            separatorSent = true;
                        }

                        IEnumerable<PenaltyLogDTO> penaltyLogs = ServerManager.Instance.PenaltyLogs
                            .Where(s => s.AccountId == character.AccountId).ToList();

                        //PenaltyLogDTO penalty = penaltyLogs.LastOrDefault(s => s.DateEnd > DateTime.Now);
                        Session.SendPacket(Session.Character.GenerateSay("----- PENALTIES -----", 13));

                        #region Warnings

                        Session.SendPacket(Session.Character.GenerateSay("----- WARNINGS -----", 13));
                        foreach (PenaltyLogDTO penaltyLog in penaltyLogs.Where(s => s.Penalty == PenaltyType.Warning)
                            .OrderBy(s => s.DateStart))
                        {
                            WritePenalty(penaltyLog);
                        }

                        if (!separatorSent)
                        {
                            Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
                        }

                        separatorSent = false;

                        #endregion

                        #region Mutes

                        Session.SendPacket(Session.Character.GenerateSay("----- MUTES -----", 13));
                        foreach (PenaltyLogDTO penaltyLog in penaltyLogs.Where(s => s.Penalty == PenaltyType.Muted)
                            .OrderBy(s => s.DateStart))
                        {
                            WritePenalty(penaltyLog);
                        }

                        if (!separatorSent)
                        {
                            Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
                        }

                        separatorSent = false;

                        #endregion

                        #region Bans

                        Session.SendPacket(Session.Character.GenerateSay("----- BANS -----", 13));
                        foreach (PenaltyLogDTO penaltyLog in penaltyLogs.Where(s => s.Penalty == PenaltyType.Banned)
                            .OrderBy(s => s.DateStart))
                        {
                            WritePenalty(penaltyLog);
                        }

                        if (!separatorSent)
                        {
                            Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
                        }

                        #endregion

                        Session.SendPacket(Session.Character.GenerateSay("----- SUMMARY -----", 13));
                        Session.SendPacket(Session.Character.GenerateSay(
                            $"Warnings: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Warning)}", 13));
                        Session.SendPacket(
                            Session.Character.GenerateSay(
                                $"Mutes: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Muted)}", 13));
                        Session.SendPacket(
                            Session.Character.GenerateSay(
                                $"Bans: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Banned)}", 13));
                        Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(returnHelp, 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(returnHelp, 10));
            }
        }

        /// <summary>
        /// $MapDance Command
        /// </summary>
        /// <param name="packet"></param>
        public void MapDance(MapDancePacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[MapDance]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            if (Session.HasCurrentMapInstance)
            {
                Session.CurrentMapInstance.IsDancing = !Session.CurrentMapInstance.IsDancing;
                if (Session.CurrentMapInstance.IsDancing)
                {
                    Session.Character.Dance();
                    Session.CurrentMapInstance?.Broadcast("dance 2");
                }
                else
                {
                    Session.Character.Dance();
                    Session.CurrentMapInstance?.Broadcast("dance");
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
        }

        /// <summary>
        /// $MapPVP Command
        /// </summary>
        /// <param name="packet"></param>
        public void MapPvp(MapPVPPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[MapPVP]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            Session.CurrentMapInstance.IsPVP = !Session.CurrentMapInstance.IsPVP;
            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        /// <summary>
        /// $Morph Command
        /// </summary>
        /// <param name="packet"></param>
        public void Morph(MorphPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Morph]MorphId: {packet.MorphId} MorphDesign: {packet.MorphDesign} Upgrade: {packet.Upgrade} MorphId: {packet.ArenaWinner}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.MorphId < 30 && packet.MorphId > 0)
                {
                    Session.Character.UseSp = true;
                    Session.Character.Morph = packet.MorphId;
                    Session.Character.MorphUpgrade = packet.Upgrade;
                    Session.Character.MorphUpgrade2 = packet.MorphDesign;
                    Session.Character.ArenaWinner = packet.ArenaWinner;
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateCMode());
                }
                else if (packet.MorphId > 30)
                {
                    Session.Character.IsVehicled = true;
                    Session.Character.Morph = packet.MorphId;
                    Session.Character.ArenaWinner = packet.ArenaWinner;
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateCMode());
                }
                else
                {
                    Session.Character.IsVehicled = false;
                    Session.Character.UseSp = false;
                    Session.Character.ArenaWinner = 0;
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateCMode());
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MorphPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Mute Command
        /// </summary>
        /// <param name="packet"></param>
        public void Mute(MutePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Mute]CharacterName: {packet.CharacterName} Reason: {packet.Reason} Until: {DateTime.Now.AddMinutes(packet.Duration)}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Duration == 0)
                {
                    packet.Duration = 60;
                }

                packet.Reason = packet.Reason?.Trim();
                MuteMethod(packet.CharacterName, packet.Reason, packet.Duration);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MutePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Packet Command
        /// </summary>
        /// <param name="packet"></param>
        public void PacketCallBack(PacketCallbackPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Packet]Packet: {packet.Packet}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                Session.SendPacket(packet.Packet);
                Session.SendPacket(Session.Character.GenerateSay(packet.Packet, 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(PacketCallbackPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Maintenance Command
        /// </summary>
        /// <param name="packet"></param>
        public void PlanMaintenance(MaintenancePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Maintenance]Delay: {packet.Delay} Duration: {packet.Duration} Reason: {packet.Reason}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                DateTime dateStart = DateTime.Now.AddMinutes(packet.Delay);
                MaintenanceLogDTO maintenance = new MaintenanceLogDTO
                {
                    DateEnd = dateStart.AddMinutes(packet.Duration),
                    DateStart = dateStart,
                    Reason = packet.Reason
                };
                DAOFactory.MaintenanceLogDAO.Insert(maintenance);
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MaintenancePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $PortalTo Command
        /// </summary>
        /// <param name="packet"></param>
        public void PortalTo(PortalToPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[PortalTo]DestinationMapId: {packet.DestinationMapId} DestinationMapX: {packet.DestinationX} DestinationY: {packet.DestinationY}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                AddPortal(packet.DestinationMapId, packet.DestinationX, packet.DestinationY,
                    packet.PortalType == null ? (short)-1 : (short)packet.PortalType, false);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(PortalToPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Position Command
        /// </summary>
        /// <param name="packet"></param>
        public void Position(PositionPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), "[Position]");
            Session.SendPacket(Session.Character.GenerateSay(
                $"Map:{Session.Character.MapInstance.Map.MapId} - X:{Session.Character.PositionX} - Y:{Session.Character.PositionY} - Dir:{Session.Character.Direction} - Cell:{Session.CurrentMapInstance.Map.JaggedGrid[Session.Character.PositionX][Session.Character.PositionY]?.Value}",
                12));
        }

        /// <summary>
        /// $Promote Command
        /// </summary>
        /// <param name="packet"></param>
        public void Promote(PromotePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Promote]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = packet.CharacterName;
                try
                {
                    AccountDTO account = DAOFactory.AccountDAO.LoadById(DAOFactory.CharacterDAO.LoadByName(name).AccountId);
                    if (account?.Authority >= AuthorityType.User || account.Authority.Equals(AuthorityType.BitchNiggerFaggot))
                    {
                        if (account.Authority < Session.Account.Authority)
                        {
                            AuthorityType newAuthority = AuthorityType.User;
                            switch (account.Authority)
                            {
                                case AuthorityType.User:
                                    newAuthority = AuthorityType.GS;
                                    break;
                                case AuthorityType.GS:
                                    newAuthority = AuthorityType.GM;
                                    break;
                                case AuthorityType.GM:
                                    newAuthority = AuthorityType.SGM;
                                    break;
                                case AuthorityType.SGM:
                                    newAuthority = AuthorityType.GA;
                                    break;
                                case AuthorityType.GA:
                                    newAuthority = AuthorityType.TM;
                                    break;
                                case AuthorityType.TM:
                                    newAuthority = AuthorityType.CM;
                                    break;
                                case AuthorityType.CM:
                                    newAuthority = AuthorityType.DEV;
                                    break;
                                default:
                                    newAuthority = account.Authority;
                                    break;
                            }
                            account.Authority = newAuthority;
                            DAOFactory.AccountDAO.InsertOrUpdate(ref account);
                            ClientSession session =
                                ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.Name == name);

                            if (session != null)
                            {
                                session.Account.Authority = newAuthority;
                                session.Character.Authority = newAuthority;
                                ServerManager.Instance.ChangeMap(session.Character.CharacterId);
                                DAOFactory.AccountDAO.WriteGeneralLog(session.Account.AccountId, session.IpAddress,
                                    session.Character.CharacterId, GeneralLogType.Promotion, $"by: {Session.Character.Name}");
                            }
                            else
                            {
                                DAOFactory.AccountDAO.WriteGeneralLog(account.AccountId, "25.52.104.84", null,
                                    GeneralLogType.Promotion, $"by: {Session.Character.Name}");
                            }

                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THAT"), 10));
                        }
                    
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                    }
                }
                catch
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(PromotePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Rarify Command
        /// </summary>
        /// <param name="packet"></param>
        public void Rarify(RarifyPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Rarify]Slot: {packet.Slot} Mode: {packet.Mode} Protection: {packet.Protection}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Slot >= 0)
                {
                    ItemInstance wearableInstance = Session.Character.Inventory.LoadBySlotAndType(packet.Slot, 0);
                    wearableInstance?.RarifyItem(Session, packet.Mode, packet.Protection);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(RarifyPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $RemoveMob Packet
        /// </summary>
        /// <param name="packet"></param>
        public void RemoveMob(RemoveMobPacket packet)
        {
            if (Session.HasCurrentMapInstance)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[RemoveMob]NpcMonsterId: {Session.Character.LastNpcMonsterId}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                MapMonster monster = Session.CurrentMapInstance.GetMonsterById(Session.Character.LastNpcMonsterId);
                MapNpc npc = Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId);
                if (monster != null)
                {

                    int distance = Map.GetDistance(new MapCell
                    {
                        X = Session.Character.PositionX,
                        Y = Session.Character.PositionY
                    }, new MapCell
                    {
                        X = monster.MapX,
                        Y = monster.MapY
                    });
                    if (distance > 5)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("TOO_FAR")), 11));
                        return;
                    }

                    if (monster.IsAlive)
                    {
                        Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster,
                            monster.MapMonsterId));
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("MONSTER_REMOVED"), monster.MapMonsterId,
                                monster.Monster.Name, monster.MapId, monster.MapX, monster.MapY), 12));
                        Session.CurrentMapInstance.RemoveMonster(monster);
                        Session.CurrentMapInstance.RemovedMobNpcList.Add(monster);
                        if (DAOFactory.MapMonsterDAO.LoadById(monster.MapMonsterId) != null)
                        {
                            DAOFactory.MapMonsterDAO.DeleteById(monster.MapMonsterId);
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("MONSTER_NOT_ALIVE")), 11));
                    }
                }
                else if (npc != null)
                {

                    int distance = Map.GetDistance(new MapCell
                    {
                        X = Session.Character.PositionX,
                        Y = Session.Character.PositionY
                    }, new MapCell
                    {
                        X = npc.MapX,
                        Y = npc.MapY
                    });
                    if (distance > 5)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("TOO_FAR")), 11));
                        return;
                    }

                    if (!npc.IsMate && !npc.IsDisabled && !npc.IsProtected)
                    {
                        Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Npc, npc.MapNpcId));
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NPCMONSTER_REMOVED"), npc.MapNpcId,
                                npc.Npc.Name, npc.MapId, npc.MapX, npc.MapY), 12));
                        Session.CurrentMapInstance.RemoveNpc(npc);
                        Session.CurrentMapInstance.RemovedMobNpcList.Add(npc);
                        if (DAOFactory.ShopDAO.LoadByNpc(npc.MapNpcId) != null)
                        {
                            DAOFactory.ShopDAO.DeleteByNpcId(npc.MapNpcId);
                        }

                        if (DAOFactory.MapNpcDAO.LoadById(npc.MapNpcId) != null)
                        {
                            DAOFactory.MapNpcDAO.DeleteById(npc.MapNpcId);
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NPC_CANNOT_BE_REMOVED")), 11));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NPCMONSTER_NOT_FOUND"), 11));
                }
            }
        }

        /// <summary>
        /// $RemovePortal Command
        /// </summary>
        /// <param name="packet"></param>
        public void RemovePortal(RemovePortalPacket packet)
        {
            if (Session.HasCurrentMapInstance)
            {
                Portal portal = Session.CurrentMapInstance.Portals.Find(s =>
                    s.SourceMapInstanceId == Session.Character.MapInstanceId && Map.GetDistance(
                        new MapCell { X = s.SourceX, Y = s.SourceY },
                        new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY }) < 10);
                if (portal != null)
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[RemovePortal]MapId: {portal.SourceMapId} MapX: {portal.SourceX} MapY: {portal.SourceY}");
                    LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                    Session.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("NEAREST_PORTAL"), portal.SourceMapId,
                            portal.SourceX, portal.SourceY), 12));
                    portal.IsDisabled = true;
                    Session.CurrentMapInstance?.Broadcast(portal.GenerateGp());
                    Session.CurrentMapInstance.Portals.Remove(portal);
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_PORTAL_FOUND"), 11));
                }
            }
        }

        /// <summary>
        /// $Resize Command
        /// </summary>
        /// <param name="packet"></param>
        public void Resize(ResizePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Resize]Size: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value >= 0)
                {
                    Session.Character.Size = packet.Value;
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateScal());
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ResizePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Restart Command
        /// </summary>
        /// <param name="packet"></param>
        public void Restart(RestartPacket packet)
        {
            int time = packet.Time > 0 ? packet.Time : 5;

            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Restart]Time: {time}");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            if (ServerManager.Instance.TaskShutdown != null)
            {
                ServerManager.Instance.ShutdownStop = true;
                ServerManager.Instance.TaskShutdown = null;
            }
            else
            {
                ServerManager.Instance.IsReboot = true;
                ServerManager.Instance.TaskShutdown = ServerManager.Instance.ShutdownTaskAsync(time);
            }

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        /// <summary>
        /// $RestartAll Command
        /// </summary>
        /// <param name="packet"></param>
        public void RestartAll(RestartAllPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[RestartAll]");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string worldGroup = !string.IsNullOrEmpty(packet.WorldGroup) ? packet.WorldGroup : ServerManager.Instance.ServerGroup;

                int time = packet.Time;

                if (time < 1)
                {
                    time = 5;
                }

                CommunicationServiceClient.Instance.Restart(worldGroup, time);

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(RestartAllPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $SearchItem Command
        /// </summary>
        /// <param name="packet"></param>
        public void SearchItem(SearchItemPacket packet)
        {
            if (packet != null)
            {
                string contents = packet.Contents;
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[SearchItem]Contents: {(string.IsNullOrEmpty(contents) ? "none" : contents)}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = "";
                byte page = 0;
                if (!string.IsNullOrEmpty(contents))
                {
                    string[] packetsplit = contents.Split(' ');
                    bool withPage = byte.TryParse(packetsplit[0], out page);
                    name = packetsplit.Length == 1 && withPage
                        ? ""
                        : packetsplit.Skip(withPage ? 1 : 0).Aggregate((a, b) => a + ' ' + b);
                }

                IEnumerable<ItemDTO> itemlist = DAOFactory.ItemDAO.FindByName(name).OrderBy(s => s.VNum)
                    .Skip(page * 200).Take(200).ToList();
                if (itemlist.Any())
                {
                    foreach (ItemDTO item in itemlist)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            $"[SearchItem:{page}]Item: {(string.IsNullOrEmpty(item.Name[Session.Account.Language]) ? "none" : item.Name[Session.Account.Language])} VNum: {item.VNum}",
                            12));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_NOT_FOUND"), 11));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SearchItemPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $SearchMonster Command
        /// </summary>
        /// <param name="packet"></param>
        public void SearchMonster(SearchMonsterPacket packet)
        {
            if (packet != null)
            {
                string contents = packet.Contents;
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[SearchMonster]Contents: {(string.IsNullOrEmpty(contents) ? "none" : contents)}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = "";
                byte page = 0;
                if (!string.IsNullOrEmpty(contents))
                {
                    string[] packetsplit = contents.Split(' ');
                    bool withPage = byte.TryParse(packetsplit[0], out page);
                    name = packetsplit.Length == 1 && withPage
                        ? ""
                        : packetsplit.Skip(withPage ? 1 : 0).Aggregate((a, b) => a + ' ' + b);
                }

                IEnumerable<NpcMonsterDTO> monsterlist = DAOFactory.NpcMonsterDAO.FindByName(name)
                    .OrderBy(s => s.NpcMonsterVNum).Skip(page * 200).Take(200).ToList();
                if (monsterlist.Any())
                {
                    foreach (NpcMonsterDTO npcMonster in monsterlist)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            $"[SearchMonster:{page}]Monster: {(string.IsNullOrEmpty(npcMonster.Name) ? "none" : npcMonster.Name)} VNum: {npcMonster.NpcMonsterVNum}",
                            12));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MONSTER_NOT_FOUND"), 11));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SearchMonsterPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $SetPerfection Command
        /// </summary>
        /// <param name="packet"></param>
        public void SetPerfection(SetPerfectionPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[SetPerfection]Slot: {packet.Slot} Type: {packet.Type} Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Slot >= 0)
                {
                    ItemInstance specialistInstance =
                        Session.Character.Inventory.LoadBySlotAndType(packet.Slot, 0);

                    if (specialistInstance != null)
                    {
                        switch (packet.Type)
                        {
                            case 0:
                                specialistInstance.SpStoneUpgrade = packet.Value;
                                break;

                            case 1:
                                specialistInstance.SpDamage = packet.Value;
                                break;

                            case 2:
                                specialistInstance.SpDefence = packet.Value;
                                break;

                            case 3:
                                specialistInstance.SpElement = packet.Value;
                                break;

                            case 4:
                                specialistInstance.SpHP = packet.Value;
                                break;

                            case 5:
                                specialistInstance.SpFire = packet.Value;
                                break;

                            case 6:
                                specialistInstance.SpWater = packet.Value;
                                break;

                            case 7:
                                specialistInstance.SpLight = packet.Value;
                                break;

                            case 8:
                                specialistInstance.SpDark = packet.Value;
                                break;

                            default:
                                Session.SendPacket(Session.Character.GenerateSay(UpgradeCommandPacket.ReturnHelp(),
                                    10));
                                break;
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(UpgradeCommandPacket.ReturnHelp(), 10));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UpgradeCommandPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Shout Command
        /// </summary>
        /// <param name="packet"></param>
        public void Shout(ShoutPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Shout]Message: {packet.Message}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = null,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = packet.Message,
                    Type = MessageType.Shout
                });
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ShoutPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $ShoutHere Command
        /// </summary>
        /// <param name="packet"></param>
        public void ShoutHere(ShoutHerePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[ShoutHere]Message: {packet.Message}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                ServerManager.Shout(packet.Message);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ShoutHerePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Shutdown Command
        /// </summary>
        /// <param name="packet"></param>
        public void Shutdown(ShutdownPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Shutdown]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            if (ServerManager.Instance.TaskShutdown != null)
            {
                ServerManager.Instance.ShutdownStop = true;
                ServerManager.Instance.TaskShutdown = null;
            }
            else
            {
                ServerManager.Instance.TaskShutdown = ServerManager.Instance.ShutdownTaskAsync();
                ServerManager.Instance.TaskShutdown.Start();
            }
        }

        /// <summary>
        /// $ShutdownAll Command
        /// </summary>
        /// <param name="packet"></param>
        public void ShutdownAll(ShutdownAllPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[ShutdownAll]");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (!string.IsNullOrEmpty(packet.WorldGroup))
                {
                    CommunicationServiceClient.Instance.Shutdown(packet.WorldGroup);
                }
                else
                {
                    CommunicationServiceClient.Instance.Shutdown(ServerManager.Instance.ServerGroup);
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ShutdownAllPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Sort Command
        /// </summary>
        /// <param name="packet"></param>
        public void Sort(SortPacket packet)
        {
            if (packet?.InventoryType.HasValue == true)
            {
                Logger.LogUserEvent("USERCOMMAND", Session.GenerateIdentity(),
                    $"[Sort]InventoryType: {packet.InventoryType}");

                if (packet.InventoryType == InventoryType.Equipment
                    || packet.InventoryType == InventoryType.Etc || packet.InventoryType == InventoryType.Main)
                {
                    Session.Character.Inventory.Reorder(Session, packet.InventoryType.Value);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SortPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Speed Command
        /// </summary>
        /// <param name="packet"></param>
        public void Speed(SpeedPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Speed]Value: {packet.Value}");
                if (packet.Value < 60)
                {
                    Session.Character.Speed = packet.Value;
                    Session.Character.IsCustomSpeed = true;
                    Session.SendPacket(Session.Character.GenerateCond());
                }
                if (packet.Value == 0)
                {
                    Session.Character.IsCustomSpeed = false;
                    Session.Character.LoadSpeed();
                    Session.SendPacket(Session.Character.GenerateCond());
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SpeedPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $SPRefill Command
        /// </summary>
        /// <param name="packet"></param>
        public void SpRefill(SPRefillPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[SPRefill]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            Session.Character.SpPoint = 10000;
            Session.Character.SpAdditionPoint = 1000000;
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_REFILL"), 0));
            Session.SendPacket(Session.Character.GenerateSpPoint());
        }

        /// <summary>
        /// $Event Command
        /// </summary>
        /// <param name="packet"></param>
        public void StartEvent(EventPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Event]EventType: {packet.EventType.ToString()}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.LvlBracket >= 0)
                {
                    EventHelper.GenerateEvent(packet.EventType, packet.LvlBracket);
                }
                else
                {
                    EventHelper.GenerateEvent(packet.EventType);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(EventPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $GlobalEvent Command
        /// </summary>
        /// <param name="packet"></param>
        public void StartGlobalEvent(GlobalEventPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[GlobalEvent]EventType: {packet.EventType.ToString()}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                CommunicationServiceClient.Instance.RunGlobalEvent(packet.EventType);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(EventPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Stat Command
        /// </summary>
        /// <param name="packet"></param>
        public void Stat(StatCommandPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Stat]");

            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("XP_RATE_NOW")}: {ServerManager.Instance.Configuration.RateXP} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("DROP_RATE_NOW")}: {ServerManager.Instance.Configuration.RateDrop} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("GOLD_RATE_NOW")}: {ServerManager.Instance.Configuration.RateGold} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("GOLD_DROPRATE_NOW")}: {ServerManager.Instance.Configuration.RateGoldDrop} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("HERO_XPRATE_NOW")}: {ServerManager.Instance.Configuration.RateHeroicXP} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("FAIRYXP_RATE_NOW")}: {ServerManager.Instance.Configuration.RateFairyXP} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("REPUTATION_RATE_NOW")}: {ServerManager.Instance.Configuration.RateReputation} ",
                13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"{Language.Instance.GetMessageFromKey("SERVER_WORKING_TIME")}: {(Process.GetCurrentProcess().StartTime - DateTime.Now).ToString(@"d\ hh\:mm\:ss")} ",
                13));

            foreach (string message in CommunicationServiceClient.Instance.RetrieveServerStatistics())
            {
                Session.SendPacket(Session.Character.GenerateSay(message, 13));
            }
        }

        /// <summary>
        /// $Sudo Command
        /// </summary>
        /// <param name="packet"></param>
        public void SudoCommand(SudoPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Sudo]CharacterName: {packet.CharacterName} CommandContents:{packet.CommandContents}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.CharacterName == "*")
                {
                    foreach (ClientSession sess in Session.CurrentMapInstance.Sessions.ToList().Where(s => s.Character?.Authority <= Session.Character.Authority))
                    {
                        sess.ReceivePacket(packet.CommandContents, true);
                    }
                }
                else
                {
                    ClientSession session = ServerManager.Instance.GetSessionByCharacterName(packet.CharacterName);

                    if (session != null && !string.IsNullOrWhiteSpace(packet.CommandContents))
                    {
                        if (session.Character?.Authority <= Session.Character.Authority)
                        {
                            session.ReceivePacket(packet.CommandContents, true);
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANT_DO_THAT"), 0));
                        }
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SudoPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Mob Command
        /// </summary>
        /// <param name="packet"></param>
        public void Mob(MobPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Mob]NpcMonsterVNum: {packet.NpcMonsterVNum} Amount: {packet.Amount} IsMoving: {packet.IsMoving}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (Session.IsOnMap && Session.HasCurrentMapInstance)
                {
                    NpcMonster npcmonster = ServerManager.GetNpcMonster(packet.NpcMonsterVNum);
                    if (npcmonster == null)
                    {
                        return;
                    }

                    Random random = new Random();
                    for (int i = 0; i < packet.Amount; i++)
                    {
                        List<MapCell> possibilities = new List<MapCell>();
                        for (short x = -4; x < 5; x++)
                        {
                            for (short y = -4; y < 5; y++)
                            {
                                possibilities.Add(new MapCell { X = x, Y = y });
                            }
                        }

                        foreach (MapCell possibilitie in possibilities.OrderBy(s => random.Next()))
                        {
                            short mapx = (short)(Session.Character.PositionX + possibilitie.X);
                            short mapy = (short)(Session.Character.PositionY + possibilitie.Y);
                            if (!Session.CurrentMapInstance?.Map.IsBlockedZone(mapx, mapy) ?? false)
                            {
                                break;
                            }
                        }

                        if (Session.CurrentMapInstance != null)
                        {
                            MapMonster monster = new MapMonster
                            {
                                MonsterVNum = packet.NpcMonsterVNum,
                                MapY = Session.Character.PositionY,
                                MapX = Session.Character.PositionX,
                                MapId = Session.Character.MapInstance.Map.MapId,
                                Position = Session.Character.Direction,
                                IsMoving = packet.IsMoving,
                                MapMonsterId = Session.CurrentMapInstance.GetNextMonsterId(),
                                ShouldRespawn = false
                            };
                            monster.Initialize(Session.CurrentMapInstance);
                            Session.CurrentMapInstance.AddMonster(monster);
                            Session.CurrentMapInstance.Broadcast(monster.GenerateIn());
                        }
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MobPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $MobRain Command
        /// </summary>
        /// <param name="mobRain"></param>
        public void MobRain(MobRainPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[MobRain]NpcMonsterVNum: {packet.NpcMonsterVNum} Amount: {packet.Amount} IsMoving: {packet.IsMoving}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (Session.IsOnMap && Session.HasCurrentMapInstance)
                {
                    NpcMonster npcmonster = ServerManager.GetNpcMonster(packet.NpcMonsterVNum);
                    if (npcmonster == null)
                    {
                        return;
                    }

                    List<MonsterToSummon> SummonParameters = new List<MonsterToSummon>();
                    SummonParameters.AddRange(Session.Character.MapInstance.Map.GenerateMonsters(packet.NpcMonsterVNum, packet.Amount, packet.IsMoving, new List<EventContainer>()));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(1), new EventContainer(Session.CurrentMapInstance, EventActionType.SPAWNMONSTERS, SummonParameters));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MobRainPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $SNPC Command
        /// </summary>
        /// <param name="packet"></param>
        public void Npc(NPCPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[NPC]NpcMonsterVNum: {packet.NpcMonsterVNum} Amount: {packet.Amount} IsMoving: {packet.IsMoving}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (Session.IsOnMap && Session.HasCurrentMapInstance)
                {
                    NpcMonster npcmonster = ServerManager.GetNpcMonster(packet.NpcMonsterVNum);
                    if (npcmonster == null)
                    {
                        return;
                    }

                    Random random = new Random();
                    for (int i = 0; i < packet.Amount; i++)
                    {
                        List<MapCell> possibilities = new List<MapCell>();
                        for (short x = -4; x < 5; x++)
                        {
                            for (short y = -4; y < 5; y++)
                            {
                                possibilities.Add(new MapCell { X = x, Y = y });
                            }
                        }

                        foreach (MapCell possibilitie in possibilities.OrderBy(s => random.Next()))
                        {
                            short mapx = (short)(Session.Character.PositionX + possibilitie.X);
                            short mapy = (short)(Session.Character.PositionY + possibilitie.Y);
                            if (!Session.CurrentMapInstance?.Map.IsBlockedZone(mapx, mapy) ?? false)
                            {
                                break;
                            }
                        }

                        if (Session.CurrentMapInstance != null)
                        {
                            MapNpc npc = new MapNpc
                            {
                                NpcVNum = packet.NpcMonsterVNum,
                                MapY = Session.Character.PositionY,
                                MapX = Session.Character.PositionX,
                                MapId = Session.Character.MapInstance.Map.MapId,
                                Position = Session.Character.Direction,
                                IsMoving = packet.IsMoving,
                                ShouldRespawn = false,
                                MapNpcId = Session.CurrentMapInstance.GetNextNpcId()
                            };
                            npc.Initialize(Session.CurrentMapInstance);
                            Session.CurrentMapInstance.AddNPC(npc);
                            Session.CurrentMapInstance.Broadcast(npc.GenerateIn());
                        }
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(NPCPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Teleport Command
        /// </summary>
        /// <param name="packet"></param>
        public void Teleport(TeleportPacket packet)
        {
            if (packet != null)
            {
                if (Session.Character.HasShopOpened || Session.Character.InExchangeOrTrade)
                {
                    Session.Character.DisposeShopAndExchange();
                }

                if (Session.Character.IsChangingMapInstance)
                {
                    return;
                }

                ClientSession session = ServerManager.Instance.GetSessionByCharacterName(packet.Data);
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (session != null)
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[Teleport]CharacterName: {packet.Data}");

                    short mapX = session.Character.PositionX;
                    short mapY = session.Character.PositionY;
                    if (session.Character.Miniland == session.Character.MapInstance)
                    {
                        ServerManager.Instance.JoinMiniland(Session, session);
                    }
                    else
                    {
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            session.Character.MapInstanceId, mapX, mapY);
                    }
                }
                else if (short.TryParse(packet.Data, out short mapId))
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[Teleport]MapId: {packet.Data} MapX: {packet.X} MapY: {packet.Y}");

                    if (ServerManager.GetBaseMapInstanceIdByMapId(mapId) != default)
                    {
                        if (packet.X == 0 && packet.Y == 0)
                        {
                            ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, ServerManager.GetBaseMapInstanceIdByMapId(mapId));
                        }
                        else
                        {
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapId, packet.X, packet.Y);
                        }
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAP_NOT_FOUND"), 0));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(TeleportPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Summon Command
        /// </summary>
        /// <param name="packet"></param>
        public void Summon(SummonPacket packet)
        {
            Random random = new Random();
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Summon]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.CharacterName == "*")
                {
                    Parallel.ForEach(
                        ServerManager.Instance.Sessions.Where(s =>
                            s.Character != null && s.Character.CharacterId != Session.Character.CharacterId), session =>
                        {
                            // clear any shop or trade on target character
                            Session.Character.DisposeShopAndExchange();
                            if (!session.Character.IsChangingMapInstance && Session.HasCurrentMapInstance)
                            {
                                List<MapCell> possibilities = new List<MapCell>();
                                for (short x = -6, y = -6; x < 6 && y < 6; x++, y++)
                                {
                                    possibilities.Add(new MapCell { X = x, Y = y });
                                }

                                short mapXPossibility = Session.Character.PositionX;
                                short mapYPossibility = Session.Character.PositionY;
                                foreach (MapCell possibility in possibilities.OrderBy(s => random.Next()))
                                {
                                    mapXPossibility = (short)(Session.Character.PositionX + possibility.X);
                                    mapYPossibility = (short)(Session.Character.PositionY + possibility.Y);
                                    if (!Session.CurrentMapInstance.Map.IsBlockedZone(mapXPossibility, mapYPossibility))
                                    {
                                        break;
                                    }
                                }

                                if (Session.Character.Miniland == Session.Character.MapInstance)
                                {
                                    ServerManager.Instance.JoinMiniland(session, Session);
                                }
                                else
                                {
                                    ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                        Session.Character.MapInstanceId, mapXPossibility, mapYPossibility);
                                }
                            }
                        });
                }
                else
                {
                    ClientSession targetSession =
                        ServerManager.Instance.GetSessionByCharacterName(packet.CharacterName);
                    if (targetSession?.Character.IsChangingMapInstance == false)
                    {
                        Session.Character.DisposeShopAndExchange();
                        ServerManager.Instance.ChangeMapInstance(targetSession.Character.CharacterId,
                            Session.Character.MapInstanceId, (short)(Session.Character.PositionX + 1),
                            (short)(Session.Character.PositionY + 1));
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SummonPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Unban Command
        /// </summary>
        /// <param name="packet"></param>
        public void Unban(UnbanPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Unban]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = packet.CharacterName;
                CharacterDTO chara = DAOFactory.CharacterDAO.LoadByName(name);
                if (chara != null)
                {
                    PenaltyLogDTO log = ServerManager.Instance.PenaltyLogs.Find(s =>
                        s.AccountId == chara.AccountId && s.Penalty == PenaltyType.Banned && s.DateEnd > DateTime.Now);
                    if (log != null)
                    {
                        log.DateEnd = DateTime.Now.AddSeconds(-1);
                        Character.InsertOrUpdatePenalty(log);
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"),
                            10));
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_BANNED"), 10));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UnbanPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Undercover Command
        /// </summary>
        /// <param name="packet"></param>
        public void Undercover(UndercoverPacket packet)
        {
            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Undercover]");
            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
            Session.Character.Undercover = !Session.Character.Undercover;
            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.CurrentMapInstance.MapInstanceId, Session.Character.PositionX, Session.Character.PositionY);
        }

        /// <summary>
        /// $Unmute Command
        /// </summary>
        /// <param name="packet"></param>
        public void Unmute(UnmutePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Unmute]CharacterName: {packet.CharacterName}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string name = packet.CharacterName;
                CharacterDTO chara = DAOFactory.CharacterDAO.LoadByName(name);
                if (chara != null)
                {
                    if (ServerManager.Instance.PenaltyLogs.Any(s =>
                        s.AccountId == chara.AccountId && s.Penalty == (byte)PenaltyType.Muted
                        && s.DateEnd > DateTime.Now))
                    {
                        PenaltyLogDTO log = ServerManager.Instance.PenaltyLogs.Find(s =>
                            s.AccountId == chara.AccountId && s.Penalty == (byte)PenaltyType.Muted
                            && s.DateEnd > DateTime.Now);
                        if (log != null)
                        {
                            log.DateEnd = DateTime.Now.AddSeconds(-1);
                            Character.InsertOrUpdatePenalty(log);
                        }

                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"),
                            10));
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_MUTED"), 10));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UnmutePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $AddMonster Command
        /// </summary>
        /// <param name="packet"></param>
        public void BackMob(BackMobPacket packet)
        {
            if (packet != null)
            {
                if (!Session.HasCurrentMapInstance)
                {
                    return;
                }

                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[BackMob]");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                object lastObject = Session.CurrentMapInstance.RemovedMobNpcList.LastOrDefault();

                if (lastObject is MapMonster mapMonster)
                {
                    MapMonsterDTO backMonst = new MapMonsterDTO
                    {
                        MonsterVNum = mapMonster.MonsterVNum,
                        MapX = mapMonster.MapX,
                        MapY = mapMonster.MapY,
                        MapId = Session.Character.MapInstance.Map.MapId,
                        Position = Session.Character.Direction,
                        IsMoving = mapMonster.IsMoving,
                        MapMonsterId = ServerManager.Instance.GetNextMobId()
                    };
                    if (!DAOFactory.MapMonsterDAO.DoesMonsterExist(backMonst.MapMonsterId))
                    {
                        DAOFactory.MapMonsterDAO.Insert(backMonst);
                        if (DAOFactory.MapMonsterDAO.LoadById(backMonst.MapMonsterId) is MapMonsterDTO monsterDTO)
                        {
                            MapMonster monster = new MapMonster(monsterDTO);
                            monster.Initialize(Session.CurrentMapInstance);
                            Session.CurrentMapInstance.AddMonster(monster);
                            Session.CurrentMapInstance?.Broadcast(monster.GenerateIn());
                            Session.CurrentMapInstance.RemovedMobNpcList.Remove(mapMonster);
                            Session.SendPacket(Session.Character.GenerateSay($"MapMonster VNum: {backMonst.MonsterVNum} recovered sucessfully", 10));
                        }
                    }
                }
                else if (lastObject is MapNpc mapNpc)
                {
                    MapNpcDTO backNpc = new MapNpcDTO
                    {
                        NpcVNum = mapNpc.NpcVNum,
                        MapX = mapNpc.MapX,
                        MapY = mapNpc.MapY,
                        MapId = Session.Character.MapInstance.Map.MapId,
                        Position = Session.Character.Direction,
                        IsMoving = mapNpc.IsMoving,
                        MapNpcId = ServerManager.Instance.GetNextMobId()
                    };
                    if (!DAOFactory.MapNpcDAO.DoesNpcExist(backNpc.MapNpcId))
                    {
                        DAOFactory.MapNpcDAO.Insert(backNpc);
                        if (DAOFactory.MapNpcDAO.LoadById(backNpc.MapNpcId) is MapNpcDTO npcDTO)
                        {
                            MapNpc npc = new MapNpc(npcDTO);
                            npc.Initialize(Session.CurrentMapInstance);
                            Session.CurrentMapInstance.AddNPC(npc);
                            Session.CurrentMapInstance?.Broadcast(npc.GenerateIn());
                            Session.CurrentMapInstance.RemovedMobNpcList.Remove(mapNpc);
                            Session.SendPacket(Session.Character.GenerateSay($"MapNpc VNum: {backNpc.NpcVNum} recovered sucessfully", 10));
                        }
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BackMobPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Unstuck Command
        /// </summary>
        /// <param name="packet"></param>
        public void Unstuck(UnstuckPacket packet)
        {
            if (Session?.Character != null)
            {

                if (Session.Character.Miniland == Session.Character.MapInstance)
                {
                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else if (!Session.Character.IsSeal 
                      && !Session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.TalentArenaMapInstance)
                      && !Session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.IceBreakerInstance))
                {
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                        Session.Character.MapInstanceId, Session.Character.PositionX, Session.Character.PositionY,
                        true);
                    Session.SendPacket(StaticPacketHelper.Cancel(2));
                }
            }
        }

        /// <summary>
        /// $Upgrade Command
        /// </summary>
        /// <param name="packet"></param>
        public void Upgrade(UpgradeCommandPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Upgrade]Slot: {packet.Slot} Mode: {packet.Mode} Protection: {packet.Protection}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Slot >= 0)
                {
                    ItemInstance wearableInstance =
                        Session.Character.Inventory.LoadBySlotAndType(packet.Slot, 0);
                    wearableInstance?.UpgradeItem(Session, packet.Mode, packet.Protection, true);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UpgradeCommandPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $MapStat Command
        /// </summary>
        /// <param name="packet"></param>
        public void MapStats(MapStatisticsPacket packet)
        {

            // lower the boilerplate
            void SendMapStats(MapDTO map, GameObject.MapInstance mapInstance)
            {
                if (map != null && mapInstance != null)
                {
                    Session.SendPacket(Session.Character.GenerateSay("-------------MapData-------------", 10));
                    Session.SendPacket(Session.Character.GenerateSay(
                        $"MapId: {map.MapId}\n" +
                        $"MapMusic: {map.Music}\n" +
                        $"MapName: {map.Name}\n" +
                        $"MapShopAllowed: {map.ShopAllowed}", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------------------------------", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------MapInstanceData---------", 10));
                    Session.SendPacket(Session.Character.GenerateSay(
                        $"MapInstanceId: {mapInstance.MapInstanceId}\n" +
                        $"MapInstanceType: {mapInstance.MapInstanceType}\n" +
                        $"MapMonsterCount: {mapInstance.Monsters.Count}\n" +
                        $"MapNpcCount: {mapInstance.Npcs.Count}\n" +
                        $"MapPortalsCount: {mapInstance.Portals.Count}\n" +
                        $"MapInstanceUserShopCount: {mapInstance.UserShops.Count}\n" +
                        $"SessionCount: {mapInstance.Sessions.Count()}\n" +
                        $"MapInstanceXpRate: {mapInstance.XpRate}\n" +
                        $"MapInstanceDropRate: {mapInstance.DropRate}\n" +
                        $"MapInstanceMusic: {mapInstance.InstanceMusic}\n" +
                        $"ShopsAllowed: {mapInstance.ShopAllowed}\n" +
                        $"DropAllowed: {mapInstance.DropAllowed}\n" +
                        $"IsPVP: {mapInstance.IsPVP}\n" +
                        $"IsSleeping: {mapInstance.IsSleeping}\n" +
                        $"Dance: {mapInstance.IsDancing}", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------------------------------", 10));
                }
            }

            if (packet != null)
            {
                if (packet.MapId.HasValue)
                {
                    MapDTO map = DAOFactory.MapDAO.LoadById(packet.MapId.Value);
                    GameObject.MapInstance mapInstance = ServerManager.GetMapInstanceByMapId(packet.MapId.Value);
                    if (map != null && mapInstance != null)
                    {
                        SendMapStats(map, mapInstance);
                    }
                }
                else if (Session.HasCurrentMapInstance)
                {
                    MapDTO map = DAOFactory.MapDAO.LoadById(Session.CurrentMapInstance.Map.MapId);
                    if (map != null)
                    {
                        SendMapStats(map, Session.CurrentMapInstance);
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MapStatisticsPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Warn Command
        /// </summary>
        /// <param name="packet"></param>
        public void Warn(WarningPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[Warn]CharacterName: {packet.CharacterName} Reason: {packet.Reason}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                string characterName = packet.CharacterName;
                CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(characterName);
                if (character != null)
                {
                    ClientSession session = ServerManager.Instance.GetSessionByCharacterName(characterName);
                    session?.SendPacket(UserInterfaceHelper.GenerateInfo(
                        string.Format(Language.Instance.GetMessageFromKey("WARNING"), packet.Reason)));
                    Character.InsertOrUpdatePenalty(new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = packet.Reason,
                        Penalty = PenaltyType.Warning,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now,
                        AdminName = Session.Character.Name
                    });
                    switch (DAOFactory.PenaltyLogDAO.LoadByAccount(character.AccountId)
                        .Count(p => p.Penalty == PenaltyType.Warning))
                    {
                        case 1:
                            break;

                        case 2:
                            MuteMethod(characterName, "Auto-Warning mute: 2 strikes", 30);
                            break;

                        case 3:
                            MuteMethod(characterName, "Auto-Warning mute: 3 strikes", 60);
                            break;

                        case 4:
                            MuteMethod(characterName, "Auto-Warning mute: 4 strikes", 720);
                            break;

                        case 5:
                            MuteMethod(characterName, "Auto-Warning mute: 5 strikes", 1440);
                            break;

                        case 69:
                            BanMethod(characterName, 7, "LOL SIXTY NINE AMIRITE?");
                            break;

                        default:
                            MuteMethod(characterName, "You've been THUNDERSTRUCK",
                                6969); // imagined number as for I = √(-1), complex z = a + bi
                            break;
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(WarningPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $WigColor Command
        /// </summary>
        /// <param name="packet"></param>
        public void WigColor(WigColorPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[WigColor]Color: {packet.Color}");

                ItemInstance wig =
                    Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);
                if (wig != null)
                {
                    wig.Design = packet.Color;
                    Session.SendPacket(Session.Character.GenerateEq());
                    Session.SendPacket(Session.Character.GenerateEquipment());
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateIn());
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WIG"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(WigColorPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $XpRate Command
        /// </summary>
        /// <param name="packet"></param>
        public void XpRate(XpRatePacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[XpRate]Value: {packet.Value}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (packet.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateXP = packet.Value;

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("XP_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(XpRatePacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Zoom Command
        /// </summary>
        /// <param name="packet"></param>
        public void Zoom(ZoomPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Zoom]Value: {packet.Value}");

                Session.SendPacket(
                    UserInterfaceHelper.GenerateGuri(15, packet.Value, Session.Character.CharacterId));
            }

            Session.Character.GenerateSay(ZoomPacket.ReturnHelp(), 10);
        }

        /// <summary>
        /// $Act4 Command
        /// </summary>
        /// <param name="packet"></param>
        public void Act4(Act4Packet packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Act4]");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (ServerManager.Instance.IsAct4Online())
                {
                    switch (Session.Character.Faction)
                    {
                        case FactionType.None:
#warning change port alveus
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, 51, 41);
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo("You need to be part of a faction to join Act 4"));
                            return;

                        case FactionType.Angel:
                            Session.Character.MapId = 130;
                            Session.Character.MapX = 12;
                            Session.Character.MapY = 40;
                            break;

                        case FactionType.Demon:
                            Session.Character.MapId = 131;
                            Session.Character.MapX = 12;
                            Session.Character.MapY = 40;
                            break;
                    }

                    Session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP, ServerManager.Instance.Configuration.Act4Port, 1);
                }
                else
                {
#warning change port alveus
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, 51, 41);
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("ACT4_OFFLINE")));
                }
            }

            Session.Character.GenerateSay(Act4Packet.ReturnHelp(), 10);
        }

        /// <summary>
        /// $LeaveAct4 Command
        /// </summary>
        /// <param name="packet"></param>
        public void LeaveAct4(LeaveAct4Packet packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[LeaveAct4]");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (Session.Character.Channel.ChannelId == 51)
                {
                    string connection = CommunicationServiceClient.Instance.RetrieveOriginWorld(Session.Character.AccountId);
                    if (string.IsNullOrWhiteSpace(connection))
                    {
                        return;
                    }
#warning change port alveus
                    Session.Character.MapId = 145;
                    Session.Character.MapX = 51;
                    Session.Character.MapY = 41;
                    int port = Convert.ToInt32(connection.Split(':')[1]);
                    Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                }
            }

            Session.Character.GenerateSay(LeaveAct4Packet.ReturnHelp(), 10);
        }

        /// <summary>
        /// $Miniland Command
        /// </summary>
        /// <param name="packet"></param>
        public void Miniland(MinilandPacket packet)
        {
            if (packet != null)
            {
                if (string.IsNullOrEmpty(packet.CharacterName))
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Miniland]");

                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else
                {
                    ClientSession session = ServerManager.Instance.GetSessionByCharacterName(packet.CharacterName);
                    if (session != null)
                    {
                        Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Miniland]CharacterName: {packet.CharacterName}");

                        ServerManager.Instance.JoinMiniland(Session, session);

                    }
                }
            }

            Session.Character.GenerateSay(MinilandPacket.ReturnHelp(), 10);
        }

        /// <summary>
        /// $Gogo Command
        /// </summary>
        /// <param name="packet"></param>
        public void Gogo(GogoPacket packet)
        {
            if (packet != null)
            {
                if (Session.Character.HasShopOpened || Session.Character.InExchangeOrTrade)
                {
                    Session.Character.DisposeShopAndExchange();
                }

                if (Session.Character.IsChangingMapInstance)
                {
                    return;
                }
                
                if (Session.CurrentMapInstance != null)
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[Gogo]MapId: {Session.CurrentMapInstance.Map.MapId} MapX: {packet.X} MapY: {packet.Y}");
                    LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                    if (packet.X == 0 && packet.Y == 0)
                    {
                        ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, Session.CurrentMapInstance.MapInstanceId);
                    }
                    else
                    {
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.CurrentMapInstance.MapInstanceId, packet.X, packet.Y);
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GogoPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $MapReset Command
        /// </summary>
        /// <param name="packet"></param>
        public void MapReset(MapResetPacket packet)
        {
            if (packet != null)
            {
                if (Session.Character.IsChangingMapInstance)
                {
                    return;
                }
                if (Session.CurrentMapInstance != null)
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[MapReset]MapId: {Session.CurrentMapInstance.Map.MapId}");
                    LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                    GameObject.MapInstance newMapInstance = ServerManager.ResetMapInstance(Session.CurrentMapInstance);

                    Parallel.ForEach(Session.CurrentMapInstance.Sessions, sess =>
                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, newMapInstance.MapInstanceId, sess.Character.PositionX, sess.Character.PositionY));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MapResetPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Drop Command
        /// </summary>
        /// <param name="packet"></param>
        public void Drop(DropPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                       $"[Drop]ItemVNum: {packet.VNum} Amount: {packet.Amount} Count: {packet.Count} Time: {packet.Time}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                short vnum = packet.VNum;
                short amount = packet.Amount;
                if (amount < 1) { amount = 1; }
                else if (amount > 999) { amount = 999; }
                int count = packet.Count;
                if (count < 1) { count = 1; }
                int time = packet.Time;
                
                GameObject.MapInstance instance = Session.CurrentMapInstance;

                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(observer =>
                {
                    {
                        for (int i = 0; i < count; i++)
                        {
                            MonsterMapItem droppedItem = new MonsterMapItem(Session.Character.PositionX, Session.Character.PositionY, vnum, amount);
                            instance.DroppedList[droppedItem.TransportId] = droppedItem;
                            instance.Broadcast(
                                $"drop {droppedItem.ItemVNum} {droppedItem.TransportId} {droppedItem.PositionX} {droppedItem.PositionY} {(droppedItem.GoldAmount > 1 ? droppedItem.GoldAmount : droppedItem.Amount)} 0 -1");

                            System.Threading.Thread.Sleep(time * 1000 / count);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// $ChangeShopName Packet
        /// </summary>
        /// <param name="packet"></param>
        public void ChangeShopName(ChangeShopNamePacket packet)
        {
            if (Session.HasCurrentMapInstance)
            {
                if (!string.IsNullOrEmpty(packet.Name))
                {
                    if (Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId) is MapNpc npc)
                    {
                        if (npc.Shop is Shop shop)
                        {
                            Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                                $"[ChangeShopName]ShopId: {shop.ShopId} Name: {packet.Name}");
                            LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                            if (DAOFactory.ShopDAO.LoadById(shop.ShopId) is ShopDTO shopDTO)
                            {
                                shop.Name = packet.Name;
                                shopDTO.Name = packet.Name;
                                DAOFactory.ShopDAO.Update(ref shopDTO);

                                Session.CurrentMapInstance.Broadcast($"shop 2 {npc.MapNpcId} {npc.Shop.ShopId} {npc.Shop.MenuType} {npc.Shop.ShopType} {npc.Shop.Name}");
                            }
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NPCMONSTER_NOT_FOUND"), 11));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(ChangeShopNamePacket.ReturnHelp(), 10));
                }
            }
        }

        /// <summary>
        /// $CustomNpcMonsterName Packet
        /// </summary>
        /// <param name="packet"></param>
        public void CustomNpcMonsterName(ChangeNpcMonsterNamePacket packet)
        {
            if (Session.HasCurrentMapInstance)
            {
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId) is MapNpc npc)
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[CustomNpcName]MapNpcId: {npc.MapNpcId} Name: {packet.Name}");

                    if (DAOFactory.MapNpcDAO.LoadById(npc.MapNpcId) is MapNpcDTO npcDTO)
                    {
                        npc.Name = packet.Name;
                        npcDTO.Name = packet.Name;
                        DAOFactory.MapNpcDAO.Update(ref npcDTO);

                        Session.CurrentMapInstance.Broadcast(npc.GenerateIn());
                    }
                }
                else if (Session.CurrentMapInstance.GetMonsterById(Session.Character.LastNpcMonsterId) is MapMonster monster)
                {
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                        $"[CustomNpcName]MapMonsterId: {monster.MapMonsterId} Name: {packet.Name}");

                    if (DAOFactory.MapMonsterDAO.LoadById(monster.MapMonsterId) is MapMonsterDTO monsterDTO)
                    {
                        monster.Name = packet.Name;
                        monsterDTO.Name = packet.Name;
                        DAOFactory.MapMonsterDAO.Update(ref monsterDTO);

                        Session.CurrentMapInstance.Broadcast(monster.GenerateIn());
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NPCMONSTER_NOT_FOUND"), 11));
                }
            }
        }

        /// <summary>
        /// $AddQuest
        /// </summary>
        /// <param name="packet"></param>
        public void AddQuest(AddQuestPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                                       $"[AddQuest]QuestId: {packet.QuestId}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                if (ServerManager.Instance.Quests.Any(q => q.QuestId == packet.QuestId))
                {
                    Session.Character.AddQuest(packet.QuestId, false);
                    return;
                }

                Session.SendPacket(Session.Character.GenerateSay("This Quest doesn't exist", 11));
            }
        }


        /// <summary>
        /// $ClassPack
        /// </summary>
        /// <param name="packet"></param>
        public void ClassPack(ClassPackPacket packet)
        {
            if (packet != null)
            {
                if (packet.Class < 1 || packet.Class > 3)
                {
                    Session.SendPacket(Session.Character.GenerateSay("Invalid class", 11));
                    Session.SendPacket(Session.Character.GenerateSay(ClassPackPacket.ReturnHelp(), 10));
                    return;
                }

                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                                       $"[ClassPack]Class: {packet.Class}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                switch (packet.Class)
                {
                    case 1:
                        Session.Character.Inventory.AddNewToInventory(4075, 1);
                        Session.Character.Inventory.AddNewToInventory(4076, 1);
                        Session.Character.Inventory.AddNewToInventory(4129, 1);
                        Session.Character.Inventory.AddNewToInventory(4130, 1);
                        Session.Character.Inventory.AddNewToInventory(4131, 1);
                        Session.Character.Inventory.AddNewToInventory(4132, 1);
                        Session.Character.Inventory.AddNewToInventory(1685, 999);
                        Session.Character.Inventory.AddNewToInventory(1686, 999);
                        Session.Character.Inventory.AddNewToInventory(5087, 999);
                        Session.Character.Inventory.AddNewToInventory(5203, 999);
                        Session.Character.Inventory.AddNewToInventory(5372, 999);
                        Session.Character.Inventory.AddNewToInventory(5431, 999);
                        Session.Character.Inventory.AddNewToInventory(5432, 999);
                        Session.Character.Inventory.AddNewToInventory(5498, 999);
                        Session.Character.Inventory.AddNewToInventory(5499, 999);
                        Session.Character.Inventory.AddNewToInventory(5553, 999);
                        Session.Character.Inventory.AddNewToInventory(5560, 999);
                        Session.Character.Inventory.AddNewToInventory(5591, 999);
                        Session.Character.Inventory.AddNewToInventory(5837, 999);
                        Session.Character.Inventory.AddNewToInventory(4875, 1, Upgrade: 14);
                        Session.Character.Inventory.AddNewToInventory(4873, 1, Upgrade: 14);
                        Session.Character.Inventory.AddNewToInventory(1012, 999);
                        Session.Character.Inventory.AddNewToInventory(1012, 999);
                        Session.Character.Inventory.AddNewToInventory(1244, 999);
                        Session.Character.Inventory.AddNewToInventory(1244, 999);
                        Session.Character.Inventory.AddNewToInventory(2072, 999);
                        Session.Character.Inventory.AddNewToInventory(2071, 999);
                        Session.Character.Inventory.AddNewToInventory(2070, 999);
                        Session.Character.Inventory.AddNewToInventory(2160, 999);
                        Session.Character.Inventory.AddNewToInventory(4138, 1);
                        Session.Character.Inventory.AddNewToInventory(4146, 1);
                        Session.Character.Inventory.AddNewToInventory(4142, 1);
                        Session.Character.Inventory.AddNewToInventory(4150, 1);
                        Session.Character.Inventory.AddNewToInventory(4353, 1);
                        Session.Character.Inventory.AddNewToInventory(4124, 1);
                        Session.Character.Inventory.AddNewToInventory(4172, 1);
                        Session.Character.Inventory.AddNewToInventory(4183, 1);
                        Session.Character.Inventory.AddNewToInventory(4187, 1);
                        Session.Character.Inventory.AddNewToInventory(4283, 1);
                        Session.Character.Inventory.AddNewToInventory(4285, 1);
                        Session.Character.Inventory.AddNewToInventory(4177, 1);
                        Session.Character.Inventory.AddNewToInventory(4179, 1);
                        Session.Character.Inventory.AddNewToInventory(4244, 1);
                        Session.Character.Inventory.AddNewToInventory(4252, 1);
                        Session.Character.Inventory.AddNewToInventory(4256, 1);
                        Session.Character.Inventory.AddNewToInventory(4248, 1);
                        Session.Character.Inventory.AddNewToInventory(3116, 1);
                        Session.Character.Inventory.AddNewToInventory(1277, 999);
                        Session.Character.Inventory.AddNewToInventory(1274, 999);
                        Session.Character.Inventory.AddNewToInventory(1280, 999);
                        Session.Character.Inventory.AddNewToInventory(2419, 999);
                        Session.Character.Inventory.AddNewToInventory(1914, 1);
                        Session.Character.Inventory.AddNewToInventory(1296, 999);
                        Session.Character.Inventory.AddNewToInventory(5916, 999);
                        Session.Character.Inventory.AddNewToInventory(3001, 1);
                        Session.Character.Inventory.AddNewToInventory(3003, 1);
                        Session.Character.Inventory.AddNewToInventory(4490, 1);
                        Session.Character.Inventory.AddNewToInventory(4699, 1);
                        Session.Character.Inventory.AddNewToInventory(4099, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(900, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(907, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(908, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4883, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4889, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4895, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4371, 1);
                        Session.Character.Inventory.AddNewToInventory(4353, 1);
                        Session.Character.Inventory.AddNewToInventory(4277, 1);
                        Session.Character.Inventory.AddNewToInventory(4309, 1);
                        Session.Character.Inventory.AddNewToInventory(4271, 1);
                        Session.Character.Inventory.AddNewToInventory(901, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(902, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(909, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(910, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4500, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4497, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4493, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4489, 1, Upgrade: 15);
                        break;
                    case 2:
                        Session.Character.Inventory.AddNewToInventory(4075, 1);
                        Session.Character.Inventory.AddNewToInventory(4076, 1);
                        Session.Character.Inventory.AddNewToInventory(4129, 1);
                        Session.Character.Inventory.AddNewToInventory(4130, 1);
                        Session.Character.Inventory.AddNewToInventory(4131, 1);
                        Session.Character.Inventory.AddNewToInventory(4132, 1);
                        Session.Character.Inventory.AddNewToInventory(1685, 999);
                        Session.Character.Inventory.AddNewToInventory(1686, 999);
                        Session.Character.Inventory.AddNewToInventory(5087, 999);
                        Session.Character.Inventory.AddNewToInventory(5203, 999);
                        Session.Character.Inventory.AddNewToInventory(5372, 999);
                        Session.Character.Inventory.AddNewToInventory(5431, 999);
                        Session.Character.Inventory.AddNewToInventory(5432, 999);
                        Session.Character.Inventory.AddNewToInventory(5498, 999);
                        Session.Character.Inventory.AddNewToInventory(5499, 999);
                        Session.Character.Inventory.AddNewToInventory(5553, 999);
                        Session.Character.Inventory.AddNewToInventory(5560, 999);
                        Session.Character.Inventory.AddNewToInventory(5591, 999);
                        Session.Character.Inventory.AddNewToInventory(5837, 999);
                        Session.Character.Inventory.AddNewToInventory(4875, 1, Upgrade: 14);
                        Session.Character.Inventory.AddNewToInventory(4873, 1, Upgrade: 14);
                        Session.Character.Inventory.AddNewToInventory(1012, 999);
                        Session.Character.Inventory.AddNewToInventory(1012, 999);
                        Session.Character.Inventory.AddNewToInventory(1244, 999);
                        Session.Character.Inventory.AddNewToInventory(1244, 999);
                        Session.Character.Inventory.AddNewToInventory(2072, 999);
                        Session.Character.Inventory.AddNewToInventory(2071, 999);
                        Session.Character.Inventory.AddNewToInventory(2070, 999);
                        Session.Character.Inventory.AddNewToInventory(2160, 999);
                        Session.Character.Inventory.AddNewToInventory(4138, 1);
                        Session.Character.Inventory.AddNewToInventory(4146, 1);
                        Session.Character.Inventory.AddNewToInventory(4142, 1);
                        Session.Character.Inventory.AddNewToInventory(4150, 1);
                        Session.Character.Inventory.AddNewToInventory(4353, 1);
                        Session.Character.Inventory.AddNewToInventory(4124, 1);
                        Session.Character.Inventory.AddNewToInventory(4172, 1);
                        Session.Character.Inventory.AddNewToInventory(4183, 1);
                        Session.Character.Inventory.AddNewToInventory(4187, 1);
                        Session.Character.Inventory.AddNewToInventory(4283, 1);
                        Session.Character.Inventory.AddNewToInventory(4285, 1);
                        Session.Character.Inventory.AddNewToInventory(4177, 1);
                        Session.Character.Inventory.AddNewToInventory(4179, 1);
                        Session.Character.Inventory.AddNewToInventory(4244, 1);
                        Session.Character.Inventory.AddNewToInventory(4252, 1);
                        Session.Character.Inventory.AddNewToInventory(4256, 1);
                        Session.Character.Inventory.AddNewToInventory(4248, 1);
                        Session.Character.Inventory.AddNewToInventory(3116, 1);
                        Session.Character.Inventory.AddNewToInventory(1277, 999);
                        Session.Character.Inventory.AddNewToInventory(1274, 999);
                        Session.Character.Inventory.AddNewToInventory(1280, 999);
                        Session.Character.Inventory.AddNewToInventory(2419, 999);
                        Session.Character.Inventory.AddNewToInventory(1914, 1);
                        Session.Character.Inventory.AddNewToInventory(1296, 999);
                        Session.Character.Inventory.AddNewToInventory(5916, 999);
                        Session.Character.Inventory.AddNewToInventory(3001, 1);
                        Session.Character.Inventory.AddNewToInventory(3003, 1);
                        Session.Character.Inventory.AddNewToInventory(4490, 1);
                        Session.Character.Inventory.AddNewToInventory(4699, 1);
                        Session.Character.Inventory.AddNewToInventory(4099, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(900, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(907, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(908, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4885, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4890, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4897, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4372, 1);
                        Session.Character.Inventory.AddNewToInventory(4310, 1);
                        Session.Character.Inventory.AddNewToInventory(4354, 1);
                        Session.Character.Inventory.AddNewToInventory(4279, 1);
                        Session.Character.Inventory.AddNewToInventory(4273, 1);
                        Session.Character.Inventory.AddNewToInventory(903, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(904, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(911, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(912, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4501, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4498, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4488, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4492, 1, Upgrade: 15);
                        break;
                    case 3:
                        Session.Character.Inventory.AddNewToInventory(4075, 1);
                        Session.Character.Inventory.AddNewToInventory(4076, 1);
                        Session.Character.Inventory.AddNewToInventory(4129, 1);
                        Session.Character.Inventory.AddNewToInventory(4130, 1);
                        Session.Character.Inventory.AddNewToInventory(4131, 1);
                        Session.Character.Inventory.AddNewToInventory(4132, 1);
                        Session.Character.Inventory.AddNewToInventory(1685, 999);
                        Session.Character.Inventory.AddNewToInventory(1686, 999);
                        Session.Character.Inventory.AddNewToInventory(5087, 999);
                        Session.Character.Inventory.AddNewToInventory(5203, 999);
                        Session.Character.Inventory.AddNewToInventory(5372, 999);
                        Session.Character.Inventory.AddNewToInventory(5431, 999);
                        Session.Character.Inventory.AddNewToInventory(5432, 999);
                        Session.Character.Inventory.AddNewToInventory(5498, 999);
                        Session.Character.Inventory.AddNewToInventory(5499, 999);
                        Session.Character.Inventory.AddNewToInventory(5553, 999);
                        Session.Character.Inventory.AddNewToInventory(5560, 999);
                        Session.Character.Inventory.AddNewToInventory(5591, 999);
                        Session.Character.Inventory.AddNewToInventory(5837, 999);
                        Session.Character.Inventory.AddNewToInventory(4875, 1, Upgrade: 14);
                        Session.Character.Inventory.AddNewToInventory(4873, 1, Upgrade: 14);
                        Session.Character.Inventory.AddNewToInventory(1012, 999);
                        Session.Character.Inventory.AddNewToInventory(1012, 999);
                        Session.Character.Inventory.AddNewToInventory(1244, 999);
                        Session.Character.Inventory.AddNewToInventory(1244, 999);
                        Session.Character.Inventory.AddNewToInventory(2072, 999);
                        Session.Character.Inventory.AddNewToInventory(2071, 999);
                        Session.Character.Inventory.AddNewToInventory(2070, 999);
                        Session.Character.Inventory.AddNewToInventory(2160, 999);
                        Session.Character.Inventory.AddNewToInventory(4138, 1);
                        Session.Character.Inventory.AddNewToInventory(4146, 1);
                        Session.Character.Inventory.AddNewToInventory(4142, 1);
                        Session.Character.Inventory.AddNewToInventory(4150, 1);
                        Session.Character.Inventory.AddNewToInventory(4353, 1);
                        Session.Character.Inventory.AddNewToInventory(4124, 1);
                        Session.Character.Inventory.AddNewToInventory(4172, 1);
                        Session.Character.Inventory.AddNewToInventory(4183, 1);
                        Session.Character.Inventory.AddNewToInventory(4187, 1);
                        Session.Character.Inventory.AddNewToInventory(4283, 1);
                        Session.Character.Inventory.AddNewToInventory(4285, 1);
                        Session.Character.Inventory.AddNewToInventory(4177, 1);
                        Session.Character.Inventory.AddNewToInventory(4179, 1);
                        Session.Character.Inventory.AddNewToInventory(4244, 1);
                        Session.Character.Inventory.AddNewToInventory(4252, 1);
                        Session.Character.Inventory.AddNewToInventory(4256, 1);
                        Session.Character.Inventory.AddNewToInventory(4248, 1);
                        Session.Character.Inventory.AddNewToInventory(3116, 1);
                        Session.Character.Inventory.AddNewToInventory(1277, 999);
                        Session.Character.Inventory.AddNewToInventory(1274, 999);
                        Session.Character.Inventory.AddNewToInventory(1280, 999);
                        Session.Character.Inventory.AddNewToInventory(2419, 999);
                        Session.Character.Inventory.AddNewToInventory(1914, 1);
                        Session.Character.Inventory.AddNewToInventory(1296, 999);
                        Session.Character.Inventory.AddNewToInventory(5916, 999);
                        Session.Character.Inventory.AddNewToInventory(3001, 1);
                        Session.Character.Inventory.AddNewToInventory(3003, 1);
                        Session.Character.Inventory.AddNewToInventory(4490, 1);
                        Session.Character.Inventory.AddNewToInventory(4699, 1);
                        Session.Character.Inventory.AddNewToInventory(4099, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(900, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(907, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(908, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4887, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4892, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4899, 1, null, 7, 10);
                        Session.Character.Inventory.AddNewToInventory(4311, 1);
                        Session.Character.Inventory.AddNewToInventory(4373, 1);
                        Session.Character.Inventory.AddNewToInventory(4281, 1);
                        Session.Character.Inventory.AddNewToInventory(4355, 1);
                        Session.Character.Inventory.AddNewToInventory(4275, 1);
                        Session.Character.Inventory.AddNewToInventory(905, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(906, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(913, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(914, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4502, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4499, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4491, 1, Upgrade: 15);
                        Session.Character.Inventory.AddNewToInventory(4487, 1, Upgrade: 15);
                        break;
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ClassPackPacket.ReturnHelp(), 10));
            }
        }

        /// <summary>
        /// $Home Command
        /// </summary>
        /// <param name="packet"></param>
        public void Home(HomePacket packet)
        {
            if (packet != null)
            {
                if (Session.Character.Channel.ChannelId != 51)
                {
                    LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                    Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[Home]");
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 1, 79, 117);
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("You can't do it if you are in act4."), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HomePacket.ReturnHelp(), 10));
            }
        }
        
        /// <summary>
        /// private addMate method
        /// </summary>
        /// <param name="vnum"></param>
        /// <param name="level"></param>
        /// <param name="mateType"></param>
        private void AddMate(short vnum, byte level, MateType mateType)
        {
            NpcMonster mateNpc = ServerManager.GetNpcMonster(vnum);
            if (Session.CurrentMapInstance == Session.Character.Miniland && mateNpc != null)
            {
                level = level == 0 ? (byte)1 : level;
                Mate mate = new Mate(Session.Character, mateNpc, level, mateType);
                Session.Character.AddPet(mate);
            }
            else
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_IN_MINILAND"), 0));
            }
        }

        /// <summary>
        /// $ReloadSI Command
        /// </summary>
        /// <param name="packet"></param>
        public void ReloadSI(ReloadSIPacket packet)
        {
            if (packet != null)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(), $"[ReloadSI]");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                ServerManager.Instance.LoadScriptedInstances();
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ReloadSIPacket.ReturnHelp(), 10));
            }
        }
        
        /// <summary>
        /// private add portal command
        /// </summary>
        /// <param name="destinationMapId"></param>
        /// <param name="destinationX"></param>
        /// <param name="destinationY"></param>
        /// <param name="type"></param>
        /// <param name="insertToDatabase"></param>
        private void AddPortal(short destinationMapId, short destinationX, short destinationY, short type,
            bool insertToDatabase)
        {
            if (Session.HasCurrentMapInstance)
            {
                Portal portal = new Portal
                {
                    SourceMapId = Session.Character.MapId,
                    SourceX = Session.Character.PositionX,
                    SourceY = Session.Character.PositionY,
                    DestinationMapId = destinationMapId,
                    DestinationX = destinationX,
                    DestinationY = destinationY,
                    DestinationMapInstanceId = insertToDatabase ? Guid.Empty :
                        destinationMapId == 20000 ? Session.Character.Miniland.MapInstanceId : Guid.Empty,
                    Type = type
                };
                if (insertToDatabase)
                {
                    DAOFactory.PortalDAO.Insert(portal);
                }

                Session.CurrentMapInstance.Portals.Add(portal);
                Session.CurrentMapInstance?.Broadcast(portal.GenerateGp());
            }
        }

        /// <summary>
        /// private ban method
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="duration"></param>
        /// <param name="reason"></param>
        private void BanMethod(string characterName, int duration, string reason)
        {
            CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(characterName);
            if (character != null)
            {
                ServerManager.Instance.Kick(characterName);
                PenaltyLogDTO log = new PenaltyLogDTO
                {
                    AccountId = character.AccountId,
                    Reason = reason?.Trim(),
                    Penalty = PenaltyType.Banned,
                    DateStart = DateTime.Now,
                    DateEnd = duration == 0 ? DateTime.Now.AddYears(15) : DateTime.Now.AddDays(duration),
                    AdminName = Session.Character.Name
                };
                Character.InsertOrUpdatePenalty(log);
                ServerManager.Instance.BannedCharacters.Add(character.CharacterId);
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"),
                    10));
            }
        }

        /// <summary>
        /// private mute method
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="reason"></param>
        /// <param name="duration"></param>
        private void MuteMethod(string characterName, string reason, int duration)
        {
            CharacterDTO characterToMute = DAOFactory.CharacterDAO.LoadByName(characterName);
            if (characterToMute != null)
            {
                ClientSession session = ServerManager.Instance.GetSessionByCharacterName(characterName);
                if (session?.Character.IsMuted() == false)
                {
                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
                        string.Format(Language.Instance.GetMessageFromKey("MUTED_PLURAL"), reason, duration)));
                }

                PenaltyLogDTO log = new PenaltyLogDTO
                {
                    AccountId = characterToMute.AccountId,
                    Reason = reason,
                    Penalty = PenaltyType.Muted,
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now.AddMinutes(duration),
                    AdminName = Session.Character.Name
                };
                Character.InsertOrUpdatePenalty(log);
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"),
                    10));
            }
        }

        /// <summary>
        /// Helper method used for sending stats of desired character
        /// </summary>
        /// <param name="characterDto"></param>
        private void SendStats(CharacterDTO characterDto)
        {
            Session.SendPacket(Session.Character.GenerateSay("----- CHARACTER -----", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Name: {characterDto.Name}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Id: {characterDto.CharacterId}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"State: {characterDto.State}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Gender: {characterDto.Gender}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Class: {characterDto.Class}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Level: {characterDto.Level}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"JobLevel: {characterDto.JobLevel}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"HeroLevel: {characterDto.HeroLevel}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Gold: {characterDto.Gold}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Bio: {characterDto.Biography}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"MapId: {characterDto.MapId}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"MapX: {characterDto.MapX}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"MapY: {characterDto.MapY}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Reputation: {characterDto.Reputation}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Dignity: {characterDto.Dignity}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Rage: {characterDto.RagePoint}", 13));
            Session.SendPacket(Session.Character.GenerateSay($"Compliment: {characterDto.Compliment}", 13));
            Session.SendPacket(Session.Character.GenerateSay(
                $"Fraction: {(characterDto.Faction == FactionType.Demon ? Language.Instance.GetMessageFromKey("DEMON") : Language.Instance.GetMessageFromKey("ANGEL"))}",
                13));
            Session.SendPacket(Session.Character.GenerateSay("----- --------- -----", 13));
            AccountDTO account = DAOFactory.AccountDAO.LoadById(characterDto.AccountId);
            if (account != null)
            {
                Session.SendPacket(Session.Character.GenerateSay("----- ACCOUNT -----", 13));
                Session.SendPacket(Session.Character.GenerateSay($"Id: {account.AccountId}", 13));
                Session.SendPacket(Session.Character.GenerateSay($"Name: {account.Name}", 13));
                Session.SendPacket(Session.Character.GenerateSay($"Authority: {account.Authority}", 13));
                Session.SendPacket(Session.Character.GenerateSay($"RegistrationIP: {account.RegistrationIP}", 13));
                Session.SendPacket(Session.Character.GenerateSay($"Email: {account.Email}", 13));
                Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
                IEnumerable<PenaltyLogDTO> penaltyLogs = ServerManager.Instance.PenaltyLogs
                    .Where(s => s.AccountId == account.AccountId).ToList();
                PenaltyLogDTO penalty = penaltyLogs.LastOrDefault(s => s.DateEnd > DateTime.Now);
                Session.SendPacket(Session.Character.GenerateSay("----- PENALTY -----", 13));
                if (penalty != null)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"Type: {penalty.Penalty}", 13));
                    Session.SendPacket(Session.Character.GenerateSay($"AdminName: {penalty.AdminName}", 13));
                    Session.SendPacket(Session.Character.GenerateSay($"Reason: {penalty.Reason}", 13));
                    Session.SendPacket(Session.Character.GenerateSay($"DateStart: {penalty.DateStart}", 13));
                    Session.SendPacket(Session.Character.GenerateSay($"DateEnd: {penalty.DateEnd}", 13));
                }

                Session.SendPacket(
                    Session.Character.GenerateSay($"Bans: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Banned)}",
                        13));
                Session.SendPacket(
                    Session.Character.GenerateSay($"Mutes: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Muted)}",
                        13));
                Session.SendPacket(
                    Session.Character.GenerateSay(
                        $"Warnings: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Warning)}", 13));
                Session.SendPacket(Session.Character.GenerateSay("----- ------- -----", 13));
            }

            Session.SendPacket(Session.Character.GenerateSay("----- SESSION -----", 13));
            foreach (long[] connection in CommunicationServiceClient.Instance.RetrieveOnlineCharacters(characterDto
                .CharacterId))
            {
                if (connection != null)
                {
                    CharacterDTO character = DAOFactory.CharacterDAO.LoadById(connection[0]);
                    if (character != null)
                    {
                        Session.SendPacket(Session.Character.GenerateSay($"Character Name: {character.Name}", 13));
                        Session.SendPacket(Session.Character.GenerateSay($"ChannelId: {connection[1]}", 13));
                        Session.SendPacket(Session.Character.GenerateSay("-----", 13));
                    }
                }
            }

            Session.SendPacket(Session.Character.GenerateSay("----- ------------ -----", 13));
        }

        #endregion
    }
}