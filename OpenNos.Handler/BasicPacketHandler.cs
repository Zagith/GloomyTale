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
using OpenNos.Core.Extensions;
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
    public class BasicPacketHandler : IPacketHandler
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
        public void BlBlacklistAdd(BlPacket blPacket)
        {
            if (blPacket.CharacterName != null && ServerManager.Instance.GetSessionByCharacterName(blPacket.CharacterName) is ClientSession receiverSession)
            {
                BlacklistAdd(new BlInsPacket { CharacterId = receiverSession.Character.CharacterId });
            }
        }
        
        /// <summary>
        /// blins packet
        /// </summary>
        /// <param name="blInsPacket"></param>
        public void BlacklistAdd(BlInsPacket blInsPacket)
        {
            if (Session.Character.CharacterId == blInsPacket.CharacterId)
            {
                return;
            }

            if (DAOFactory.CharacterDAO.LoadById(blInsPacket.CharacterId) is CharacterDTO character
             && DAOFactory.AccountDAO.LoadById(character.AccountId).Authority >= AuthorityType.TMOD)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_BLACKLIST_TEAM")));
                return;
            }

            Session.Character.AddRelation(blInsPacket.CharacterId, CharacterRelationType.Blocked);
            Session.SendPacket(
                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_ADDED")));
            Session.SendPacket(Session.Character.GenerateBlinit());
        }

        /// <summary>
        /// bldel packet
        /// </summary>
        /// <param name="blDelPacket"></param>
        public void BlacklistDelete(BlDelPacket blDelPacket)
        {
            Session.Character.DeleteBlackList(blDelPacket.CharacterId);
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(
                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_DELETED")));
        }

        /// <summary>
        /// gop packet
        /// </summary>
        /// <param name="characterOptionPacket"></param>
        public void CharacterOptionChange(CharacterOptionPacket characterOptionPacket)
        {
            if (Session.Character == null)
            {
                return;
            }

            switch (characterOptionPacket.Option)
            {
                case CharacterOption.BuffBlocked:
                    Session.Character.BuffBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.BuffBlocked
                            ? "BUFF_BLOCKED"
                            : "BUFF_UNLOCKED"), 0));
                    break;

                case CharacterOption.EmoticonsBlocked:
                    Session.Character.EmoticonsBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.EmoticonsBlocked
                            ? "EMO_BLOCKED"
                            : "EMO_UNLOCKED"), 0));
                    break;

                case CharacterOption.ExchangeBlocked:
                    Session.Character.ExchangeBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.ExchangeBlocked
                            ? "EXCHANGE_BLOCKED"
                            : "EXCHANGE_UNLOCKED"), 0));
                    break;

                case CharacterOption.FriendRequestBlocked:
                    Session.Character.FriendRequestBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.FriendRequestBlocked
                            ? "FRIEND_REQ_BLOCKED"
                            : "FRIEND_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupRequestBlocked:
                    Session.Character.GroupRequestBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.GroupRequestBlocked
                            ? "GROUP_REQ_BLOCKED"
                            : "GROUP_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.PetAutoRelive:
                    Session.Character.IsPetAutoRelive = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.IsPetAutoRelive 
                        ? "PET_AUTO_RELIVE_ENABLED" 
                        : "PET_AUTO_RELIVE_DISABLED"), 0));
                    break;

                case CharacterOption.PartnerAutoRelive:
                    Session.Character.IsPartnerAutoRelive = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.IsPartnerAutoRelive 
                        ? "PARTNER_AUTO_RELIVE_ENABLED" 
                        : "PARTNER_AUTO_RELIVE_DISABLED"), 0));
                    break;

                case CharacterOption.HeroChatBlocked:
                    Session.Character.HeroChatBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.HeroChatBlocked
                            ? "HERO_CHAT_BLOCKED"
                            : "HERO_CHAT_UNLOCKED"), 0));
                    break;

                case CharacterOption.HpBlocked:
                    Session.Character.HpBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.HpBlocked ? "HP_BLOCKED" : "HP_UNLOCKED"),
                        0));
                    break;

                case CharacterOption.MinilandInviteBlocked:
                    Session.Character.MinilandInviteBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.MinilandInviteBlocked
                            ? "MINI_INV_BLOCKED"
                            : "MINI_INV_UNLOCKED"), 0));
                    break;

                case CharacterOption.MouseAimLock:
                    Session.Character.MouseAimLock = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.MouseAimLock
                            ? "MOUSE_LOCKED"
                            : "MOUSE_UNLOCKED"), 0));
                    break;

                case CharacterOption.QuickGetUp:
                    Session.Character.QuickGetUp = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.QuickGetUp
                            ? "QUICK_GET_UP_ENABLED"
                            : "QUICK_GET_UP_DISABLED"), 0));
                    break;

                case CharacterOption.WhisperBlocked:
                    Session.Character.WhisperBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.WhisperBlocked
                            ? "WHISPER_BLOCKED"
                            : "WHISPER_UNLOCKED"), 0));
                    break;

                case CharacterOption.FamilyRequestBlocked:
                    Session.Character.FamilyRequestBlocked = !characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.FamilyRequestBlocked
                            ? "FAMILY_REQ_LOCKED"
                            : "FAMILY_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupSharing:
                    Group grp = ServerManager.Instance.Groups.Find(
                        g => g != null && g.IsMemberOfGroup(Session.Character.CharacterId));
                    if (grp == null)
                    {
                        return;
                    }

                    if (!grp.IsLeader(Session))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_MASTER"), 0));
                        return;
                    }

                    if (!characterOptionPacket.IsActive)
                    {
                        grp.SharingMode = 1;

                        Session.CurrentMapInstance?.Broadcast(Session,
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING"), 0),
                            ReceiverType.Group);
                    }
                    else
                    {
                        grp.SharingMode = 0;

                        Session.CurrentMapInstance?.Broadcast(Session,
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING_BY_ORDER"), 0),
                            ReceiverType.Group);
                    }
                    break;
            }

            Session.SendPacket(Session.Character.GenerateStat());
        }

        /// <summary>
        /// compl packet
        /// </summary>
        /// <param name="complimentPacket"></param>
        public void Compliment(ComplimentPacket complimentPacket)
        {
            if (complimentPacket != null)
            {
                if (Session.Character.CharacterId == complimentPacket.CharacterId)
                {
                    return;
                }

                ClientSession sess = ServerManager.Instance.GetSessionByCharacterId(complimentPacket.CharacterId);
                if (sess != null)
                {
                    if (Session.Character.Level >= 30)
                    {
                        GeneralLogDTO dto =
                            Session.Character.GeneralLogs.LastOrDefault(s =>
                                s.LogData == "World" && s.LogType == "Connection");
                        GeneralLogDTO lastcompliment =
                            Session.Character.GeneralLogs.LastOrDefault(s =>
                                s.LogData == "World" && s.LogType == nameof(Compliment));
                        if (dto?.Timestamp.AddMinutes(60) <= DateTime.Now)
                        {
                            if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now.Date)
                            {
                                sess.Character.Compliment++;
                                Session.SendPacket(Session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"),
                                        sess.Character.Name), 12));
                                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                                {
                                    AccountId = Session.Account.AccountId,
                                    CharacterId = Session.Character.CharacterId,
                                    IpAddress = Session.IpAddress,
                                    LogData = "World",
                                    LogType = nameof(Compliment),
                                    Timestamp = DateTime.Now
                                });

                                Session.CurrentMapInstance?.Broadcast(Session,
                                    Session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"),
                                            Session.Character.Name), 12), ReceiverType.OnlySomeone,
                                    characterId: complimentPacket.CharacterId);
                            }
                            else
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                            }
                        }
                        else if (dto != null)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"),
                                    (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"),
                                11));
                    }
                }
            }
        }

        /// <summary>
        /// dir packet
        /// </summary>
        /// <param name="directionPacket"></param>
        public void Dir(DirectionPacket directionPacket)
        {
            if (directionPacket.CharacterId == Session.Character.CharacterId)
            {
                Session.Character.Direction = directionPacket.Direction;
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateDir());
            }
        }

        /// <summary>
        /// $fl packet
        /// </summary>
        /// <param name="flPacket"></param>
        public void FlRelationAdd(FlPacket flPacket)
        {
            if (flPacket.CharacterName != null && ServerManager.Instance.GetSessionByCharacterName(flPacket.CharacterName) is ClientSession receiverSession)
            {
                RelationAdd(new FInsPacket { Type = 1, CharacterId = receiverSession.Character.CharacterId });
            }
        }

        /// <summary>
        /// fins packet
        /// </summary>
        /// <param name="fInsPacket"></param>
        public void RelationAdd(FInsPacket fInsPacket)
        {
            long characterId = fInsPacket.CharacterId;

            if (Session.Character.CharacterId == characterId)
            {
                return;
            }

            ClientSession otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
            if (otherSession != null)
            {
                if (Session.Character.Timespace != null || otherSession.Character.Timespace != null)
                {
                    return;
                }
                if (!Session.Character.IsFriendlistFull())
                {
                    if (!Session.Character.IsFriendOfCharacter(characterId) && (fInsPacket.Type == 1 || fInsPacket.Type == 2)
                     || !Session.Character.IsMarried && (fInsPacket.Type == 34 || fInsPacket.Type == 69))
                    {
                        if (!Session.Character.IsBlockedByCharacter(characterId))
                        {
                            if (!Session.Character.IsBlockingCharacter(characterId))
                            {
                                if (otherSession.Character.MarryRequestCharacters.GetAllItems()
                                       .Contains(Session.Character.CharacterId))
                                {
                                    switch (fInsPacket.Type)
                                    {
                                        case 34:
                                            Session.Character.DeleteRelation(characterId, CharacterRelationType.Friend);
                                            Session.Character.AddRelation(characterId, CharacterRelationType.Spouse);
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("MARRIAGE_ACCEPT")}");
                                            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("MARRIAGE_ACCEPT_SHOUT"), Session.Character.Name, otherSession.Character.Name), 0));
                                            break;

                                        case 69:
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("MARRIAGE_REJECTED")}");
                                            //ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MARRIAGE_REJECT_SHOUT"), 1));
                                            break;
                                    }
                                }
                                if (otherSession.Character.FriendRequestCharacters.GetAllItems()
                                    .Contains(Session.Character.CharacterId))
                                {
                                    switch (fInsPacket.Type)
                                    {
                                        case 1:
                                            Session.Character.AddRelation(characterId, CharacterRelationType.Friend);
                                            Session.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                            break;

                                        case 2:
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_REJECTED")}");
                                            break;

                                        default:
                                            if (Session.Character.IsFriendlistFull())
                                            {
                                                Session.SendPacket(
                                                    $"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                                otherSession.SendPacket(
                                                    $"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                            }
                                            break;
                                    }
                                }
                                else if (fInsPacket.Type != 34 && fInsPacket.Type != 69)
                                {
                                    if (otherSession.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                                    {
                                        return;
                                    }

                                    if (otherSession.Character.FriendRequestBlocked)
                                    {
                                        Session.SendPacket(
                                            $"info {Language.Instance.GetMessageFromKey("FRIEND_REJECTED")}");
                                        return;
                                    }

                                    otherSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#fins^1^{Session.Character.CharacterId} #fins^2^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("FRIEND_ADD"), Session.Character.Name)}"));
                                    Session.Character.FriendRequestCharacters.Add(characterId);
                                }
                            }
                            else
                            {
                                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKING")}");
                            }
                        }
                        else
                        {
                            Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")}");
                        }
                    }
                    else
                    {
                        Session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_FRIEND")}");
                    }
                }
                else
                {
                    Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                }
            }
        }

        /// <summary>
        /// fdel packet
        /// </summary>
        /// <param name="fDelPacket"></param>
        public void FriendDelete(FDelPacket fDelPacket)
        {
            if (Session.Character.CharacterRelations.Any(s => s.RelatedCharacterId == fDelPacket.CharacterId && s.RelationType == CharacterRelationType.Spouse))
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("CANT_DELETE_COUPLE")}");
                return;
            }
            Session.Character.DeleteRelation(fDelPacket.CharacterId, CharacterRelationType.Friend);
            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_DELETED")));
        }

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
        /// btk packet
        /// </summary>
        /// <param name="btkPacket"></param>
        public void FriendTalk(BtkPacket btkPacket)
        {
            if (string.IsNullOrEmpty(btkPacket.Message))
            {
                return;
            }

            string message = btkPacket.Message;
            if (message.Length > 60)
            {
                message = message.Substring(0, 60);
            }

            message = message.Trim();

            CharacterDTO character = DAOFactory.CharacterDAO.LoadById(btkPacket.CharacterId);
            if (character != null)
            {
                int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = character.CharacterId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = PacketFactory.Serialize(Session.Character.GenerateTalk(message)),
                    Type = MessageType.PrivateChat
                });

                if (!sentChannelId.HasValue) //character is even offline on different world
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_OFFLINE")));
                }
                else
                {
                    LogHelper.Instance.InsertChatLog(ChatType.Friend, Session.Character.CharacterId, message, Session.IpAddress);
                }
            }
        }

        /// <summary>
        /// pcl packet
        /// </summary>
        /// <param name="getGiftPacket"></param>
        public void GetGift(GetGiftPacket getGiftPacket)
        {
            int giftId = getGiftPacket.GiftId;
            if (Session.Character.MailList.ContainsKey(giftId))
            {
                MailDTO mail = Session.Character.MailList[giftId];
                if (getGiftPacket.Type == 4 && mail.AttachmentVNum != null)
                {
                    if (Session.Character.Inventory.CanAddItem((short)mail.AttachmentVNum))
                    {
                        ItemInstance newInv = Session.Character.Inventory.AddNewToInventory((short)mail.AttachmentVNum,
                                mail.AttachmentAmount, Upgrade: mail.AttachmentUpgrade,
                                Rare: (sbyte)mail.AttachmentRarity, Design: mail.AttachmentDesign)
                            .FirstOrDefault();
                        if (newInv != null)
                        {
                            if (newInv.Rare != 0)
                            {
                                newInv.SetRarityPoint();
                            }

                            if (newInv.Item.EquipmentSlot == EquipmentType.Gloves || newInv.Item.EquipmentSlot == EquipmentType.Boots)
                            {
                                newInv.DarkResistance = (short)(newInv.Item.DarkResistance * newInv.Upgrade);
                                newInv.LightResistance = (short)(newInv.Item.LightResistance * newInv.Upgrade);
                                newInv.WaterResistance = (short)(newInv.Item.WaterResistance * newInv.Upgrade);
                                newInv.FireResistance = (short)(newInv.Item.FireResistance * newInv.Upgrade);
                            }

                            Logger.LogUserEvent("PARCEL_GET", Session.GenerateIdentity(),
                                $"IIId: {newInv.Id} ItemVNum: {newInv.ItemVNum} Amount: {mail.AttachmentAmount} Sender: {mail.SenderId}");

                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("ITEM_GIFTED"), newInv.Item.Name[Session.Account.Language],
                                    mail.AttachmentAmount), 12));

                            DAOFactory.MailDAO.DeleteById(mail.MailId);

                            Session.SendPacket($"parcel 2 1 {giftId}");

                            Session.Character.MailList.Remove(giftId);
                        }
                    }
                    else
                    {
                        Session.SendPacket("parcel 5 1 0");
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                0));
                    }
                }
                else if (getGiftPacket.Type == 5)
                {
                    Session.SendPacket($"parcel 7 1 {giftId}");

                    if (DAOFactory.MailDAO.LoadById(mail.MailId) != null)
                    {
                        DAOFactory.MailDAO.DeleteById(mail.MailId);
                    }

                    if (Session.Character.MailList.ContainsKey(giftId))
                    {
                        Session.Character.MailList.Remove(giftId);
                    }
                }
            }
        }

        /// <summary>
        /// ncif packet
        /// </summary>
        /// <param name="ncifPacket"></param>
        public void GetNamedCharacterInformation(NcifPacket ncifPacket)
        {
            switch (ncifPacket.Type)
            {
                // characters
                case 1:
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(ncifPacket.TargetId)?.Character
                        ?.GenerateStatInfo());
                    break;

                // npcs/mates
                case 2:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Npcs.Where(n => n.MapNpcId == (int)ncifPacket.TargetId).ToList()
                            .ForEach(npc =>
                            {
                                NpcMonster npcinfo = ServerManager.GetNpcMonster(npc.NpcVNum);
                                if (npcinfo == null)
                                {
                                    return;
                                }

                                Session.Character.LastNpcMonsterId = npc.MapNpcId;
                                Session.SendPacket(
                                    $"st 2 {ncifPacket.TargetId} {npcinfo.Level} {npcinfo.HeroLevel} {(int)((float)npc.CurrentHp / (float)npc.MaxHp * 100)} {(int)((float)npc.CurrentMp / (float)npc.MaxMp * 100)} {npc.CurrentHp} {npc.CurrentMp}{npc.Buff.GetAllItems().Aggregate("", (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}");
                            });
                        Parallel.ForEach(Session.CurrentMapInstance.Sessions, session =>
                        {
                            Mate mate = session.Character.Mates.Find(
                                s => s.MateTransportId == (int)ncifPacket.TargetId);
                            if (mate != null)
                            {
                                Session.SendPacket(mate.GenerateStatInfo());
                            }
                        });
                    }

                    break;

                // monsters
                case 3:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Monsters.Where(m => m.MapMonsterId == (int)ncifPacket.TargetId)
                            .ToList().ForEach(monster =>
                            {
                                NpcMonster monsterinfo = ServerManager.GetNpcMonster(monster.MonsterVNum);
                                if (monsterinfo == null)
                                {
                                    return;
                                }

                                Session.Character.LastNpcMonsterId = monster.MapMonsterId;
                                Session.SendPacket(
                                    $"st 3 {ncifPacket.TargetId} {monsterinfo.Level} {monsterinfo.HeroLevel} {(int)((float)monster.CurrentHp / (float)monster.MaxHp * 100)} {(int)((float)monster.CurrentMp / (float)monster.MaxMp * 100)} {monster.CurrentHp} {monster.CurrentMp}{monster.Buff.GetAllItems().Aggregate("", (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}");
                            });
                    }

                    break;
            }
        }

        /// <summary>
        /// RstartPacket packet
        /// </summary>
        /// <param name="rStartPacket"></param>
        public void GetRStart(RStartPacket rStartPacket)
        {
            if (Session.Character.Timespace != null)
            {
                if (rStartPacket.Type == 1 && Session.Character.Timespace.InstanceBag != null && Session.Character.Timespace.InstanceBag.Lock == false)
                {
                    if (Session.Character.Timespace.SpNeeded?[(byte)Session.Character.Class] != 0)
                    {
                        ItemInstance specialist = Session.Character.Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                        if (specialist == null || specialist.ItemVNum != Session.Character.Timespace.SpNeeded?[(byte)Session.Character.Class])
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TS_SP_NEEDED"), 0));
                            return;
                        }
                    }
                    Session.Character.Timespace.InstanceBag.Lock = true;
                    Preq(new PreqPacket());
                    Session.Character.Timespace._mapInstanceDictionary.ToList().SelectMany(s => s.Value.Sessions).Where(s => s.Character?.Timespace != null).ToList().ForEach(s =>
                    {
                        s.Character.GeneralLogs.Add(new GeneralLogDTO
                        {
                            AccountId = s.Account.AccountId,
                            CharacterId = s.Character.CharacterId,
                            IpAddress = s.IpAddress,
                            LogData = s.Character.Timespace.Id.ToString(),
                            LogType = "InstanceEntry",
                            Timestamp = DateTime.Now
                        });
                    });
                }
            }
        }

        /// <summary>
        /// npinfo packet
        /// </summary>
        /// <param name="npinfoPacket"></param>
        public void GetStats(NpinfoPacket npinfoPacket)
        {
            Session.SendPackets(Session.Character.GenerateStatChar());
            
            if (npinfoPacket.Page != Session.Character.ScPage)
            {
                Session.Character.ScPage = npinfoPacket.Page;
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
                Session.SendPackets(Session.Character.GenerateScN());
            }            
        }

        /// <summary>
        /// $pinv packet
        /// </summary>
        /// <param name="pinvPacket"></param>
        public void PinvGroupJoin(PinvPacket pinvPacket)
        {
            if (pinvPacket.CharacterName != null && ServerManager.Instance.GetSessionByCharacterName(pinvPacket.CharacterName) is ClientSession receiverSession)
            {
                GroupJoin(new PJoinPacket { RequestType = GroupRequestType.Requested, CharacterId = receiverSession.Character.CharacterId });
            }
        }

        /// <summary>
        /// pjoin packet
        /// </summary>
        /// <param name="pjoinPacket"></param>
        public void GroupJoin(PJoinPacket pjoinPacket)
        {
            if (pjoinPacket != null)
            {
                bool createNewGroup = true;
                ClientSession targetSession = ServerManager.Instance.GetSessionByCharacterId(pjoinPacket.CharacterId);

                if ((targetSession == null && !pjoinPacket.RequestType.Equals(GroupRequestType.Sharing)) 
                || targetSession?.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance 
                || ServerManager.Instance.ArenaMembers.ToList().Any(s => s.Session?.Character?.CharacterId == Session.Character.CharacterId)
                || (targetSession != null && ServerManager.Instance.ChannelId == 51 && targetSession.Character.Faction != Session.Character.Faction)
                || Session.Character.Timespace != null 
                || targetSession?.Character.Timespace != null)
                {
                    return;
                }

                if (pjoinPacket.RequestType.Equals(GroupRequestType.Requested)
                    || pjoinPacket.RequestType.Equals(GroupRequestType.Invited))
                {
                    if (pjoinPacket.CharacterId == 0)
                    {
                        return;
                    }

                    if (ServerManager.Instance.IsCharactersGroupFull(pjoinPacket.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        return;
                    }

                    if (ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId)
                        && ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("ALREADY_IN_GROUP")));
                        return;
                    }

                    if (Session.Character.CharacterId != pjoinPacket.CharacterId && targetSession != null)
                    {
                        if (Session.Character.IsBlockedByCharacter(pjoinPacket.CharacterId))
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(
                                    Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                            return;
                        }

                        if (targetSession.Character.IsBlockedByCharacter(Session.Character.CharacterId))
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(
                                    Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKING")));
                            return;
                        }

                        if (targetSession.Character.GroupRequestBlocked)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_BLOCKED"),
                                    0));
                        }
                        else
                        {
                            // save sent group request to current character
                            Session.Character.GroupSentRequestCharacterIds.Add(targetSession.Character.CharacterId);
                            if (Session.Character.Group == null || Session.Character.Group.GroupType == GroupType.Group)
                            {
                                if (targetSession.Character?.Group == null
                                    || targetSession.Character?.Group.GroupType == GroupType.Group)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                                        string.Format(Language.Instance.GetMessageFromKey("GROUP_REQUEST"),
                                            targetSession.Character.Name)));
                                    targetSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#pjoin^3^{Session.Character.CharacterId} #pjoin^4^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU"), Session.Character.Name)}"));
                                }
                                else
                                {
                                    //can't invite raid member
                                }
                            }
                            else if (Session.Character.Group.IsLeader(Session))
                            {
                                targetSession.SendPacket(
                                    $"qna #rd^1^{Session.Character.CharacterId}^1 {string.Format(Language.Instance.GetMessageFromKey("INVITE_RAID"), Session.Character.Name)}");
                            }
                        }
                    }
                }
                else if (pjoinPacket.RequestType.Equals(GroupRequestType.Sharing))
                {
                    if (Session.Character.Group != null)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_SHARE_INFO")));
                        Session.Character.Group.Sessions
                            .Where(s => s.Character.CharacterId != Session.Character.CharacterId).ToList().ForEach(s =>
                            {
                                s.SendPacket(UserInterfaceHelper.GenerateDialog(
                                    $"#pjoin^6^{Session.Character.CharacterId} #pjoin^7^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU_SHARE"), Session.Character.Name)}"));
                                Session.Character.GroupSentRequestCharacterIds.Add(s.Character.CharacterId);
                            });
                    }
                }
                else if (pjoinPacket.RequestType.Equals(GroupRequestType.Accepted))
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems()
                            .Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    try
                    {
                        targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId)
                        && ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId))
                    {
                        // everyone is in group, return
                        return;
                    }

                    if (ServerManager.Instance.IsCharactersGroupFull(pjoinPacket.CharacterId)
                        || ServerManager.Instance.IsCharactersGroupFull(Session.Character.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        targetSession?.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                        return;
                    }

                    // get group and add to group
                    if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                    {
                        // target joins source
                        Group currentGroup =
                            ServerManager.Instance.GetGroupByCharacterId(Session.Character.CharacterId);

                        if (currentGroup != null)
                        {
                            currentGroup.JoinGroup(targetSession);
                            targetSession?.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOINED_GROUP"),
                                    10));
                            createNewGroup = false;
                        }
                    }
                    else if (ServerManager.Instance.IsCharacterMemberOfGroup(pjoinPacket.CharacterId))
                    {
                        // source joins target
                        Group currentGroup = ServerManager.Instance.GetGroupByCharacterId(pjoinPacket.CharacterId);

                        if (currentGroup != null)
                        {
                            createNewGroup = false;
                            if (currentGroup.GroupType == GroupType.Group)
                            {
                                currentGroup.JoinGroup(Session);
                            }
                            else
                            {
                                if (!currentGroup.Raid.InstanceBag.Lock)
                                {
                                    if (Session.Character.Level < currentGroup.Raid?.LevelMinimum)
                                    {
                                        Session.SendPacket(Session.Character.GenerateSay(
                                            Language.Instance.GetMessageFromKey("LOW_LVL"), 10));
                                        return;
                                    }

                                    if (currentGroup.JoinGroup(Session))
                                    {
                                        Session.SendPacket(
                                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_JOIN"),
                                            10));
                                        if (Session.Character.Level > currentGroup.Raid?.LevelMaximum)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay(
                                                Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));
                                            if (Session.Character.Level
                                                >= currentGroup.Raid?.LevelMaximum + 10 /* && AlreadySuccededToday*/)
                                            {
                                                //modal 1 ALREADY_SUCCEDED_AS_ASSISTANT
                                            }
                                        }

                                        Session.SendPacket(Session.Character.GenerateRaid(2));
                                        Session.SendPacket(Session.Character.GenerateRaid(1));
                                        currentGroup.Sessions.ForEach(s =>
                                        {
                                            s.SendPacket(currentGroup.GenerateRdlst());
                                            s.SendPacket(s.Character.GenerateSay(
                                                string.Format(Language.Instance.GetMessageFromKey("JOIN_TEAM"),
                                                    Session.Character.Name), 10));
                                            s.SendPacket(s.Character.GenerateRaid(0));
                                        });
                                    }
                                    else
                                    {
                                        if (ServerManager.Instance.RainbowBattleMembersRegistered.Count() > 0)
                                        {
                                            if (ServerManager.Instance.RainbowBattleMembersRegistered?.Where(s => s.Session == Session).Count() > 0)
                                            {
                                                return;
                                            }
                                        }
                                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("BA_JOIN"), 10));

                                        currentGroup.JoinGroup(Session);
                                        Session.SendPacket(Session.Character.GenerateFbt(1));
                                        currentGroup.Sessions.ForEach(s =>
                                        {
                                            s.SendPacket(currentGroup.GenerateFblst());
                                            s.SendPacket(s.Character.GenerateSay(
                                                string.Format(Language.Instance.GetMessageFromKey("JOIN_TEAM_BA"),
                                                    Session.Character.Name), 10));
                                            s.SendPacket(s.Character.GenerateFbt(0));
                                        });

                                        ServerManager.Instance.RainbowBattleMembers.Add(new RainbowBattleMember
                                        {
                                            RainbowBattleType = EventType.RAINBOWBATTLE,
                                            Session = Session,
                                            GroupId = currentGroup.GroupId,
                                        });
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(
                                    Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_ALREADY_STARTED"),
                                        10));
                                }
                            }
                        }
                    }

                    if (createNewGroup)
                    {
                        Group group = new Group
                        {
                            GroupType = GroupType.Group
                        };
                        group.JoinGroup(pjoinPacket.CharacterId);
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("GROUP_JOIN"),
                                targetSession?.Character.Name), 10));
                        group.JoinGroup(Session.Character.CharacterId);
                        ServerManager.Instance.AddGroup(group);
                        targetSession?.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_ADMIN")));

                        // set back reference to group
                        Session.Character.Group = group;
                        if (targetSession != null)
                        {
                            targetSession.Character.Group = @group;
                        }
                    }

                    if (Session.Character?.Group?.GroupType == GroupType.Group)
                    {
                        // player join group
                        ServerManager.Instance.UpdateGroup(pjoinPacket.CharacterId);
                        Session.CurrentMapInstance?.Broadcast(Session.Character.GeneratePidx());
                    }
                }
                else if (pjoinPacket.RequestType == GroupRequestType.Declined)
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems().Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                    targetSession?.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("REFUSED_GROUP_REQUEST"),
                            Session.Character.Name), 10));
                }
                else if (pjoinPacket.RequestType == GroupRequestType.AcceptedShare)
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems().Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("ACCEPTED_SHARE"),
                            targetSession?.Character.Name), 0));
                    if (Session.Character?.Group?.IsMemberOfGroup(pjoinPacket.CharacterId) == true && targetSession != null)
                    {
                        Session.Character.SetReturnPoint(targetSession.Character.Return.DefaultMapId,
                            targetSession.Character.Return.DefaultX, targetSession.Character.Return.DefaultY);
                        targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("CHANGED_SHARE"), targetSession.Character.Name), 0));
                    }
                }
                else if (pjoinPacket.RequestType == GroupRequestType.DeclinedShare)
                {
                    if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems()
                            .Contains(Session.Character.CharacterId) == false)
                    {
                        return;
                    }

                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REFUSED_SHARE"), 0));
                }
            }
        }

        /// <summary>
        /// pleave packet
        /// </summary>
        /// <param name="pleavePacket"></param>
        public void GroupLeave(PLeavePacket pleavePacket) => ServerManager.Instance.GroupLeave(Session);

        /// <summary>
        /// ; packet
        /// </summary>
        /// <param name="groupSayPacket"></param>
        public void GroupTalk(GroupSayPacket groupSayPacket)
        {
            if (!string.IsNullOrEmpty(groupSayPacket.Message))
            {

                ServerManager.Instance.Broadcast(Session, Session.Character.GenerateSpk(groupSayPacket.Message, 3),
                    ReceiverType.Group);
                LogHelper.Instance.InsertChatLog(ChatType.Friend, Session.Character.CharacterId, groupSayPacket.Message, Session.IpAddress);
            }
        }

        /// <summary>
        /// tit_eq packet
        /// </summary>
        /// <param name="titeqPacket"></param>
        public void TitEq(TiteqPacket titEqPacket)
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
                        /*Session.SendPacket(new InfoPacket
                        {
                            Message = Session.GetMessageFromKey(LanguageKey.TITLE_VISIBILITY_CHANGED)
                        });*/
                        break;
                    default:
                        
                        if (tit.Active == false)
                        {
                           
                           Session.SendPacket(Session.Character.GenerateEffs(titEqPacket.Type));
                        }
                        foreach (var title in Session.Character.Titles.Where(s => s.TitleType != titEqPacket.TitleVNum))
                        {
                            title.Active = false;
                        }
                        tit.Active = !tit.Active;
                        /*Session.SendPacket(new InfoPacket
                        {
                            Message = Session.GetMessageFromKey(LanguageKey.TITLE_EFFECT_CHANGED)
                        });*/
                        break;
                }

                Session.SendPackets(Session.Character.GenerateStatChar());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateTitleInfo());
                Session.SendPacket(Session.Character.GenerateTitle());
            }                
        }

        /// <summary>
        /// hero packet
        /// </summary>
        /// <param name="heroPacket"></param>
        public void Hero(HeroPacket heroPacket)
        {
            if (!string.IsNullOrEmpty(heroPacket.Message))
            {
                if (Session.Character.IsReputationHero() >= 3 && Session.Character.Reputation > 5000000)
                {
                    heroPacket.Message = heroPacket.Message.Trim();
                    ServerManager.Instance.Broadcast(Session, $"msg 5 [{Session.Character.Name}]:{heroPacket.Message}",
                        ReceiverType.AllNoHeroBlocked);
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_HERO"), 11));
                }
            }
        }

        /// <summary>
        /// PreqPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Preq(PreqPacket packet)
        {
            if (Session.Character.IsSeal)
            {
                return;
            }

            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;

            double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;

            if (Session.Account?.Authority != AuthorityType.Administrator
                && (timeSpanSinceLastPortal < 4 || !Session.HasCurrentMapInstance))
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                return;
            }

            if (Session.CurrentMapInstance.Portals.Concat(Session.Character.GetExtraPortal())
                    .FirstOrDefault(s => Session.Character.PositionY >= s.SourceY - 1
                        && Session.Character.PositionY <= s.SourceY + 1
                        && Session.Character.PositionX >= s.SourceX - 1
                        && Session.Character.PositionX <= s.SourceX + 1) is Portal portal)
            {
                switch (portal.Type)
                {
                    case (sbyte)PortalType.MapPortal:
                    case (sbyte)PortalType.TSNormal:
                    case (sbyte)PortalType.Open:
                    case (sbyte)PortalType.Miniland:
                    case (sbyte)PortalType.TSEnd:
                    case (sbyte)PortalType.Exit:
                    case (sbyte)PortalType.Effect:
                    case (sbyte)PortalType.ShopTeleport:
                        break;

                    case (sbyte)PortalType.Raid:
                        if (Session.Character.Group?.Raid != null)
                        {
                            if (Session.Character.Group.IsLeader(Session))
                            {
                                Session.SendPacket(
                                    $"qna #mkraid^0^275 {Language.Instance.GetMessageFromKey("RAID_START_QUESTION")}");
                            }
                            else
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("NOT_TEAM_LEADER"), 10));
                            }
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NEED_TEAM"), 10));
                        }

                        return;

                    case (sbyte)PortalType.BlueRaid:
                    case (sbyte)PortalType.DarkRaid:
                        if (!packet.Parameter.HasValue)
                        {
                            Session.SendPacket($"qna #preq^1 {string.Format(Language.Instance.GetMessageFromKey("ACT4_RAID_ENTER"), Session.Character.Level * 5)}");
                            return;
                        }
                        else
                        {
                            if (packet.Parameter == 1)
                            {
                                if ((int)Session.Character.Faction == portal.Type - 9 && Session.Character.Family?.Act4Raid != null)
                                {
                                    if (Session.Character.Level > 49)
                                    {
                                        if (Session.Character.Contributi > 10000)
                                        {
                                            Session.Character.SetContributi(-10000);

                                            Session.Character.LastPortal = currentRunningSeconds;

                                            switch (Session.Character.Family.Act4Raid.MapInstanceType)
                                            {
                                                case MapInstanceType.Act4Viserion:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 43, 179);
                                                    break;

                                                case MapInstanceType.Act4Orias:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 15, 9);
                                                    break;

                                                case MapInstanceType.Act4Zanarkand:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 24, 6);
                                                    break;

                                                case MapInstanceType.Act4Demetra:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 20, 20);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(
                                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_CONTRIBUTI"),
                                                    10));
                                        }
                                    }
                                    else
                                    {
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOW_LVL"),
                                                10));
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(
                                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"),
                                            10));
                                }
                            }
                        }

                        return;

                    default:
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                        return;
                }
                
                if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.TimeSpaceInstance
                    && Session?.Character?.Timespace != null && !Session.Character.Timespace.InstanceBag.Lock)
                {
                    if (Session.Character.CharacterId == Session.Character.Timespace.InstanceBag.CreatorId)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateDialog(
                            $"#rstart^1 rstart {Language.Instance.GetMessageFromKey("FIRST_ROOM_START")}"));
                    }

                    return;
                }

                if (Session?.CurrentMapInstance?.MapInstanceType != MapInstanceType.BaseMapInstance && portal.DestinationMapId == 134)
                {
                    if (!packet.Parameter.HasValue)
                    {
                        Session.SendPacket($"qna #preq^1 {Language.Instance.GetMessageFromKey("ACT4_RAID_EXIT")}");
                        return;
                    }
                }

                portal.OnTraversalEvents.ForEach(e => EventHelper.Instance.RunEvent(e));
                if (portal.DestinationMapInstanceId == default)
                {
                    return;
                }

                if (ServerManager.Instance.ChannelId == 51)
                {
                    /*ScriptedInstance raid = Session.Character.Family.Act4Raid;
                    if (raid.ContributesMinimum > Session.Character.Contributi)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LOW_RAID_CONTRIBUTES"), raid.LevelMinimum), 10));
                        return;
                    }*/
                    if ((Session.Character.Faction == FactionType.Angel && portal.DestinationMapId == 131)
                        || (Session.Character.Faction == FactionType.Demon && portal.DestinationMapId == 130))
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                        return;
                    }

                    if ((portal.DestinationMapId == 130 || portal.DestinationMapId == 131)
                        && timeSpanSinceLastPortal < 15)
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                        return;
                    }
                }

#warning TODO: optimize this shit

                StaticBonusDTO vipBonus =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
                if (portal.PortalId == 583 && vipBonus == null)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"This map requires the object: " + portal.NomeOggetto + "and VIP bonus!", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side1 && portal.Side == (int)SideReputMapType.Side1)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 1", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side2 && portal.Side == (int)SideReputMapType.Side2)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 2", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side3 && portal.Side == (int)SideReputMapType.Side3)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 3", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side4 && portal.Side == (int)SideReputMapType.Side4)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 4", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side5 && portal.Side == (int)SideReputMapType.Side5)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 5", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side6 && portal.Side == (int)SideReputMapType.Side6)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 6", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side7 && portal.Side == (int)SideReputMapType.Side7)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 7", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side8 && portal.Side == (int)SideReputMapType.Side8)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 8", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side9 && portal.Side == (int)SideReputMapType.Side9)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 9", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side10 && portal.Side == (int)SideReputMapType.Side10)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 10", 10));
                    return;
                }
                if (portal.RequiredItem != 0)
                {
                    if (Session.Character.Inventory.CountItem(portal.RequiredItem) == 0)
                    {
                        Session.SendPacket(Session.Character.GenerateSay("This map requires the object: " + portal.NomeOggetto + "!", 10));
                        return;
                    }
                }
                Session.SendPacket(Session.CurrentMapInstance.GenerateRsfn());

                if (ServerManager.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType
                    != MapInstanceType.BaseMapInstance
                    && ServerManager.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType
                    != MapInstanceType.CaligorInstance
                    && ServerManager.GetMapInstance(portal.DestinationMapInstanceId).MapInstanceType
                    == MapInstanceType.BaseMapInstance)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                        Session.Character.MapX, Session.Character.MapY);
                }
                else if (portal.DestinationMapInstanceId == Session.Character.Miniland.MapInstanceId)
                {
                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else if (portal.DestinationMapId == 20000)
                {
                    ClientSession sess = ServerManager.Instance.Sessions.FirstOrDefault(s =>
                        s.Character.Miniland.MapInstanceId == portal.DestinationMapInstanceId);
                    if (sess != null)
                    {
                        ServerManager.Instance.JoinMiniland(Session, sess);
                    }
                }
                else
                {
                    if (ServerManager.Instance.ChannelId == 51)
                    {
                        short destinationX = portal.DestinationX;
                        short destinationY = portal.DestinationY;

                        if (portal.DestinationMapInstanceId == CaligorRaid.CaligorMapInstance?.MapInstanceId) /* Caligor Raid Map */
                        {
                            if (!packet.Parameter.HasValue)
                            {
                                Session.SendPacket($"qna #preq^1 {Language.Instance.GetMessageFromKey("CALIGOR_RAID_ENTER")}");
                                return;
                            }
                        }
                        else if (portal.DestinationMapId == 153) /* Unknown Land */
                        {
                            if (Session.Character.MapInstance == CaligorRaid.CaligorMapInstance && !packet.Parameter.HasValue)
                            {
                                Session.SendPacket($"qna #preq^1 {Language.Instance.GetMessageFromKey("CALIGOR_RAID_EXIT")}");
                                return;
                            }
                            else if (Session.Character.MapInstance != CaligorRaid.CaligorMapInstance)
                            {
                                if (destinationX <= 0 && destinationY <= 0)
                                {
                                    switch (Session.Character.Faction)
                                    {
                                        case FactionType.Angel:
                                            destinationX = 50;
                                            destinationY = 172;
                                            break;
                                        case FactionType.Demon:
                                            destinationX = 130;
                                            destinationY = 172;
                                            break;
                                    }
                                }
                            }
                        }

                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            portal.DestinationMapInstanceId, destinationX, destinationY);
                    }
                    else
                    {
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                    }
                }
                Session.Character.LastPortal = currentRunningSeconds;
            }
        }

        /// <summary>
        /// pulse packet
        /// </summary>
        /// <param name="pulsepacket"></param>
        public void Pulse(PulsePacket pulsepacket)
        {
            if (Session.Character.LastPulse.AddMilliseconds(80000) >= DateTime.Now
                && DateTime.Now >= Session.Character.LastPulse.AddMilliseconds(40000))
            {
                Session.Character.LastPulse = DateTime.Now;
            }
            else
            {
                Session.Disconnect();
            }

            Session.Character.MuteMessage();
            Session.Character.DeleteTimeout();
            CommunicationServiceClient.Instance.PulseAccount(Session.Account.AccountId);
        }

        /// <summary>
        /// rlPacket packet
        /// </summary>
        /// <param name="rlPacket"></param>
        public void RaidListRegister(RlPacket rlPacket)
        {
            switch (rlPacket.Type)
            {
                case 0: // Show the Raid List
                    if (Session.Character.Group?.IsLeader(Session) == true
                        && Session.Character.Group.GroupType != GroupType.Group
                        && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(1));
                    }
                    else if (Session.Character.Group != null && Session.Character.Group.GroupType != GroupType.Group
                             && Session.Character.Group.IsLeader(Session))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(2));
                    }
                    else if (Session.Character.Group != null)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(3));
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(0));
                    }

                    break;

                case 1: // Register a team
                    if (Session.Character.Group != null
                        && Session.Character.Group.GroupType != GroupType.Group
                        && Session.Character.Group.IsLeader(Session)
                        && !ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Add(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(1));
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("RAID_REGISTERED")));
                        ServerManager.Instance.Broadcast(Session,
                            $"qnaml 100 #rl {string.Format(Language.Instance.GetMessageFromKey("SEARCH_TEAM_MEMBERS"), Session.Character.Name, Session.Character.Group.Raid?.Label)}",
                            ReceiverType.AllExceptGroup);
                    }

                    break;

                case 2: // Cancel the team registration
                    if (Session.Character.Group != null
                        && Session.Character.Group.GroupType != GroupType.Group
                        && Session.Character.Group.IsLeader(Session)
                        && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Remove(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(2));
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("RAID_UNREGISTERED")));
                    }

                    break;

                case 3: // Become a team member
                    ClientSession targetSession = ServerManager.Instance.GetSessionByCharacterName(rlPacket.CharacterName);

                    if (targetSession?.Character?.Group == null)
                    {
                        return;
                    }

                    if (targetSession.Character.CharacterId == Session.Character.CharacterId)
                    {
                        return;
                    }

                    if (!targetSession.Character.Group.IsLeader(targetSession))
                    {
                        return;
                    }

                    if (!ServerManager.Instance.GroupList.Any(group => group.GroupId == targetSession.Character.Group.GroupId))
                    {
                        return;
                    }

                    targetSession.Character.GroupSentRequestCharacterIds.Add(Session.Character.CharacterId);

                    GroupJoin(new PJoinPacket
                    {
                        RequestType = GroupRequestType.Accepted,
                        CharacterId = targetSession.Character.CharacterId
                    });

                    break;
            }
        }

        /// <summary>
        /// rdPacket packet
        /// </summary>
        /// <param name="rdPacket"></param>
        public void RaidManage(RdPacket rdPacket)
        {
            Group grp;
            switch (rdPacket.Type)
            {
                // Join Raid
                case 1:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        return;
                    }

                    ClientSession target = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);
                    if (rdPacket.Parameter == null && target?.Character?.Group == null && Session.Character.Group != null && Session.Character.Group.IsLeader(Session) && Session.Character.Group?.Sessions.FirstOrDefault() == Session)
                    {
                        GroupJoin(new PJoinPacket
                        {
                            RequestType = GroupRequestType.Invited,
                            CharacterId = rdPacket.CharacterId
                        });
                    }
                    else if (Session.Character.Group == null)
                    {
                        GroupJoin(new PJoinPacket
                        {
                            RequestType = GroupRequestType.Accepted,
                            CharacterId = rdPacket.CharacterId
                        });
                    }

                    break;

                // Leave Raid
                case 2:
                    if (Session.Character.Group == null)
                    {
                        return;
                    }

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("LEFT_RAID")),
                            0));
                    if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                            Session.Character.MapX, Session.Character.MapY);
                    }

                    grp = Session.Character.Group;
                    Session.SendPacket(Session.Character.GenerateRaid(1, true));
                    Session.SendPacket(Session.Character.GenerateRaid(2, true));
                    if (grp != null)
                    {
                        grp.LeaveGroup(Session);
                        grp.Sessions.ForEach(s =>
                        {
                            s.SendPacket(grp.GenerateRdlst());
                            s.SendPacket(s.Character.GenerateRaid(0));
                        });
                    }
                    break;

                // Kick from Raid
                case 3:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        return;
                    }

                    if (Session.Character.Group?.IsLeader(Session) == true)
                    {
                        ClientSession chartokick = ServerManager.Instance.GetSessionByCharacterId(rdPacket.CharacterId);
                        if (chartokick.Character?.Group == null)
                        {
                            return;
                        }

                        chartokick.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KICK_RAID"), 0));
                        grp = chartokick.Character?.Group;
                        chartokick.SendPacket(chartokick.Character?.GenerateRaid(1, true));
                        chartokick.SendPacket(chartokick.Character?.GenerateRaid(2, true));
                        grp?.LeaveGroup(chartokick);
                        grp?.Sessions.ForEach(s =>
                        {
                            s.SendPacket(grp.GenerateRdlst());
                            s.SendPacket(s.Character.GenerateRaid(0));
                        });
                    }

                    break;

                // Disolve Raid
                case 4:
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        return;
                    }

                    if (Session.Character.Group?.IsLeader(Session) == true)
                    {
                        grp = Session.Character.Group;

                        ClientSession[] grpmembers = new ClientSession[40];
                        grp.Sessions.CopyTo(grpmembers);
                        foreach (ClientSession targetSession in grpmembers)
                        {
                            if (targetSession != null)
                            {
                                targetSession.SendPacket(targetSession.Character.GenerateRaid(1, true));
                                targetSession.SendPacket(targetSession.Character.GenerateRaid(2, true));
                                targetSession.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("RAID_DISOLVED"), 0));
                                grp.LeaveGroup(targetSession);
                            }
                        }

                        ServerManager.Instance.GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                        ServerManager.Instance.ThreadSafeGroupList.Remove(grp.GroupId);
                    }

                    break;
            }
        }
        public void QtPacket(QtPacket qtPacket)
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
        }

        /// <summary>
        /// req_info packet
        /// </summary>
        /// <param name="reqInfoPacket"></param>
        public void ReqInfo(ReqInfoPacket reqInfoPacket)
        {
            if (Session.Character != null)
            {
                if (reqInfoPacket.Type == 6)
                {
                    if (reqInfoPacket.MateVNum.HasValue)
                    {
                        Mate mate = Session.CurrentMapInstance?.Sessions?.FirstOrDefault(s => s?.Character?.Mates != null && s.Character.Mates.Any(o => o.MateTransportId == reqInfoPacket.MateVNum.Value))?
                            .Character.Mates.FirstOrDefault(m => m.MateTransportId == reqInfoPacket.MateVNum.Value);

                        if (mate != null)
                        {
                            Session.SendPacket(mate.GenerateEInfo());
                        }
                    }
                }
                else if (reqInfoPacket.Type == 5)
                {
                    NpcMonster npc = ServerManager.GetNpcMonster((short)reqInfoPacket.TargetVNum);

                    if (Session.CurrentMapInstance?.GetMonsterById(Session.Character.LastNpcMonsterId)
                        is MapMonster monster && monster.Monster?.OriginalNpcMonsterVNum == reqInfoPacket.TargetVNum)
                    {
                        npc = ServerManager.GetNpcMonster(monster.Monster.NpcMonsterVNum);
                    }

                    if (npc != null)
                    {
                        Session.SendPacket(npc.GenerateEInfo(Session.Account.Language));
                    }
                }
                else if (reqInfoPacket.Type == 12)
                {
                    if (Session.Character.Inventory != null)
                    {
                        Session.SendPacket(Session.Character.Inventory.LoadBySlotAndType((short)reqInfoPacket.TargetVNum, InventoryType.Equipment)?.GenerateReqInfo());
                    }
                }
                else
                {
                    if (ServerManager.Instance.GetSessionByCharacterId(reqInfoPacket.TargetVNum)?.Character is Character character)
                    {
                        Session.SendPacket(character.GenerateReqInfo());
                    }
                }
            }
        }

        /// <summary>
        /// rest packet
        /// </summary>
        /// <param name="sitpacket"></param>
        public void Rest(SitPacket sitpacket)
        {
            if (Session.Character.MeditationDictionary.Count != 0)
            {
                Session.Character.MeditationDictionary.Clear();
            }

            sitpacket.Users?.ForEach(u =>
            {
                if (u.UserType == 1)
                {
                    Session.Character.Rest();
                }
                else
                {
                    Session.CurrentMapInstance.Broadcast(Session.Character.Mates
                        .Find(s => s.MateTransportId == (int)u.UserId)?.GenerateRest(sitpacket.Users[0] != u));
                }
            });
        }

        /// <summary>
        /// fbPacket packet
        /// </summary>
        /// <param name="fbPacket"></param>
        public void RainbowBattleManage(FbPacket fbPacket)
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
        }

        /// <summary>
        /// revival packet
        /// </summary>
        /// <param name="revivalPacket"></param>
        public void Revive(RevivalPacket revivalPacket)
        {
            if (Session.Character.Hp > 0)
            {
                return;
            }

            switch (revivalPacket.Type)
            {
                case 0:
                    switch (Session.CurrentMapInstance.MapInstanceType)
                    {
                        case MapInstanceType.LodInstance:
                            const int saver = 1211;
                            if (Session.Character.Inventory.CountItem(saver) < 1)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_SAVER"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(saver);
                                Session.Character.Hp = (int)Session.Character.HPLoad();
                                Session.Character.Mp = (int)Session.Character.MPLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;

                        case MapInstanceType.Act4Demetra:
                        case MapInstanceType.Act4Zanarkand:
                        case MapInstanceType.Act4Orias:
                        case MapInstanceType.Act4Viserion:
                            if (Session.Character.Reputation < Session.Character.Level * 10)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_REPUT"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                //Session.Character.GetReputation(Session.Character.Level * -10);
                                Session.Character.Hp = (int)Session.Character.HPLoad();
                                Session.Character.Mp = (int)Session.Character.MPLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;

                        case MapInstanceType.CaligorInstance:
                            Session.Character.Hp = (int)Session.Character.HPLoad();
                            Session.Character.Mp = (int)Session.Character.MPLoad();
                            short x = 0;
                            short y = 0;
                            switch (Session.Character.Faction)
                            {
                                case FactionType.Angel:
                                    x = 50;
                                    y = 172;
                                    break;
                                case FactionType.Demon:
                                    x = 130;
                                    y = 172;
                                    break;
                            }
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.MapInstance.MapInstanceId, x, y);
                            Session.Character.AddBuff(new Buff(169, Session.Character.Level), Session.Character.BattleEntity);
                            break;

                        default:
                            const int seed = 1012;
                            if (Session.Character.Inventory.CountItem(seed) < 10 && Session.Character.Level > 20)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_POWER_SEED"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_SEED_SAY"), 0));
                            }
                            else
                            {
                                if (Session.Character.Level > 20)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("SEED_USED"), 10), 10));
                                    Session.Character.Inventory.RemoveItemAmount(seed, 10);
                                    Session.Character.Hp = (int)(Session.Character.HPLoad() / 2);
                                    Session.Character.Mp = (int)(Session.Character.MPLoad() / 2);
                                    Session.Character.AddBuff(new Buff(44, Session.Character.Level), Session.Character.BattleEntity);
                                }
                                else
                                {
                                    Session.Character.Hp = (int)Session.Character.HPLoad();
                                    Session.Character.Mp = (int)Session.Character.MPLoad();
                                }

                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }
                            break;
                    }

                    break;

                case 1:
                    ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        if (Session.Character.Level > 20)
                        {
                            Session.Character.AddBuff(new Buff(44, Session.Character.Level), Session.Character.BattleEntity);
                        }
                    }
                    break;

                case 2:
                    if ((Session.CurrentMapInstance == ServerManager.Instance.ArenaInstance || Session.CurrentMapInstance == ServerManager.Instance.FamilyArenaInstance) && 
                        Session.Character.Gold >= 100)
                    {
                        Session.Character.Hp = (int)Session.Character.HPLoad();
                        Session.Character.Mp = (int)Session.Character.MPLoad();
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                        Session.SendPacket(Session.Character.GenerateStat());
                        Session.Character.Gold -= 100;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.Character.LastPVPRevive = DateTime.Now;
                        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(observer =>
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PVP_ACTIVE"), 10)));
                    }
                    else
                    {
                        ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    }

                    break;
            }
            Session.Character.BattleEntity.SendBuffsPacket();
        }

        /// <summary>
        /// say packet
        /// </summary>
        /// <param name="sayPacket"></param>
        public void Say(SayPacket sayPacket)
        {
            if (string.IsNullOrEmpty(sayPacket.Message))
            {
                return;
            }

            var penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            var message = sayPacket.Message;
            if (Session.Character.IsMuted() && penalty != null)
            {
                if (Session.Character.Gender == GenderType.Female)
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
                else
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        var member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
            }
            else
            {
                LogHelper.Instance.InsertChatLog(ChatType.General, Session.Character.CharacterId, message, Session.IpAddress);

                byte type = CharacterHelper.AuthorityChatColor(Session.Character.Authority);

                ConcurrentBag<ArenaTeamMember> member = null;
                lock (ServerManager.Instance.ArenaTeams)
                {
                    member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                }
                if (Session.Character.Authority >= AuthorityType.GS)
                {
                    type = CharacterHelper.AuthorityChatColor(Session.Character.Authority);
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), 1), ReceiverType.AllExceptMe);
                    }
                    message = $"[{Session.Character.Authority} {Session.Character.Name}]: " + message;
                }

                if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                {
                    ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                    member.Where(s => s.ArenaTeamType == member2?.ArenaTeamType && s != member2).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), type, Session.Account.Authority >= AuthorityType.GS)));
                }
                else if (ServerManager.Instance.ChannelId == 51 && Session.Account.Authority < AuthorityType.TMOD)
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type, false), ReceiverType.AllExceptMeAct4);
                }
                else
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type, Session.Character.Authority >= AuthorityType.GS), ReceiverType.AllExceptMe);
                }
            }
            
        }

        /// <summary>
        /// pst packet
        /// </summary>
        /// <param name="pstPacket"></param>
        public void SendMail(PstPacket pstPacket)
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
        }

        /// <summary>
        /// qset packet
        /// </summary>
        /// <param name="qSetPacket"></param>
        public void SetQuicklist(QSetPacket qSetPacket)
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

                    break;

                case 3:

                    // Remove from Quicklist
                    Session.Character.QuicklistEntries.RemoveAll(n =>
                        n.Q1 == q1 && n.Q2 == q2
                        && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    Session.SendPacket($"qset {q1} {q2} 7.7.-1.0");
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// game_start packet
        /// </summary>
        /// <param name="gameStartPacket"></param>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void StartGame(GameStartPacket gameStartPacket)
        {
            #region System Code Lock
            /*if (Session.Character.LockCode != null)
            {
                #region Lock Code
                Session.Character.HeroChatBlocked = true;
                Session.Character.ExchangeBlocked = true;
                Session.Character.WhisperBlocked = true;
                Session.Character.Invisible = true;
                Session.Character.NoAttack = true;
                Session.Character.NoMove = true;
                Session.Character.VerifiedLock = false;
                #endregion
                Session.SendPacket(Session.Character.GenerateSay($"Your account is locked. Please, use $Unlock command.", 12));
            }
            else
            {
                #region Unlock Code
                Session.Character.HeroChatBlocked = false;
                Session.Character.ExchangeBlocked = false;
                Session.Character.WhisperBlocked = false;
                Session.Character.Invisible = false;
                Session.Character.NoAttack = false;
                Session.Character.NoMove = false;
                Session.Character.VerifiedLock = true;
                #endregion
                Session.SendPacket(Session.Character.GenerateSay($"Your account dont have a lock. If you need more security, use $SetLock and a code.", 12));
            }*/
            #endregion
            if (Session?.Character == null || Session.IsOnMap || !Session.HasSelectedCharacter)
            {
                // character should have been selected in SelectCharacter
                return;
            }

            bool shouldRespawn = false;

            if (Session.Character.MapInstance?.Map?.MapTypes != null)
            {
                if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4)
                    && ServerManager.Instance.ChannelId != 51)
                {
                    if (ServerManager.Instance.IsAct4Online())
                    {
                        Session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP, ServerManager.Instance.Configuration.Act4Port, 2);
                        return;
                    }

                    shouldRespawn = true;
                }
            }

            Session.CurrentMapInstance = Session.Character.MapInstance;

            if (ServerManager.Instance.Configuration.SceneOnCreate
                && Session.Character.GeneralLogs.CountLinq(s => s.LogType == "Connection") < 2)
            {
                Session.SendPacket("scene 40");
            }

            Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WELCOME_SERVER"), ServerManager.Instance.ServerGroup), 10));
            Session.Character.hasVerifiedSecondPassword = true;
            Session.SendPacket(UserInterfaceHelper.GenerateInfo("Please enter your pin with $Pw. If you don't have a pin, use $SetPw"));
            Session.Character.PinAsk = Observable.Interval(TimeSpan.FromSeconds(15)).Subscribe(x =>
            {
                if (!Session.Character.hasVerifiedSecondPassword)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo("Please enter your pin with $Pw. If you have no pin, use $SetPw"));
                }
            });

            Session.Character.LoadSpeed();
            Session.Character.LoadSkills();
            Session.SendPacket(Session.Character.GenerateTit());
            Session.SendPacket(Session.Character.GenerateSpPoint());
            Session.SendPacket("rsfi 1 1 0 9 0 9");

            Session.Character.Quests?.Where(q => q?.Quest?.TargetMap != null).ToList()
                .ForEach(qst => Session.SendPacket(qst.Quest.TargetPacket()));
            
            if (Session.Character.Hp <= 0 && (!Session.Character.IsSeal || ServerManager.Instance.ChannelId != 51))
            {
                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
            }
            else
            {
                if (shouldRespawn)
                {
                    RespawnMapTypeDTO resp = Session.Character.Respawn;
                    short x = (short)(resp.DefaultX + ServerManager.RandomNumber(-3, 3));
                    short y = (short)(resp.DefaultY + ServerManager.RandomNumber(-3, 3));
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, resp.DefaultMapId, x, y);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
                }
            }

            Session.SendPacket(Session.Character.GenerateSki());
            Session.SendPacket(
                $"fd {Session.Character.Reputation} 0 {(int)Session.Character.Dignity} {Math.Abs(Session.Character.GetDignityIco())}");
            Session.SendPacket(Session.Character.GenerateFd());
            Session.SendPacket("rage 0 250000");
            Session.SendPacket("rank_cool 0 0 18000");
            ItemInstance specialistInstance = Session.Character.Inventory.LoadBySlotAndType(8, InventoryType.Wear);

            StaticBonusDTO medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

            if (medal != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOGIN_MEDAL"), 12));
            }

            if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
            {
                Session.SendPacket("ib 1278 1");
            }

            if (Session.Character.MapInstance?.Map?.MapTypes?.Any(m => m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness) == true)
            {
                Session.SendPacket("bc 0 0 0");
            }

            if (specialistInstance != null)
            {
                Session.SendPacket(Session.Character.GenerateSpPoint());
            }

            Session.SendPacket("scr 0 0 0 0 0 0");
            for (int i = 0; i < 10; i++)
            {
                Session.SendPacket($"bn {i} {Language.Instance.GetMessageFromKey($"BN{i}")}");
            }

            Session.SendPacket(Session.Character.GenerateExts());
            Session.SendPacket(Session.Character.GenerateMlinfo());
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());

            Session.SendPacket(Session.Character.GeneratePinit());
            Session.SendPackets(Session.Character.GeneratePst());
            Session.SendPacket(Session.Character.GenerateTitle());
            Session.SendPacket("zzim");
            Session.SendPacket(
                $"twk 1 {Session.Character.CharacterId} {Session.Account.Name} {Session.Character.Name} shtmxpdlfeoqkr");

            long? familyId = DAOFactory.FamilyCharacterDAO.LoadByCharacterId(Session.Character.CharacterId)?.FamilyId;
            if (familyId.HasValue)
            {
                Session.Character.Family = ServerManager.Instance.FamilyList[familyId.Value];
            }

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                if (Session.Character.Faction != (FactionType)Session.Character.Family.FamilyFaction)
                {
                    Session.Character.Faction
                        = (FactionType)Session.Character.Family.FamilyFaction;
                }

                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());

                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo("--- Family Message ---\n" +
                                                         Session.Character.Family.FamilyMessage));
                }
            }
            Session.SendPacket(Session.Character.GetSqst());
            Session.SendPacket("act6");
            Session.SendPacket(Session.Character.GenerateFaction());
            Session.SendPackets(Session.Character.GenerateScP());
            Session.SendPackets(Session.Character.GenerateScN());
#pragma warning disable 618
            Session.Character.GenerateStartupInventory();
#pragma warning restore 618

            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPackets(Session.Character.GenerateQuicklist());

            string clinit = "clinit";
            string flinit = "flinit";
            string kdlinit = "kdlinit";
            foreach (CharacterDTO character in ServerManager.Instance.TopComplimented)
            {
                clinit +=
                    $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Compliment}|{character.Name}";
            }

            foreach (CharacterDTO character in ServerManager.Instance.TopReputation)
            {
                flinit +=
                    $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Reputation}|{character.Name}";
            }

            foreach (CharacterDTO character in ServerManager.Instance.TopPoints)
            {
                kdlinit +=
                    $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Act4Points}|{character.Name}";
            }

            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateTitleInfo());
            Session.SendPacket(Session.Character.GenerateFinit());
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(clinit);
            Session.SendPacket(flinit);
            Session.SendPacket(kdlinit);

            Session.Character.LastPVPRevive = DateTime.Now;

            IEnumerable<PenaltyLogDTO> warning = DAOFactory.PenaltyLogDAO.LoadByAccount(Session.Character.AccountId)
                .Where(p => p.Penalty == PenaltyType.Warning);
            if (warning.Any())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                    string.Format(Language.Instance.GetMessageFromKey("WARNING_INFO"), warning.Count())));
            }

            // finfo - friends info
            Session.Character.LoadMail();
            Session.Character.LoadSentMail();
            Session.Character.DeleteTimeout();

            if (Session.Character.Authority == AuthorityType.BitchNiggerFaggot)
            {
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = null,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message =
                        $"User {Session.Character.Name} with rank BitchNiggerFaggot has logged in, don't trust *it*!",
                    Type = MessageType.Shout
                });
            }

            /*QuestModel quest = ServerManager.Instance.QuestList.Where(s => s.QuestGiver.Type == QuestGiverType.InitialQuest).FirstOrDefault();
            if(quest != null)
           {
                quest = quest.Copy();
          
                int current = 0;
                int max = 0;

                if (quest.KillObjectives != null)
                {
                    max = quest.KillObjectives[0].GoalAmount;
                    current = quest.KillObjectives[0].CurrentAmount;
                }

                if(quest.WalkObjective != null)
                {
                    Session.SendPacket($"target {quest.WalkObjective.MapX} {quest.WalkObjective.MapY} {quest.WalkObjective.MapId} {quest.QuestDataVNum}");
                }

                //Quest Packet Definition: qstlist {Unknown}.{QuestVNUM}.{QuestVNUM}.{GoalType}.{Current}.{Goal}.{Finished}.{GoalType}.{Current}.{Goal}.{Finished}.{GoalType}.{Current}.{Goal}.{Finished}.{ShowDialog}
                //Same for qsti
                Session.SendPacket($"qstlist 5.{quest.QuestDataVNum}.{quest.QuestDataVNum}.{quest.QuestGoalType}.{current}.{max}.0.0.0.0.0.0.0.0.0.1");

            }
            */
            if (Session.Character.Quests.Any())
            {
                Session.SendPacket(Session.Character.GenerateQuestsPacket());
            }

            if (Session.Character.IsSeal)
            {
                if (ServerManager.Instance.ChannelId == 51)
                {
                    Session.Character.SetSeal();
                }
                else
                {
                    Session.Character.IsSeal = false;
                }
            }
        }

        /// <summary>
        /// walk packet
        /// </summary>
        /// <param name="walkPacket"></param>
        public void Walk(WalkPacket walkPacket)
        {
            if (Session.Character.CanMove())
            {
                if (Session.Character.MeditationDictionary.Count != 0)
                {
                    Session.Character.MeditationDictionary.Clear();
                }

                double currentRunningSeconds =
                    (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                int distance =
                    Map.GetDistance(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                        new MapCell { X = walkPacket.XCoordinate, Y = walkPacket.YCoordinate });

                if (Session.HasCurrentMapInstance
                    && !Session.CurrentMapInstance.Map.IsBlockedZone(walkPacket.XCoordinate, walkPacket.YCoordinate)
                    && !Session.Character.IsChangingMapInstance && !Session.Character.HasShopOpened)
                {
                    Session.Character.PyjamaDead = false;
                    if (!Session.Character.InvisibleGm)
                    {
                        Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.Move(UserType.Player,
                            Session.Character.CharacterId, walkPacket.XCoordinate, walkPacket.YCoordinate,
                            Session.Character.Speed));
                    }
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.WalkDisposable?.Dispose();
                    walk();
                    int interval = 100 - Session.Character.Speed * 5 + 100 > 0 ? 100 - Session.Character.Speed * 5 + 100 : 0;
                    Session.Character.WalkDisposable = Observable.Interval(TimeSpan.FromMilliseconds(interval)).Subscribe(obs =>
                    {
                        walk();
                    });
                    void walk()
                    {
                        MapCell nextCell = Map.GetNextStep(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY }, new MapCell { X = walkPacket.XCoordinate, Y = walkPacket.YCoordinate }, 1);

                        Session.Character.GetDir(Session.Character.PositionX, Session.Character.PositionY, nextCell.X, nextCell.Y);

                        if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                        {
                            Session.Character.MapX = nextCell.X;
                            Session.Character.MapY = nextCell.Y;
                        }

                        Session.Character.PositionX = nextCell.X;
                        Session.Character.PositionY = nextCell.Y;

                        Session.Character.LastMove = DateTime.Now;

                        if (Session.Character.IsVehicled)
                        {
                            Session.Character.Mates.Where(s => s.IsTeamMember || s.IsTemporalMate).ToList().ForEach(s =>
                            {
                                s.PositionX = Session.Character.PositionX;
                                s.PositionY = Session.Character.PositionY;
                            });
                        }

                        if (Session.Character.LastMonsterAggro.AddSeconds(5) > DateTime.Now)
                        {
                            Session.Character.UpdateBushFire();
                        }
                        
                        Session.CurrentMapInstance?.OnAreaEntryEvents
                            ?.Where(s => s.InZone(Session.Character.PositionX, Session.Character.PositionY)).ToList()
                            .ForEach(e => e.Events.ForEach(evt => EventHelper.Instance.RunEvent(evt)));
                        Session.CurrentMapInstance?.OnAreaEntryEvents?.RemoveAll(s =>
                            s.InZone(Session.Character.PositionX, Session.Character.PositionY));

                        Session.CurrentMapInstance?.OnMoveOnMapEvents?.ForEach(e => EventHelper.Instance.RunEvent(e));
                        Session.CurrentMapInstance?.OnMoveOnMapEvents?.RemoveAll(s => s != null);

                        if (Session.Character.PositionX == walkPacket.XCoordinate && Session.Character.PositionY == walkPacket.YCoordinate)
                        {
                            Session.Character.WalkDisposable?.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// / packet
        /// </summary>
        /// <param name="whisperPacket"></param>
        public void Whisper(WhisperPacket whisperPacket)
        {
            try
            {
                // TODO: Implement WhisperSupport
                if (string.IsNullOrEmpty(whisperPacket.Message))
                {
                    return;
                }

                string characterName =
                    whisperPacket.Message.Split(' ')[
                            whisperPacket.Message.StartsWith("GM ", StringComparison.CurrentCulture) ? 1 : 0].Replace("[Angel]", "").Replace("[Demon]", "");

                Enum.GetNames(typeof(AuthorityType)).ToList().ForEach(at => characterName = characterName.Replace($"[{at}]", ""));

                string message = "";
                string[] packetsplit = whisperPacket.Message.Split(' ');
                for (int i = packetsplit[0] == "GM" ? 2 : 1; i < packetsplit.Length; i++)
                {
                    message += packetsplit[i] + " ";
                }

                if (message.Length > 60)
                {
                    message = message.Substring(0, 60);
                }

                message = message.Trim();
                Session.SendPacket(Session.Character.GenerateSpk(message, 5));
                CharacterDTO receiver = DAOFactory.CharacterDAO.LoadByName(characterName);
                int? sentChannelId = null;
                if (receiver != null)
                {
                    if (receiver.CharacterId == Session.Character.CharacterId)
                    {
                        return;
                    }

                    if (Session.Character.IsBlockedByCharacter(receiver.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                        return;
                    }

                    ClientSession receiverSession =
                        ServerManager.Instance.GetSessionByCharacterId(receiver.CharacterId);
                    if (receiverSession?.CurrentMapInstance?.Map.MapId == Session.CurrentMapInstance?.Map.MapId
                        && Session.Account.Authority >= AuthorityType.TMOD)
                    {
                        receiverSession?.SendPacket(Session.Character.GenerateSay(message, 2));
                    }

                    sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = receiver.CharacterId,
                        SourceCharacterId = Session.Character.CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = Session.Character.Authority >= AuthorityType.GS
                            ? Session.Character.GenerateSay(
                                $"(whisper)(From {Session.Character.Authority} {Session.Character.Name}):{message}", 11)
                            : Session.Character.GenerateSpk(message,
                                Session.Account.Authority >= AuthorityType.TMOD ? 15 : 5),
                        Type = Enum.GetNames(typeof(AuthorityType)).Any(a => 
                        {
                            if (a.Equals(packetsplit[0]))
                            {
                                Enum.TryParse(a, out AuthorityType auth);
                                if (auth  >= AuthorityType.TMOD)
                                {
                                    return true;
                                }
                            }
                            return false;
                        })
                        || Session.Account.Authority >= AuthorityType.TMOD
                        ? MessageType.WhisperGM : MessageType.Whisper
                    });
                }

                if (sentChannelId == null)
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED")));
                }
                else
                {
                    LogHelper.Instance.InsertChatLog(ChatType.Whisper, Session.Character.CharacterId, message, Session.IpAddress);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Whisper failed.", e);
            }
        }

        /// <summary>
        /// Anti-Cheat Heartbeat Packet
        /// </summary>
        /// <param name="ntcpAcPacket"></param>
        public void NtcpAcPacket(NtcpAcPacket ntcpAcPacket)
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
        }

        #endregion
    }
}