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

        public Item Item => _item ?? (_item = (this as SpecialistInstance).IsPartnerEquipment && (this as BoxInstance).HoldingVNum != 0 ? ServerManager.GetItem((this as BoxInstance).HoldingVNum) : ServerManager.GetItem(ItemVNum));

        public ClientSession CharacterSession => ServerManager.Instance.GetSessionByCharacterId(CharacterId);
        #endregion

        #region Methods

        public ItemInstance DeepCopy() => (ItemInstance)MemberwiseClone();

        public string GenerateFStash() => $"f_stash {GenerateStashPacket()}";

        public string GenerateInventoryAdd()
        {
            switch (Type)
            {
                case InventoryType.Equipment:
                    return $"ivn 0 {Slot}.{ItemVNum}.{Rare}.{(Item.IsColored ? Design : Upgrade)}.{(this as SpecialistInstance)?.SpStoneUpgrade}";

                case InventoryType.Main:
                    return $"ivn 1 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Etc:
                    return $"ivn 2 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Miniland:
                    return $"ivn 3 {Slot}.{ItemVNum}.{Amount}";

                case InventoryType.Specialist:
                    return $"ivn 6 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.{(this as SpecialistInstance)?.SpStoneUpgrade}";

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

        public string GenerateReqInfo()
        {
            byte type = 0;
            if (BoundCharacterId != null && BoundCharacterId != CharacterId)
            {
                type = 2;
            }
            return $"r_info {ItemVNum} {type} {0}";
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