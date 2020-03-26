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
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler
{
    public class BasicPacketHandler
    {
        #region Instantiation

        public BasicPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// $bl packet
        /// </summary>
        /// <param name="blPacket"></param>
        /*public void BlBlacklistAdd(BlPacket blPacket)
        {
            if (blPacket.CharacterName != null && ServerManager.Instance.GetSessionByCharacterName(blPacket.CharacterName) is ClientSession receiverSession)
            {
                BlacklistAdd(new BlInsPacket { CharacterId = receiverSession.Character.CharacterId });
            }
        }*/

        /// <summary>
        /// $fl packet
        /// </summary>
        /// <param name="flPacket"></param>
        /*public void FlRelationAdd(FlPacket flPacket)
        {
            if (flPacket.CharacterName != null && ServerManager.Instance.GetSessionByCharacterName(flPacket.CharacterName) is ClientSession receiverSession)
            {
                RelationAdd(new FInsPacket { Type = 1, CharacterId = receiverSession.Character.CharacterId });
            }
        }*/

        /// <summary>
        /// csp packet
        /// </summary>
        /// <param name="cspPacket"></param>
        public void SendBubbleMessage(CspPacket cspPacket)
        {
            if (cspPacket.CharacterId == Session.Character.CharacterId && Session.Character.BubbleMessage != null)
            {
                Session.Character.MapInstance.Broadcast(Session.Character.GenerateBubbleMessagePacket());
            }
        }

        /// <summary>
        /// npinfo packet
        /// </summary>
        /// <param name="npinfoPacket"></param>
        /*public void GetStats(NpinfoPacket npinfoPacket)
        {
            Session.SendPackets(Session.Character.GenerateStatChar());

            if (npinfoPacket.Page != Session.Character.ScPage)
            {
                Session.Character.ScPage = npinfoPacket.Page;
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
                Session.SendPackets(Session.Character.GenerateScN());
            }
        }*/

        /// <summary>
        /// $pinv packet
        /// </summary>
        /// <param name="pinvPacket"></param>
        /*public void PinvGroupJoin(PinvPacket pinvPacket)
        {
            if (pinvPacket.CharacterName != null && ServerManager.Instance.GetSessionByCharacterName(pinvPacket.CharacterName) is ClientSession receiverSession)
            {
                GroupJoin(new PJoinPacket { RequestType = GroupRequestType.Requested, CharacterId = receiverSession.Character.CharacterId });
            }
        }*/


        /// <summary>
        /// tit_eq packet
        /// </summary>
        /// <param name="titeqPacket"></param>
        /*public void TitEq(TiteqPacket titEqPacket)
        {
            var tit = Session.Character.Titles.FirstOrDefault(s => s.TitleType == titEqPacket.TitleVNum);
            if (tit != null)
            {
                switch (titEqPacket.Type)
                {
                    case TiteqPacketType.Wiev:
                        if (tit.Visible == false)
                        {
                            Session.SendPacket(Session.Character.GenerateEffs(titEqPacket.Type));
                        }
                        foreach (var title in Session.Character.Titles.Where(s => s.TitleType != titEqPacket.TitleVNum))
                        {
                            title.Visible = false;
                        }
                        tit.Visible = !tit.Visible;
                        break;
                    default:
                        {
                            if (tit.Active == false)
                            {
                                Session.SendPacket(Session.Character.GenerateEffs(titEqPacket.Type));
                                Session.Character.ActiveTitle = tit;
                            }
                            else
                            {
                                Session.Character.ActiveTitle = null;
                            }
                            foreach (var title in Session.Character.Titles.Where(s => s.TitleType != titEqPacket.TitleVNum))
                            {
                                title.Active = false;
                            }
                            tit.Active = !tit.Active;
                            Session.Character.GenerateEquipment();
                            Session?.SendPacket(Session.Character.GenerateStat());
                            /*if (tit.TitleType == 9395)
                            {
                                Session.Character.HPLoad();
                                Session?.SendPacket(Session.Character.GenerateStat());
                            }
                        }
                        break;
                }

                Session.SendPackets(Session.Character.GenerateStatChar());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateTitleInfo());
                Session.SendPacket(Session.Character.GenerateTitle());
            }
        }*/

        /*public void QtPacket(QtPacket qtPacket)
        {
            switch (qtPacket.Type)
            {
                // On Target Dest
                case 1:
                    Session.Character.IncrementQuests(QuestType.GoTo, Session.CurrentMapInstance.Map.MapId, Session.Character.PositionX, Session.Character.PositionY);
                    break;

                // Give Up Quest
                case 3:
                    CharacterQuest charQuest = Session.Character.Quests?.FirstOrDefault(q => q.QuestNumber == qtPacket.Data);
                    if (charQuest == null || charQuest.IsMainQuest)
                    {
                        return;
                    }
                    Session.Character.RemoveQuest(charQuest.QuestId, true);
                    break;

                // Ask for rewards
                case 4:
                    break;
            }
        }*/



        /// <summary>
        /// fbPacket packet
        /// </summary>
        /// <param name="fbPacket"></param>
        /*public void RainbowBattleManage(FbPacket fbPacket)
        {
            Group grp;
            switch (fbPacket.Type)
            {
                // Join BaTeam
                case 1:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BAInstance)
                    {
                        return;
                    }

                    ClientSession target = ServerManager.Instance.GetSessionByCharacterId(fbPacket.CharacterId);
                    if (fbPacket.Parameter == null && target?.Character?.Group == null
                        && Session.Character.Group.IsLeader(Session))
                    {
                        GroupJoin(new PJoinPacket
                        {
                            RequestType = GroupRequestType.Invited,
                            CharacterId = fbPacket.CharacterId
                        });
                    }
                    else if (Session.Character.Group == null)
                    {
                        GroupJoin(new PJoinPacket
                        {
                            RequestType = GroupRequestType.Accepted,
                            CharacterId = fbPacket.CharacterId
                        });
                    }

                    break;

                // Leave BaTeam
                case 2:
                    ClientSession sender = ServerManager.Instance.GetSessionByCharacterId(fbPacket.CharacterId);
                    if (sender?.Character?.Group == null)
                    {
                        return;
                    }

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("LEFT_TEAM")),
                            0));
                    if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.BAInstance)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                            Session.Character.MapX, Session.Character.MapY);
                    }

                    grp = sender.Character?.Group;
                    Session.SendPacket(Session.Character.GenerateFbt(1, true));
                    Session.SendPacket(Session.Character.GenerateFbt(2, true));

                    grp.Sessions.ForEach(s =>
                    {
                        s.SendPacket(grp.GenerateFblst());
                        //s.SendPacket(grp.GeneraterRaidmbf(s));
                        s.SendPacket(s.Character.GenerateFbt(0));
                    });

                    RainbowBattleMember rainbowbattleTeamMember = ServerManager.Instance.RainbowBattleMembers.Where(s => s.Session == Session).First();
                    if (rainbowbattleTeamMember != null)
                    {
                        ServerManager.Instance.RainbowBattleMembers.Remove(rainbowbattleTeamMember);
                    }


                    RainbowBattleMember rainbowbattleTeamMemberRegistered = ServerManager.Instance.RainbowBattleMembersRegistered.Where(s => s.Session == Session).First();
                    if (rainbowbattleTeamMemberRegistered != null)
                    {
                        ServerManager.Instance.RainbowBattleMembersRegistered.Remove(rainbowbattleTeamMember);
                    }
                    break;

                // Kick from BaTeam
                case 3:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BAInstance)
                    {
                        return;
                    }

                    if (Session.Character.Group?.IsLeader(Session) == true)
                    {
                        ClientSession chartokick = ServerManager.Instance.GetSessionByCharacterId(fbPacket.CharacterId);
                        if (chartokick.Character?.Group == null)
                        {
                            return;
                        }

                        RainbowBattleMember rainbowbattleTeamMemberRegisteredd = ServerManager.Instance.RainbowBattleMembersRegistered.Where(s => s.Session == chartokick).First();
                        if (rainbowbattleTeamMemberRegisteredd != null)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANNOT_KICK_BA"), 0));
                            return;
                        }

                        chartokick.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KICK_BA"), 0));
                        grp = chartokick.Character?.Group;
                        chartokick.SendPacket(chartokick.Character?.GenerateFbt(1, true));
                        chartokick.SendPacket(chartokick.Character?.GenerateFbt(2, true));
                        grp?.LeaveGroup(chartokick);
                        grp?.Sessions.ForEach(s =>
                        {
                            s.SendPacket(grp.GenerateRdlst());
                            s.SendPacket(s.Character.GenerateFbt(0));
                        });
                        RainbowBattleMember rainbowbattleTeamMemberr = ServerManager.Instance.RainbowBattleMembers.Where(s => s.Session == chartokick).First();
                        if (rainbowbattleTeamMemberr != null)
                        {
                            ServerManager.Instance.RainbowBattleMembers.Remove(rainbowbattleTeamMemberr);
                        }
                    }

                    break;

                // Disolve BaTeam
                case 4:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BAInstance)
                    {
                        return;
                    }

                    if (Session.Character.Group?.IsLeader(Session) == true)
                    {
                        RainbowBattleMember rainbowbattleTeamMemberRegistereddd = ServerManager.Instance.RainbowBattleMembersRegistered?.Where(s => s.Session == Session).First();
                        if (rainbowbattleTeamMemberRegistereddd != null)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANNOT_DISSOLVE_BA"), 0));
                            return;
                        }

                        grp = Session.Character.Group;

                        ClientSession[] grpmembers = new ClientSession[40];
                        grp.Sessions.CopyTo(grpmembers);
                        foreach (ClientSession targetSession in grpmembers)
                        {
                            if (targetSession != null)
                            {
                                targetSession.SendPacket(targetSession.Character.GenerateFbt(1, true));
                                targetSession.SendPacket(targetSession.Character.GenerateFbt(2, true));
                                targetSession.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("BA_DISOLVED"), 0));
                                grp.LeaveGroup(targetSession);


                                RainbowBattleMember rainbowbattleTeamMemberrr = ServerManager.Instance.RainbowBattleMembers.Where(s => s.Session == targetSession).First();
                                if (rainbowbattleTeamMemberrr != null)
                                {
                                    ServerManager.Instance.RainbowBattleMembers.Remove(rainbowbattleTeamMemberrr);
                                }
                            }
                        }

                        ServerManager.Instance.GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                        ServerManager.Instance.ThreadSafeGroupList.Remove(grp.GroupId);
                    }

                    break;
            }
        }*/


        /// <summary>
        /// pst packet
        /// </summary>
        /// <param name="pstPacket"></param>
        /*public void SendMail(PstPacket pstPacket)
        {
            if (pstPacket?.Data != null)
            {
                CharacterDTO receiver = DAOFactory.CharacterDAO.LoadByName(pstPacket.Receiver);
                if (receiver != null)
                {
                    string[] datasplit = pstPacket.Data.Split(' ');
                    if (datasplit.Length < 2)
                    {
                        return;
                    }

                    if (datasplit[1].Length > 250)
                    {
                        //PenaltyLogDTO log = new PenaltyLogDTO
                        //{
                        //    AccountId = Session.Character.AccountId,
                        //    Reason = "You are an idiot!",
                        //    Penalty = PenaltyType.Banned,
                        //    DateStart = DateTime.Now,
                        //    DateEnd = DateTime.Now.AddYears(69),
                        //    AdminName = "Your mom's ass"
                        //};
                        //Session.Character.InsertOrUpdatePenalty(log);
                        //ServerManager.Instance.Kick(Session.Character.Name);
                        return;
                    }

                    ItemInstance headWearable =
                        Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Hat, InventoryType.Wear);
                    byte color = headWearable?.Item.IsColored == true
                        ? (byte)headWearable.Design
                        : (byte)Session.Character.HairColor;
                    MailDTO mailcopy = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = true,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0
                            ? (short)-1
                            : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };
                    MailDTO mail = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = false,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0
                            ? (short)-1
                            : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };

                    MailServiceClient.Instance.SendMail(mailcopy);
                    MailServiceClient.Instance.SendMail(mail);

                    //Session.Character.MailList.Add((Session.Character.MailList.Count > 0 ? Session.Character.MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mailcopy);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAILED"),
                        11));
                    //Session.SendPacket(Session.Character.GeneratePost(mailcopy, 2));
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else if (pstPacket != null && int.TryParse(pstPacket.Id.ToString(), out int id)
                     && byte.TryParse(pstPacket.Type.ToString(), out byte type))
            {
                if (pstPacket.Argument == 3)
                {
                    if (Session.Character.MailList.ContainsKey(id))
                    {
                        if (!Session.Character.MailList[id].IsOpened)
                        {
                            Session.Character.MailList[id].IsOpened = true;
                            MailDTO mailupdate = Session.Character.MailList[id];
                            DAOFactory.MailDAO.InsertOrUpdate(ref mailupdate);
                        }

                        Session.SendPacket(Session.Character.GeneratePostMessage(Session.Character.MailList[id], type));
                    }
                }
                else if (pstPacket.Argument == 2)
                {
                    if (Session.Character.MailList.ContainsKey(id))
                    {
                        MailDTO mail = Session.Character.MailList[id];
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAIL_DELETED"), 11));
                        Session.SendPacket($"post 2 {type} {id}");
                        if (DAOFactory.MailDAO.LoadById(mail.MailId) != null)
                        {
                            DAOFactory.MailDAO.DeleteById(mail.MailId);
                        }

                        if (Session.Character.MailList.ContainsKey(id))
                        {
                            Session.Character.MailList.Remove(id);
                        }
                    }
                }
            }
        }*/

        /// <summary>
        /// qset packet
        /// </summary>
        /// <param name="qSetPacket"></param>
        /*public void SetQuicklist(QSetPacket qSetPacket)
        {
            short data1 = 0, data2 = 0, type = qSetPacket.Type, q1 = qSetPacket.Q1, q2 = qSetPacket.Q2;
            if (qSetPacket.Data1.HasValue)
            {
                data1 = qSetPacket.Data1.Value;
            }

            if (qSetPacket.Data2.HasValue)
            {
                data2 = qSetPacket.Data2.Value;
            }

            switch (type)
            {
                case 0:
                case 1:

                    // client says qset 0 1 3 2 6 answer -> qset 1 3 0.2.6.0
                    Session.Character.QuicklistEntries.RemoveAll(n =>
                        n.Q1 == q1 && n.Q2 == q2
                        && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    Session.Character.QuicklistEntries.Add(new QuicklistEntryDTO
                    {
                        CharacterId = Session.Character.CharacterId,
                        Type = type,
                        Q1 = q1,
                        Q2 = q2,
                        Slot = data1,
                        Pos = data2,
                        Morph = Session.Character.UseSp ? (short)Session.Character.Morph : (short)0
                    });

                    if(Session.Character.Morph == 29 || (Session.Character.Morph == 30 && data2 != 15))
                    {
                        Session.Character.QuicklistEntries.RemoveAll(n =>
                        n.Q1 == q1 && n.Q2 == q2
                        && (Session.Character.UseSp ? n.Morph == (Session.Character.Morph == 29 ? 30 : 29) : n.Morph == 0));
                        if (type == 1 && Session.Character.Morph == 29)
                            data2 = data2 == 7 ? (short)(data2 + 9) : (short)(data2 + 8);
                        else if(type == 1 && Session.Character.Morph == 30)
                            data2 = data2 == 16 ? (short)(data2 - 9) : (short)(data2 - 8);
                        Session.Character.QuicklistEntries.Add(new QuicklistEntryDTO
                        {
                            CharacterId = Session.Character.CharacterId,
                            Type = type,
                            Q1 = q1,
                            Q2 = q2,
                            Slot = data1,
                            Pos = data2,
                            Morph = (short)(Session.Character.Morph == 29 ? 30 : 29)
                        });
                    }
                    Session.SendPacket($"qset {q1} {q2} {type}.{data1}.{data2}.0");
                    break;

                case 2:

                    // DragDrop / Reorder qset type to1 to2 from1 from2 vars -> q1 q2 data1 data2
                    QuicklistEntryDTO qlFrom = Session.Character.QuicklistEntries.SingleOrDefault(n =>
                        n.Q1 == data1 && n.Q2 == data2
                        && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    if (qlFrom != null)
                    {
                        QuicklistEntryDTO qlTo = Session.Character.QuicklistEntries.SingleOrDefault(n =>
                            n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp
                                ? n.Morph == Session.Character.Morph
                                : n.Morph == 0));
                        qlFrom.Q1 = q1;
                        qlFrom.Q2 = q2;
                        if (qlTo == null)
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket(
                                $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // old 'from' is now empty.
                            Session.SendPacket($"qset {data1} {data2} 7.7.-1.0");
                        }
                        else
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket(
                                $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // 'from' is now 'to' because they exchanged
                            qlTo.Q1 = data1;
                            qlTo.Q2 = data2;
                            Session.SendPacket($"qset {qlTo.Q1} {qlTo.Q2} {qlTo.Type}.{qlTo.Slot}.{qlTo.Pos}.0");
                        }
                    }
                    if (Session.Character.Morph == 29 || Session.Character.Morph == 30)
                    {
                         qlFrom = Session.Character.QuicklistEntries.SingleOrDefault(n =>
                            n.Q1 == data1 && n.Q2 == data2
                                          && (Session.Character.UseSp ? n.Morph == (Session.Character.Morph == 29 ? 30 : 29) : n.Morph == 0));
                         if (qlFrom != null)
                         {
                            QuicklistEntryDTO qlTo = Session.Character.QuicklistEntries.SingleOrDefault(n =>
                                n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp
                                    ? n.Morph == (Session.Character.Morph == 29 ? 30 : 29)
                                    : n.Morph == 0));
                            qlFrom.Q1 = q1;
                            qlFrom.Q2 = q2;
                            if (qlTo == null)
                            {
                                // Put 'from' to new position (datax)
                                Session.SendPacket(
                                    $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                                // old 'from' is now empty.
                                Session.SendPacket($"qset {data1} {data2} 7.7.-1.0");
                            }
                            else
                            {
                                // Put 'from' to new position (datax)
                                Session.SendPacket(
                                    $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                                // 'from' is now 'to' because they exchanged
                                qlTo.Q1 = data1;
                                qlTo.Q2 = data2;
                                Session.SendPacket($"qset {qlTo.Q1} {qlTo.Q2} {qlTo.Type}.{qlTo.Slot}.{qlTo.Pos}.0");
                            }
                        }
                    }
                    break;

                case 3:

                    // Remove from Quicklist
                    Session.Character.QuicklistEntries.RemoveAll(n =>
                        n.Q1 == q1 && n.Q2 == q2
                        && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    if(Session.Character.Morph == 29 || Session.Character.Morph == 30)
                        Session.Character.QuicklistEntries.RemoveAll(n =>
                            n.Q1 == q1 && n.Q2 == q2
                                       && (Session.Character.UseSp ? n.Morph == (Session.Character.Morph == 29 ? 30 : 29) : n.Morph == 0));
                    Session.SendPacket($"qset {q1} {q2} 7.7.-1.0");
                    break;

                default:
                    return;
            }
        }*/
        
       


        /// <summary>
        /// Anti-Cheat Heartbeat Packet
        /// </summary>
        /// <param name="ntcpAcPacket"></param>
        /*public void NtcpAcPacket(NtcpAcPacket ntcpAcPacket)
        {
            if (Session == null || ntcpAcPacket == null || ntcpAcPacket.Type != 1)
            {
                return;
            }

            #region Packet Manipulation

            if (ntcpAcPacket.TrapValue != 0 || ntcpAcPacket.Crc32?.Length != 8)
            {
                Session.BanForCheatUsing(1);
                return;
            }

            byte[] data = Convert.FromBase64String(ntcpAcPacket.Data);
            byte[] encryptedKey = Convert.FromBase64String(ntcpAcPacket.EncryptedKey);

            if (!AntiCheatHelper.IsValidSignature(ntcpAcPacket.Signature, data, encryptedKey, ntcpAcPacket.Crc32)
                || data.Length != encryptedKey.Length)
            {
                Session.BanForCheatUsing(2);
                return;
            }

            #endregion

            #region Crc32

            if (string.IsNullOrEmpty(Session.Crc32))
            {
                Session.Crc32 = ntcpAcPacket.Crc32;
            }
            else if (ntcpAcPacket.Crc32 != Session.Crc32)
            {
                Session.BanForCheatUsing(3);
                return;
            }

            #endregion

            #region Data

            if (Encoding.UTF8.GetString(data) != AntiCheatHelper.Encrypt(Encoding.UTF8.GetBytes(Session.LastAntiCheatData), encryptedKey))
            {
                Session.BanForCheatUsing(4);
                return;
            }

            #endregion

            #region IsAntiCheatAlive

            Session.IsAntiCheatAlive = true;

            #endregion
        }*/

        #endregion
    }
}