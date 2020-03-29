using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenNos.Handler
{
    public class GuriPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GuriPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// guri packet
        /// </summary>
        /// <param name="guriPacket"></param>
        public void Guri(GuriPacket guriPacket)
        {
            if (guriPacket != null)
            {
                if (guriPacket.Data.HasValue && guriPacket.Type == 10 && guriPacket.Data.Value >= 973
                    && guriPacket.Data.Value <= 999 && !Session.Character.EmoticonsBlocked)
                {
                    if (guriPacket.User == Session.Character.CharacterId)
                    {
                        Session.CurrentMapInstance?.Broadcast(Session,
                            StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId,
                                guriPacket.Data.Value + 4099), ReceiverType.AllNoEmoBlocked);
                    }
                    else if (int.TryParse(guriPacket.User.ToString(), out int mateTransportId))
                    {
                        Mate mate = Session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate != null)
                        {
                            Session.CurrentMapInstance?.Broadcast(Session,
                                StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId,
                                    guriPacket.Data.Value + 4099), ReceiverType.AllNoEmoBlocked);
                        }
                    }
                }
                else if (guriPacket.Type == 204)
                {
                    if (guriPacket.Argument == 0 && short.TryParse(guriPacket.User.ToString(), out short slot))
                    {
                        ItemInstance shell =
                            Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                        if (shell?.ShellEffects.Count == 0 && shell.Upgrade > 0 && shell.Rare > 0
                            && Session.Character.Inventory.CountItem(1429) >= ((shell.Upgrade / 10) + shell.Rare))
                        {
                            if (!ShellGeneratorHelper.Instance.ShellTypes.TryGetValue(shell.ItemVNum, out byte shellType))
                            {
                                // SHELL TYPE NOT IMPLEMENTED
                                return;
                            }

                            /*if (shellType != 8 && shellType != 9)
                            {
                                if (shell.Upgrade < 50)
                                {
                                    return;
                                }
                            }*/

                            /*if (shellType == 8 || shellType == 9)
                            {
                                switch (shell.Upgrade)
                                {
                                    case 25:
                                    case 30:
                                    case 40:
                                    case 55:
                                    case 60:
                                    case 65:
                                    case 70:
                                    case 75:
                                    case 80:
                                    case 85:
                                        break;
                                    default:
                                        Session.Character.Inventory.RemoveItemFromInventory(shell.Id);
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("STOP_SPAWNING_BROKEN_SHELL"), 0));
                                        return;
                                }
                            }*/

                            List<ShellEffectDTO> shellOptions = ShellGeneratorHelper.Instance.GenerateShell(shellType, shell.Rare, shell.Upgrade);

                            /*if (!shellOptions.Any())
                            {
                                Session.Character.Inventory.RemoveItemFromInventory(shell.Id);
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("STOP_SPAWNING_BROKEN_SHELL"), 0));
                                return;
                            }*/

                            shell.ShellEffects.AddRange(shellOptions);

                            DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(shell.ShellEffects, shell.EquipmentSerialId);

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_IDENTIFIED"), 0));
                            Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3006));
                            Session.Character.Inventory.RemoveItemAmount(1429, (shell.Upgrade / 10) + shell.Rare);
                        }
                    }
                }
                else if (guriPacket.Type == 205)
                {
                    if (guriPacket.Argument == 0 && short.TryParse(guriPacket.User.ToString(), out short slot))
                    {
                        const int perfumeVnum = 1428;

                        InventoryType perfumeInventoryType = (InventoryType)guriPacket.Argument;

                        ItemInstance equipmentInstance = Session.Character.Inventory.LoadBySlotAndType(slot, perfumeInventoryType);

                        if (equipmentInstance?.BoundCharacterId == null || equipmentInstance.BoundCharacterId == Session.Character.CharacterId || (equipmentInstance.Item.ItemType != ItemType.Weapon && equipmentInstance.Item.ItemType != ItemType.Armor))
                        {
                            return;
                        }

                        int perfumesNeeded = ShellGeneratorHelper.Instance.PerfumeFromItemLevelAndShellRarity(equipmentInstance.Item.LevelMinimum, (byte)equipmentInstance.Rare);

                        if (Session.Character.Inventory.CountItem(perfumeVnum) < perfumesNeeded)
                        {
                            return;
                        }

                        Session.Character.Inventory.RemoveItemAmount(perfumeVnum, perfumesNeeded);

                        equipmentInstance.BoundCharacterId = Session.Character.CharacterId;

                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("BOUND_TO_YOU"), 0));
                    }
                }
                else if (guriPacket.Type == 300)
                {
                    if (guriPacket.Argument == 8023 && short.TryParse(guriPacket.User.ToString(), out short slot))
                    {
                        ItemInstance box = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            box.Item.Use(Session, ref box, 1, new[] { guriPacket.Data.ToString() });
                        }
                    }
                }
                else if (guriPacket.Type == 306)
                {
                    if (short.TryParse(guriPacket.User.ToString(), out short slot))
                    {
                        ItemInstance title = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Main);
                        if (title != null)
                        {
                            title.Item.Use(Session, ref title, 1, new[] { guriPacket.Data.ToString() });
                        }
                    }
                }
                else if (guriPacket.Type == 199 && guriPacket.Argument == 2)
                {
                    if (Session.Character.IsSeal)
                    {
                        return;
                    }
                    short[] listWingOfFriendship = { 2160, 2312, 10048 };
                    short vnumToUse = -1;
                    foreach (short vnum in listWingOfFriendship)
                    {
                        if (Session.Character.Inventory.CountItem(vnum) > 0)
                        {
                            vnumToUse = vnum;
                            break;
                        }
                    }
                    bool isCouple = Session.Character.IsCoupleOfCharacter(guriPacket.User);
                    if (vnumToUse != -1 || isCouple)
                    {
                        ClientSession session = ServerManager.Instance.GetSessionByCharacterId(guriPacket.User);
                        if (session != null && !session.Character.IsChangingMapInstance)
                        {
                            if (Session.Character.IsFriendOfCharacter(guriPacket.User))
                            {
                                if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance)
                                {
                                    if (Session.Character.MapInstance.MapInstanceType
                                        != MapInstanceType.BaseMapInstance
                                        || (ServerManager.Instance.ChannelId == 51
                                         && Session.Character.Faction != session.Character.Faction))
                                    {
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                                        return;
                                    }
                                    if (SideHelper.SideFriendRequirements(Session, session) == false)
                                    {
                                        return;
                                    }
                                    short mapy = session.Character.PositionY;
                                    short mapx = session.Character.PositionX;
                                    short mapId = session.Character.MapInstance.Map.MapId;

                                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapId, mapx, mapy);
                                    if (!isCouple)
                                    {
                                        Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_WINGS"), 10));
                    }
                }
                else if (guriPacket.Type == 400)
                {
                    if (!Session.HasCurrentMapInstance)
                    {
                        return;
                    }

                    int mapNpcId = guriPacket.Argument;

                    MapNpc npc = Session.CurrentMapInstance.Npcs.Find(n => n.MapNpcId.Equals(mapNpcId));

                    if (npc != null && !npc.IsOut)
                    {
                        NpcMonster mapobject = ServerManager.GetNpcMonster(npc.NpcVNum);

                        int rateDrop = ServerManager.Instance.Configuration.RateDrop;
                        int delay = (int)Math.Round(
                            (3 + (mapobject.RespawnTime / 1000d)) * Session.Character.TimesUsed);
                        delay = delay > 11 ? 8 : delay;
                        if (npc.NpcVNum == 1346 || npc.NpcVNum == 1347 || npc.NpcVNum == 2350)
                        {
                            delay = 0;
                        }
                        if (Session.Character.LastMapObject.AddSeconds(delay) < DateTime.Now)
                        {
                            if (mapobject.Drops.Any(s => s.MonsterVNum != null) && mapobject.VNumRequired > 10
                                && Session.Character.Inventory.CountItem(mapobject.VNumRequired)
                                < mapobject.AmountRequired)
                            {
                                if (ServerManager.GetItem(mapobject.VNumRequired) is Item requiredItem)
                                    Session.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), mapobject.AmountRequired, requiredItem.Name[Session.Account.Language]), 0));
                                return;
                            }

                            Random random = new Random();
                            double randomAmount = ServerManager.RandomNumber() * random.NextDouble();
                            List<DropDTO> drops = mapobject.Drops.Where(s => s.MonsterVNum == npc.NpcVNum).ToList();
                            if (drops?.Count > 0)
                            {
                                int count = 0;
                                int probabilities = drops.Sum(s => s.DropChance);
                                int rnd = ServerManager.RandomNumber(0, probabilities);
                                int currentrnd = 0;
                                DropDTO firstDrop = mapobject.Drops.Find(s => s.MonsterVNum == npc.NpcVNum);

                                if (npc.NpcVNum == 2004 /* Ice Flower */ && firstDrop != null)
                                {
                                    ItemInstance newInv = Session.Character.Inventory.AddNewToInventory(firstDrop.ItemVNum, (short)firstDrop.Amount).FirstOrDefault();

                                    if (newInv != null)
                                    {
                                        Session.Character.IncrementQuests(QuestType.Collect1, firstDrop.ItemVNum);
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), $"{newInv.Item.Name[Session.Account.Language]} x {firstDrop.Amount}"), 0));
                                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"), $"{newInv.Item.Name[Session.Account.Language]} x {firstDrop.Amount}"), 11));
                                    }
                                    else
                                    {
                                        Session.Character.GiftAdd(firstDrop.ItemVNum, (short)firstDrop.Amount);
                                    }

                                    Session.CurrentMapInstance.Broadcast(npc.GenerateOut());

                                    return;
                                }
                                else if (randomAmount * 1000 <= probabilities)
                                {
                                    foreach (DropDTO drop in drops.OrderBy(s => ServerManager.RandomNumber()))
                                    {
                                        short vnum = drop.ItemVNum;
                                        short amount = (short)drop.Amount;
                                        int dropChance = drop.DropChance;
                                        currentrnd += drop.DropChance;
                                        if (currentrnd >= rnd)
                                        {
                                            ItemInstance newInv = Session.Character.Inventory.AddNewToInventory(vnum, amount)
                                                .FirstOrDefault();
                                            if (newInv != null)
                                            {
                                                if (dropChance != 100000)
                                                {
                                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                                        string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"),
                                                            $"{newInv.Item.Name[Session.Account.Language]} x {amount}"), 0));
                                                }
                                                Session.SendPacket(Session.Character.GenerateSay(
                                                    string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"),
                                                        $"{newInv.Item.Name[Session.Account.Language]} x {amount}"), 11));
                                                Session.Character.IncrementQuests(QuestType.Collect1, vnum);
                                            }
                                            else
                                            {
                                                Session.Character.GiftAdd(vnum, amount);
                                            }
                                            count++;
                                            if (dropChance != 100000)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (count > 0)
                                {
                                    Session.Character.LastMapObject = DateTime.Now;
                                    Session.Character.TimesUsed++;
                                    if (Session.Character.TimesUsed >= 4)
                                    {
                                        Session.Character.TimesUsed = 0;
                                    }

                                    if (mapobject.VNumRequired > 10)
                                    {
                                        Session.Character.Inventory.RemoveItemAmount(npc.Npc.VNumRequired, npc.Npc.AmountRequired);
                                    }

                                    if (npc.NpcVNum == 1346 || npc.NpcVNum == 1347 || npc.NpcVNum == 2350)
                                    {
                                        npc.SetDeathStatement();
                                        npc.RunDeathEvent();
                                        Session.Character.MapInstance.Broadcast(npc.GenerateOut());
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("TRY_FAILED"), 0));
                                }
                            }
                            else if (Session.CurrentMapInstance.Npcs.Where(s => s.Npc.Race == 8 && s.Npc.RaceType == 5 && s.MapNpcId != npc.MapNpcId) is IEnumerable<MapNpc> mapTeleportNpcs)
                            {
                                if (npc.Npc.VNumRequired > 0 && npc.Npc.AmountRequired > 0)
                                {
                                    if (Session.Character.Inventory.CountItem(npc.Npc.VNumRequired) >= npc.Npc.AmountRequired)
                                    {
                                        if (npc.Npc.AmountRequired > 1)
                                        {
                                            Session.Character.Inventory.RemoveItemAmount(npc.Npc.VNumRequired, npc.Npc.AmountRequired);
                                        }
                                    }
                                    else
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_ITEM_REQUIRED"), 0));
                                        return;
                                    }
                                }
                                if (DAOFactory.TeleporterDAO.LoadFromNpc(npc.MapNpcId).FirstOrDefault() is TeleporterDTO teleport)
                                {
                                    Session.Character.PositionX = teleport.MapX;
                                    Session.Character.PositionY = teleport.MapY;
                                    Session.CurrentMapInstance.Broadcast(Session.Character.GenerateTp());
                                    foreach (Mate mate in Session.Character.Mates.Where(s => s.IsTeamMember && s.IsAlive))
                                    {
                                        mate.PositionX = teleport.MapX;
                                        mate.PositionY = teleport.MapY;
                                        Session.CurrentMapInstance.Broadcast(mate.GenerateTp());
                                    }
                                }
                                else
                                {
                                    MapNpc nearestTeleportNpc = null;
                                    foreach (MapNpc teleportNpc in mapTeleportNpcs)
                                    {
                                        if (nearestTeleportNpc == null)
                                        {
                                            nearestTeleportNpc = teleportNpc;
                                        }
                                        else if (
                                            Map.GetDistance(
                                            new MapCell { X = npc.MapX, Y = npc.MapY },
                                            new MapCell { X = teleportNpc.MapX, Y = teleportNpc.MapY })
                                            <
                                            Map.GetDistance(
                                            new MapCell { X = npc.MapX, Y = npc.MapY },
                                            new MapCell { X = nearestTeleportNpc.MapX, Y = nearestTeleportNpc.MapY }))
                                        {
                                            nearestTeleportNpc = teleportNpc;
                                        }
                                    }
                                    if (nearestTeleportNpc != null)
                                    {
                                        Session.Character.PositionX = nearestTeleportNpc.MapX;
                                        Session.Character.PositionY = nearestTeleportNpc.MapY;
                                        Session.CurrentMapInstance.Broadcast(Session.Character.GenerateTp());
                                        foreach (Mate mate in Session.Character.Mates.Where(s => s.IsTeamMember && s.IsAlive))
                                        {
                                            mate.PositionX = nearestTeleportNpc.MapX;
                                            mate.PositionY = nearestTeleportNpc.MapY;
                                            Session.CurrentMapInstance.Broadcast(mate.GenerateTp());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("TRY_FAILED_WAIT"),
                                    (int)(Session.Character.LastMapObject.AddSeconds(delay) - DateTime.Now)
                                    .TotalSeconds), 0));
                        }
                    }
                }
                else if (guriPacket.Type == 1502)
                {
                    short relictVNum = 0;
                    if (guriPacket.Argument == 10000)
                    {
                        relictVNum = 1878;
                    }
                    else if (guriPacket.Argument == 30000)
                    {
                        relictVNum = 1879;
                    }
                    if (relictVNum > 0 && Session.Character.Inventory.CountItem(relictVNum) > 0)
                    {
                        IEnumerable<RollGeneratedItemDTO> roll = DAOFactory.RollGeneratedItemDAO.LoadByItemVNum(relictVNum);
                        IEnumerable<RollGeneratedItemDTO> rollGeneratedItemDtos = roll as IList<RollGeneratedItemDTO> ?? roll.ToList();
                        if (!rollGeneratedItemDtos.Any())
                        {
                            Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_RELICT"), GetType(), relictVNum));
                            return;
                        }
                        int probabilities = rollGeneratedItemDtos.Sum(s => s.Probability);
                        int rnd = 0;
                        int rnd2 = ServerManager.RandomNumber(0, probabilities);
                        int currentrnd = 0;
                        foreach (RollGeneratedItemDTO rollitem in rollGeneratedItemDtos.OrderBy(s => ServerManager.RandomNumber()))
                        {
                            byte rare = rollitem.ItemGeneratedRare;
                            if (rollitem.IsRareRandom)
                            {
                                if (rollitem.ItemGeneratedUpgradeMax != 0)
                                {
                                    rare = (byte)ServerManager.RandomNumber(rollitem.ItemGeneratedRare, rollitem.ItemGeneratedUpgradeMax);
                                }
                                else
                                {
                                    rnd = ServerManager.RandomNumber(0, 100);

                                    for (int j = 7; j >= 0; j--)
                                    {
                                        if (rnd < ItemHelper.RareRate[j])
                                        {
                                            rare = (byte)j;
                                            break;
                                        }
                                    }
                                    if (rare < 1)
                                    {
                                        rare = 1;
                                    }
                                }
                            }

                            if (rollitem.Probability == 10000)
                            {
                                Session.Character.GiftAdd(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, rare, design: rollitem.ItemGeneratedDesign);
                                continue;
                            }
                            currentrnd += rollitem.Probability;
                            if (currentrnd < rnd2)
                            {
                                continue;
                            }
                            Session.Character.GiftAdd(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, rare, design: rollitem.ItemGeneratedDesign);//, rollitem.ItemGeneratedUpgrade);
                            break;
                        }
                        Session.Character.Inventory.RemoveItemAmount(relictVNum);
                        Session.Character.Gold -= guriPacket.Argument;
                        Session.Character.GenerateGold();
                        Session.SendPacket("shop_end 1");
                    }
                }
                else if (guriPacket.Type == 501)
                {
                    if (ServerManager.Instance.IceBreakerInWaiting && IceBreaker.Map.Sessions.Count() < IceBreaker.MaxAllowedPlayers
                     && Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance && Session.Character.Group?.Raid == null)
                    {
                        if (Session.Character.Gold >= 500)
                        {
                            Session.Character.Gold -= 500;
                            Session.SendPacket(Session.Character.GenerateGold());
                            Session.Character.RemoveVehicle();
                            ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, IceBreaker.Map.MapInstanceId);
                            ConcurrentBag<ClientSession> NewIceTeam = new ConcurrentBag<ClientSession>();
                            NewIceTeam.Add(Session);
                            IceBreaker.IceBreakerTeams.Add(NewIceTeam);
                        }
                        else
                        {
                            Session.SendPacket(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"));
                        }
                    }
                }
                else if (guriPacket.Type == 502)
                {
                    long? targetId = guriPacket.User;

                    if (targetId == null)
                    {
                        return;
                    }

                    ClientSession target = ServerManager.Instance.GetSessionByCharacterId(targetId.Value);

                    if (target?.Character?.MapInstance == null)
                    {
                        return;
                    }

                    if (target.Character.MapInstance.MapInstanceType == MapInstanceType.IceBreakerInstance)
                    {
                        if (target.Character.LastPvPKiller == null
                            || target.Character.LastPvPKiller != Session)
                        {
                            IceBreaker.FrozenPlayers.Remove(target);
                            IceBreaker.AlreadyFrozenPlayers.Add(target);
                            target.Character.NoMove = false;
                            target.Character.NoAttack = false;
                            target.SendPacket(target.Character.GenerateCond());
                            target.Character.MapInstance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ICEBREAKER_PLAYER_UNFROZEN"), target.Character.Name), 0));

                            if (!IceBreaker.IceBreakerTeams.FirstOrDefault(s => s.Contains(Session)).Contains(target))
                            {
                                IceBreaker.IceBreakerTeams.Remove(IceBreaker.IceBreakerTeams.FirstOrDefault(s => s.Contains(target)));
                                IceBreaker.IceBreakerTeams.FirstOrDefault(s => s.Contains(Session)).Add(target);
                            }
                        }
                    }
                    else
                    {
                        target.Character.RemoveBuff(569);
                    }
                }
                else if (guriPacket.Type == 513)
                {
                    if (Session?.Character?.MapInstance == null)
                    {
                        return;
                    }

                    if (Session.Character.IsLaurenaMorph())
                    {
                        Session.Character.MapInstance.Broadcast(Session.Character.GenerateEff(4054));
                        Session.Character.ClearLaurena();
                    }
                }
                else if (guriPacket.Type == 506) // Meteore event
                {
                    if (ServerManager.Instance.IsInstantBattle == true)
                    {
                        Session.Character.IsWaitingForEvent |= ServerManager.Instance.EventInWaiting;
                    }
                    else
                    {
                        if (ServerManager.Instance.EventInWaiting == true && Session.Character.IsWaitingForEvent == false)
                        {
                            Session.SendPacket("bsinfo 0 4 30 0");
                            Session.SendPacket("esf 2");
                            Session.Character.IsWaitingForEvent = true;
                        }
                    }
                }
                else if (guriPacket.Type == 710)
                {
                    if (guriPacket.Value != null)
                    {
                        if (!Session.CurrentMapInstance.Npcs.Any(n => n.MapNpcId.Equals(guriPacket.Data)))
                        {
                            return;
                        }

                        Session.Character.TeleportOnMap((short)guriPacket.Argument, (short)guriPacket.User);
                    }
                }
                else if (guriPacket.Type == 711)
                {
                    TeleporterDTO tp = Session.CurrentMapInstance.Npcs.FirstOrDefault(n => n.Teleporters.Any(t => t?.TeleporterId == guriPacket.Argument))?.Teleporters.FirstOrDefault(t => t?.Type == TeleporterType.TeleportOnOtherMap);
                    if (tp == null)
                    {
                        return;
                    }
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
                }
                else if (guriPacket.Type == 750)
                {
                    const short baseVnum = 1623;
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
                    if (Enum.TryParse(guriPacket.Argument.ToString(), out FactionType faction)
                    && Session.Character.Inventory.CountItem(baseVnum + (byte)faction) > 0)
                    {
                        if ((byte)faction < 3) // Single family change
                        {
                            if (Session.Character.Faction == faction)
                            {
                                return;
                            }
                            if (Session.Character.Family != null)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"),
                                        0));
                                return;
                            }
                            Session.Character.Inventory.RemoveItemAmount(baseVnum + (byte)faction);
                            Session.Character.ChangeFaction(faction);
                        }
                        else // Family faction change
                        {
                            faction -= 2;
                            if ((FactionType)Session.Character.Family.FamilyFaction == faction)
                            {
                                return;
                            }
                            if (Session.Character.Family == null)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY"),
                                        0));
                                return;
                            }
                            if (Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"),
                                        0));
                                return;
                            }
                            if (Session.Character.Family.LastFactionChange > DateTime.Now.AddDays(-1).Ticks)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED"), 0));
                                return;
                            }

                            Session.Character.Inventory.RemoveItemAmount(baseVnum + (byte)faction + 2);
                            Session.Character.Family.ChangeFaction((byte)faction, Session);
                        }
                    }
                }
                else if (guriPacket.Type == 2)
                {
                    Session.CurrentMapInstance?.Broadcast(
                        UserInterfaceHelper.GenerateGuri(2, 1, Session.Character.CharacterId),
                        Session.Character.PositionX, Session.Character.PositionY);
                }
                else if (guriPacket.Type == 4)
                {
                    const int speakerVNum = 2173;
                    const int limitedSpeakerVNum = 10028;
                    if (guriPacket.Argument == 1)
                    {
                        if (guriPacket.Value.Contains("^"))
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INVALID_NAME")));
                            return;
                        }
                        short[] listPetnameVNum = { 2157, 10023 };
                        short vnumToUse = -1;
                        foreach (short vnum in listPetnameVNum)
                        {
                            if (Session.Character.Inventory.CountItem(vnum) > 0)
                            {
                                vnumToUse = vnum;
                            }
                        }
                        Mate mate = Session.Character.Mates.Find(s => s.MateTransportId == guriPacket.Data);
                        if (mate != null && Session.Character.Inventory.CountItem(vnumToUse) > 0)
                        {
                            mate.Name = guriPacket.Value.Truncate(16);
                            Session.CurrentMapInstance?.Broadcast(mate.GenerateOut(), ReceiverType.AllExceptMe);
                            Parallel.ForEach(Session.CurrentMapInstance.Sessions.Where(s => s.Character != null), s =>
                            {
                                if (ServerManager.Instance.ChannelId != 51 || Session.Character.Faction == s.Character.Faction)
                                {
                                    s.SendPacket(mate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                                }
                                else
                                {
                                    s.SendPacket(mate.GenerateIn(true, ServerManager.Instance.ChannelId == 51, s.Account.Authority));
                                }
                            });
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_NAME_PET")));
                            Session.SendPacket(Session.Character.GeneratePinit());
                            Session.SendPackets(Session.Character.GeneratePst());
                            Session.SendPackets(Session.Character.GenerateScP());
                            Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                        }
                    }

                    // presentation message
                    if (guriPacket.Argument == 2)
                    {
                        int presentationVNum = Session.Character.Inventory.CountItem(1117) > 0
                            ? 1117
                            : (Session.Character.Inventory.CountItem(9013) > 0 ? 9013 : -1);
                        if (presentationVNum != -1)
                        {
                            string message = "";
                            string[] valuesplit = guriPacket.Value?.Split(' ');
                            if (valuesplit == null)
                            {
                                return;
                            }

                            for (int i = 0; i < valuesplit.Length; i++)
                            {
                                message += valuesplit[i] + "^";
                            }

                            message = message.Substring(0, message.Length - 1); // Remove the last ^
                            message = message.Trim();
                            if (message.Length > 60)
                            {
                                message = message.Substring(0, 60);
                            }

                            Session.Character.Biography = message;
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("INTRODUCTION_SET"),
                                    10));
                            Session.Character.Inventory.RemoveItemAmount(presentationVNum);
                        }
                    }

                    // Speaker
                    if (guriPacket.Argument == 3 && (Session.Character.Inventory.CountItem(speakerVNum) > 0 || Session.Character.Inventory.CountItem(limitedSpeakerVNum) > 0))
                    {
                        string sayPacket = "";
                        string message =
                            $"<{Language.Instance.GetMessageFromKey("SPEAKER")}> [{Session.Character.Name}]:";
                        byte sayItemInventory = 0;
                        short sayItemSlot = 0;
                        int baseLength = message.Length;
                        string[] valuesplit = guriPacket.Value?.Split(' ');
                        if (valuesplit == null)
                        {
                            return;
                        }

                        if (guriPacket.Data == 999 && (valuesplit.Length < 3 || !byte.TryParse(valuesplit[0], out sayItemInventory) || !short.TryParse(valuesplit[1], out sayItemSlot)))
                        {
                            return;
                        }

                        for (int i = 0 + (guriPacket.Data == 999 ? 2 : 0); i < valuesplit.Length; i++)
                        {
                            message += valuesplit[i] + " ";
                        }

                        if (message.Length > 120 + baseLength)
                        {
                            message = message.Substring(0, 120 + baseLength);
                        }

                        message = message.Trim();

                        if (guriPacket.Data == 999)
                        {
                            sayPacket = Session.Character.GenerateSayItem(message, 13, sayItemInventory, sayItemSlot);
                        }
                        else
                        {
                            sayPacket = Session.Character.GenerateSay(message, 13);
                        }

                        if (Session.Character.IsMuted())
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(
                                    Language.Instance.GetMessageFromKey("SPEAKER_CANT_BE_USED"), 10));
                            return;
                        }

                        if (Session.Character.Inventory.CountItem(limitedSpeakerVNum) > 0)
                        {
                            Session.Character.Inventory.RemoveItemAmount(limitedSpeakerVNum);
                        }
                        else
                        {
                            Session.Character.Inventory.RemoveItemAmount(speakerVNum);
                        }
                        LogHelper.Instance.InsertChatLog(ChatType.Speaker, Session.Character.CharacterId, message, Session.IpAddress);
                        if (ServerManager.Instance.ChannelId == 51)
                        {
                            ServerManager.Instance.Broadcast(Session, sayPacket, ReceiverType.AllExceptMeAct4);
                            Session.SendPacket(sayPacket);
                        }
                        else
                        {
                            ServerManager.Instance.Broadcast(Session, sayPacket, ReceiverType.All);
                        }
                    }

                    // Bubble

                    if (guriPacket.Argument == 4)
                    {
                        int bubbleVNum = Session.Character.Inventory.CountItem(2174) > 0
                            ? 2174
                            : (Session.Character.Inventory.CountItem(10029) > 0 ? 10029 : -1);
                        if (bubbleVNum != -1)
                        {
                            string message = "";
                            string[] valuesplit = guriPacket.Value?.Split(' ');
                            if (valuesplit == null)
                            {
                                return;
                            }

                            for (int i = 0; i < valuesplit.Length; i++)
                            {
                                message += valuesplit[i] + "^";
                            }

                            message = message.Substring(0, message.Length - 1); // Remove the last ^
                            message = message.Trim();
                            if (message.Length > 60)
                            {
                                message = message.Substring(0, 60);
                            }
                            LogHelper.Instance.InsertChatLog(ChatType.Speaker, Session.Character.CharacterId, message, Session.IpAddress);
                            Session.Character.BubbleMessage = message;
                            Session.Character.BubbleMessageEnd = DateTime.Now.AddMinutes(30);
                            Session.SendPacket($"csp_r {Session.Character.BubbleMessage}");
                            Session.Character.Inventory.RemoveItemAmount(bubbleVNum);
                        }
                    }

                    if (guriPacket.Argument == 11 && !string.IsNullOrWhiteSpace(guriPacket.Value))
                    {
                        if (string.IsNullOrWhiteSpace(Session.Account.TotpSecret))
                        {
                            if (CryptographyBase.Sha512(guriPacket.Value) == Session.Account.Password)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^51 #revival^51 Please don't use the primary password!"));
                                return;
                            }
                            Session.Account.TotpSecret = guriPacket.Value;
                            Session.Character.Save();
                            Session.SendPacket(Session.Character.GenerateSay($"Done! Your second password (or pin) is now: {guriPacket.Value}. Do not forget it.", 10));
                            Session.Account.hasVerifiedSecondPassword = true;
                        }
                        else if (Session.Account.TotpSecret == guriPacket.Value)
                        {
                            Session.Account.hasVerifiedSecondPassword = true;
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay($"Wrong password.", 10));
                            Session.Disconnect();
                        }
                    }
                    if (guriPacket.Argument == 12 && !string.IsNullOrWhiteSpace(guriPacket.Value))
                    {
                        CharacterDTO findCharName = DAOFactory.CharacterDAO.LoadByName(guriPacket.Value);

                        if(findCharName == null)
                        {
                            Session.Character.Name = guriPacket.Value;
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    "Name Changed!", 0));
                            string connection = CommunicationServiceClient.Instance.RetrieveWorld(Session.Character.AccountId);
                            Guid? removeItem = DAOFactory.ItemInstanceDAO.LoadByCharacterIdAndItemId(Session.Character.CharacterId, (short)guriPacket.User)?.Id;
                            if (removeItem != null)
                            {
                                Session.Character.Inventory.RemoveItemFromInventory(removeItem.Value);
                            }
                            if (string.IsNullOrWhiteSpace(connection))
                            {
                                return;
                            }
                            int port = Convert.ToInt32(connection.Split(':')[1]);
                            Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                        }
                        else
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    "Name Already Exist.", 0));
                            Session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 12, Session.Character.CharacterId, 2));
                        }
                    }
                }
                else if (guriPacket.Type == 199 && guriPacket.Argument == 1)
                {
                    if (Session.Character.IsSeal)
                    {
                        return;
                    }
                    short[] listWingOfFriendship = { 2160, 2312, 10048 };
                    short vnumToUse = -1;
                    foreach (short vnum in listWingOfFriendship)
                    {
                        if (Session.Character.Inventory.CountItem(vnum) > 0)
                        {
                            vnumToUse = vnum;
                        }
                    }
                    bool isCouple = Session.Character.IsCoupleOfCharacter(guriPacket.User);
                    if (vnumToUse != -1 || isCouple)
                    {
                        ClientSession session = ServerManager.Instance.GetSessionByCharacterId(guriPacket.User);
                        if (session != null)
                        {
                            if (Session.Character.IsFriendOfCharacter(guriPacket.User))
                            {
                                if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance)
                                {
                                    if (Session.Character.MapInstance.MapInstanceType
                                        != MapInstanceType.BaseMapInstance
                                        || (ServerManager.Instance.ChannelId == 51
                                         && Session.Character.Faction != session.Character.Faction))
                                    {
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                                        return;
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                            return;
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_WINGS"), 10));
                        return;
                    }
                    if (!Session.Character.IsFriendOfCharacter(guriPacket.User))
                    {
                        Session.SendPacket(Language.Instance.GetMessageFromKey("CHARACTER_NOT_IN_FRIENDLIST"));
                        return;
                    }

                    Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 7, $"#guri^199^2^{guriPacket.User}"));
                }
                else if (guriPacket.Type == 201)
                {
                    if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
                    {
                        Session.SendPacket(Session.Character.GenerateStashAll());
                    }
                }
                else if (guriPacket.Type == 202)
                {
                    //Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PARTNER_BACKPACK"), 10));
                    Session.SendPacket(Session.Character.GeneratePStashAll());
                }
                else if (guriPacket.Type == 208 && guriPacket.Argument == 0)
                {
                    if (short.TryParse(guriPacket.Value, out short mountSlot)
                        && short.TryParse(guriPacket.User.ToString(), out short pearlSlot))
                    {
                        ItemInstance mount = Session.Character.Inventory.LoadBySlotAndType(mountSlot, InventoryType.Main);
                        ItemInstance pearl = Session.Character.Inventory.LoadBySlotAndType(pearlSlot, InventoryType.Equipment);

                        if (mount?.Item == null || pearl?.Item == null)
                        {
                            return;
                        }

                        if (!pearl.Item.IsHolder)
                        {
                            return;
                        }

                        if (pearl.HoldingVNum > 0)
                        {
                            return;
                        }

                        if (pearl.Item.ItemType == ItemType.Box && pearl.Item.ItemSubType == 4)
                        {
                            if (mount.Item.ItemType != ItemType.Special || mount.Item.ItemSubType != 0 || mount.Item.Speed < 1)
                            {
                                return;
                            }
                            if (mount.Item.Effect != 1000 && pearl.Item.VNum != 4106)
                            {
                                return;
                            }

                            Session.Character.Inventory.RemoveItemFromInventory(mount.Id);

                            pearl.HoldingVNum = mount.ItemVNum;

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MOUNT_SAVED"), 0));
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MOUNT_SAVED"), 10));
                        }
                    }
                }
                else if (guriPacket.Type == 209 && guriPacket.Argument == 0)
                {
                    if (short.TryParse(guriPacket.Value, out short fairySlot)
                        && short.TryParse(guriPacket.User.ToString(), out short pearlSlot))
                    {
                        ItemInstance fairy = Session.Character.Inventory.LoadBySlotAndType(fairySlot, InventoryType.Equipment);
                        ItemInstance pearl = Session.Character.Inventory.LoadBySlotAndType(pearlSlot, InventoryType.Equipment);

                        if (fairy?.Item == null || pearl?.Item == null)
                        {
                            return;
                        }

                        if (!pearl.Item.IsHolder)
                        {
                            return;
                        }

                        if (pearl.HoldingVNum > 0)
                        {
                            return;
                        }

                        if (pearl.Item.ItemType == ItemType.Box && pearl.Item.ItemSubType == 5)
                        {
                            if (fairy.Item.ItemType != ItemType.Jewelery || fairy.Item.ItemSubType != 3 || fairy.Item.IsDroppable || pearl.Item.VNum != 4194)
                            {
                                return;
                            }

                            Session.Character.Inventory.RemoveItemFromInventory(fairy.Id);

                            pearl.HoldingVNum = fairy.ItemVNum;
                            pearl.ElementRate = fairy.ElementRate;

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("FAIRY_SAVED"), 0));
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("FAIRY_SAVED"), 10));
                        }
                    }
                }
                else if (guriPacket.Type == 203 && guriPacket.Argument == 0)
                {
                    // SP points initialization
                    int[] listPotionResetVNums = { 1366, 1427, 5115, 9040 };
                    int vnumToUse = -1;
                    foreach (int vnum in listPotionResetVNums)
                    {
                        if (Session.Character.Inventory.CountItem(vnum) > 0)
                        {
                            vnumToUse = vnum;
                        }
                    }

                    if (vnumToUse != -1)
                    {
                        if (Session.Character.UseSp)
                        {
                            ItemInstance specialistInstance =
                                Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp,
                                    InventoryType.Wear);
                            if (specialistInstance != null)
                            {
                                specialistInstance.SlDamage = 0;
                                specialistInstance.SlDefence = 0;
                                specialistInstance.SlElement = 0;
                                specialistInstance.SlHP = 0;

                                specialistInstance.DamageMinimum = 0;
                                specialistInstance.DamageMaximum = 0;
                                specialistInstance.HitRate = 0;
                                specialistInstance.CriticalLuckRate = 0;
                                specialistInstance.CriticalRate = 0;
                                specialistInstance.DefenceDodge = 0;
                                specialistInstance.DistanceDefenceDodge = 0;
                                specialistInstance.ElementRate = 0;
                                specialistInstance.DarkResistance = 0;
                                specialistInstance.LightResistance = 0;
                                specialistInstance.FireResistance = 0;
                                specialistInstance.WaterResistance = 0;
                                specialistInstance.CriticalDodge = 0;
                                specialistInstance.CloseDefence = 0;
                                specialistInstance.DistanceDefence = 0;
                                specialistInstance.MagicDefence = 0;
                                specialistInstance.HP = 0;
                                specialistInstance.MP = 0;

                                Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                Session.Character.Inventory.DeleteFromSlotAndType((byte)EquipmentType.Sp,
                                    InventoryType.Wear);
                                Session.Character.Inventory.AddToInventoryWithSlotAndType(specialistInstance,
                                    InventoryType.Wear, (byte)EquipmentType.Sp);
                                Session.SendPacket(Session.Character.GenerateCond());
                                Session.SendPacket(specialistInstance.GenerateSlInfo(Session));
                                Session.SendPacket(Session.Character.GenerateLev());
                                Session.SendPackets(Session.Character.GenerateStatChar());
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("POINTS_RESET"),
                                        0));
                            }
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(
                                    Language.Instance.GetMessageFromKey("TRANSFORMATION_NEEDED"), 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_POINTS"),
                                10));
                    }
                }
                else if (guriPacket.Type == 8888)
                {
                    if (Session.Character.Inventory.CountItem(15297) >= 10)
                    {

                        DateTime now = DateTime.Now;
                        IEnumerable<MapNpcDTO> npcs = DAOFactory.MapNpcDAO.LoadFromMap(Session.Character.MapId);
                        foreach (MapNpcDTO npc in npcs.Where(n => n.MapNpcId.Equals(5)))
                        {
                            ShopDTO shop = DAOFactory.ShopDAO.LoadByNpc(npc.MapNpcId);
                            IEnumerable<FortuneWheelDTO> roll = DAOFactory.FortuneWheelDAO.LoadByShopId(shop.ShopId).ToList();
                            int probabilities = roll.Sum(s => s.Probability);
                            int rnd = ServerManager.RandomNumber(0, probabilities);
                            int currentrnd = 0;
                            foreach (FortuneWheelDTO rollitem in roll)
                            {
                                currentrnd += rollitem.Probability;
                                if (currentrnd >= rnd)
                                {
                                    Item i = ServerManager.GetItem(rollitem.ItemGeneratedVNum);
                                    sbyte rare = (sbyte)rollitem.Rare;
                                    byte upgrade = rollitem.Upgrade;
                                    Session.Character.GiftAdd(rollitem.ItemGeneratedVNum, (short)rollitem.ItemGeneratedAmount, (byte)rare, upgrade);
                                    Session.SendPacket($"rdi {rollitem.ItemGeneratedVNum} {rollitem.ItemGeneratedAmount}");
                                    Session.Character.Inventory.RemoveItemAmount(2009, 10);
                                    return;

                                }
                            }
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEM"), 0));
                    }
                }
                else if (guriPacket.Type == 8889)
                {
                    if (Session.Character.Gold >= 5000000)
                    {

                        DateTime now = DateTime.Now;
                        MapNpc npc = Session.CurrentMapInstance.Npcs.FirstOrDefault(n => n.MapNpcId.Equals(2));
                        IEnumerable<FortuneWheelDTO> roll = DAOFactory.FortuneWheelDAO.LoadByShopId(npc.Shop.ShopId).ToList();
                        int probabilities = roll.Sum(s => s.Probability);
                        int rnd = ServerManager.RandomNumber(0, probabilities);
                        int currentrnd = 0;
                        foreach (FortuneWheelDTO rollitem in roll)
                        {
                            currentrnd += rollitem.Probability;
                            if (currentrnd >= rnd)
                            {
                                Item i = ServerManager.GetItem(rollitem.ItemGeneratedVNum);
                                sbyte rare = (sbyte)rollitem.Rare;
                                byte upgrade = rollitem.Upgrade;
                                Session.Character.GiftAdd(rollitem.ItemGeneratedVNum, (short)rollitem.ItemGeneratedAmount, (byte)rare, upgrade);
                                Session.SendPacket($"rdi {rollitem.ItemGeneratedVNum} {rollitem.ItemGeneratedAmount}");
                                Session.Character.Gold -= 5000000;
                                Session.SendPacket(Session.Character.GenerateGold());
                                return;

                            }
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD"), 0));
                    }
                }
                else if (guriPacket.Type == 7600)
                {
                    if (Session.Character.Inventory.CountItem(1216) >= 1)
                    {
                        ItemInstance specialistInstance = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                        if (specialistInstance != null)
                        {
                            CustomHelper.Instance.SpeedPerfection(Session, specialistInstance);
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay("You must wear the Special Card to perfect", 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay("You need {item name} for perfect the sp", 10));
                    }
                }
            }
        }
        #endregion
    }
}
