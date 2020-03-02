using GloomyTale.Core;
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Data.Interfaces;
using GloomyTale.Domain;
using GloomyTale.GameObject.Helpers;
using GloomyTale.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GloomyTale.GameObject.Items.Instance
{
    public class WearableInstance : ItemInstance, IWearableInstance
    {
        #region Members

        private List<CellonOptionDTO> _cellonOptions;
        private List<ShellEffectDTO> _shellEffects;

        #endregion

        #region Instantiation

        public WearableInstance()
        {
            new Random();
        }

        public WearableInstance(Guid id)
        {
            Id = id;
            new Random();
        }

        public WearableInstance(short vNum, byte amount) : base(vNum, amount)
        {
            new Random();
        }

        #endregion

        #region Properties

        public List<CellonOptionDTO> CellonOptions => _cellonOptions ?? (_cellonOptions = DAOFactory.Instance.CellonOptionDAO.GetOptionsByWearableInstanceId(EquipmentSerialId == Guid.Empty ? EquipmentSerialId = Guid.NewGuid() : EquipmentSerialId).ToList());

        public List<ShellEffectDTO> ShellEffects => _shellEffects ?? (_shellEffects = DAOFactory.Instance.ShellEffectDAO.LoadByEquipmentSerialId(EquipmentSerialId == Guid.Empty ? EquipmentSerialId = Guid.NewGuid() : EquipmentSerialId).ToList());

        public byte Ammo { get; set; }

        public byte Cellon { get; set; }

        public short CloseDefence { get; set; }

        public short Concentrate { get; set; }

        public short CriticalDodge { get; set; }

        public byte CriticalLuckRate { get; set; }

        public short CriticalRate { get; set; }

        public short DamageMaximum { get; set; }

        public short DamageMinimum { get; set; }

        public byte DarkElement { get; set; }

        public short DarkResistance { get; set; }

        public short DefenceDodge { get; set; }

        public short DistanceDefence { get; set; }

        public short DistanceDefenceDodge { get; set; }

        public bool IsPartnerEquipment { get; set; }

        public short ElementRate { get; set; }

        public byte FireElement { get; set; }

        public short FireResistance { get; set; }

        public short HitRate { get; set; }

        public short HP { get; set; }

        public bool IsEmpty { get; set; }

        public bool IsFixed { get; set; }

        public byte LightElement { get; set; }

        public short LightResistance { get; set; }

        public short MagicDefence { get; set; }

        public byte MaxElementRate { get; set; }

        public short MP { get; set; }

        public sbyte? ShellRarity { get; set; }

        public byte WaterElement { get; set; }

        public short WaterResistance { get; set; }

        public long XP { get; set; }

        #endregion

        #region Methods

        public string GenerateEInfo()
        {
            EquipmentType equipmentslot = Item.EquipmentSlot;
            ItemType itemType = Item.ItemType;
            byte itemClass = Item.Class;
            byte subtype = Item.ItemSubType;
            DateTime test = ItemDeleteTime ?? DateTime.Now;
            long time = ItemDeleteTime != null ? (long)test.Subtract(DateTime.Now).TotalSeconds : 0;
            long seconds = IsBound ? time : Item.ItemValidTime;
            if (seconds < 0)
            {
                seconds = 0;
            }
            
            switch (itemType)
            {
                case ItemType.Weapon:
                    switch (equipmentslot)
                    {
                        case EquipmentType.MainWeapon:
                            return $"e_info {(itemClass == 4 ? 1 : itemClass == 8 ? 5 : 0)} {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.SellToNpcPrice} {(IsPartnerEquipment ? $"{(this as BoxInstance)?.HoldingVNum}" : "-1")} {(ShellRarity == null ? "0" : $"{ShellRarity}")} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate("", (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}"; // Shell Rare, CharacterId, ShellEffectCount, ShellEffects

                        case EquipmentType.SecondaryWeapon:
                            return $"e_info {(itemClass <= 2 ? 1 : 0)} {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.SellToNpcPrice} {(IsPartnerEquipment ? $"{(this as BoxInstance)?.HoldingVNum}" : "-1")} {(ShellRarity == null ? "0" : $"{ShellRarity}")} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate("", (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}";
                    }
                    break;

                case ItemType.Armor:

                    return $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.SellToNpcPrice} {(IsPartnerEquipment ? $"{(this as BoxInstance)?.HoldingVNum}" : "-1")} {(ShellRarity == null ? "0" : $"{ShellRarity}")} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate("", (result, effect) => result += $"{((byte)effect.EffectLevel > 12 ? (byte)effect.EffectLevel - 12 : (byte)effect.EffectLevel)}.{(effect.Effect > 50 ? effect.Effect - 50 : effect.Effect)}.{(byte)effect.Value} ")}";

                case ItemType.Fashion:
                    switch (equipmentslot)
                    {
                        case EquipmentType.CostumeHat:
                            return $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.SellToNpcPrice} {(Item.ItemValidTime == 0 ? -1 : 0)} 2 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}";

                        case EquipmentType.CostumeSuit:
                            return $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.SellToNpcPrice} {(Item.ItemValidTime == 0 ? -1 : 0)} 1 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}"; // 1 = IsCosmetic -1 = no shells

                        default:
                            return $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.SellToNpcPrice} {Upgrade} 0 -1"; // after Item.Price theres TimesConnected {(Item.ItemValidTime == 0 ? -1 : Item.ItemValidTime / (3600))}
                    }

                case ItemType.Jewelery:
                    switch (equipmentslot)
                    {
                        case EquipmentType.Amulet:
                            if (DurabilityPoint > 0)
                            {
                                return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {DurabilityPoint} 100 0 {Item.SellToNpcPrice}";
                            }
                            return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {seconds * 10} 0 0 {Item.SellToNpcPrice}";

                        case EquipmentType.Fairy:
                            return $"e_info 4 {ItemVNum} {Item.Element} {ElementRate + Item.ElementRate} 0 0 0 0 0"; // last IsNosmall

                        default:
                            string cellon = "";
                            foreach (CellonOptionDTO option in CellonOptions)
                            {
                                cellon += $" {(byte)option.Type} {option.Level} {option.Value}";
                            }
                            return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {Item.MaxCellonLvl} {Item.MaxCellon} {CellonOptions.Count} {Item.SellToNpcPrice}{cellon}";
                    }
                case ItemType.Specialist:
                    return $"e_info 8 {ItemVNum}";

                case ItemType.Box:
                    if (GetType() != typeof(BoxInstance))
                    {
                        return $"e_info 7 {ItemVNum} 0";
                    }
                    var box = (BoxInstance)this;
                    switch (subtype)
                    {
                        case 0:
                        case 1:
                            return box.HoldingVNum == 0 ?
                                $"e_info 7 {ItemVNum} 0" : $"e_info 7 {ItemVNum} 1 {box.HoldingVNum} {box.SpLevel} {XP} 100 {box.SpDamage} {box.SpDefence}";

                        case 2:
                            Item spitem = ServerManager.GetItem(box.HoldingVNum);
                            return box.HoldingVNum == 0 ?
                                $"e_info 7 {ItemVNum} 0" :
                                $"e_info 7 {ItemVNum} 1 {box.HoldingVNum} {box.SpLevel} {XP} {CharacterHelper.SPXPData[box.SpLevel == 0 ? 0 : box.SpLevel - 1]} {Upgrade} {CharacterHelper.SlPoint(box.SlDamage, 0)} {CharacterHelper.SlPoint(box.SlDefence, 1)} {CharacterHelper.SlPoint(box.SlElement, 2)} {CharacterHelper.SlPoint(box.SlHP, 3)} {CharacterHelper.SPPoint(box.SpLevel, Upgrade) - box.SlDamage - box.SlHP - box.SlElement - box.SlDefence} {box.SpStoneUpgrade} {spitem.FireResistance} {spitem.WaterResistance} {spitem.LightResistance} {spitem.DarkResistance} {box.SpDamage} {box.SpDefence} {box.SpElement} {box.SpHP} {box.SpFire} {box.SpWater} {box.SpLight} {box.SpDark}";

                        case 4:
                            return box.HoldingVNum == 0 ?
                                $"e_info 11 {ItemVNum} 0" :
                                $"e_info 11 {ItemVNum} 1 {box.HoldingVNum}";

                        case 5:
                            Item fairyitem = ServerManager.GetItem(box.HoldingVNum);
                            return box.HoldingVNum == 0 ?
                                $"e_info 12 {ItemVNum} 0" :
                                $"e_info 12 {ItemVNum} 1 {box.HoldingVNum} {ElementRate + fairyitem.ElementRate}";

                        default:
                            return $"e_info 8 {ItemVNum} {Design} {Rare}";
                    }

                case ItemType.Shell:
                    return $"e_info 9 {ItemVNum} {Upgrade} {Rare} {Item.SellToNpcPrice} {ShellEffects.Count}{ShellEffects.Aggregate("", (current, option) => current + $" {((byte)option.EffectLevel > 12 ? (byte)option.EffectLevel - 12 : (byte)option.EffectLevel)}.{(option.Effect > 50 ? option.Effect - 50 : option.Effect)}.{option.Value}")}";
            }
            return "";
        }

        public void GenerateHeroicShell(RarifyProtection protection, bool forced = false)
        {
            if (!Item.IsHeroic || Rare <= 0)
            {
                return;
            }
            byte shellType = (byte)(Item.ItemType == ItemType.Armor ? 11 : 10);
            if (shellType != 11 && shellType != 10)
            {
                return;
            }
            ShellEffects.Clear();
            int shellLevel = Item.LevelMinimum == 25 ? 101 : 106;
            ShellEffects.AddRange(ShellGeneratorHelper.Instance.GenerateShell(shellType, Rare == 8 ? 7 : Rare, shellLevel));
        }

        public void RarifyItem(RarifyMode mode, RarifyProtection protection, bool isCommand = false, byte forceRare = 0)
        {
            if (CharacterSession == null)
            {
                return;
            }

            const short goldprice = 500;
            const double reducedpricefactor = 0.5;
            const double reducedchancefactor = 1.1;
            const byte cella = 5;
            const int cellaVnum = 1014;
            const int scrollVnum = 1218;
            double rnd;
            byte[] rarifyRate = new byte[ItemHelper.RarifyRate.Length];
            ItemHelper.RarifyRate.CopyTo(rarifyRate, 0);

            if (CharacterSession?.HasCurrentMapInstance == false)
            {
                return;
            }
            if (mode != RarifyMode.Drop || Item.ItemType == ItemType.Shell)
            {
                rarifyRate[0] = 0;
                rarifyRate[1] = 0;
                rarifyRate[2] = 0;
                rnd = ServerManager.RandomNumber(0, 80);
            }
            else
            {
                rnd = ServerManager.RandomNumber(0, 1000) / 10D;
            }
            if (protection == RarifyProtection.RedAmulet ||
                protection == RarifyProtection.HeroicAmulet ||
                protection == RarifyProtection.RandomHeroicAmulet)
            {
                for (byte i = 0; i < rarifyRate.Length; i++)
                {
                    rarifyRate[i] = (byte)(rarifyRate[i] * reducedchancefactor);
                }
            }
            if (CharacterSession != null)
            {
                switch (mode)
                {
                    case RarifyMode.Free:
                        break;

                    case RarifyMode.Success:
                        if (Item.IsHeroic && Rare >= 8 || !Item.IsHeroic && Rare <= 7)
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("ALREADY_MAX_RARE"), 10));
                            return;
                        }
                        Rare += 1;
                        SetRarityPoint();
                        ItemInstance inventory = CharacterSession?.Character.Inventory.GetItemInstanceById(Id);
                        if (inventory != null)
                        {
                            CharacterSession.SendPacket(inventory.GenerateInventoryAdd());
                        }
                        return;

                    case RarifyMode.Reduced:
                        if (CharacterSession.Character.Gold < (long)(goldprice * reducedpricefactor))
                        {
                            return;
                        }
                        if (CharacterSession.Character.Inventory.CountItem(cellaVnum) < cella * reducedpricefactor)
                        {
                            return;
                        }
                        CharacterSession.Character.Inventory.RemoveItemAmount(cellaVnum, (int)(cella * reducedpricefactor));
                        CharacterSession.Character.Gold -= (long)(goldprice * reducedpricefactor);
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
                        break;

                    case RarifyMode.Normal:
                        if (CharacterSession.Character.Gold < goldprice)
                        {
                            return;
                        }
                        if (CharacterSession.Character.Inventory.CountItem(cellaVnum) < cella)
                        {
                            return;
                        }
                        if (protection == RarifyProtection.Scroll && !isCommand
                            && CharacterSession.Character.Inventory.CountItem(scrollVnum) < 1)
                        {
                            return;
                        }
                        if ((protection == RarifyProtection.Scroll || protection == RarifyProtection.BlueAmulet || protection == RarifyProtection.RedAmulet) && !isCommand && Item.IsHeroic)
                        {
                            CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IS_HEROIC"), 0));
                            return;
                        }
                        if ((protection == RarifyProtection.HeroicAmulet ||
                             protection == RarifyProtection.RandomHeroicAmulet) && !Item.IsHeroic)
                        {
                            CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_NOT_HEROIC"), 0));
                            return;
                        }
                        if (Item.IsHeroic && Rare == 8)
                        {
                            rarify(7, true);
                            ItemInstance inv = CharacterSession?.Character.Inventory.GetItemInstanceById(Id);
                            if (inv != null)
                            {
                                CharacterSession.SendPacket(inv.GenerateInventoryAdd());
                            }
                            return;
                        }

                        if (protection == RarifyProtection.Scroll && !isCommand)
                        {
                            CharacterSession.Character.Inventory.RemoveItemAmount(scrollVnum);
                            CharacterSession.SendPacket("shop_end 2");
                        }
                        CharacterSession.Character.Gold -= goldprice;
                        CharacterSession.Character.Inventory.RemoveItemAmount(cellaVnum, cella);
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
                        break;

                    case RarifyMode.Drop:
                        break;

                    case RarifyMode.HeroEquipmentDowngrade:
                        {
                            rarify(7, true);
                            return;
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            void rarify(sbyte rarity, bool isHeroEquipmentDowngrade = false)
            {
                Rare = rarity;
                if (mode != RarifyMode.Drop)
                {
                    Logger.Log.LogUserEvent("GAMBLE", CharacterSession.GenerateIdentity(), $"[RarifyItem]Protection: {protection.ToString()} IIId: {Id} ItemVnum: {ItemVNum} Result: Success");

                    CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey(isHeroEquipmentDowngrade ? "RARIFY_DOWNGRADE_SUCCESS" : "RARIFY_SUCCESS"), Rare), 12));
                    CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey(isHeroEquipmentDowngrade ? "RARIFY_DOWNGRADE_SUCCESS" : "RARIFY_SUCCESS"), Rare), 0));
                    CharacterSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3005), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                    CharacterSession.SendPacket("shop_end 1");
                }
                SetRarityPoint();

                if (!isHeroEquipmentDowngrade)
                {
                    GenerateHeroicShell(protection);
                }
            }

            if (forceRare != 0)
            {
                rarify((sbyte)forceRare);
                return;
            }
            if (Item.IsHeroic && protection != RarifyProtection.None)
            {
                if (rnd < rarifyRate[10])
                {
                    rarify(8);
                    if (mode != RarifyMode.Drop && CharacterSession != null)
                    {
                        ItemInstance inventory = CharacterSession.Character.Inventory.GetItemInstanceById(Id);
                        if (inventory != null)
                        {
                            CharacterSession.SendPacket(inventory.GenerateInventoryAdd());
                        }
                    }
                    return;
                }
            }
            /*if (rnd < rare[10] && !(protection == RarifyProtection.Scroll && Rare >= 8))
            {
                rarify(8);
            }*/
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
            else if (Rare < 1 && Item.ItemType == ItemType.Shell)
            {
                Rare = 1;
            }
            else if (mode != RarifyMode.Drop && CharacterSession != null)
            {
                switch (protection)
                {
                    case RarifyProtection.BlueAmulet:
                    case RarifyProtection.RedAmulet:
                    case RarifyProtection.HeroicAmulet:
                    case RarifyProtection.RandomHeroicAmulet:
                        CharacterSession.Character.RemoveBuff(62);
                        ItemInstance amulet = CharacterSession.Character.Inventory.LoadBySlotAndType((short)EquipmentType.Amulet, InventoryType.Wear);
                        amulet.DurabilityPoint -= 1;
                        if (amulet.DurabilityPoint <= 0)
                        {
                            CharacterSession.Character.DeleteItemByItemInstanceId(amulet.Id);
                            CharacterSession.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateEquipment());
                        }
                        else
                        {
                            CharacterSession.Character.AddBuff(new Buff(62, CharacterSession.Character.Level), CharacterSession.Character.BattleEntity);
                        }
                        break;
                    case RarifyProtection.None:
                        CharacterSession.Character.DeleteItemByItemInstanceId(Id);
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 11));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 0));
                        return;
                }
                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 11));
                CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 0));
                CharacterSession.CurrentMapInstance.Broadcast(CharacterSession.Character.GenerateEff(3004), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                return;
            }
            if (mode != RarifyMode.Drop && CharacterSession != null)
            {
                ItemInstance inventory = CharacterSession.Character.Inventory.GetItemInstanceById(Id);
                if (inventory != null)
                {
                    CharacterSession.SendPacket(inventory.GenerateInventoryAdd());
                }
            }
        }

        public void SetRarityPoint()
        {
            switch (Item.EquipmentSlot)
            {
                case EquipmentType.MainWeapon:
                case EquipmentType.SecondaryWeapon:
                    {
                        int point = CharacterHelper.RarityPoint(Rare, Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum, false);
                        Concentrate = 0;
                        HitRate = 0;
                        DamageMinimum = 0;
                        DamageMaximum = 0;
                        if (Rare >= 0)
                        {
                            for (int i = 0; i < point; i++)
                            {
                                int rndn = ServerManager.RandomNumber(0, 3);
                                if (rndn == 0)
                                {
                                    Concentrate++;
                                    HitRate++;
                                }
                                else
                                {
                                    DamageMinimum++;
                                    DamageMaximum++;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i > Rare * 10; i--)
                            {
                                DamageMinimum--;
                                DamageMaximum--;
                            }
                        }
                    }
                    break;

                case EquipmentType.Armor:
                    {
                        int point = CharacterHelper.RarityPoint(Rare, Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum, true);
                        DefenceDodge = 0;
                        DistanceDefenceDodge = 0;
                        DistanceDefence = 0;
                        MagicDefence = 0;
                        CloseDefence = 0;
                        double NewDistanceDefence = 0;
                        double NewMagicDefence = 0;
                        double NewCloseDefence = 0;
                        if (Rare >= 0)
                        {
                            for (int i = 0; i < point; i++)
                            {
                                int rndn = ServerManager.RandomNumber(0, 5);
                                if (rndn == 0)
                                {
                                    DefenceDodge++;
                                    DistanceDefenceDodge++;
                                }
                                else
                                {
                                    NewDistanceDefence = NewDistanceDefence + 0.9;
                                    NewMagicDefence = NewMagicDefence + 0.35;
                                    NewCloseDefence = NewCloseDefence + 0.95;
                                }
                            }
                            DistanceDefence = (short)NewDistanceDefence;
                            MagicDefence = (short)NewMagicDefence;
                            CloseDefence = (short)NewCloseDefence;
                        }
                        else
                        {
                            for (int i = 0; i > Rare * 10; i--)
                            {
                                DistanceDefence--;
                                MagicDefence--;
                                CloseDefence--;
                            }
                        }
                    }
                    break;
            }
        }

        public void Sum(WearableInstance itemToSum)
        {
            if (CharacterSession == null)
            {
                return;
            }
            if (!CharacterSession.HasCurrentMapInstance)
            {
                return;
            }
            if (Upgrade < 6)
            {
                short[] upsuccess = { 100, 100, 85, 70, 50, 20 };
                int[] goldprice = { 1500, 3000, 6000, 12000, 24000, 48000 };
                short[] sand = { 5, 10, 15, 20, 25, 30 };
                const int sandVnum = 1027;
                if (Upgrade + itemToSum.Upgrade < 6 && ((itemToSum.Item.EquipmentSlot == EquipmentType.Gloves && Item.EquipmentSlot == EquipmentType.Gloves) || (Item.EquipmentSlot == EquipmentType.Boots && itemToSum.Item.EquipmentSlot == EquipmentType.Boots)))
                {
                    if (CharacterSession.Character.Gold < goldprice[Upgrade])
                    {
                        return;
                    }
                    if (CharacterSession.Character.Inventory.CountItem(sandVnum) < sand[Upgrade])
                    {
                        return;
                    }
                    CharacterSession.Character.Inventory.RemoveItemAmount(sandVnum, (byte)sand[Upgrade]);
                    CharacterSession.Character.Gold -= goldprice[Upgrade];

                    int rnd = ServerManager.RandomNumber();
                    if (rnd < upsuccess[Upgrade + itemToSum.Upgrade])
                    {
                        Logger.Log.LogUserEvent("SUM_ITEM", CharacterSession.GenerateIdentity(), $"[SumItem]ItemId {Id} ItemToSumId: {itemToSum.Id} Upgrade: {Upgrade} ItemToSumUpgrade: {itemToSum.Upgrade} Result: Success");

                        Upgrade += (byte)(itemToSum.Upgrade + 1);
                        DarkResistance += (short)(itemToSum.DarkResistance + itemToSum.Item.DarkResistance);
                        LightResistance += (short)(itemToSum.LightResistance + itemToSum.Item.LightResistance);
                        WaterResistance += (short)(itemToSum.WaterResistance + itemToSum.Item.WaterResistance);
                        FireResistance += (short)(itemToSum.FireResistance + itemToSum.Item.FireResistance);
                        CharacterSession.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                        CharacterSession.SendPacket($"pdti 10 {ItemVNum} 1 27 {Upgrade} 0");
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 0));
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 12));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, CharacterSession.Character.CharacterId, 1324));
                        CharacterSession.SendPacket(GenerateInventoryAdd());
                    }
                    else
                    {
                        Logger.Log.LogUserEvent("SUM_ITEM", CharacterSession.GenerateIdentity(), $"[SumItem]ItemId {Id} ItemToSumId: {itemToSum.Id} Upgrade: {Upgrade} ItemToSumUpgrade: {itemToSum.Upgrade} Result: Fail");

                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_FAILED"), 0));
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_FAILED"), 11));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, CharacterSession.Character.CharacterId, 1332));
                        CharacterSession.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                        CharacterSession.Character.DeleteItemByItemInstanceId(Id);
                    }
                    CharacterSession.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, CharacterSession.Character.CharacterId), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                    CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
                    CharacterSession.SendPacket("shop_end 1");
                }
            }
        }

        public void UpgradeItem(UpgradeMode mode, UpgradeProtection protection, bool isCommand = false)
        {
            if (CharacterSession == null)
            {
                return;
            }
            if (!CharacterSession.HasCurrentMapInstance)
            {
                return;
            }
            if (Upgrade < 10)
            {
                byte[] upfail;
                byte[] upfix;
                int[] goldprice;
                short[] cella;
                byte[] gem;

                if (Rare >= 8)
                {
                    upfix = ItemHelper.R8ItemUpgradeFixRate;
                    upfail = ItemHelper.R8ItemUpgradeFailRate;

                    goldprice = new[] { 5000, 15000, 30000, 100000, 300000, 800000, 1500000, 4000000, 7000000, 10000000 };
                    cella = new short[] { 40, 100, 160, 240, 320, 440, 560, 760, 960, 1200 };
                    gem = new byte[] { 2, 2, 4, 4, 6, 2, 2, 4, 4, 6 };
                }
                else
                {
                    upfix = ItemHelper.ItemUpgradeFixRate;
                    upfail = ItemHelper.ItemUpgradeFailRate;

                    goldprice = new[] { 500, 1500, 3000, 10000, 30000, 80000, 150000, 400000, 700000, 1000000 };
                    cella = new short[] { 20, 50, 80, 120, 160, 220, 280, 380, 480, 600 };
                    gem = new byte[] { 1, 1, 2, 2, 3, 1, 1, 2, 2, 3 };
                }

                const short cellaVnum = 1014;
                const short gemVnum = 1015;
                const short gemFullVnum = 1016;
                const double reducedpricefactor = 0.5;
                const short normalScrollVnum = 1218;
                const short goldScrollVnum = 5369;

                if (IsFixed)
                {
                    CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_IS_FIXED"), 10));
                    CharacterSession.SendPacket("shop_end 1");
                    return;
                }
                switch (mode)
                {
                    case UpgradeMode.Free:
                        break;

                    case UpgradeMode.Reduced:
                        if (CharacterSession.Character.Gold < (long)(goldprice[Upgrade] * reducedpricefactor))
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        if (CharacterSession.Character.Inventory.CountItem(cellaVnum) < cella[Upgrade] * reducedpricefactor)
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(cellaVnum).Name, cella[Upgrade] * reducedpricefactor), 10));
                            return;
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand && CharacterSession.Character.Inventory.CountItem(goldScrollVnum) < 1)
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(goldScrollVnum).Name, cella[Upgrade] * reducedpricefactor), 10));
                            return;
                        }
                        if (Upgrade < 5)
                        {
                            if (CharacterSession.Character.Inventory.CountItem(gemVnum) < gem[Upgrade])
                            {
                                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            CharacterSession.Character.Inventory.RemoveItemAmount(gemVnum, gem[Upgrade]);
                        }
                        else
                        {
                            if (CharacterSession.Character.Inventory.CountItem(gemFullVnum) < gem[Upgrade])
                            {
                                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemFullVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            CharacterSession.Character.Inventory.RemoveItemAmount(gemFullVnum, gem[Upgrade]);
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand)
                        {
                            CharacterSession.Character.Inventory.RemoveItemAmount(goldScrollVnum);
                            CharacterSession.SendPacket("shop_end 2");
                        }
                        CharacterSession.Character.Gold -= (long)(goldprice[Upgrade] * reducedpricefactor);
                        CharacterSession.Character.Inventory.RemoveItemAmount(cellaVnum, (int)(cella[Upgrade] * reducedpricefactor));
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
                        break;

                    case UpgradeMode.Normal:
                        if (CharacterSession.Character.Inventory.CountItem(cellaVnum) < cella[Upgrade])
                        {
                            return;
                        }
                        if (CharacterSession.Character.Gold < goldprice[Upgrade])
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand && CharacterSession.Character.Inventory.CountItem(normalScrollVnum) < 1)
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(normalScrollVnum).Name, 1), 10));
                            return;
                        }
                        if (Upgrade < 5)
                        {
                            if (CharacterSession.Character.Inventory.CountItem(gemVnum) < gem[Upgrade])
                            {
                                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            CharacterSession.Character.Inventory.RemoveItemAmount(gemVnum, gem[Upgrade]);
                        }
                        else
                        {
                            if (CharacterSession.Character.Inventory.CountItem(gemFullVnum) < gem[Upgrade])
                            {
                                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemFullVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            CharacterSession.Character.Inventory.RemoveItemAmount(gemFullVnum, gem[Upgrade]);
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand)
                        {
                            CharacterSession.Character.Inventory.RemoveItemAmount(normalScrollVnum);
                            CharacterSession.SendPacket("shop_end 2");
                        }
                        CharacterSession.Character.Inventory.RemoveItemAmount(cellaVnum, cella[Upgrade]);
                        CharacterSession.Character.Gold -= goldprice[Upgrade];
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
                        break;
                }
                var wearable = CharacterSession.Character.Inventory.LoadByItemInstance<WearableInstance>(Id);
                int rnd = ServerManager.RandomNumber();
                if (Rare == 8)
                {
                    if (rnd < upfail[Upgrade])
                    {
                        Logger.Log.LogUserEvent("UPGRADE_ITEM", CharacterSession.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fail");

                        if (protection == UpgradeProtection.None)
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                            CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                            CharacterSession.Character.DeleteItemByItemInstanceId(Id);
                        }
                        else
                        {
                            CharacterSession.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterSession.Character.CharacterId, 3004), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"), 11));
                            CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                        }
                    }
                    else if (rnd < upfix[Upgrade])
                    {
                        Logger.Log.LogUserEvent("UPGRADE_ITEM", CharacterSession.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fixed");

                        CharacterSession.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterSession.Character.CharacterId, 3004), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                        wearable.IsFixed = true;
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 0));
                    }
                    else
                    {
                        Logger.Log.LogUserEvent("UPGRADE_ITEM", CharacterSession.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Success");

                        CharacterSession.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterSession.Character.CharacterId, 3005), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 0));
                        wearable.Upgrade++;
                        if (wearable.Upgrade > 4)
                        {
                            CharacterSession.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, CharacterSession.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                        }
                        CharacterSession.SendPacket(wearable.GenerateInventoryAdd());
                    }
                }
                else
                {
                    if (rnd < upfix[Upgrade])
                    {
                        Logger.Log.LogUserEvent("UPGRADE_ITEM", CharacterSession.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fixed");

                        CharacterSession.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterSession.Character.CharacterId, 3004), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                        wearable.IsFixed = true;
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 0));
                    }
                    else if (rnd < upfail[Upgrade] + upfix[Upgrade])
                    {
                        Logger.Log.LogUserEvent("UPGRADE_ITEM", CharacterSession.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fail");

                        if (protection == UpgradeProtection.None)
                        {
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                            CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                            CharacterSession.Character.DeleteItemByItemInstanceId(Id);
                        }
                        else
                        {
                            CharacterSession.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterSession.Character.CharacterId, 3004), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"), 11));
                            CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                        }
                    }
                    else
                    {
                        Logger.Log.LogUserEvent("UPGRADE_ITEM", CharacterSession.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Success");

                        CharacterSession.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterSession.Character.CharacterId, 3005), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 0));
                        wearable.Upgrade++;
                        if (wearable.Upgrade > 4)
                        {
                            CharacterSession.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, CharacterSession.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                        }
                        CharacterSession.SendPacket(wearable.GenerateInventoryAdd());
                    }
                }
                CharacterSession.SendPacket("shop_end 1");
            }
        }

        public void ConvertToPartnerEquipment()
        {
            if (CharacterSession == null)
            {
                return;
            }
            const int sandVnum = 1027;
            long goldprice = 2000 + Item.LevelMinimum * 300;

            if (CharacterSession.Character.Gold >= goldprice && CharacterSession.Character.Inventory.CountItem(sandVnum) >= Item.LevelMinimum)
            {
                CharacterSession.Character.Inventory.RemoveItemAmount(sandVnum, Item.LevelMinimum);
                CharacterSession.Character.Gold -= goldprice;

                IsPartnerEquipment = true;
                ShellEffects.Clear();
                ShellRarity = null;
                DAOFactory.Instance.ShellEffectDAO.DeleteByEquipmentSerialId(EquipmentSerialId);
                BoundCharacterId = null;
                var box = (BoxInstance)this;
                box.HoldingVNum = ItemVNum;

                switch (Item.EquipmentSlot)
                {
                    case EquipmentType.MainWeapon:
                    case EquipmentType.SecondaryWeapon:
                        switch (Item.Class)
                        {
                            case 2:
                                ItemVNum = 990;
                                break;
                            case 4:
                                ItemVNum = 991;
                                break;
                            case 8:
                                ItemVNum = 992;
                                break;
                        }
                        break;
                    case EquipmentType.Armor:
                        switch (Item.Class)
                        {
                            case 2:
                                ItemVNum = 997;
                                break;
                            case 4:
                                ItemVNum = 996;
                                break;
                            case 8:
                                ItemVNum = 995;
                                break;
                        }
                        break;
                }
                CharacterSession.SendPacket(GenerateInventoryAdd());
                CharacterSession.SendPacket("shop_end 1");
            }
        }

        public void OptionItem(short cellonVNum)
        {
            if (CharacterSession == null)
            {
                return;
            }

            if (EquipmentSerialId == Guid.Empty)
            {
                EquipmentSerialId = Guid.NewGuid();
            }
            if (Item.MaxCellon <= CellonOptions.Count)
            {
                CharacterSession.SendPacket($"info {Language.Instance.GetMessageFromKey("MAX_OPTIONS")}");
                CharacterSession.SendPacket("shop_end 1");
                return;
            }
            if (CharacterSession.Character.Inventory.CountItem(cellonVNum) > 0)
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

                if (Item.MaxCellonLvl > dataIndex && CharacterSession.Character.Gold >= goldAmount)
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
                    Logger.Log.LogUserEvent("OPTION", CharacterSession.GenerateIdentity(), $"[OptionItem]Serial: {EquipmentSerialId} Type: {value[0]} Value: {value[1]}");
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

                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), Rare), 12));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), Rare), 0));
                        CharacterSession.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3005), CharacterSession.Character.PositionX, CharacterSession.Character.PositionY);
                        CharacterSession.SendPacket("shop_end 1");
                    }
                    else
                    {
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), Rare), 11));
                        CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), Rare), 0));
                        CharacterSession.SendPacket("shop_end 1");
                    }
                    CharacterSession.Character.Inventory.RemoveItemAmount(cellonVNum);
                    CharacterSession.Character.Gold -= goldAmount;
                    CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
                }
            }
            DAOFactory.Instance.CellonOptionDAO.Save(CellonOptions);
            foreach (CellonOptionDTO effect in CellonOptions)
            {
                effect.EquipmentSerialId = EquipmentSerialId;
                effect.CellonOptionId = effect.CellonOptionId;
            }
        }
        #endregion
    }
}
