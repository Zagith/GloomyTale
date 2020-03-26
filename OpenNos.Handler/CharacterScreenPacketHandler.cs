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
using OpenNos.Core.Handling;
using OpenNos.Core.Interfaces.Packets.ClientPackets;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace OpenNos.Handler
{
    public class CharacterScreenPacketHandler
    {
        #region Instantiation

        public CharacterScreenPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /*public void CreateCharacterAction(ICharacterCreatePacket characterCreatePacket, ClassType classType)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            Logger.LogUserEvent("CREATECHARACTER", Session.GenerateIdentity(), $"[CreateCharacter]Name: {characterCreatePacket.Name} Slot: {characterCreatePacket.Slot} Gender: {characterCreatePacket.Gender} HairStyle: {characterCreatePacket.HairStyle} HairColor: {characterCreatePacket.HairColor}");

            if (characterCreatePacket.Slot <= 3
                && DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, characterCreatePacket.Slot) == null
                && characterCreatePacket.Name != null
                && (characterCreatePacket.Gender == GenderType.Male || characterCreatePacket.Gender == GenderType.Female)
                && (characterCreatePacket.HairStyle == HairStyleType.HairStyleA || (classType != ClassType.MartialArtist && characterCreatePacket.HairStyle == HairStyleType.HairStyleB))
                && Enumerable.Range(0, 10).Contains((byte)characterCreatePacket.HairColor)
                && (characterCreatePacket.Name.Length >= 4 && characterCreatePacket.Name.Length <= 14))
            {
                if (classType == ClassType.MartialArtist)
                {
                    IEnumerable<CharacterDTO> characterDTOs = DAOFactory.CharacterDAO.LoadByAccount(Session.Account.AccountId);

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

                if (DAOFactory.CharacterDAO.LoadByName(characterCreatePacket.Name) != null)
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
                    Gold = 15000,
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

                DAOFactory.CharacterDAO.InsertOrUpdate(ref characterDTO);

                if (classType == ClassType.MartialArtist)
                {
                    DAOFactory.CharacterQuestDAO.InsertOrUpdate(new CharacterQuestDTO
                    {
                        CharacterId = characterDTO.CharacterId,
                        QuestId = 6275,
                        IsMainQuest = false
                    });
                }

                if (classType != ClassType.MartialArtist)
                {
                    DAOFactory.CharacterQuestDAO.InsertOrUpdate(new CharacterQuestDTO
                    {
                        CharacterId = characterDTO.CharacterId,
                        QuestId = 1997,
                        IsMainQuest = true
                    });

                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 200 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 201 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 209 });

                    using (Inventory inventory = new Inventory(new Character(characterDTO)))
                    {
                        inventory.AddNewToInventory(15299, 1, InventoryType.Main);
                        inventory.ForEach(i => DAOFactory.ItemInstanceDAO.InsertOrUpdate(i));
                        LoadCharacters(characterCreatePacket.OriginalContent);
                    }
                }
                else
                {
                    for (short skillVNum = 1525; skillVNum <= 1539; skillVNum++)
                    {
                        DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO
                        {
                            CharacterId = characterDTO.CharacterId,
                            SkillVNum = skillVNum
                        });
                    }

                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 1565 });

                    #region passive Skills
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 22 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 25 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 29 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 38 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 42 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 46 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 50 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 54 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 57 });
                    DAOFactory.CharacterSkillDAO.InsertOrUpdate(new CharacterSkillDTO { CharacterId = characterDTO.CharacterId, SkillVNum = 60 });
                    #endregion
                    using (Inventory inventory = new Inventory(new Character(characterDTO)))
                    {
                        inventory.AddNewToInventory(5832, 1, InventoryType.Main, 5);
                        inventory.ForEach(i => DAOFactory.ItemInstanceDAO.InsertOrUpdate(i));
                        LoadCharacters(characterCreatePacket.OriginalContent);
                    }
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

            Logger.LogUserEvent("DELETECHARACTER", Session.GenerateIdentity(),
                $"[DeleteCharacter]Name: {characterDeletePacket.Slot}");
            AccountDTO account = DAOFactory.AccountDAO.LoadById(Session.Account.AccountId);
            if (account == null)
            {
                return;
            }

            if (account.Password.ToLower() == CryptographyBase.Sha512(characterDeletePacket.Password))
            {
                CharacterDTO character =
                    DAOFactory.CharacterDAO.LoadBySlot(account.AccountId, characterDeletePacket.Slot);
                if (character == null)
                {
                    return;
                }

                //DAOFactory.GeneralLogDAO.SetCharIdNull(Convert.ToInt64(character.CharacterId));
                DAOFactory.CharacterDAO.DeleteByPrimaryKey(account.AccountId, characterDeletePacket.Slot);
                LoadCharacters("");
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BAD_PASSWORD")}");
            }
        }*/


        #endregion
    }
}