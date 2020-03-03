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

using GloomyTale.Core;
using GloomyTale.Core.Extensions;
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.GameObject.Helpers;
using System;
using System.Linq;
using GloomyTale.GameObject.Networking;
using System.Collections.Generic;
using System.Threading.Tasks;
using GloomyTale.GameObject.Items.Instance;

namespace GloomyTale.GameObject
{
    public class SpecialItem : Item
    {
        #region Instantiation

        public SpecialItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0, string[] packetsplit = null)
        {
            if (session.Character.IsVehicled && Effect != 1000)
            {
                if (VNum == 5119 || VNum == 9071) // Speed Booster
                {
                    if (!session.Character.Buff.Any(s => s.Card.CardId == 336))
                    {
                        session.Character.VehicleItem.BCards.ForEach(s => s.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                        session.CurrentMapInstance.Broadcast($"eff 1 {session.Character.VisualId} 885");
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                }
                else
                {
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                }
                return;
            }

            if (VNum == 1400)
            {
                Mate equipedMate = session.Character.Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);

                if (equipedMate != null)
                {
                    equipedMate.RemoveTeamMember();
                    session.Character.MapInstance.Broadcast(equipedMate.GenerateOut());
                }

                Mate mate = new Mate(session.Character, ServerManager.GetNpcMonster(317), 24, MateType.Partner);
                session.Character.Mates?.Add(mate);
                mate.RefreshStats();
                session.SendPacket($"ctl 2 {mate.PetId} 3");
                session.Character.MapInstance.Broadcast(mate.GenerateIn());
                session.SendPacket(UserInterfaceHelper.GeneratePClear());
                session.SendPackets(session.Character.GenerateScP());
                session.SendPackets(session.Character.GenerateScN());
                session.SendPacket(session.Character.GeneratePinit());
                session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                    .OrderBy(s => s.MateType)
                    .Select(s => s.GeneratePst()));
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
            }

            if (VNum == 1419)
            {
                Mate equipedMate = session.Character.Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);

                if (equipedMate != null)
                {
                    equipedMate.RemoveTeamMember();
                    session.Character.MapInstance.Broadcast(equipedMate.GenerateOut());
                }

                Mate mate = new Mate(session.Character, ServerManager.GetNpcMonster(318), 31, MateType.Partner);
                session.Character.Mates?.Add(mate);
                mate.RefreshStats();
                session.SendPacket($"ctl 2 {mate.PetId} 3");
                session.Character.MapInstance.Broadcast(mate.GenerateIn());
                session.SendPacket(UserInterfaceHelper.GeneratePClear());
                session.SendPackets(session.Character.GenerateScP());
                session.SendPackets(session.Character.GenerateScN());
                session.SendPacket(session.Character.GeneratePinit());
                session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                    .OrderBy(s => s.MateType)
                    .Select(s => s.GeneratePst()));
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
            }

            if (VNum == 1431)
            {
                Mate equipedMate = session.Character.Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);

                if (equipedMate != null)
                {
                    equipedMate.RemoveTeamMember();
                    session.Character.MapInstance.Broadcast(equipedMate.GenerateOut());
                }

                Mate mate = new Mate(session.Character, ServerManager.GetNpcMonster(319), 48, MateType.Partner);
                session.Character.Mates?.Add(mate);
                mate.RefreshStats();
                session.SendPacket($"ctl 2 {mate.PetId} 3");
                session.Character.MapInstance.Broadcast(mate.GenerateIn());
                session.SendPacket(UserInterfaceHelper.GeneratePClear());
                session.SendPackets(session.Character.GenerateScP());
                session.SendPackets(session.Character.GenerateScN());
                session.SendPacket(session.Character.GeneratePinit());
                session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                    .OrderBy(s => s.MateType)
                    .Select(s => s.GeneratePst()));
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
            }

            if (VNum == 5511)
            {
                session.Character.GeneralLogs.Where(s => s.LogType == "InstanceEntry" && (short.Parse(s.LogData) == 16 || short.Parse(s.LogData) == 17) && s.Timestamp.Date == DateTime.Today).ToList().ForEach(s =>
                {
                    s.LogType = "NulledInstanceEntry";
                    DAOFactory.Instance.GeneralLogDAO.InsertOrUpdate(ref s);
                });
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                return;
            }

            if (session.CurrentMapInstance?.MapInstanceType != MapInstanceType.TalentArenaMapInstance
            && (VNum == 5936 || VNum == 5937 || VNum == 5938 || VNum == 5939 || VNum == 5940 || VNum == 5942 || VNum == 5943 || VNum == 5944 || VNum == 5945 || VNum == 5946))
            {
                return;
            }
            if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance
            && VNum != 5936 && VNum != 5937 && VNum != 5938 && VNum != 5939 && VNum != 5940 && VNum != 5942 && VNum != 5943 && VNum != 5944 && VNum != 5945 && VNum != 5946)
            {
                return;
            }

            if (BCards.Count > 0 && Effect != 1000)
            {
                if (BCards.Any(s => s.Type == (byte)BCardType.CardType.Buff && s.SubType == 1 && new Buff((short)s.SecondData, session.Character.Level, false).Card.BCards.Any(newbuff => session.Character.Buff.GetAllItems().Any(b => b.Card.BCards.Any(buff => 
                    buff.CardId != newbuff.CardId 
                 && ((buff.Type == 33 && buff.SubType == 5 && (newbuff.Type == 33 || newbuff.Type == 58)) || (newbuff.Type == 33 && newbuff.SubType == 5 && (buff.Type == 33 || buff.Type == 58))
                 || (buff.Type == 33 && (buff.SubType == 1 || buff.SubType == 3) && (newbuff.Type == 58 && (newbuff.SubType == 1))) || (buff.Type == 33 && (buff.SubType == 2 || buff.SubType == 4) && (newbuff.Type == 58 && (newbuff.SubType == 3)))
                 || (newbuff.Type == 33 && (newbuff.SubType == 1 || newbuff.SubType == 3) && (buff.Type == 58 && (buff.SubType == 1))) || (newbuff.Type == 33 && (newbuff.SubType == 2 || newbuff.SubType == 4) && (buff.Type == 58 && (buff.SubType == 3)))
                 || (buff.Type == 33 && newbuff.Type == 33 && buff.SubType == newbuff.SubType) || (buff.Type == 58 && newbuff.Type == 58 && buff.SubType == newbuff.SubType)))))))
                {
                    return;
                }
                BCards.ForEach(c => c.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                return;
            }

            switch (Effect)
            {
                // Seal Mini-Game
                case 1717:
                    switch (EffectValue)
                    {
                        case 1:// King Ratufu Mini Game
                               // Not Created for moment .
                            break;
                        case 2: // Sheep Mini Game
                            /*session.SendPacket($"say 1 {session.Character.CharacterId} 10 Registration starts in 5 seconds.");
                            EventHelper.Instance.GenerateEvent(EventType., false);
                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);*/
                            break;
                        case 3: // Meteor Mini Game
                            session.SendPacket($"say 1 {session.Character.VisualId} 10 Registration starts in 5 seconds.");
                            EventHelper.GenerateEvent(EventType.METEORITEGAME);
                            //session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            break;
                    }
                    break;

                ////btk register
                case 1227:
                    if (ServerManager.Instance.CanRegisterRainbowBattle == true)
                    {
                        if (session.Character.Family != null)
                        {
                            if (session.Character.Family.FamilyCharacters.Where(s => s.CharacterId == session.Character.VisualId).First().Authority == FamilyAuthority.Head || session.Character.Family.FamilyCharacters.Where(s => s.CharacterId == session.Character.VisualId).First().Authority == FamilyAuthority.Familykeeper)
                            {
                                if (ServerManager.Instance.IsCharacterMemberOfGroup(session.Character.VisualId))
                                {
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAINBOWBATTLE_OPEN_GROUP"), 12));
                                    return;
                                }
                                Group group = new Group
                                {
                                    GroupType = GroupType.BigTeam
                                };
                                group.JoinGroup(session.Character.VisualId);
                                ServerManager.Instance.AddGroup(group);
                                session.SendPacket(session.Character.GenerateFbt(2));
                                session.SendPacket(session.Character.GenerateFbt(0));
                                session.SendPacket(session.Character.GenerateFbt(1));
                                session.SendPacket(group.GenerateFblst());
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOWBATTLE_LEADER"), session.Character.Name), 0));
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RAINBOWBATTLE_LEADER"), session.Character.Name), 10));

                                ServerManager.Instance.RainbowBattleMembers.Add(new RainbowBattleMember
                                {
                                    RainbowBattleType = EventType.RAINBOWBATTLE,
                                    Session = session,
                                    GroupId = group.GroupId,
                                });

                                ItemInstance RainbowBattleSeal = session.Character.Inventory.LoadBySlotAndType(inv.Slot, InventoryType.Main);
                                session.Character.Inventory.RemoveItemFromInventory(RainbowBattleSeal.Id);
                            }
                        }
                    }
                    break;
                // Honour Medals
                case 69:
                    //session.Character.Reputation += ReputPrice;
                    session.SendPacket(session.Character.GenerateFd());
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_INCREASE"), ReputPrice), 11));
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // TimeSpace Stones
                case 140:
                    if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        if (ServerManager.Instance.TimeSpaces.FirstOrDefault(s => s.Id == EffectValue) is ScriptedInstance timeSpace)
                        {
                            session.Character.EnterInstance(timeSpace);
                        }
                    }
                    break;

                // SP Potions
                case 150:
                case 151:
                    {
                        if (session.Character.SpAdditionPoint >= 1000000)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_POINTS_FULL"), 0));
                            break;
                        }

                        session.Character.SpAdditionPoint += EffectValue;

                        if (session.Character.SpAdditionPoint > 1000000)
                        {
                            session.Character.SpAdditionPoint = 1000000;
                        }

                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SP_POINTSADDED"), EffectValue), 0));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSpPoint());
                    }
                    break;

                // Specialist Medal
                case 204:
                    {
                        if (session.Character.SpPoint >= 10000
                            && session.Character.SpAdditionPoint >= 1000000)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_POINTS_FULL"), 0));
                            break;
                        }

                        session.Character.SpPoint += EffectValue;

                        if (session.Character.SpPoint > 10000)
                        {
                            session.Character.SpPoint = 10000;
                        }

                        session.Character.SpAdditionPoint += EffectValue * 3;

                        if (session.Character.SpAdditionPoint > 1000000)
                        {
                            session.Character.SpAdditionPoint = 1000000;
                        }

                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SP_POINTSADDEDBOTH"), EffectValue, EffectValue * 3), 0));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSpPoint());
                    }
                    break;

                // Raid Seals
                case 301:
                    ItemInstance raidSeal = session.Character.Inventory.LoadBySlotAndType(inv.Slot, InventoryType.Main);

                    if (raidSeal != null)
                    {
                        ScriptedInstance raid = ServerManager.Instance.Raids.FirstOrDefault(s => s.Id == raidSeal.Item.EffectValue)?.Copy();

                        if (raid != null)
                        {
                            if (ServerManager.Instance.ChannelId == 51 || session.CurrentMapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                            {
                                return;
                            }

                            if (ServerManager.Instance.IsCharacterMemberOfGroup(session.Character.VisualId))
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_OPEN_GROUP"), 12));
                                return;
                            }

                            var entries = raid.DailyEntries - session.Character.GeneralLogs.CountLinq(s => s.LogType == "InstanceEntry" && short.Parse(s.LogData) == raid.Id && s.Timestamp.Date == DateTime.Today);

                            if (raid.DailyEntries > 0 && entries <= 0)
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANCE_NO_MORE_ENTRIES"), 0));
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("INSTANCE_NO_MORE_ENTRIES"), 10));
                                return;
                            }

                            if (session.Character.Level > raid.LevelMaximum || session.Character.Level < raid.LevelMinimum)
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));
                                return;
                            }

                            Group group = new Group
                            {
                                GroupType = raid.IsGiantTeam ? GroupType.GiantTeam : GroupType.BigTeam,
                                Raid = raid
                            };

                            if (group.JoinGroup(session))
                            {
                                ServerManager.Instance.AddGroup(group);
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAID_LEADER"), session.Character.Name), 0));
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RAID_LEADER"), session.Character.Name), 10));
                                session.SendPacket(session.Character.GenerateRaid(2));
                                session.SendPacket(session.Character.GenerateRaid(0));
                                session.SendPacket(session.Character.GenerateRaid(1));
                                session.SendPacket(group.GenerateRdlst());
                                session.Character.Inventory.RemoveItemFromInventory(raidSeal.Id);
                            }
                        }
                    }
                    break;

                // Partner Suits/Skins
                case 305:
                    Mate mate = session.Character.Mates.Find(s => s.MateTransportId == int.Parse(packetsplit[3]));
                    if (mate != null && EffectValue == mate.NpcMonsterVNum && mate.Skin == 0)
                    {
                        mate.Skin = Morph;
                        session.SendPacket(mate.GenerateCMode(mate.Skin));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                //suction Funnel (Quest Item / QuestId = 1724)
                case 400:
                    if (session.Character == null || session.Character.Quests.All(q => q.QuestId != 1724))
                    {
                        break;
                    }
                    if (session.Character.Quests.FirstOrDefault(q => q.QuestId == 1724) is CharacterQuest kenkoQuest)
                    {
                        MapMonster kenko = session.CurrentMapInstance?.Monsters.FirstOrDefault(m => m.MapMonsterId == session.Character.LastNpcMonsterId && m.MonsterVNum > 144 && m.MonsterVNum < 154);
                        if (kenko == null || session.Character.Inventory.CountItem(1174) > 0)
                        {
                            break;
                        }
                        if (session.Character.LastFunnelUse.AddSeconds(30) <= DateTime.Now)
                        {
                            if (kenko.CurrentHp / kenko.MaxHp * 100 < 30)
                            {
                                if (ServerManager.RandomNumber() < 30)
                                {
                                    kenko.SetDeathStatement();
                                    session.Character.MapInstance.Broadcast(StaticPacketHelper.Out(VisualType.Monster, kenko.MapMonsterId));
                                    session.Character.Inventory.AddNewToInventory(1174); // Kenko Bead
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KENKO_CATCHED"), 0));
                                }
                                else { session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_CATCH_FAIL"), 0)); }
                                session.Character.LastFunnelUse = DateTime.Now;
                            }
                            else { session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HP_TOO_HIGH"), 0)); }
                        }
                    }
                    break;

                // Fairy Booster
                case 250:
                    if (!session.Character.Buff.ContainsKey(131))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 131 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(VisualType.Player, session.Character.VisualId, 3014), session.Character.PositionX, session.Character.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Rainbow Pearl/Magic Eraser
                case 666:
                    if (EffectValue == 1 && byte.TryParse(packetsplit[9], out byte islot))
                    {
                        var wearInstance = session.Character.Inventory.LoadBySlotAndType<WearableInstance>(islot, InventoryType.Equipment);

                        if (wearInstance != null && (wearInstance.Item.ItemType == ItemType.Weapon || wearInstance.Item.ItemType == ItemType.Armor) && wearInstance.ShellEffects.Count != 0 && !wearInstance.Item.IsHeroic)
                        {
                            wearInstance.ShellEffects.Clear();
                            wearInstance.ShellRarity = null;
                            DAOFactory.Instance.ShellEffectDAO.DeleteByEquipmentSerialId(wearInstance.EquipmentSerialId);
                            if (wearInstance.EquipmentSerialId == Guid.Empty)
                            {
                                wearInstance.EquipmentSerialId = Guid.NewGuid();
                            }
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_DELETE"), 0));
                        }
                    }
                    else
                    {
                        session.SendPacket("guri 18 0");
                    }
                    break;

                // Atk/Def/HP/Exp potions
                case 6600:
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Ancelloan's Blessing
                case 208:
                    if (!session.Character.Buff.ContainsKey(121))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 121 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Guardian Angel's Blessing
                case 210:
                    if (!session.Character.Buff.ContainsKey(122))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 122 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                case 2081:
                    if (!session.Character.Buff.ContainsKey(146))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 146 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Divorce letter
                case 6969:
                    if (session.Character.Group != null)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ALLOWED_IN_GROUP"), 0));
                        return;
                    }
                    CharacterRelationDTO rel = session.Character.CharacterRelations.FirstOrDefault(s => s.RelationType == CharacterRelationType.Spouse);
                    if (rel != null)
                    {
                        session.Character.DeleteRelation(rel.CharacterId == session.Character.VisualId ? rel.RelatedCharacterId : rel.CharacterId, CharacterRelationType.Spouse);
                        session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DIVORCED")));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // Cupid's arrow
                case 34:
                    if (packetsplit != null && packetsplit.Length > 3)
                    {
                        if (long.TryParse(packetsplit[3], out long characterId))
                        {
                            if (session.Character.VisualId == characterId)
                            {
                                return;
                            }
                            if (session.Character.CharacterRelations.Any(s => s.RelationType == CharacterRelationType.Spouse))
                            {
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_MARRIED")}");
                                return;
                            }
                            if (session.Character.Group != null)
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ALLOWED_IN_GROUP"), 0));
                                return;
                            }
                            if (!session.Character.IsFriendOfCharacter(characterId))
                            {
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("MUST_BE_FRIENDS")}");
                                return;
                            }
                            ClientSession otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
                            if (otherSession != null)
                            {
                                if (otherSession.Character.Group != null)
                                {
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OTHER_PLAYER_IN_GROUP"), 0));
                                    return;
                                }
                                otherSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                    $"#fins^34^{session.Character.VisualId} #fins^69^{session.Character.VisualId} {string.Format(Language.Instance.GetMessageFromKey("MARRY_REQUEST"), session.Character.Name)}"));
                                session.Character.MarryRequestCharacters.Add(characterId);
                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                            }
                        }
                    }
                    break;

                case 100: // Miniland Signpost
                    {
                        if (session.Character.BattleEntity.GetOwnedNpcs().Any(s => session.Character.BattleEntity.IsSignpost(s.NpcVNum)))
                        {
                            return;
                        }
                        if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance && new short[] { 1, 145 }.Contains(session.CurrentMapInstance.Map.MapId))
                        {
                            MapNpc signPost = new MapNpc
                            {
                                NpcVNum = (short)EffectValue,
                                MapX = session.Character.PositionX,
                                MapY = session.Character.PositionY,
                                MapId = session.CurrentMapInstance.Map.MapId,
                                ShouldRespawn = false,
                                IsMoving = false,
                                MapNpcId = session.CurrentMapInstance.GetNextNpcId(),
                                Owner = session.Character.BattleEntity,
                                Dialog = 10000,
                                Position = 2,
                                Name = $"{session.Character.Name}'s^[Miniland]"
                            };
                            switch (EffectValue)
                            {
                                case 1428:
                                case 1499:
                                case 1519:
                                    signPost.AliveTime = 3600;
                                    break;
                                default:
                                    signPost.AliveTime = 1800;
                                    break;
                            }
                            signPost.Initialize(session.CurrentMapInstance);
                            session.CurrentMapInstance.AddNPC(signPost);
                            session.CurrentMapInstance.Broadcast(signPost.GenerateIn());
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    break;

                case 550: // Campfire and other craft npcs
                    {
                        if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                        {
                            short dialog = 10023;
                            switch (EffectValue)
                            {
                                case 956:
                                    dialog = 10023;
                                    break;
                                case 957:
                                    dialog = 10024;
                                    break;
                                case 959:
                                    dialog = 10026;
                                    break;
                            }
                            MapNpc campfire = new MapNpc
                            {
                                NpcVNum = (short)EffectValue,
                                MapX = session.Character.PositionX,
                                MapY = session.Character.PositionY,
                                MapId = session.CurrentMapInstance.Map.MapId,
                                ShouldRespawn = false,
                                IsMoving = false,
                                MapNpcId = session.CurrentMapInstance.GetNextNpcId(),
                                Owner = session.Character.BattleEntity,
                                Dialog = dialog,
                                Position = 2,
                            };
                            campfire.AliveTime = 180;
                            campfire.Initialize(session.CurrentMapInstance);
                            session.CurrentMapInstance.AddNPC(campfire);
                            session.CurrentMapInstance.Broadcast(campfire.GenerateIn());
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    break;

                // Faction Egg
                case 570:
                    if (session.Character.Faction == (FactionType)EffectValue)
                    {
                        return;
                    }
                    if (EffectValue < 3)
                    {
                        session.SendPacket(session.Character.Family == null
                            ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                            : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"),
                            0));
                    }
                    else
                    {
                        session.SendPacket(session.Character.Family != null
                            ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                            : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY"),
                            0));
                    }

                    break;

                // SP Wings
                case 650:
                    ItemInstance specialistInstance = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                    if (session.Character.UseSp && specialistInstance != null && !session.Character.IsSeal)
                    {
                        if (Option == 0)
                        {
                            session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^3 {Language.Instance.GetMessageFromKey("ASK_WINGS_CHANGE")}");
                        }
                        else
                        {
                            void disposeBuff(short vNum)
                            {
                                if (session.Character.BuffObservables.ContainsKey(vNum))
                                {
                                    session.Character.BuffObservables[vNum].Dispose();
                                    session.Character.BuffObservables.Remove(vNum);
                                }
                                session.Character.RemoveBuff(vNum);
                            }

                            disposeBuff(387);
                            disposeBuff(395);
                            disposeBuff(396);
                            disposeBuff(397);
                            disposeBuff(398);
                            disposeBuff(410);
                            disposeBuff(411);
                            disposeBuff(444);
                            disposeBuff(663);
                            disposeBuff(686);
                            disposeBuff(755);

                            specialistInstance.Design = (byte)EffectValue;

                            session.Character.MorphUpgrade2 = EffectValue;
                            session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                            session.SendPacket(session.Character.GenerateStat());
                            session.SendPackets(session.Character.GenerateStatChar());
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_SP"), 0));
                    }
                    break;

                // Self-Introduction
                case 203:
                    if (!session.Character.IsVehicled && Option == 0)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 2, session.Character.VisualId, 1));
                    }
                    break;

                // Magic Lamp
                case 651:
                    if (session.Character.Inventory.All(i => i.Value.Type != InventoryType.Wear))
                    {
                        if (Option == 0)
                        {
                            session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^3 {Language.Instance.GetMessageFromKey("ASK_USE")}");
                        }
                        else
                        {
                            session.Character.ChangeSex();
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                    }
                    break;

                // Vehicles
                case 1000:
                    if (EffectValue != 0
                     || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.EventGameInstance
                     || session.CurrentMapInstance?.MapInstanceType == (MapInstanceType.TalentArenaMapInstance)
                     || session.CurrentMapInstance?.MapInstanceType == (MapInstanceType.IceBreakerInstance)
                     || session.Character.IsSeal || session.Character.IsMorphed)
                    {
                        return;
                    }
                    short morph = Morph;
                    byte speed = Speed;
                    if (Morph < 0)
                    {
                        switch (VNum)
                        {
                            case 5923:
                                morph = 2513;
                                speed = 14;
                                break;
                        }
                    }
                    if (morph > 0)
                    {
                        if (Option == 0 && !session.Character.IsVehicled)
                        {
                            if (session.Character.Buff.Any(s => s.Card.BuffType == BuffType.Bad))
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANT_TRASFORM_WITH_DEBUFFS"),
                                    0));
                                return;
                            }
                            if (session.Character.IsSitting)
                            {
                                session.Character.IsSitting = false;
                                session.CurrentMapInstance?.Broadcast(session.Character.GenerateRest());
                            }
                            session.Character.LastDelay = DateTime.Now;
                            session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 3, $"#u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^2"));
                        }
                        else
                        {
                            if (!session.Character.IsVehicled && Option != 0)
                            {
                                DateTime delay = DateTime.Now.AddSeconds(-4);
                                if (session.Character.LastDelay > delay && session.Character.LastDelay < delay.AddSeconds(2))
                                {
                                    session.Character.IsVehicled = true;
                                    session.Character.VehicleSpeed = speed;
                                    session.Character.VehicleItem = this;
                                    session.Character.LoadSpeed();
                                    session.Character.MorphUpgrade = 0;
                                    session.Character.MorphUpgrade2 = 0;
                                    switch (VNum)
                                    {
                                        case 15000:
                                        case 15001:
                                        case 15002:
                                        case 15003:
                                        case 15004:
                                        case 15005:
                                        case 15006:
                                        case 15007:
                                        case 15008:
                                        case 15009:
                                        case 15010:
                                        case 15011:
                                        case 15012:
                                        case 15013:
                                        case 15294:
                                        case 15293:
                                        case 15292:
                                        case 15291:
                                        case 15289:
                                        case 15288:
                                        case 15287:
                                            session.Character.Morph = Morph;
                                            break;
                                        default:
                                            session.Character.Morph = Morph + (byte)session.Character.Gender;
                                            break;
                                    }
                                    session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(VisualType.Player, session.Character.VisualId, 196), session.Character.PositionX, session.Character.PositionY);
                                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                                    session.SendPacket(session.Character.GenerateCond());
                                    session.Character.LastSpeedChange = DateTime.Now;
                                    session.Character.Mates.Where(s => s.IsTeamMember).ToList()
                                        .ForEach(s => session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                                    if (Morph < 0)
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                }
                            }
                            else if (session.Character.IsVehicled)
                            {
                                session.Character.RemoveVehicle();
                                foreach (Mate teamMate in session.Character.Mates.Where(m => m.IsTeamMember))
                                {
                                    teamMate.PositionX =
                                        (short)(session.Character.PositionX + (teamMate.MateType == MateType.Partner ? -1 : 1));
                                    teamMate.PositionY = (short)(session.Character.PositionY + 1);
                                    if (session.Character.MapInstance.Map.IsBlockedZone(teamMate.PositionX, teamMate.PositionY))
                                    {
                                        teamMate.PositionX = session.Character.PositionX;
                                        teamMate.PositionY = session.Character.PositionY;
                                    }
                                    teamMate.UpdateBushFire();
                                    Parallel.ForEach(session.CurrentMapInstance.Sessions.Where(s => s.Character != null), s =>
                                    {
                                        if (ServerManager.Instance.ChannelId != 51 || session.Character.Faction == s.Character.Faction)
                                        {
                                            s.SendPacket(teamMate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                                        }
                                        else
                                        {
                                            s.SendPacket(teamMate.GenerateIn(true, ServerManager.Instance.ChannelId == 51, s.Account.Authority));
                                        }
                                    });
                                }
                                session.SendPacket(session.Character.GeneratePinit());
                                session.Character.Mates.ForEach(s => session.SendPacket(s.GenerateScPacket()));
                                session.SendPackets(session.Character.GeneratePst());
                            }
                        }
                    }
                    break;

                // Sealed Vessel
                case 1002:
                    int type, secondaryType, inventoryType, slot;
                    if (packetsplit != null && int.TryParse(packetsplit[2], out type) && int.TryParse(packetsplit[3], out secondaryType) && int.TryParse(packetsplit[4], out inventoryType) && int.TryParse(packetsplit[5], out slot))
                    {
                        int packetType;
                        switch (EffectValue)
                        {
                            case 69:
                                if (int.TryParse(packetsplit[6], out packetType))
                                {
                                    switch (packetType)
                                    {
                                        case 0:
                                            session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                            break;
                                        case 1:
                                            int rnd = ServerManager.RandomNumber(0, 1000);
                                            if (rnd < 5)
                                            {
                                                short[] vnums =
                                                {
                                                        5560, 5591, 4099, 907, 1160, 4705, 4706, 4707, 4708, 4709, 4710, 4711, 4712, 4713, 4714,
                                                        4715, 4716
                                                    };
                                                byte[] counts = { 1, 1, 1, 1, 10, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                                                int item = ServerManager.RandomNumber(0, 17);
                                                session.Character.GiftAdd(vnums[item], counts[item]);
                                            }
                                            else if (rnd < 30)
                                            {
                                                short[] vnums = { 361, 362, 363, 366, 367, 368, 371, 372, 373 };
                                                session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 9)], 1);
                                            }
                                            else
                                            {
                                                short[] vnums =
                                                {
                                                        1161, 2282, 1030, 1244, 1218, 5369, 1012, 1363, 1364, 2160, 2173, 5959, 5983, 2514,
                                                        2515, 2516, 2517, 2518, 2519, 2520, 2521, 1685, 1686, 5087, 5203, 2418, 2310, 2303,
                                                        2169, 2280, 5892, 5893, 5894, 5895, 5896, 5897, 5898, 5899, 5332, 5105, 2161, 2162
                                                    };
                                                byte[] counts =
                                                {
                                                        10, 10, 20, 5, 1, 1, 99, 1, 1, 5, 5, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 5, 20,
                                                        20, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                                                    };
                                                int item = ServerManager.RandomNumber(0, 42);
                                                session.Character.GiftAdd(vnums[item], counts[item]);
                                            }
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            break;
                                    }
                                }
                                break;
                            default:
                                if (int.TryParse(packetsplit[6], out packetType))
                                {
                                    if (session.Character.MapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act4))
                                    {
                                        return;
                                    }

                                    if (session.Account.IsLimited)
                                    {
                                        session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("LIMITED_ACCOUNT")));
                                        return;
                                    }

                                    switch (packetType)
                                    {
                                        case 0:
                                            session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7, $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                            break;

                                        case 1:
                                            if (session.HasCurrentMapInstance && (session.Character.MapInstance == session.Character.Miniland  || session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance) && (session.Character.LastVessel.AddSeconds(1) <= DateTime.Now || session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.FastVessels)))
                                            {
                                                short[] vnums = { 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405 };
                                                short vnum = vnums[ServerManager.RandomNumber(0, 20)];

                                                NpcMonster npcmonster = ServerManager.GetNpcMonster(vnum);
                                                if (npcmonster == null)
                                                {
                                                    return;
                                                }
                                                MapMonster monster = new MapMonster
                                                {
                                                    MonsterVNum = vnum,
                                                    MapX = session.Character.PositionX,
                                                    MapY = session.Character.PositionY,
                                                    MapId = session.Character.MapInstance.Map.MapId,
                                                    Position = session.Character.Direction,
                                                    IsMoving = true,
                                                    MapMonsterId = session.CurrentMapInstance.GetNextMonsterId(),
                                                    ShouldRespawn = false
                                                };
                                                monster.Initialize(session.CurrentMapInstance);
                                                session.CurrentMapInstance.AddMonster(monster);
                                                session.CurrentMapInstance.Broadcast(monster.GenerateIn());
                                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                                session.Character.LastVessel = DateTime.Now;
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;

                // Golden Bazaar Medal
                case 1003:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BazaarMedalGold
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Silver Bazaar Medal
                case 1004:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalGold))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BazaarMedalSilver
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Pet Slot Expansion
                case 1006:
                    if (Option == 0)
                    {
                        session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^2 {Language.Instance.GetMessageFromKey("ASK_PET_MAX")}");
                    }
                    else if ((inv.Item?.IsSoldable == true && session.Character.MaxMateCount < 90) || session.Character.MaxMateCount < 30)
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.MaxMateCount += 10;
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GET_PET_PLACES"), 10));
                        session.SendPacket(session.Character.GenerateScpStc());
                    }
                    break;

                // Permanent Backpack Expansion
                case 601:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.BackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddYears(15),
                            StaticBonusType = StaticBonusType.BackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Permanent Partner's Backpack
                case 602:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddYears(15),
                            StaticBonusType = StaticBonusType.PetBackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Permanent Pet Basket
                case 603:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBasket))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddYears(15),
                            StaticBonusType = StaticBonusType.PetBasket
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket("ib 1278 1");
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Pet Basket
                case 1007:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBasket))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.PetBasket
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket("ib 1278 1");
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Partner's Backpack
                case 1008:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.PetBackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Backpack Expansion
                case 1009:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.BackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Sealed Tarot Card
                case 1005:
                    session.Character.GiftAdd((short)(VNum - Effect), 1);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Tarot Card Game
                case 1894:
                    if (EffectValue == 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            session.Character.GiftAdd((short)(Effect + ServerManager.RandomNumber(0, 10)), 1);
                        }
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                // Sealed Tarot Card
                case 2152:
                    session.Character.GiftAdd((short)(VNum + Effect), 1);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Transformation scrolls
                case 1001:
                    if (session.Character.IsMorphed)
                    {
                        session.Character.IsMorphed = false;
                        session.Character.Morph = 0;
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                    }
                    else if (!session.Character.UseSp && !session.Character.IsVehicled)
                    {
                        if (Option == 0)
                        {
                            session.Character.LastDelay = DateTime.Now;
                            session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 3, $"#u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^1"));
                        }
                        else
                        {
                            int[] possibleTransforms = null;

                            switch (EffectValue)
                            {
                                case 1: // Halloween
                                    possibleTransforms = new int[]
                                    {
                                    404, //Torturador pellizcador
                                    405, //Torturador enrollador
                                    406, //Torturador de acero
                                    446, //Guerrero yak
                                    447, //Mago yak
                                    441, //Guerrero de la muerte
                                    276, //Rey polvareda
                                    324, //Princesa Catrisha
                                    248, //Bruja oscura
                                    249, //Bruja de sangre
                                    438, //Bruja blanca fuerte
                                    236, //Guerrero esqueleto
                                    245, //Sombra nocturna
                                    439, //Guerrero esqueleto resucitado
                                    272, //Arquero calavera
                                    274, //Guerrero calavera
                                    2691, //Frankenstein
                                    };
                                    break;

                                case 2: // Ice Costume
                                    break;

                                case 3: // Bushtail Costume
                                    break;
                            }

                            if (possibleTransforms != null)
                            {
                                session.Character.IsMorphed = true;
                                session.Character.Morph = 1000 + possibleTransforms[ServerManager.RandomNumber(0, possibleTransforms.Length)];
                                session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                                if (VNum != 1914)
                                {
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                            }
                        }
                    }
                    break;

                // Return Command
                case 10010: // Return NosVille
                    {
                        if (session.Character.HasShopOpened || session.Character.InExchangeOrTrade)
                        {
                            session.Character.Dispose();
                        }

                        if (session.Character.IsChangingMapInstance)
                        {
                            return;
                        }

                        StaticBonusDTO VipBonus =
                                session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
                        if (VipBonus == null)
                        {
                            session.SendPacket(session.Character.GenerateSay("You need a Vip packet to use this item.", 11));
                            return;
                        }

                        if (ServerManager.Instance.ChannelId != 51)
                        {
                            ServerManager.Instance.ChangeMap(session.Character.VisualId, 129, 127, 73);
                            session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(23));
                        }
                        else
                        {
                            if (session.Character.LastSkillUse.AddSeconds(20) < DateTime.Now || session.Character.LastDefence.AddSeconds(20) < DateTime.Now)
                            {
                                if (session.Character.Faction == FactionType.Angel)
                                {
                                    ServerManager.Instance.ChangeMap(session.Character.VisualId, 130, 41, 41);
                                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(23));
                                }
                                else if (session.Character.Faction == FactionType.Demon)
                                {
                                    ServerManager.Instance.ChangeMap(session.Character.VisualId, 131, 41, 41);
                                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(3));
                                }
                            }
                            else
                            {
                                session.SendPacket(
                                        session.Character.GenerateSay(
                                                Language.Instance.GetMessageFromKey("CANT_USE_THAT_IN_BATTLE"), 10));
                            }
                        }
                    }
                    break;

                case 29999:
                    StaticBonusDTO vipBonus =
                                session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
                    if (vipBonus == null)
                    {
                        session.SendPacket(session.Character.GenerateSay("You need a Vip packet to use this item.", 11));
                        return;
                    }
                    if (session.Character.Compliment < 500)
                    {
                        session.Character.Compliment += 500;
                        ServerManager.Instance.ChangeMap(session.Character.VisualId);
                    }
                    break;

                case 10011: // Change Class Seaquenzial
                    {
                        if (Option == 0)
                        {
                            session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^3 Do you really want change your class?");
                        }
                        else
                        {
                            switch (EffectValue)
                            {
                                case 0:
                                    ChangeClassHelper.Instance.SequenzialChangeClass(session);
                                    break;
                                case 1:
                                    ChangeClassHelper.Instance.SequenzialChangeClass(session, true);
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    break;

                case 10012: // Change Class Single
                    {
                        if (Option == 0)
                        {
                            session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^3 Do you really want change your class?");
                        }
                        else
                        {
                            ChangeClassHelper.Instance.ChangeClass(session, inv);
                        }
                    }
                    break;

                case 10016:
                    session.Character.Size = inv.Item.EffectValue;
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateScal());
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Vip Medal
                case 30000:
                    StaticBonusDTO vipBonuss =
                                session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
                    if (vipBonuss == null)
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.VisualId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.VIP
                        });
                        session.Character.Compliment += 500;
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                        ServerManager.Instance.ChangeMap(session.Character.VisualId);
                    }
                    else
                        session.SendPacket(session.Character.GenerateSay("Already in use.", 12));
                    break;

                //Max Perfections
                case 30001:
                    {
                        var SP = session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>((byte)EquipmentType.Sp, InventoryType.Wear);
                        if (!session.Character.UseSp && SP != null && SP.SpStoneUpgrade <= 99)
                        {
                            if (Option == 0)
                            {
                                session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^3 Do you want use all perfection for your wearable sp?");
                            }
                            else
                            {
                                CustomHelper.Instance.SpeedPerfection(session, SP, inv);
                            }
                        }
                    }
                    break;

                // Reset perfection
                case 30002:
                    {
                        var SP = session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>((byte)EquipmentType.Sp, InventoryType.Wear);
                        if (!session.Character.UseSp && SP != null && SP.SpStoneUpgrade > 0)
                        {
                            if (Option == 0)
                            {
                                session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^3 Do you want reset all perfections of your wearable sp?");
                            }
                            else
                            {
                                switch (EffectValue)
                                {
                                    case 0:
                                        CustomHelper.Instance.RemovePerfection(session, SP, inv);
                                        break;

                                    case 1:
                                        CustomHelper.Instance.RemovePerfection(session, SP);
                                        break;

                                    default:
                                        return;
                                }
                            }
                        }
                    }
                    break;

                default:
                    switch (EffectValue)
                    {
                        // Angel Base Flag
                        case 965:
                        // Demon Base Flag
                        case 966:
                            if (ServerManager.Instance.ChannelId == 51 && session.CurrentMapInstance?.Map.MapId != 130 && session.CurrentMapInstance?.Map.MapId != 131 && EffectValue - 964 == (short)session.Character.Faction)
                            {
                                session.CurrentMapInstance?.SummonMonster(new MonsterToSummon((short)EffectValue, new MapCell { X = session.Character.PositionX, Y = session.Character.PositionY }, null, false, isHostile: false, aliveTime: 1800));
                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            }
                            break;

                        default:
                            switch (VNum)
                            {
                                case 5856: // Partner Slot Expansion
                                case 9113: // Partner Slot Expansion (Limited)
                                    {
                                        if (Option == 0)
                                        {
                                            session.SendPacket($"qna #u_i^1^{session.Character.VisualId}^{(byte)inv.Type}^{inv.Slot}^2 {Language.Instance.GetMessageFromKey("ASK_PARTNER_MAX")}");
                                        }
                                        else if (session.Character.MaxPartnerCount < 12)
                                        {
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            session.Character.MaxPartnerCount++;
                                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GET_PARTNER_PLACES"), 10));
                                            session.SendPacket(session.Character.GenerateScpStc());
                                        }
                                    }
                                    break;

                                case 5931: // Tique de habilidad de compañero (una)
                                    {
                                        if (session?.Character?.Mates == null)
                                        {
                                            return;
                                        }

                                        if (packetsplit.Length != 10 || !byte.TryParse(packetsplit[8], out byte petId) || !byte.TryParse(packetsplit[9], out byte castId))
                                        {
                                            return;
                                        }

                                        if (castId < 0 || castId > 2)
                                        {
                                            return;
                                        }

                                        Mate partner = session.Character.Mates.ToList().FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner && s.PetId == petId);

                                        if (partner?.Sp == null || partner.IsUsingSp)
                                        {
                                            return;
                                        }

                                        PartnerSkill skill = partner.Sp.GetSkill(castId);

                                        if (skill?.Skill == null)
                                        {
                                            return;
                                        }

                                        if (skill.Level == (byte)PartnerSkillLevelType.S)
                                        {
                                            return;
                                        }

                                        if (partner.Sp.RemoveSkill(castId))
                                        {
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                                            partner.Sp.ReloadSkills();
                                            partner.Sp.FullXp();

                                            session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("PSP_SKILL_RESETTED"), 1));
                                        }

                                        session.SendPacket(partner.GenerateScPacket());
                                    }
                                    break;
                                case 5932: // Tique de habilidad de compañero (todas)
                                    {
                                        if (packetsplit.Length != 10
                                            || session?.Character?.Mates == null)
                                        {
                                            return;
                                        }

                                        if (!byte.TryParse(packetsplit[8], out byte petId) || !byte.TryParse(packetsplit[9], out byte castId))
                                        {
                                            return;
                                        }

                                        if (castId < 0 || castId > 2)
                                        {
                                            return;
                                        }

                                        Mate partner = session.Character.Mates.ToList().FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner && s.PetId == petId);

                                        if (partner?.Sp == null || partner.IsUsingSp)
                                        {
                                            return;
                                        }

                                        if (partner.Sp.GetSkillsCount() < 1)
                                        {
                                            return;
                                        }

                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                                        partner.Sp.ClearSkills();
                                        partner.Sp.FullXp();

                                        session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("PSP_ALL_SKILLS_RESETTED"), 1));

                                        session.SendPacket(partner.GenerateScPacket());
                                    }
                                    break;

                                // Event Upgrade Scrolls
                                case 5107:
                                case 5207:
                                case 5519:
                                    if (EffectValue != 0)
                                    {
                                        if (session.Character.IsSitting)
                                        {
                                            session.Character.IsSitting = false;
                                            session.SendPacket(session.Character.GenerateRest());
                                        }
                                        session.SendPacket(UserInterfaceHelper.GenerateGuri(12, 1, session.Character.VisualId, EffectValue));
                                    }
                                    break;

                                // Martial Artist Starter Pack
                                case 5832:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                                        // Steel Fist
                                        session.Character.GiftAdd(4756, 1, 5);

                                        // Trainee Martial Artist's Uniform
                                        session.Character.GiftAdd(4757, 1, 5);

                                        // Mystical Glacier Stone
                                        session.Character.GiftAdd(4504, 1);

                                        // Hero's Amulet of Fire
                                        session.Character.GiftAdd(4503, 1);

                                        // Fairy Fire/Water/Light/Dark (30%)
                                        for (short itemVNum = 884; itemVNum <= 887; itemVNum++)
                                        {
                                            session.Character.GiftAdd(itemVNum, 1);
                                        }
                                    }
                                    break;

                                // Soulstone Blessing
                                case 1362:
                                case 5195:
                                case 5211:
                                case 9075:
                                    if (!session.Character.Buff.ContainsKey(146))
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 146 });
                                    }
                                    else
                                    {
                                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                                    }
                                    break;
                                case 1428:
                                    session.SendPacket("guri 18 1");
                                    break;
                                case 1429:
                                    session.SendPacket("guri 18 0");
                                    break;
                                case 1904:
                                    short[] items = { 1894, 1895, 1896, 1897, 1898, 1899, 1900, 1901, 1902, 1903 };
                                    for (int i = 0; i < 5; i++)
                                    {
                                        session.Character.GiftAdd(items[ServerManager.RandomNumber(0, items.Length)], 1);
                                    }
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;
                                case 5370:
                                    if (session.Character.Buff.Any(s => s.Card.CardId == 393))
                                    {
                                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ALREADY_GOT_BUFF"), session.Character.Buff.FirstOrDefault(s => s.Card.CardId == 393)?.Card.Name), 10));
                                        return;
                                    }
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 393 });
                                    break;
                                case 15274:
                                    short[] vnums = new short[] { 4356, 4357, 4358, 4359 };
                                    session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 4)], 1);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;
                                case 5916:
                                case 5927:
                                    session.Character.AddStaticBuff(new StaticBuffDTO
                                    {
                                        CardId = 340,
                                        CharacterId = session.Character.VisualId,
                                        RemainingTime = 7200
                                    });
                                    session.Character.RemoveBuff(339);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;
                                case 5929:
                                case 5930:
                                    session.Character.AddStaticBuff(new StaticBuffDTO
                                    {
                                        CardId = 340,
                                        CharacterId = session.Character.VisualId,
                                        RemainingTime = 600
                                    });
                                    session.Character.RemoveBuff(339);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;

                                    // Mother Nature's Rune Pack (limited)
                                case 15295:
                                    vnums = new short[] { 8316, 8317, 8318, 8319 };
                                    session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 4)], 1);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;                                

                                default:
                                    if (inv.Item.VNum == 15282 || inv.Item.VNum == 15284)
                                    {
                                        return;
                                    }
                                    IEnumerable<RollGeneratedItemDTO> roll = DAOFactory.Instance.RollGeneratedItemDAO.LoadByItemVNum(VNum);
                                    IEnumerable<RollGeneratedItemDTO> rollGeneratedItemDtos = roll as IList<RollGeneratedItemDTO> ?? roll.ToList();
                                    if (!rollGeneratedItemDtos.Any())
                                    {
                                        Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), VNum, Effect, EffectValue));
                                        return;
                                    }
                                    int probabilities = rollGeneratedItemDtos.Where(s => s.Probability != 10000).Sum(s => s.Probability);
                                    int rnd2 = ServerManager.RandomNumber(0, probabilities);
                                    int currentrnd = 0;
                                    int rnd = ServerManager.RandomNumber(0, 100);
                                    foreach (RollGeneratedItemDTO rollitem in rollGeneratedItemDtos.Where(s => s.Probability == 10000))
                                    {
                                        sbyte rare = 0;
                                        if (rollitem.IsRareRandom)
                                        {
                                            

                                            for (int j = ItemHelper.RareRate.Length - 1; j >= 0; j--)
                                            {
                                                if (rnd < ItemHelper.RareRate[j])
                                                {
                                                    rare = (sbyte)j;
                                                    break;
                                                }
                                            }
                                            if (rare < 1)
                                            {
                                                rare = 1;
                                            }
                                        }
                                        session.Character.GiftAdd(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, (byte)rare, design: rollitem.ItemGeneratedDesign);
                                    }
                                    foreach (RollGeneratedItemDTO rollitem in rollGeneratedItemDtos.Where(s => s.Probability != 10000).OrderBy(s => ServerManager.RandomNumber()))
                                    {
                                        sbyte rare = 0;
                                        if (rollitem.IsRareRandom)
                                        {
                                            rnd = ServerManager.RandomNumber(0, 100);

                                            for (int j = ItemHelper.RareRate.Length - 1; j >= 0; j--)
                                            {
                                                if (rnd < ItemHelper.RareRate[j])
                                                {
                                                    rare = (sbyte)j;
                                                    break;
                                                }
                                            }
                                            if (rare < 1)
                                            {
                                                rare = 1;
                                            }
                                        }

                                        currentrnd += rollitem.Probability;
                                        if (currentrnd < rnd2)
                                        {
                                            continue;
                                        }
                                        /*if (rollitem.IsSuperReward)
                                        {
                                            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                            {
                                                DestinationCharacterId = null,
                                                SourceCharacterId = session.Character.CharacterId,
                                                SourceWorldId = ServerManager.Instance.WorldId,
                                                Message = Language.Instance.GetMessageFromKey("SUPER_REWARD"),
                                                Type = MessageType.Shout
                                            });
                                        }*/
                                        session.Character.GiftAdd(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, (byte)rare, design: rollitem.ItemGeneratedDesign);//, rollitem.ItemGeneratedUpgrade);
                                        break;
                                    }
                                    session.Character.Inventory.RemoveItemAmount(VNum);
                                    break;
                            }
                            break;
                    }
                    break;
            }
            session.Character.IncrementQuests(QuestType.Use, inv.ItemVNum);
        }

        #endregion
    }
}