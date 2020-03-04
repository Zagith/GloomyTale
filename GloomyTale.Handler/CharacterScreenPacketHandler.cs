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
using GloomyTale.Core.Handling;
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using GloomyTale.GameObject.Networking;
using System.Collections.Concurrent;
using GloomyTale.Core.Interfaces.Packets.ClientPackets;
using GloomyTale.GameObject.Helpers;
using GloomyTale.Core.Extensions;
using GloomyTale.GameObject.Items.Instance;

namespace GloomyTale.Handler
{
    public class CharacterScreenPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterScreenPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void CreateCharacterAction(ICharacterCreatePacket characterCreatePacket, ClassType classType)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            Logger.Log.LogUserEvent("CREATECHARACTER", Session.GenerateIdentity(), $"[CreateCharacter]Name: {characterCreatePacket.Name} Slot: {characterCreatePacket.Slot} Gender: {characterCreatePacket.Gender} HairStyle: {characterCreatePacket.HairStyle} HairColor: {characterCreatePacket.HairColor}");

            if (characterCreatePacket.Slot <= 3
                && DAOFactory.Instance.CharacterDAO.LoadBySlot(Session.Account.AccountId, characterCreatePacket.Slot) == null
                && characterCreatePacket.Name != null
                && (characterCreatePacket.Gender == GenderType.Male || characterCreatePacket.Gender == GenderType.Female)
                && (characterCreatePacket.HairStyle == HairStyleType.HairStyleA || (classType != ClassType.MartialArtist && characterCreatePacket.HairStyle == HairStyleType.HairStyleB))
                && Enumerable.Range(0, 10).Contains((byte)characterCreatePacket.HairColor)
                && (characterCreatePacket.Name.Length >= 4 && characterCreatePacket.Name.Length <= 14))
            {
                if (classType == ClassType.MartialArtist)
                {
                    IEnumerable<CharacterDTO> characterDTOs = DAOFactory.Instance.CharacterDAO.LoadByAccount(Session.Account.AccountId);

                    if (!characterDTOs.Any(s => s.Level >= 80))
                    {
                        return;
                    }

                    if (characterDTOs.Any(s => s.Class == ClassType.MartialArtist))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("MARTIAL_ARTIST_ALREADY_EXISTING")));
                        return;
                    }
                }

                Regex regex = new Regex(@"^[A-Za-z0-9_áéíóúÁÉÍÓÚäëïöüÄËÏÖÜ]+$");

                if (regex.Matches(characterCreatePacket.Name).Count != 1)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INVALID_CHARNAME")));
                    return;
                }

                if (DAOFactory.Instance.CharacterDAO.LoadByName(characterCreatePacket.Name) != null)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CHARNAME_ALREADY_TAKEN")));
                    return;
                }

                CharacterDTO characterDTO = new CharacterDTO
                {
                    AccountId = Session.Account.AccountId,
                    Slot = characterCreatePacket.Slot,
                    Class = classType,
                    Gender = characterCreatePacket.Gender,
                    HairStyle = characterCreatePacket.HairStyle,
                    HairColor = characterCreatePacket.HairColor,
                    Name = characterCreatePacket.Name,
                    MapId = 129,
                    MapX = 127,
                    MapY = 73,
                    MaxMateCount = 10,
                    MaxPartnerCount = 3,
                    SpPoint = 10000,
                    SpAdditionPoint = 0,
                    MinilandMessage = "Welcome",
                    State = CharacterState.Active,
                    MinilandPoint = 2000
                };

                switch (characterDTO.Class)
                {
                    case ClassType.MartialArtist:
                        {
                            characterDTO.Level = 81;
                            characterDTO.JobLevel = 1;
                            characterDTO.Hp = 9401;
                            characterDTO.Mp = 3156;
                        }
                        break;

                    default:
                        {
                            characterDTO.Level = 15;
                            characterDTO.JobLevel = 20;
                            characterDTO.Hp = 221;
                            characterDTO.Mp = 221;
                        }
                        break;
                }

                DAOFactory.Instance.CharacterDAO.InsertOrUpdate(ref characterDTO);

                if (classType == ClassType.MartialArtist)
                {
                    DAOFactory.Instance.CharacterQuestDAO.Save(new CharacterQuestDTO
                    {
                        CharacterId = characterDTO.CharacterId,
                        QuestId = 6275,
                        IsMainQuest = false
                    });
                }

                if (classType != ClassType.MartialArtist)
                {
                    DAOFactory.Instance.CharacterQuestDAO.Save(new CharacterQuestDTO
                    {
                        CharacterId = characterDTO.CharacterId,
                        QuestId = 1997,
                        IsMainQuest = true
                    });                    

                    DAOFactory.Instance.CharacterSkillDAO.Save(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 200 });
                    DAOFactory.Instance.CharacterSkillDAO.Save(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 201 });
                    DAOFactory.Instance.CharacterSkillDAO.Save(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 209 });

                    var inventory = new Inventory((Character)characterDTO);
                    inventory.AddNewToInventory(15299, 1, InventoryType.Main);
                    DAOFactory.Instance.ItemInstanceDAO.Save(inventory.Values);
                    LoadCharacters(characterCreatePacket.OriginalContent);
                }
                else
                {
                    for (short skillVNum = 1525; skillVNum <= 1539; skillVNum++)
                    {
                        DAOFactory.Instance.CharacterSkillDAO.Save(new CharacterSkillDTO
                        {
                            CharacterId = characterDTO.CharacterId,
                            SkillVNum = skillVNum
                        });
                    }

                    DAOFactory.Instance.CharacterSkillDAO.Save(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 1565 });

                    var inventory = new Inventory((Character)characterDTO);
                    inventory.AddNewToInventory(5832, 1, InventoryType.Main, 5);
                    DAOFactory.Instance.ItemInstanceDAO.Save(inventory.Values);
                    LoadCharacters(characterCreatePacket.OriginalContent);
                }
            }
        }

        /// <summary>
        /// Char_NEW character creation character
        /// </summary>
        /// <param name="characterCreatePacket"></param>
        public void CreateCharacter(CharacterCreatePacket characterCreatePacket)
            => CreateCharacterAction(characterCreatePacket, ClassType.Adventurer);

        /// <summary>
        /// Char_NEW_JOB character creation character
        /// </summary>
        /// <param name="characterJobCreatePacket"></param>
        public void CreateCharacterJob(CharacterJobCreatePacket characterJobCreatePacket)
            => CreateCharacterAction(characterJobCreatePacket, ClassType.MartialArtist);

        /// <summary>
        /// Char_DEL packet
        /// </summary>
        /// <param name="characterDeletePacket"></param>
        public void DeleteCharacter(CharacterDeletePacket characterDeletePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            if (characterDeletePacket.Password == null)
            {
                return;
            }

            Logger.Log.LogUserEvent("DELETECHARACTER", Session.GenerateIdentity(),
                $"[DeleteCharacter]Name: {characterDeletePacket.Slot}");
            AccountDTO account = DAOFactory.Instance.AccountDAO.LoadById(Session.Account.AccountId);
            if (account == null)
            {
                return;
            }

            if (account.Password.ToLower() == characterDeletePacket.Password.ToSha512())
            {
                CharacterDTO character =
                    DAOFactory.Instance.CharacterDAO.LoadBySlot(account.AccountId, characterDeletePacket.Slot);
                if (character == null)
                {
                    return;
                }

                //DAOFactory.Instance.GeneralLogDAO.SetCharIdNull(Convert.ToInt64(character.CharacterId));
                long Id = character.CharacterId;
                DAOFactory.Instance.CharacterDAO.DeleteByPrimaryKey(account.AccountId, characterDeletePacket.Slot);
                FamilyCharacterDTO familyCharacter = DAOFactory.Instance.FamilyCharacterDAO.LoadByCharacterId(character.CharacterId);
                if (familyCharacter == null)
                {
                    LoadCharacters(string.Empty);
                    return;
                }

                // REMOVE FROM FAMILY
                DAOFactory.Instance.FamilyCharacterDAO.Delete(Id);
                ServerManager.Instance.FamilyRefresh(familyCharacter.FamilyId);
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BAD_PASSWORD")}");
            }
        }

        /// <summary>
        /// Load Characters, this is the Entrypoint for the Client, Wait for 3 Packets.
        /// </summary>
        /// <param name="packet"></param>
        [Packet(3, "OpenNos.EntryPoint")]
        public void LoadCharacters(string packet)
        {
            string[] loginPacketParts = packet.Split(' ');
            bool isCrossServerLogin = false;

            // Load account by given SessionId
            if (Session.Account == null)
            {
                bool hasRegisteredAccountLogin = true;
                AccountDTO account = null;
                if (loginPacketParts.Length > 4)
                {
                    if (loginPacketParts.Length > 7 && loginPacketParts[4] == "DAC"
                        && loginPacketParts[8] == "CrossServerAuthenticate")
                    {
                        isCrossServerLogin = true;
                        account = DAOFactory.Instance.AccountDAO.LoadByName(loginPacketParts[5]);
                    }
                    else
                    {
                        account = DAOFactory.Instance.AccountDAO.LoadByName(loginPacketParts[4]);
                    }
                }

                try
                {
                    if (account != null)
                    {
                        if (isCrossServerLogin)
                        {
                            hasRegisteredAccountLogin =
                                CommunicationServiceClient.Instance.IsCrossServerLoginPermitted(account.AccountId,
                                    Session.SessionId);
                        }
                        else
                        {
                            hasRegisteredAccountLogin =
                                CommunicationServiceClient.Instance.IsLoginPermitted(account.AccountId,
                                    Session.SessionId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("MS Communication Failed.", ex);
                    Session.Disconnect();
                    return;
                }

                if (loginPacketParts.Length > 4 && hasRegisteredAccountLogin)
                {
                    if (account != null)
                    {
                        if (account.Password.Equals(loginPacketParts[6].ToSha512(), StringComparison.OrdinalIgnoreCase)
                            || isCrossServerLogin)
                        {
                            var accountobject = new Account
                            {
                                AccountId = account.AccountId,
                                Name = account.Name,
                                Password = account.Password.ToLower(),
                                Authority = account.Authority,
                                Language = account.Language,
                                Email = account.Email,
                                ReferrerId = account.ReferrerId,
                                RegistrationIP = account.RegistrationIP,
                                VerificationToken = account.VerificationToken
                            };

                            Session.InitializeAccount(accountobject, isCrossServerLogin);
                        }
                        else
                        {
                            Logger.Log.Debug($"Client {Session.ClientId} forced Disconnection, invalid Password.");
                            Session.Disconnect();
                            return;
                        }
                    }
                    else
                    {
                        Logger.Log.Debug($"Client {Session.ClientId} forced Disconnection, invalid AccountName.");
                        Session.Disconnect();
                        return;
                    }
                }
                else
                {
                    Logger.Log.Debug(
                        $"Client {Session.ClientId} forced Disconnection, login has not been registered or Account is already logged in.");
                    Session.Disconnect();
                    return;
                }
            }

            if (isCrossServerLogin)
            {
                if (byte.TryParse(loginPacketParts[6], out byte slot))
                {
                    SelectCharacter(new SelectPacket {Slot = slot});
                }
            }
            else
            {
                // TODO: Wrap Database access up to GO
                IEnumerable<CharacterDTO> characters = DAOFactory.Instance.CharacterDAO.LoadByAccount(Session.Account.AccountId).Where(s => s.State == CharacterState.Active);

                Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("ACCOUNT_ARRIVED"), Session.SessionId));

                // load characterlist packet for each character in CharacterDTO
                Session.SendPacket("clist_start 0");

                foreach (CharacterDTO character in characters)
                {
                    IEnumerable<ItemInstanceDTO> inventory =
                        DAOFactory.Instance.ItemInstanceDAO.LoadByType(character.CharacterId, InventoryType.Wear);

                    ItemInstance[] equipment = new ItemInstance[17];

                    foreach (ItemInstanceDTO equipmentEntry in inventory)
                    {
                        // explicit load of iteminstance
                        var currentInstance = (ItemInstance)equipmentEntry;

                        if (currentInstance != null)
                        {
                            equipment[(short) currentInstance.Item.EquipmentSlot] = currentInstance;
                        }
                    }

                    string petlist = "";

                    List<MateDTO> mates = DAOFactory.Instance.MateDAO.LoadByCharacterId(character.CharacterId).ToList();

                    for (int i = 0; i < 26; i++)
                    {
                        //0.2105.1102.319.0.632.0.333.0.318.0.317.0.9.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1
                        petlist += (i != 0 ? "." : "") + (mates.Count > i ? $"{mates[i].Skin}.{mates[i].NpcMonsterVNum}" : "-1");
                    }

                    // 1 1 before long string of -1.-1 = act completion
                    Session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte) character.Gender} {(byte) character.HairStyle} {(byte) character.HairColor} 0 {(byte) character.Class} {character.Level} {character.HeroLevel} {equipment[(byte) EquipmentType.Hat]?.ItemVNum ?? -1}.{equipment[(byte) EquipmentType.Armor]?.ItemVNum ?? -1}.{equipment[(byte) EquipmentType.WeaponSkin]?.ItemVNum ?? (equipment[(byte) EquipmentType.MainWeapon]?.ItemVNum ?? -1)}.{equipment[(byte) EquipmentType.SecondaryWeapon]?.ItemVNum ?? -1}.{equipment[(byte) EquipmentType.Mask]?.ItemVNum ?? -1}.{equipment[(byte) EquipmentType.Fairy]?.ItemVNum ?? -1}.{equipment[(byte) EquipmentType.CostumeSuit]?.ItemVNum ?? -1}.{equipment[(byte) EquipmentType.CostumeHat]?.ItemVNum ?? -1} {character.JobLevel}  1 1 {petlist} {(equipment[(byte) EquipmentType.Hat]?.Item.IsColored == true ? equipment[(byte) EquipmentType.Hat].Design : 0)} 0");
                }

                Session.SendPacket("clist_end");
            }
        }

        /// <summary>
        /// select packet
        /// </summary>
        /// <param name="selectPacket"></param>
        public void SelectCharacter(SelectPacket selectPacket)
        {
            try
            {
                #region Validate Session

                if (Session?.Account == null
                    || Session.HasSelectedCharacter)
                {
                    return;
                }

                #endregion

                #region Load Character

                CharacterDTO characterDTO = DAOFactory.Instance.CharacterDAO.LoadBySlot(Session.Account.AccountId, selectPacket.Slot);

                if (characterDTO == null)
                {
                    return;
                }

                var character = (Character)characterDTO;

                #endregion

                #region Unban Character

                if (ServerManager.Instance.BannedCharacters.Contains(character.VisualId))
                {
                    ServerManager.Instance.BannedCharacters.RemoveAll(s => s == character.VisualId);
                }

                #endregion

                #region Initialize Character

                character.Initialize();

                character.MapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(character.MapId);
                character.PositionX = character.MapX;
                character.PositionY = character.MapY;
                character.Authority = Session.Account.Authority;

                Session.SetCharacter(character);

                #endregion

                #region Load General Logs

                character.GeneralLogs = new ThreadSafeGenericList<GeneralLogDTO>();
                character.GeneralLogs.AddRange(DAOFactory.Instance.GeneralLogDAO.LoadByAccount(Session.Account.AccountId)
                    .Where(s => s.LogType == "DailyReward" || s.CharacterId == character.VisualId).ToList());

                #endregion

                #region Reset SpPoint

                if (!Session.Character.GeneralLogs.Any(s => s.Timestamp == DateTime.Now && s.LogData == "World" && s.LogType == "Connection"))
                {
                    Session.Character.SpAdditionPoint += (int)(Session.Character.SpPoint / 100D * 20D);
                    Session.Character.SpPoint = 10000;
                }

                #endregion

                #region Other Character Stuffs

                Session.Character.Respawns = DAOFactory.Instance.RespawnDAO.LoadByCharacter(Session.Character.VisualId).ToList();
                Session.Character.StaticBonusList = DAOFactory.Instance.StaticBonusDAO.LoadByCharacterId(Session.Character.VisualId).ToList();
                Session.Character.Titles = DAOFactory.Instance.CharacterTitleDAO.LoadByCharacterId(Session.Character.VisualId).ToList();
                Session.Character.LoadInventory();
                Session.Character.LoadQuicklists();
                Session.Character.GenerateMiniland();

                #endregion

                #region Quests

                //if (!DAOFactory.Instance.CharacterQuestDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.IsMainQuest)
                //    && !DAOFactory.Instance.QuestLogDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.QuestId == 1997))
                //{
                //    CharacterQuestDTO firstQuest = new CharacterQuestDTO
                //    {
                //        CharacterId = Session.Character.CharacterId,
                //        QuestId = 1997,
                //        IsMainQuest = true
                //    };

                //    DAOFactory.Instance.CharacterQuestDAO.InsertOrUpdate(firstQuest);
                //}

                DAOFactory.Instance.CharacterQuestDAO.LoadByCharacterId(Session.Character.VisualId).ToList()
                    .ForEach(qst => Session.Character.Quests.Add((CharacterQuest)qst));

                #endregion

                #region Fix Partner Slots

                if (character.MaxPartnerCount < 3)
                {
                    character.MaxPartnerCount = 3;
                }

                #endregion

                #region Load Mates

                DAOFactory.Instance.MateDAO.LoadByCharacterId(Session.Character.VisualId).ToList().ForEach(s =>
                {
                    var mate = (Mate)s;
                    mate.Owner = Session.Character;
                    mate.GenerateMateTransportId();
                    mate.Monster = ServerManager.GetNpcMonster(s.NpcMonsterVNum);

                    Session.Character.Mates.Add(mate);
                });

                #endregion

                #region Load Permanent Buff

                Session.Character.LastPermBuffRefresh = DateTime.Now;

                #endregion

                #region CharacterLife

                Session.Character.Life = Observable.Interval(TimeSpan.FromMilliseconds(300))
                    .Subscribe(x => Session.Character.CharacterLife());

                #endregion

                #region Load Amulet

                Observable.Timer(TimeSpan.FromSeconds(1))
                    .Subscribe(o =>
                    {
                        ItemInstance amulet = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Amulet, InventoryType.Wear);

                        if (amulet?.ItemDeleteTime != null || amulet?.DurabilityPoint > 0)
                        {
                            Session.Character.AddBuff(new Buff(62, Session.Character.Level), Session.Character.BattleEntity);
                        }
                    });

                #endregion

                #region Load Static Buff

                foreach (StaticBuffDTO staticBuff in DAOFactory.Instance.StaticBuffDAO.LoadByCharacterId(Session.Character.VisualId))
                {
                    if (staticBuff.CardId != 319 /* Wedding */)
                    {
                        Session.Character.AddStaticBuff(staticBuff);
                    }
                }

                #endregion

                #region Enter the World

                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = Session.Account.AccountId,
                    CharacterId = Session.Character.VisualId,
                    IpAddress = Session.IpAddress,
                    LogData = "World",
                    LogType = "Connection",
                    Timestamp = DateTime.Now
                });

                Session.SendPacket("OK");

                CommunicationServiceClient.Instance.ConnectCharacter(ServerManager.Instance.WorldId, character.VisualId);

#warning remove this comment for allow 2auth system
                /*Session.Character.HasGodMode = true;
                Session.Character.Camouflage = true;
                Session.Character.Invisible = true;*/
                character.Channel = ServerManager.Instance;

                #endregion
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Failed selecting the character.", ex);
            }
            finally
            {
                // Suspicious activity detected -- kick!
                if (Session != null && (!Session.HasSelectedCharacter || Session.Character == null))
                {
                    Session.Disconnect();
                }
            }
        }

        #endregion
    }
}