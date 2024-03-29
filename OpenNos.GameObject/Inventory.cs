﻿/*
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
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject
{
    public class Inventory : ThreadSafeSortedList<Guid, ItemInstance>
    {
        #region Members

        private const short MAX_ITEM_AMOUNT = 999;

        private readonly object _lockObject = new object();

        #endregion

        #region Instantiation

        public Inventory(Character Character) => Owner = Character;

        #endregion

        #region Properties

        private Character Owner { get; }

        #endregion

        #region Methods

        private byte GetMaxSlot(InventoryType pocket)
        {
            if (Owner == null || Owner.Session == null)
            {
                return (byte)(pocket switch
                {
                    InventoryType.Miniland => 50 + Expensions[pocket],
                    InventoryType.Specialist => 45 + Expensions[pocket],
                    InventoryType.Costume => 60 + Expensions[pocket],
                    InventoryType.Wear => 17,
                    _ => 48 + Expensions[pocket]
                });
            }
            //TODO make this configurable
            return (byte)(pocket switch
            {
                InventoryType.Miniland => 50 + Expensions[pocket],
                InventoryType.Specialist => 45 + Expensions[pocket],
                InventoryType.Costume => 60 + Expensions[pocket],
                InventoryType.Wear => 17,
                _ => Owner.Session.Character.HaveBigBackPack() ? ServerManager.Instance.Configuration.BackpackSize + Expensions[pocket] : 48 + Expensions[pocket] + ((Owner.Session.Character.HaveBackpack() ? 1 : 0) * 12)
            });
        }

        public Dictionary<InventoryType, byte> Expensions { get; set; } = new Dictionary<InventoryType, byte>
        {
            { InventoryType.Costume, 0 },
            { InventoryType.Equipment, 0 },
            { InventoryType.Etc, 0 },
            { InventoryType.Miniland, 0 },
            { InventoryType.Main, 0 },
            { InventoryType.Specialist, 0 },
            { InventoryType.Wear, 0 },
            { InventoryType.Bazaar, 0 },
            { InventoryType.Warehouse, 0 },
            { InventoryType.FamilyWareHouse, 0 },
            { InventoryType.PetWarehouse, 0 },
            { InventoryType.FirstPartnerInventory, 0 },
            { InventoryType.SecondPartnerInventory, 0 },
            { InventoryType.ThirdPartnerInventory, 0 },
            { InventoryType.FourthPartnerInventory, 0 },
            { InventoryType.FifthPartnerInventory, 0 },
            { InventoryType.SixthPartnerInventory, 0 },
            { InventoryType.SeventhPartnerInventory, 0 },
            { InventoryType.EighthPartnerInventory, 0 },
            { InventoryType.NinthPartnerInventory, 0 },
            { InventoryType.TenthPartnerInventory, 0 },
        };
        public static ItemInstance InstantiateItemInstance(short vnum, long ownerId, short amount = 1)
        {
            ItemInstance newItem = new ItemInstance { ItemVNum = vnum, Amount = amount, CharacterId = ownerId };
            if (newItem.Item != null)
            {
                switch (newItem.Item.Type)
                {
                    case InventoryType.Miniland:
                        newItem.DurabilityPoint = newItem.Item.MinilandObjectPoint / 2;
                        break;

                    case InventoryType.Equipment:
                        newItem = newItem.Item.ItemType == ItemType.Specialist ? new ItemInstance
                        {
                            ItemVNum = vnum,
                            SpLevel = 1,
                            Amount = 1
                        } : new ItemInstance
                        {
                            ItemVNum = vnum,
                            Amount = 1,
                            DurabilityPoint = newItem.Item.Effect != 790 && (newItem.Item.EffectValue < 863 || newItem.Item.EffectValue > 872) && !new int[] { 3951, 3952, 3953, 3954, 3955, 7427 }.Contains(newItem.Item.EffectValue) ? newItem.Item.EffectValue : 0
                        };
                        break;
                }
            }

            // set default itemType
            if (newItem.Item != null)
            {
                newItem.Type = newItem.Item.Type;
            }

            return newItem;
        }

        public ItemInstance AddIntoBazaarInventory(InventoryType inventory, byte slot, short amount)
        {
            ItemInstance inv = LoadBySlotAndType(slot, inventory);
            if (inv == null || amount > inv.Amount)
            {
                return null;
            }

            ItemInstance invcopy = inv.DeepCopy();
            invcopy.Id = Guid.NewGuid();
            if (inv.Item.Type == InventoryType.Equipment)
            {
                for (short i = 0; i < 255; i++)
                {
                    if (LoadBySlotAndType(i, InventoryType.Bazaar) == null)
                    {
                        invcopy.Type = InventoryType.Bazaar;
                        invcopy.Slot = i;
                        invcopy.CharacterId = Owner.CharacterId;
                        DeleteFromSlotAndType(inv.Slot, inv.Type);
                        putItem(invcopy);
                        break;
                    }
                }
                Owner.Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(inventory, slot));
                return invcopy;
            }
            if (amount >= inv.Amount)
            {
                for (short i = 0; i < 255; i++)
                {
                    if (LoadBySlotAndType(i, InventoryType.Bazaar) == null)
                    {
                        invcopy.Type = InventoryType.Bazaar;
                        invcopy.Slot = i;
                        invcopy.CharacterId = Owner.CharacterId;
                        DeleteFromSlotAndType(inv.Slot, inv.Type);
                        putItem(invcopy);
                        break;
                    }
                }
                Owner.Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(inventory, slot));
                return invcopy;
            }

            invcopy.Amount = amount;
            inv.Amount -= amount;

            for (short i = 0; i < 255; i++)
            {
                if (LoadBySlotAndType(i, InventoryType.Bazaar) == null)
                {
                    invcopy.Type = InventoryType.Bazaar;
                    invcopy.Slot = i;
                    invcopy.CharacterId = Owner.CharacterId;
                    putItem(invcopy);
                    break;
                }
            }

            Owner.Session.SendPacket(inv.GenerateInventoryAdd());
            return invcopy;
        }

        public List<ItemInstance> AddNewToInventory(short vnum, short amount = 1, InventoryType? type = null, sbyte Rare = 0, byte Upgrade = 0, short Design = 0)
        {
            if (Owner != null)
            {
                ItemInstance newItem = InstantiateItemInstance(vnum, Owner.CharacterId, amount);
                newItem.Rare = Rare;
                newItem.Upgrade = Upgrade == 0 ? newItem.Item.ItemType == ItemType.Shell ? (byte)ServerManager.RandomNumber(50, 80) : Upgrade : Upgrade;
                newItem.Design = Design;
                return AddToInventory(newItem, type);
            }
            return new List<ItemInstance>();
        }

        public List<ItemInstance> AddToInventory(ItemInstance newItem, InventoryType? type = null)
        {
            List<ItemInstance> invlist = new List<ItemInstance>();
            if (Owner != null)
            {
                ItemInstance inv = null;

                // override type if necessary
                if (type.HasValue)
                {
                    newItem.Type = type.Value;
                }

                if (newItem.Item.Effect == 420 && newItem.Item.EffectValue == 911)
                {
                    newItem.BoundCharacterId = Owner.CharacterId;
                    newItem.DurabilityPoint = (int)newItem.Item.ItemValidTime;
                }

                // check if item can be stapled
                if (newItem.Type != InventoryType.Bazaar && (newItem.Item.Type == InventoryType.Etc || newItem.Item.Type == InventoryType.Main))
                {
                    List<ItemInstance> slotNotFull = Where(i => i.Type != InventoryType.Bazaar && i.Type != InventoryType.PetWarehouse && i.Type != InventoryType.Warehouse && i.Type != InventoryType.FamilyWareHouse && i.ItemVNum.Equals(newItem.ItemVNum) && i.Amount < MAX_ITEM_AMOUNT);
                    int freeslot = GetMaxSlot(newItem.Type) - CountLinq(s => s.Type == newItem.Type);
                    if (freeslot < 0) freeslot = 0;
                    if (newItem.Amount <= (freeslot * MAX_ITEM_AMOUNT) + slotNotFull.Sum(s => MAX_ITEM_AMOUNT - s.Amount))
                    {
                        foreach (ItemInstance slot in slotNotFull)
                        {
                            int max = slot.Amount + newItem.Amount;
                            max = max > MAX_ITEM_AMOUNT ? MAX_ITEM_AMOUNT : max;
                            newItem.Amount = (short)(slot.Amount + newItem.Amount - max);
                            newItem.Amount = (short)(newItem.Amount < 0 ? 0 : newItem.Amount);
                            Logger.LogUserEvent("ITEM_CREATE", Owner.GenerateIdentity(), $"IIId: {slot.Id} ItemVNum: {slot.ItemVNum} Amount: {max - slot.Amount} MapId: {Owner.MapInstance?.Map.MapId} MapX: {Owner.PositionX} MapY: {Owner.PositionY}");

                            slot.Amount = (short)max;
                            invlist.Add(slot);
                            Owner.Session?.SendPacket(slot.GenerateInventoryAdd());
                        }
                    }
                }
                if (newItem.Amount > 0)
                {
                    // create new item
                    short? freeSlot = newItem.Type == InventoryType.Wear ? (LoadBySlotAndType((short)newItem.Item.EquipmentSlot, InventoryType.Wear) == null
                                                                         ? (short?)newItem.Item.EquipmentSlot
                                                                         : null)
                                                                         : getFreeSlot(newItem.Type);
                    if (freeSlot.HasValue)
                    {
                        inv = AddToInventoryWithSlotAndType(newItem, newItem.Type, freeSlot.Value);
                        invlist.Add(inv);
                    }
                }
            }
            return invlist;
        }

        /// <summary>
        /// Add iteminstance to inventory with specified slot and type, iteminstance will be overridden.
        /// </summary>
        /// <param name="itemInstance"></param>
        /// <param name="type"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public ItemInstance AddToInventoryWithSlotAndType(ItemInstance itemInstance, InventoryType type, short slot)
        {
            if (Owner != null)
            {
                Logger.LogUserEvent("ITEM_CREATE", Owner.GenerateIdentity(), $"IIId: {itemInstance.Id} ItemVNum: {itemInstance.ItemVNum} Amount: {itemInstance.Amount} MapId: {Owner.MapInstance?.Map.MapId} MapX: {Owner.PositionX} MapY: {Owner.PositionY}");

                itemInstance.Slot = slot;
                itemInstance.Type = type;
                itemInstance.CharacterId = Owner.CharacterId;

                if (ContainsKey(itemInstance.Id))
                {
                    Logger.Error(new InvalidOperationException("Cannot add the same ItemInstance twice to inventory."));
                    return null;
                }

                string inventoryPacket = itemInstance.GenerateInventoryAdd();
                if (!string.IsNullOrEmpty(inventoryPacket))
                {
                    Owner.Session?.SendPacket(inventoryPacket);
                }

                if (Any(s => s.Slot == slot && s.Type == type))
                {
                    return null;
                }
                this[itemInstance.Id] = itemInstance;
                return itemInstance;
            }
            return null;
        }

        //public int BackpackSize() => DEFAULT_BACKPACK_SIZE + ((Owner.HaveBackpack() ? 1 : 0) * 12);

        public bool CanAddItem(short itemVnum) => canAddItem(ServerManager.GetItem(itemVnum).Type);

        public int CountItem(int itemVNum) => Where(s => s.ItemVNum == itemVNum && s.Type != InventoryType.Wear && s.Type != InventoryType.FamilyWareHouse && s.Type != InventoryType.Bazaar && s.Type != InventoryType.Warehouse && s.Type != InventoryType.PetWarehouse).Sum(i => i.Amount);

        public int CountItemInAnInventory(InventoryType inv) => CountLinq(s => s.Type == inv);

        public int CountBazaarItems()
        {
            List<BazaarItemLink> BazaarList = ServerManager.Instance.BazaarList.GetAllItems();
            return CountLinq(s => s.Type == InventoryType.Bazaar && BazaarList.FirstOrDefault(b => b.BazaarItem.ItemInstanceId == s.Id) is BazaarItemLink bz && (bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration).AddDays(bz.BazaarItem.MedalUsed ? 30 : 7) - DateTime.Now).TotalMinutes > 0);
        }

        public Tuple<short, InventoryType> DeleteById(Guid id)
        {
            if (Owner != null)
            {
                Tuple<short, InventoryType> removedPlace;
                ItemInstance inv = this[id];

                if (inv != null)
                {
                    removedPlace = new Tuple<short, InventoryType>(inv.Slot, inv.Type);
                    Remove(inv.Id);
                }
                else
                {
                    Logger.Error(new InvalidOperationException("Expected item wasn't deleted, Type or Slot did not match!"));
                    return null;
                }

                return removedPlace;
            }
            return null;
        }

        public void DeleteFromSlotAndType(short slot, InventoryType type)
        {
            if (Owner != null)
            {
                ItemInstance inv = FirstOrDefault(i => i.Slot.Equals(slot) && i.Type.Equals(type));

                if (inv != null)
                {
                    if (Owner.Session.Character.MinilandObjects.Any(s => s.ItemInstanceId == inv.Id))
                    {
                        return;
                    }

                    Remove(inv.Id);
                }
                else
                {
                    Logger.Error(new InvalidOperationException("Expected item wasn't deleted, Type or Slot did not match!"));
                }
            }
        }

        public void DepositItem(InventoryType inventory, byte slot, short amount, byte NewSlot, ref ItemInstance item, ref ItemInstance itemdest, bool PartnerBackpack)
        {
            return;
            if (item != null && amount <= item.Amount && amount > 0)
            {
                MoveItem(inventory, PartnerBackpack ? InventoryType.PetWarehouse : InventoryType.Warehouse, slot, amount, NewSlot, out item, out itemdest);
                Owner.Session.SendPacket(item != null ? item.GenerateInventoryAdd() : UserInterfaceHelper.Instance.GenerateInventoryRemove(inventory, slot));

                if (itemdest != null)
                {
                    Owner.Session.SendPacket(PartnerBackpack ? itemdest.GeneratePStash() : itemdest.GenerateStash());
                }
            }
        }

        public bool EnoughPlace(List<ItemInstance> itemInstances)
        {
            Dictionary<InventoryType, int> place = new Dictionary<InventoryType, int>();
            foreach (IGrouping<short, ItemInstance> itemgroup in itemInstances.GroupBy(s => s.ItemVNum))
            {
                if (itemgroup.FirstOrDefault()?.Type is InventoryType type)
                {
                    List<ItemInstance> listitem = Where(i => i.Type == type);
                    if (!place.ContainsKey(type))
                    {
                        place.Add(type, (type != InventoryType.Miniland ? GetMaxSlot(type) : 50) - listitem.Count);
                    }

                    int amount = itemgroup.Sum(s => s.Amount);
                    int rest = amount % (type == InventoryType.Equipment ? 1 : 999);
                    bool needanotherslot = listitem.Where(s => s.ItemVNum == itemgroup.Key).Sum(s => MAX_ITEM_AMOUNT - s.Amount) <= rest;
                    place[type] -= (amount / (type == InventoryType.Equipment ? 1 : 999)) + (needanotherslot ? 1 : 0);

                    if (place[type] < 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void FDepositItem(InventoryType inventory, byte slot, short amount, byte newSlot, ref ItemInstance item, ref ItemInstance itemdest)
        {
            if (item != null && amount <= item.Amount && amount > 0 && item.Item.IsTradable && !item.IsBound)
            {
                FamilyCharacter fhead = Owner.Family?.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                if (fhead == null)
                {
                    return;
                }
                MoveItem(inventory, InventoryType.FamilyWareHouse, slot, amount, newSlot, out item, out itemdest);
                itemdest.CharacterId = fhead.CharacterId;
                DAOFactory.ItemInstanceDAO.InsertOrUpdate(itemdest);
                Owner.Family.SendPacket(item != null ? item.GenerateInventoryAdd() : UserInterfaceHelper.Instance.GenerateInventoryRemove(inventory, slot));
                if (itemdest != null)
                {
                    Owner.Family.SendPacket(itemdest.GenerateFStash());
                    Owner.Family?.InsertFamilyLog(FamilyLogType.WareHouseAdded, Owner.Name, message: $"{itemdest.ItemVNum}|{amount}");
                    DeleteById(itemdest.Id);
                }
            }
        }

        public ItemInstance GetItemInstanceById(Guid id) => this[id];

        public ItemInstance LoadBySlotAndType(short slot, InventoryType type)
        {
            ItemInstance retItem = null;

            try
            {
                lock (_lockObject)
                {
                    retItem = SingleOrDefault(i => i.Slot.Equals(slot) && i.Type.Equals(type));
                }
            }
            catch (InvalidOperationException ioEx)
            {
                Logger.LogUserEventError(nameof(LoadBySlotAndType), Owner?.Session?.GenerateIdentity(), "Multiple items in slot, Splitting...", ioEx);

                bool isFirstItem = true;

                foreach (ItemInstance item in Where(i => i.Slot.Equals(slot) && i.Type.Equals(type)))
                {
                    if (isFirstItem)
                    {
                        retItem = item;
                        isFirstItem = false;
                        continue;
                    }

                    ItemInstance itemInstance = FirstOrDefault(i => i.Slot.Equals(slot) && i.Type.Equals(type));

                    if (itemInstance != null)
                    {
                        short? freeSlot = getFreeSlot(type);

                        if (freeSlot.HasValue)
                        {
                            itemInstance.Slot = freeSlot.Value;
                        }
                        else
                        {
                            Remove(itemInstance.Id);
                        }
                    }
                }
            }
            return retItem;
        }

        public T LoadByVNum<T>(short vNum) where T : ItemInstance => (T)FirstOrDefault(i => i.ItemVNum.Equals(vNum));

        /// <summary>
        /// Moves one item from one <see cref="Inventory"/> to another. Example: Equipment &lt;-&gt; Wear,
        /// Equipment &lt;-&gt; Costume, Equipment &lt;-&gt; Specialist
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="targetSlot"></param>
        /// <param name="wear"></param>
        public ItemInstance MoveInInventory(short sourceSlot, InventoryType sourceType, InventoryType targetType, short? targetSlot = null, bool wear = true)
        {
            ItemInstance sourceInstance = LoadBySlotAndType(sourceSlot, sourceType);

            if (sourceInstance == null && wear)
            {
                Logger.Error(new InvalidOperationException("SourceInstance to move does not exist."));
                return null;
            }
            if (Owner != null && sourceInstance != null)
            {
                if (targetSlot.HasValue)
                {
                    if (wear)
                    {
                        // swap
                        ItemInstance targetInstance = LoadBySlotAndType(targetSlot.Value, targetType);

                        sourceInstance.Slot = targetSlot.Value;
                        sourceInstance.Type = targetType;

                        targetInstance.Slot = sourceSlot;
                        targetInstance.Type = sourceType;
                    }
                    else
                    {
                        // move source to target
                        short? freeTargetSlot = getFreeSlot(targetType);
                        if (freeTargetSlot.HasValue)
                        {
                            sourceInstance.Slot = freeTargetSlot.Value;
                            sourceInstance.Type = targetType;
                        }
                    }

                    return sourceInstance;
                }

                // check for free target slot
                short? nextFreeSlot;
                switch (targetType)
                {
                    case InventoryType.FirstPartnerInventory:
                    case InventoryType.SecondPartnerInventory:
                    case InventoryType.ThirdPartnerInventory:
                    case InventoryType.FourthPartnerInventory:
                    case InventoryType.FifthPartnerInventory:
                    case InventoryType.SixthPartnerInventory:
                    case InventoryType.SeventhPartnerInventory:
                    case InventoryType.EighthPartnerInventory:
                    case InventoryType.NinthPartnerInventory:
                    case InventoryType.TenthPartnerInventory:
                    case InventoryType.Wear:
                        nextFreeSlot = (LoadBySlotAndType((short)sourceInstance.Item.EquipmentSlot, targetType) == null
                        ? (short)sourceInstance.Item.EquipmentSlot
                        : (short)-1);
                        break;

                    default:
                        nextFreeSlot = getFreeSlot(targetType);
                        break;
                }
                if (nextFreeSlot.HasValue)
                {
                    sourceInstance.Type = targetType;
                    sourceInstance.Slot = nextFreeSlot.Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return sourceInstance;
        }

        public void MoveItem(InventoryType sourcetype, InventoryType desttype, short sourceSlot, short amount, short destinationSlot, out ItemInstance sourceInventory, out ItemInstance destinationInventory)
        {
            Logger.LogUserEvent("ITEM_MOVE", Owner.GenerateIdentity(), $"SourceType: {sourcetype.ToString()} DestType: {desttype.ToString()} SourceSlot: {sourceSlot} Amount: {amount} DestSlot: {destinationSlot}");

            // Load source and destination slots
            sourceInventory = LoadBySlotAndType(sourceSlot, sourcetype);
            destinationInventory = LoadBySlotAndType(destinationSlot, desttype);
            if (sourceInventory != null && amount <= sourceInventory.Amount)
            {
                if (destinationInventory == null)
                {
                    if (sourceInventory.Amount == amount)
                    {
                        sourceInventory.Slot = destinationSlot;
                        sourceInventory.Type = desttype;
                    }
                    else
                    {
                        ItemInstance itemDest = sourceInventory.DeepCopy();
                        sourceInventory.Amount -= amount;
                        itemDest.Amount = amount;
                        itemDest.Type = desttype;
                        itemDest.Id = Guid.NewGuid();
                        AddToInventoryWithSlotAndType(itemDest, desttype, destinationSlot);
                    }
                }
                else
                {
                    if (destinationInventory.ItemVNum == sourceInventory.ItemVNum && (byte)sourceInventory.Item.Type != 0)
                    {
                        if (destinationInventory.Amount + amount > MAX_ITEM_AMOUNT)
                        {
                            int saveItemCount = destinationInventory.Amount;
                            destinationInventory.Amount = MAX_ITEM_AMOUNT;
                            sourceInventory.Amount = (short)(saveItemCount + sourceInventory.Amount - MAX_ITEM_AMOUNT);
                        }
                        else
                        {
                            destinationInventory.Amount += amount;
                            sourceInventory.Amount -= amount;

                            // item with amount of 0 should be removed
                            if (sourceInventory.Amount == 0)
                            {
                                DeleteFromSlotAndType(sourceInventory.Slot, sourceInventory.Type);
                            }
                        }
                    }
                    else
                    {
                        if (destinationInventory.Type == sourceInventory.Type || destinationInventory.Item.Type == sourcetype)
                        {
                            // add and remove save inventory
                            destinationInventory = takeItem(destinationInventory.Slot, destinationInventory.Type);
                            if (destinationInventory == null)
                            {
                                return;
                            }

                            destinationInventory.Slot = sourceSlot;
                            destinationInventory.Type = sourcetype;
                        }
                        else
                        {
                            if (getFreeSlot(destinationInventory.Item.Type) is short freeSlot)
                            {
                                destinationInventory.Slot = freeSlot;
                                destinationInventory.Type = destinationInventory.Item.Type;
                                Owner.Session.SendPacket(destinationInventory.GenerateInventoryAdd());

                                // add and remove save inventory
                                destinationInventory = takeItem(destinationInventory.Slot, destinationInventory.Type);
                                if (destinationInventory == null)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                return;
                            }
                        }

                        sourceInventory = takeItem(sourceInventory.Slot, sourceInventory.Type);
                        if (sourceInventory == null)
                        {
                            return;
                        }

                        sourceInventory.Slot = destinationSlot;
                        sourceInventory.Type = desttype;
                        putItem(destinationInventory);
                        putItem(sourceInventory);
                    }
                }
            }
            sourceInventory = LoadBySlotAndType(sourceSlot, sourcetype);
            destinationInventory = LoadBySlotAndType(destinationSlot, desttype);
        }

        public void RemoveItemAmount(int vnum, int amount = 1)
        {
            if (Owner != null)
            {
                int remainingAmount = amount;

                foreach (ItemInstance inventory in Where(s => s.ItemVNum == vnum && s.Type != InventoryType.Wear && s.Type != InventoryType.Bazaar && s.Type != InventoryType.Warehouse && s.Type != InventoryType.PetWarehouse && s.Type != InventoryType.FamilyWareHouse).OrderBy(i => i.Slot))
                {
                    if (remainingAmount > 0)
                    {
                        if (inventory.Amount > remainingAmount)
                        {
                            // Amount completely removed
                            inventory.Amount -= (short)remainingAmount;
                            remainingAmount = 0;
                            Owner.Session.SendPacket(inventory.GenerateInventoryAdd());
                        }
                        else
                        {
                            // Amount partly removed
                            remainingAmount -= inventory.Amount;
                            DeleteById(inventory.Id);
                            Owner.Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(inventory.Type, inventory.Slot));
                        }
                    }
                    else
                    {
                        // Amount to remove reached
                        break;
                    }
                }
            }
        }

        public void RemoveItemFromInventory(Guid id, short amount = 1)
        {
            if (Owner != null)
            {
                ItemInstance inv = FirstOrDefault(i => i.Id.Equals(id));
                if (inv != null)
                {
                    inv.Amount -= amount;
                    if (inv.Amount <= 0)
                    {
                        Owner.Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(inv.Type, inv.Slot));
                        Remove(inv.Id);
                        return;
                    }
                    Owner.Session.SendPacket(inv.GenerateInventoryAdd());
                }
            }
        }

        /// <summary>
        /// Reorders item in given inventorytype
        /// </summary>
        /// <param name="session"></param>
        /// <param name="inventoryType"></param>
        public void Reorder(ClientSession session, InventoryType inventoryType)
        {
            List<ItemInstance> itemsByInventoryType = new List<ItemInstance>();
            switch (inventoryType)
            {
                case InventoryType.Costume:
                    itemsByInventoryType = Where(s => s.Type == InventoryType.Costume).OrderBy(s => s.ItemVNum).ToList();
                    break;

                case InventoryType.Specialist:
                    itemsByInventoryType = Where(s => s.Type == InventoryType.Specialist).OrderBy(s => s.Item.LevelJobMinimum).ToList();
                    break;

                default:
                    itemsByInventoryType = Where(s => s.Type == inventoryType).OrderBy(s => s.Item.Price).ToList();
                    break;
            }
            generateClearInventory(inventoryType);
            for (short i = 0; i < itemsByInventoryType.Count; i++)
            {
                ItemInstance item = itemsByInventoryType[i];
                item.Slot = i;
                this[item.Id].Slot = i;
                session.SendPacket(item.GenerateInventoryAdd());
            }
        }

        private bool canAddItem(InventoryType type) => Owner != null && getFreeSlot(type).HasValue;

        private void generateClearInventory(InventoryType type)
        {
            if (Owner != null)
            {
                for (short i = 0; i < ServerManager.Instance.Configuration.BackpackSize; i++)
                {
                    if (LoadBySlotAndType(i, type) != null)
                    {
                        Owner.Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(type, i));
                    }
                }
            }
        }

        /// <summary>
        /// Gets free slots in given inventory type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>short?; based on given inventory type</returns>
        private short? getFreeSlot(InventoryType type)
        {
            IEnumerable<int> itemInstanceSlotsByType = Where(i => i.Type == type).OrderBy(i => i.Slot).Select(i => (int)i.Slot);
            IEnumerable<int> instanceSlotsByType = itemInstanceSlotsByType as int[] ?? itemInstanceSlotsByType.ToArray();
            int backpackSize = GetMaxSlot(type);
            int maxRange = (type != InventoryType.Miniland ? backpackSize : 50) + 1;
            int? nextFreeSlot = instanceSlotsByType.Any() ? Enumerable.Range(0, maxRange).Except(instanceSlotsByType).Cast<int?>().FirstOrDefault() : 0;
            return (short?)nextFreeSlot < (type != InventoryType.Miniland ? backpackSize : 50) ? (short?)nextFreeSlot : null;
        }

        /// <summary>
        /// Puts a Single ItemInstance to the Inventory
        /// </summary>
        /// <param name="itemInstance"></param>
        private void putItem(ItemInstance itemInstance) => this[itemInstance.Id] = itemInstance;

        /// <summary>
        /// Takes a Single Inventory including ItemInstance from the List and removes it.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private ItemInstance takeItem(short slot, InventoryType type)
        {
            ItemInstance itemInstance = SingleOrDefault(i => i.Slot == slot && i.Type == type);
            if (itemInstance != null)
            {
                Remove(itemInstance.Id);
                return itemInstance;
            }
            return null;
        }

        #endregion
    }
}