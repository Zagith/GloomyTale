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
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using GloomyTale.GameObject.Networking;
using System.Text;

namespace GloomyTale.GameObject.Items.Instance
{
    public class ItemInstance : ItemInstanceDTO
    {
        #region Members

        private Random _random;
        private Item _item;
        private long _transportId;

        #endregion

        #region Instantiation

        public ItemInstance() => _random = new Random();

        public ItemInstance(short vNum, short amount)
        {
            ItemVNum = vNum;
            Amount = amount;
            Type = Item.Type;
            _random = new Random();
        }
        

        #endregion

        #region Properties

        public bool IsBound => BoundCharacterId.HasValue && Item.ItemType != ItemType.Armor && Item.ItemType != ItemType.Weapon;

        public Item Item => _item ?? (_item = IsPartnerEquipment && HoldingVNum != 0 ? ServerManager.GetItem(HoldingVNum) : ServerManager.GetItem(ItemVNum));

        public long TransportId
        {
            get
            {
                if (_transportId == 0)
                {
                    // create transportId thru factory
                    _transportId = TransportFactory.Instance.GenerateTransportId();
                }

                return _transportId;
            }
        }

        #endregion

        #region Methods

        public ItemInstance DeepCopy() => (ItemInstance)MemberwiseClone();

        public string GenerateFStash() => $"f_stash {GenerateStashPacket()}";

        public string GenerateInventoryAdd()
        {
            switch (Type)
            {
                case InventoryType.Equipment:
                    return $"ivn 0 {Slot}.{ItemVNum}.{Rare}.{(Item.IsColored ? Design : Upgrade)}.{SpStoneUpgrade}";

                case InventoryType.Main:
                    return $"ivn 1 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Etc:
                    return $"ivn 2 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Miniland:
                    return $"ivn 3 {Slot}.{ItemVNum}.{Amount}";

                case InventoryType.Specialist:
                    return $"ivn 6 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.{SpStoneUpgrade}";

                case InventoryType.Costume:
                    return $"ivn 7 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.0";
            }
            return "";
        }

        public string GeneratePStash() => $"pstash {GenerateStashPacket()}";

        public string GenerateStash() => $"stash {GenerateStashPacket()}";

        public string GenerateStashPacket()
        {
            string packet = $"{Slot}.{ItemVNum}.{(byte)Item.Type}";
            switch (Item.Type)
            {
                case InventoryType.Equipment:
                    return packet + $".{Amount}.{Rare}.{Upgrade}";

                case InventoryType.Specialist:
                    var sp = this as SpecialistInstance;
                    return packet + $".{Upgrade}.{sp.SpStoneUpgrade}.0";

                default:
                    return packet + $".{Amount}.0.0";
            }
        }

        public string GeneratePslInfo()
        {
            PartnerSp partnerSp = new PartnerSp(this);

            return $"pslinfo {Item.VNum} {Item.Element} {Item.ElementRate} {Item.LevelJobMinimum} {Item.Speed} {Item.FireResistance} {Item.WaterResistance} {Item.LightResistance} {Item.DarkResistance}{partnerSp.GenerateSkills()}";
        }

        public string GenerateSlInfo(ClientSession session = null)
        {
            int freepoint = CharacterHelper.SPPoint(SpLevel, Upgrade) - SlDamage - SlHP - SlElement - SlDefence;
            int slElementShell = 0;
            int slHpShell = 0;
            int slDefenceShell = 0;
            int slHitShell = 0;

            if (session != null)
            {
                ItemInstance mainWeapon = session.Character?.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                ItemInstance secondaryWeapon = session.Character?.Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);

                List<ShellEffectDTO> effects = new List<ShellEffectDTO>();
                if (mainWeapon?.ShellEffects != null)
                {
                    effects.AddRange(mainWeapon.ShellEffects);
                }
                if (secondaryWeapon?.ShellEffects != null)
                {
                    effects.AddRange(secondaryWeapon.ShellEffects);
                }

                int GetShellWeaponEffectValue(ShellWeaponEffectType effectType)
                {
                    return effects?.Where(s => s.Effect == (byte)effectType)?.OrderByDescending(s => s.Value)?.FirstOrDefault()?.Value ?? 0;
                }

                slElementShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLElement) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
                slHpShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLHP) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
                slDefenceShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLDefence) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
                slHitShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLDamage) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal);
            }

            int slElement = CharacterHelper.SlPoint(SlElement, 2);
            int slHp = CharacterHelper.SlPoint(SlHP, 3);
            int slDefence = CharacterHelper.SlPoint(SlDefence, 1);
            int slHit = CharacterHelper.SlPoint(SlDamage, 0);

            StringBuilder skills = new StringBuilder();

            List<CharacterSkill> skillsSp = new List<CharacterSkill>();

            foreach (Skill ski in ServerManager.GetAllSkill().Where(ski => ski.Class == Item.Morph + 31 && ski.LevelMinimum <= SpLevel).OrderBy(s => s.SkillVNum))
            {
                skillsSp.Add(new CharacterSkill
                {
                    SkillVNum = ski.SkillVNum,
                    CharacterId = CharacterId
                });
            }

            byte spdestroyed = 0;

            if (Rare == -2)
            {
                spdestroyed = 1;
            }

            if (skillsSp.Count == 0)
            {
                skills.Append("-1");
            }
            else
            {
                short firstSkillVNum = skillsSp[0].SkillVNum;

                for (int i = 1; i < 11; i++)
                {
                    if (skillsSp.Count >= i + 1 && skillsSp[i].SkillVNum <= firstSkillVNum + 10)
                    {
                        if (skills.Length > 0)
                        {
                            skills.Append(".");
                        }

                        skills.Append($"{skillsSp[i].SkillVNum}");
                    }
                }
            }

            // 10 9 8 '0 0 0 0'<- bonusdamage bonusarmor bonuselement bonushpmp its after upgrade and
            // 3 first values are not important

            return $"slinfo {(Type == InventoryType.Wear || Type == InventoryType.Specialist || Type == InventoryType.Equipment ? "0" : "2")} {ItemVNum} {Item.Morph} {SpLevel} {Item.LevelJobMinimum} {Item.ReputationMinimum} 0 {Item.Speed} 0 0 0 0 0 {Item.SpType} {Item.FireResistance} {Item.WaterResistance} {Item.LightResistance} {Item.DarkResistance} {XP} {CharacterHelper.SPXPData[SpLevel == 0 ? 0 : SpLevel - 1]} {skills} {TransportId} {freepoint} {slHit} {slDefence} {slElement} {slHp} {Upgrade} 0 0 {spdestroyed} {slHitShell} {slDefenceShell} {slElementShell} {slHpShell} {SpStoneUpgrade} {SpDamage} {SpDefence} {SpElement} {SpHP} {SpFire} {SpWater} {SpLight} {SpDark}";
        }

        public string GenerateReqInfo()
        {
            byte type = 0;
            if (BoundCharacterId != null && BoundCharacterId != CharacterId)
            {
                type = 2;
            }
            return $"r_info {ItemVNum} {type} {0}";
        }

        public void PerfectSP(ClientSession session)
        {
            short[] upsuccess = { 50, 40, 30, 20, 10 };

            int[] goldprice = { 5000, 10000, 20000, 50000, 100000 };
            byte[] stoneprice = { 1, 2, 3, 4, 5 };
            short stonevnum;
            byte upmode = 1;

            switch (Item.Morph)
            {
                case 2:
                case 6:
                case 9:
                case 12:
                    stonevnum = 2514;
                    break;

                case 3:
                case 4:
                case 14:
                    stonevnum = 2515;
                    break;

                case 5:
                case 11:
                case 15:
                    stonevnum = 2516;
                    break;

                case 10:
                case 13:
                case 7:
                    stonevnum = 2517;
                    break;

                case 17:
                case 18:
                case 19:
                    stonevnum = 2518;
                    break;

                case 20:
                case 21:
                case 22:
                    stonevnum = 2519;
                    break;

                case 23:
                case 24:
                case 25:
                    stonevnum = 2520;
                    break;

                case 26:
                case 27:
                case 28:
                    stonevnum = 2521;
                    break;

                default:
                    return;
            }
            if (SpStoneUpgrade > 99)
            {
                return;
            }
            if (SpStoneUpgrade > 80)
            {
                upmode = 5;
            }
            else if (SpStoneUpgrade > 60)
            {
                upmode = 4;
            }
            else if (SpStoneUpgrade > 40)
            {
                upmode = 3;
            }
            else if (SpStoneUpgrade > 20)
            {
                upmode = 2;
            }

            if (IsFixed)
            {
                return;
            }
            if (session.Character.Gold < goldprice[upmode - 1])
            {
                return;
            }
            if (session.Character.Inventory.CountItem(stonevnum) < stoneprice[upmode - 1])
            {
                return;
            }

            ItemInstance specialist = session.Character.Inventory.GetItemInstanceById(Id);

            int rnd = ServerManager.RandomNumber();
            if (rnd < upsuccess[upmode - 1])
            {
                byte type = (byte)ServerManager.RandomNumber(0, 16), count = 1;
                if (upmode == 4)
                {
                    count = 2;
                }
                if (upmode == 5)
                {
                    count = (byte)ServerManager.RandomNumber(3, 6);
                }

                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005), session.Character.PositionX, session.Character.PositionY);

                if (type < 3)
                {
                    specialist.SpDamage += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 0));
                }
                else if (type < 6)
                {
                    specialist.SpDefence += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 0));
                }
                else if (type < 9)
                {
                    specialist.SpElement += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 0));
                }
                else if (type < 12)
                {
                    specialist.SpHP += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 0));
                }
                else if (type == 12)
                {
                    specialist.SpFire += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 0));
                }
                else if (type == 13)
                {
                    specialist.SpWater += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 0));
                }
                else if (type == 14)
                {
                    specialist.SpLight += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 0));
                }
                else if (type == 15)
                {
                    specialist.SpDark += count;
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 0));
                }
                specialist.SpStoneUpgrade++;
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 11));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 0));
            }
            session.SendPacket(specialist.GenerateInventoryAdd());
            session.Character.Gold -= goldprice[upmode - 1];
            session.SendPacket(session.Character.GenerateGold());
            session.Character.Inventory.RemoveItemAmount(stonevnum, stoneprice[upmode - 1]);
            session.SendPacket("shop_end 1");
        }

        public void UpgradeSp(ClientSession session, UpgradeProtection protect)
        {
            if (Upgrade >= 15)
            {
                return;
            }
            
            int[] goldprice = { 200000, 200000, 200000, 200000, 200000, 500000, 500000, 500000, 500000, 500000, 1000000, 1000000, 1000000, 1000000, 1000000 };
            byte[] feather = { 3, 5, 8, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70 };
            byte[] fullmoon = { 1, 3, 5, 7, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };
            byte[] soul = { 2, 4, 6, 8, 10, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };
            const short featherVnum = 2282;
            const short fullmoonVnum = 1030;
            const short limitatedFullmoonVnum = 9131;
            const short greenSoulVnum = 2283;
            const short redSoulVnum = 2284;
            const short blueSoulVnum = 2285;
            const short dragonSkinVnum = 2511;
            const short dragonBloodVnum = 2512;
            const short dragonHeartVnum = 2513;
            const short blueScrollVnum = 1363;
            const short redScrollVnum = 1364;

            if (!session.HasCurrentMapInstance)
            {
                return;
            }
            short itemToRemove = 2283;
            if (protect != UpgradeProtection.Event)
            {
                if (session.Character.Inventory.CountItem(fullmoonVnum) < fullmoon[Upgrade] && session.Character.Inventory.CountItem(limitatedFullmoonVnum) < fullmoon[Upgrade])
                {
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(fullmoonVnum).Name, fullmoon[Upgrade])), 10));
                    return;
                }
                if (session.Character.Inventory.CountItem(featherVnum) < feather[Upgrade])
                {
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(featherVnum).Name, feather[Upgrade])), 10));
                    return;
                }
                if (session.Character.Gold < goldprice[Upgrade])
                {
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                    return;
                }

                if (Upgrade < 5)
                {
                    if (SpLevel > 20)
                    {
                        if (Item.Morph <= 16)
                        {
                            if (session.Character.Inventory.CountItem(greenSoulVnum) < soul[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(greenSoulVnum).Name, soul[Upgrade])), 10));
                                return;
                            }
                            if (protect == UpgradeProtection.Protected)
                            {
                                if (session.Character.Inventory.CountItem(blueScrollVnum) < 1)
                                {
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    return;
                                }
                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                                session.SendPacket("shop_end 2");
                            }
                        }
                        else
                        {
                            if (session.Character.Inventory.CountItem(dragonSkinVnum) < soul[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(dragonSkinVnum).Name, soul[Upgrade])), 10));
                                return;
                            }
                            if (protect == UpgradeProtection.Protected)
                            {
                                if (session.Character.Inventory.CountItem(blueScrollVnum) < 1)
                                {
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    return;
                                }
                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                                session.SendPacket("shop_end 2");
                            }
                            itemToRemove = dragonSkinVnum;
                        }
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LVL_REQUIRED"), 21), 11));
                        return;
                    }
                }
                else if (Upgrade < 10)
                {
                    if (SpLevel > 40)
                    {
                        if (Item.Morph <= 16)
                        {
                            if (session.Character.Inventory.CountItem(redSoulVnum) < soul[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(redSoulVnum).Name, soul[Upgrade])), 10));
                                return;
                            }
                            if (protect == UpgradeProtection.Protected)
                            {
                                if (session.Character.Inventory.CountItem(blueScrollVnum) < 1)
                                {
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    return;
                                }
                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                                session.SendPacket("shop_end 2");
                            }
                            itemToRemove = redSoulVnum;
                        }
                        else
                        {
                            if (session.Character.Inventory.CountItem(dragonBloodVnum) < soul[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(dragonBloodVnum).Name, soul[Upgrade])), 10));
                                return;
                            }
                            if (protect == UpgradeProtection.Protected)
                            {
                                if (session.Character.Inventory.CountItem(blueScrollVnum) < 1)
                                {
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    return;
                                }
                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                                session.SendPacket("shop_end 2");
                            }
                            itemToRemove = dragonBloodVnum;
                        }
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LVL_REQUIRED"), 41), 11));
                        return;
                    }
                }
                else if (Upgrade < 15)
                {
                    if (SpLevel > 50)
                    {
                        if (Item.Morph <= 16)
                        {
                            if (session.Character.Inventory.CountItem(blueSoulVnum) < soul[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueSoulVnum).Name, soul[Upgrade])), 10));
                                return;
                            }
                            if (protect == UpgradeProtection.Protected)
                            {
                                if (session.Character.Inventory.CountItem(redScrollVnum) < 1)
                                {
                                    return;
                                }
                                session.Character.Inventory.RemoveItemAmount(redScrollVnum);
                                session.SendPacket("shop_end 2");
                            }
                            itemToRemove = blueSoulVnum;
                        }
                        else
                        {
                            if (session.Character.Inventory.CountItem(dragonHeartVnum) < soul[Upgrade])
                            {
                                return;
                            }
                            if (protect == UpgradeProtection.Protected)
                            {
                                if (session.Character.Inventory.CountItem(redScrollVnum) < 1)
                                {
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(redScrollVnum).Name, 1)), 10));
                                    return;
                                }
                                session.Character.Inventory.RemoveItemAmount(redScrollVnum);
                                session.SendPacket("shop_end 2");
                            }
                            itemToRemove = dragonHeartVnum;
                        }
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LVL_REQUIRED"), 51), 11));
                        return;
                    }
                }
                session.Character.Gold -= goldprice[Upgrade];
                // remove feather and fullmoon before upgrading
                session.Character.Inventory.RemoveItemAmount(featherVnum, feather[Upgrade]);
                if (session.Character.Inventory.CountItem(limitatedFullmoonVnum) >= fullmoon[Upgrade])
                {
                    session.Character.Inventory.RemoveItemAmount(limitatedFullmoonVnum, fullmoon[Upgrade]);
                }
                else
                {
                    session.Character.Inventory.RemoveItemAmount(fullmoonVnum, fullmoon[Upgrade]);
                }
            }
            else
            {
                session.SendPacket("shop_end 2");
                itemToRemove = -1;
                short eventScrollVnum = -1;
                switch (ItemVNum)
                {
                    case 900: // Pyjama
                        eventScrollVnum = 5207;
                        break;
                    case 907: // Chicken
                        eventScrollVnum = 5107;
                        break;
                    case 4099: // Pirate
                        eventScrollVnum = 5519;
                        break;
                }
                if (eventScrollVnum < 0)
                {
                    return;
                }
                if (session.Character.Inventory.CountItem(eventScrollVnum) > 0)
                {
                    session.Character.Inventory.RemoveItemAmount(eventScrollVnum);
                }
                else
                {
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(eventScrollVnum).Name, 1)), 10));
                    return;
                }
            }

            ItemInstance wearable = session.Character.Inventory.GetItemInstanceById(Id);
            ItemInstance inventory = session.Character.Inventory.GetItemInstanceById(Id);
            int rnd = ServerManager.RandomNumber();
            if (rnd < ItemHelper.SpDestroyRate[Upgrade])
            {
                if (protect == UpgradeProtection.Protected || protect == UpgradeProtection.Event)
                {
                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.PositionX, session.Character.PositionY);
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED_SAVED"), 11));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED_SAVED"), 0));
                }
                else
                {
                    session.Character.Inventory.RemoveItemAmount(itemToRemove, soul[Upgrade]);
                    wearable.Rare = -2;
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_DESTROYED"), 11));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_DESTROYED"), 0));
                    session.SendPacket(wearable.GenerateInventoryAdd());
                }
            }
            else if (rnd < ItemHelper.SpUpFailRate[Upgrade])
            {
                if (protect == UpgradeProtection.Protected || protect == UpgradeProtection.Event)
                {
                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.PositionX, session.Character.PositionY);
                }
                else
                {
                    session.Character.Inventory.RemoveItemAmount(itemToRemove, soul[Upgrade]);
                }
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"), 11));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"), 0));
            }
            else
            {
                if (protect == UpgradeProtection.Protected || protect == UpgradeProtection.Event)
                {
                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.PositionX, session.Character.PositionY);
                }
                session.Character.Inventory.RemoveItemAmount(itemToRemove, soul[Upgrade]);
                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005), session.Character.PositionX, session.Character.PositionY);
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 12));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 0));
                wearable.Upgrade++;
                if (wearable.Upgrade > 8)
                {
                    session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                }
                session.SendPacket(wearable.GenerateInventoryAdd());
            }
            session.SendPacket(session.Character.GenerateGold());
            session.SendPacket(session.Character.GenerateEq());
            session.SendPacket("shop_end 1");
        }        

        public void OptionItem(ClientSession session, short cellonVNum)
        {
            if (EquipmentSerialId == Guid.Empty)
            {
                EquipmentSerialId = Guid.NewGuid();
            }
            if (Item.MaxCellon <= CellonOptions.Count)
            {
                session.SendPacket($"info {Language.Instance.GetMessageFromKey("MAX_OPTIONS")}");
                session.SendPacket("shop_end 1");
                return;
            }
            if (session.Character.Inventory.CountItem(cellonVNum) > 0)
            {
                byte dataIndex = 0;
                int goldAmount = 0;
                switch (cellonVNum)
                {
                    case 1017:
                        dataIndex = 0;
                        goldAmount = 700;
                        break;

                    case 1018:
                        dataIndex = 1;
                        goldAmount = 1400;
                        break;

                    case 1019:
                        dataIndex = 2;
                        goldAmount = 3000;
                        break;

                    case 1020:
                        dataIndex = 3;
                        goldAmount = 5000;
                        break;

                    case 1021:
                        dataIndex = 4;
                        goldAmount = 10000;
                        break;

                    case 1022:
                        dataIndex = 5;
                        goldAmount = 20000;
                        break;

                    case 1023:
                        dataIndex = 6;
                        goldAmount = 32000;
                        break;

                    case 1024:
                        dataIndex = 7;
                        goldAmount = 58000;
                        break;

                    case 1025:
                        dataIndex = 8;
                        goldAmount = 95000;
                        break;

                    case 1026:
                        return;
                        //no data known, not implemented in the client at all right now
                        //dataIndex = 9;
                        //break;
                }

                if (Item.MaxCellonLvl > dataIndex && session.Character.Gold >= goldAmount)
                {
                    short[][] minimumData = {
                    new short[] { 30, 50, 5, 8, 0, 0 },             //lv1
                    new short[] { 120, 150, 14, 16, 0, 0 },         //lv2
                    new short[] { 220, 280, 22, 28, 0, 0 },         //lv3
                    new short[] { 330, 350, 30, 38, 0, 0 },         //lv4
                    new short[] { 430, 450, 40, 50, 0, 0 },         //lv5
                    new short[] { 600, 600, 55, 65, 1, 1 },         //lv6
                    new short[] { 800, 800, 75, 75, 8, 11 },        //lv7
                    new short[] { 1000, 1000, 100, 100, 13, 21 },   //lv8
                    new short[] { 1100, 1100, 110, 110, 14, 22 },   //lv9
                    new short[] { 0, 0, 0, 0, 0, 0 }                //lv10 (NOT EXISTING!)
                    };
                    short[][] maximumData = {
                    new short[] { 100, 150, 10, 15, 0, 0 },         //lv1
                    new short[] { 200, 250, 20, 25, 0, 0 },         //lv1
                    new short[] { 300, 330, 28, 35, 0, 0 },         //lv1
                    new short[] { 400, 420, 38, 45, 0, 0 },         //lv1
                    new short[] { 550, 550, 50, 60, 0, 0 },         //lv1
                    new short[] { 750, 750, 70, 80, 7, 10 },        //lv1
                    new short[] { 1000, 1000, 90,90, 12, 20 },      //lv1
                    new short[] { 1300, 1300, 120, 120, 17, 35 },   //lv1
                    new short[] { 1500, 1500, 135, 135, 21, 45 },   //lv1
                    new short[] { 0, 0, 0, 0, 0, 0 }                //lv10 (NOT EXISTING!)
                    };

                    short[] generateOption()
                    {
                        byte option = 0;
                        if (dataIndex < 5)
                        {
                            option = (byte)ServerManager.RandomNumber(0, 4);
                        }
                        else
                        {
                            option = (byte)ServerManager.RandomNumber(0, 6);
                        }

                        if (CellonOptions.Any(s => s.Type == (CellonOptionType)option))
                        {
                            return new short[] { -1, -1 };
                        }

                        return new short[] { option, (short)ServerManager.RandomNumber(minimumData[dataIndex][option], maximumData[dataIndex][option] + 1) };
                    }

                    short[] value = generateOption();
                    Logger.Log.LogUserEvent("OPTION", session.GenerateIdentity(), $"[OptionItem]Serial: {EquipmentSerialId} Type: {value[0]} Value: {value[1]}");
                    if (value[0] != -1)
                    {
                        CellonOptionDTO cellonOptionDTO = new CellonOptionDTO
                        {
                            EquipmentSerialId = EquipmentSerialId,
                            Level = (byte)(dataIndex + 1),
                            Type = (CellonOptionType)value[0],
                            Value = value[1]
                        };

                        CellonOptions.Add(cellonOptionDTO);

                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), Rare), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), Rare), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3005), session.Character.PositionX, session.Character.PositionY);
                        session.SendPacket("shop_end 1");
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), Rare), 11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), Rare), 0));
                        session.SendPacket("shop_end 1");
                    }
                    session.Character.Inventory.RemoveItemAmount(cellonVNum);
                    session.Character.Gold -= goldAmount;
                    session.SendPacket(session.Character.GenerateGold());
                }
            }

            foreach (CellonOptionDTO effect in CellonOptions)
            {
                effect.EquipmentSerialId = EquipmentSerialId;
                effect.CellonOptionId = DAOFactory.Instance.CellonOptionDAO.InsertOrUpdate(effect).CellonOptionId;
            }
        }       

        public void RarifyBoxItem(ClientSession session, RarifyMode mode, RarifyProtection protection, bool isCommand = false, byte forceRare = 0)
        {
            const short goldprice = 5000;
            double rnd;
            byte[] rarifyRate = new byte[ItemHelper.RarifyRate.Length];
            ItemHelper.RarifyRate.CopyTo(rarifyRate, 0);

            if (session?.HasCurrentMapInstance == false)
            {
                return;
            }
            rnd = ServerManager.RandomNumber(0, 1000) / 10D;
            
            if (session != null)
            {
                switch (mode)
                {
                    case RarifyMode.Normal:
                        if (session.Character.Gold < goldprice)
                        {
                            return;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            void rarify(sbyte rarity, bool isHeroEquipmentDowngrade = false)
            {
                Rare = rarity;
                if (mode != RarifyMode.Drop)
                {
                    Logger.Log.LogUserEvent("GAMBLE", session.GenerateIdentity(), $"[RarifyItem]Protection: {protection.ToString()} IIId: {Id} ItemVnum: {ItemVNum} Result: Success");

                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey(isHeroEquipmentDowngrade ? "RARIFY_DOWNGRADE_SUCCESS" : "RARIFY_SUCCESS"), Rare), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey(isHeroEquipmentDowngrade ? "RARIFY_DOWNGRADE_SUCCESS" : "RARIFY_SUCCESS"), Rare), 0));
                    session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3005), session.Character.PositionX, session.Character.PositionY);
                    session.SendPacket("shop_end 1");
                }
            }

            if (forceRare != 0)
            {
                rarify((sbyte)forceRare);
                return;
            }
            if (rnd < rarifyRate[10] && !(protection == RarifyProtection.Scroll && Rare >= 8))
            {
                rarify(8);
            }
            if (rnd < rarifyRate[9] && !(protection == RarifyProtection.Scroll && Rare >= 7))
            {
                rarify(7);
            }
            else if (rnd < rarifyRate[8] && !(protection == RarifyProtection.Scroll && Rare >= 6))
            {
                rarify(6);
            }
            else if (rnd < rarifyRate[7] && !(protection == RarifyProtection.Scroll && Rare >= 5))
            {
                rarify(5);
            }
            else if (rnd < rarifyRate[6] && !(protection == RarifyProtection.Scroll && Rare >= 4))
            {
                rarify(4);
            }
            else if (rnd < rarifyRate[5] && !(protection == RarifyProtection.Scroll && Rare >= 3))
            {
                rarify(3);
            }
            else if (rnd < rarifyRate[4] && !(protection == RarifyProtection.Scroll && Rare >= 2))
            {
                rarify(2);
            }
            else if (rnd < rarifyRate[3] && !(protection == RarifyProtection.Scroll && Rare >= 1))
            {
                rarify(1);
            }
            else if (rnd < rarifyRate[2] && !(protection == RarifyProtection.Scroll && Rare >= 0))
            {
                rarify(0);
            }
            else if (rnd < rarifyRate[1] && !(protection == RarifyProtection.Scroll && Rare >= -1))
            {
                rarify(-1);
            }
            else if (rnd < rarifyRate[0] && !(protection == RarifyProtection.Scroll && Rare >= -2))
            {
                rarify(-2);
            }
            else if (mode != RarifyMode.Drop && session != null)
            {
                switch (protection)
                {
                    
                    case RarifyProtection.None:
                        session.Character.DeleteItemByItemInstanceId(Id);
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 0));
                        return;
                }
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 11));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 0));
                session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.PositionX, session.Character.PositionY);
                return;
            }
            if (mode != RarifyMode.Drop && session != null)
            {
                ItemInstance inventory = session.Character.Inventory.GetItemInstanceById(Id);
                if (inventory != null)
                {
                    session.SendPacket(inventory.GenerateInventoryAdd());
                }
            }
        }

        #endregion
    }
}