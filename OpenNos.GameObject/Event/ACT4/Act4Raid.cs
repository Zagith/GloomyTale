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
using OpenNos.Domain;
using OpenNos.GameObject.Event.ACT4;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Event
{
    public static class Act4Raid
    {
        public static List<MapMonster> Guardians { get; set; }
        #region Methods

        public static void GenerateRaid(MapInstanceType raidType, byte faction)
        {
            Guardians = new List<MapMonster>();
            MapInstance bitoren = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(134));
            Act4DiscordRich discord = new Act4DiscordRich(faction, raidType, false);
            bitoren.CreatePortal(new Portal
            {
                SourceMapId = 134,
                SourceX = 140,
                SourceY = 100,
                DestinationMapId = 0,
                DestinationX = 1,
                DestinationY = 1,
                Type = (short)(9 + faction)
            });

            #region Guardian Spawning

            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 147,
                MapY = 88,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });
            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 149,
                MapY = 94,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });
            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 147,
                MapY = 101,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });
            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 139,
                MapY = 105,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });
            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 132,
                MapY = 101,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });
            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 129,
                MapY = 94,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });
            Guardians.Add(new MapMonster
            {
                MonsterVNum = (short)(678 + faction),
                MapX = 132,
                MapY = 88,
                MapId = 134,
                Position = 2,
                IsMoving = false,
                MapMonsterId = bitoren.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = true
            });

            #endregion

            foreach (MapMonster monster in Guardians)
            {
                monster.Initialize(bitoren);
                bitoren.AddMonster(monster);
                bitoren.Broadcast(monster.GenerateIn());
            }

            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT4_BITOREN_RAID_OPEN"), 0));
            Act4RaidThread raidThread = new Act4RaidThread();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run(raidType, faction));
        }

        #endregion
    }

    public class Act4RaidThread
    {
        #region Members

        private readonly List<long> _wonFamilies = new List<long>();

        private short _bossMapId = 2539;

        private bool _bossMove;

        private short _bossVNum = 2034;

        private short _bossX = 43;

        private short _bossY = 61;

        private short _destPortalX = 43;

        private short _destPortalY = 143;

        private byte _faction;

        private short _mapId = 2617;

        private MapInstanceType _raidType;

        private short _sourcePortalX = 102;

        private short _sourcePortalY = 15;

        private int _raidTime = 3600;

        private const int _interval = 60;

        #endregion

        #region Methods

        public void Run(MapInstanceType raidType, byte faction)
        {
            _raidType = raidType;
            _faction = faction;
            switch (raidType)
            {
                // Morcos is default
                case MapInstanceType.Act4Orias:
                    _mapId = 2616;
                    _bossMapId = 2509;
                    _bossVNum = 2327;
                    _bossX = 50;
                    _bossY = 44;
                    _sourcePortalX = 102;
                    _sourcePortalY = 15;
                    _destPortalX = 47;
                    _destPortalY = 76;
                    _bossMove = false;
                    break;

                case MapInstanceType.Act4Zanarkand:
                    _mapId = 2618;
                    _bossMapId = 136;
                    _bossVNum = 2049;
                    _bossX = 55;
                    _bossY = 12;
                    _sourcePortalX = 102;
                    _sourcePortalY = 15;
                    _destPortalX = 144;
                    _destPortalY = 129;
                    _bossMove = true;
                    break;

                case MapInstanceType.Act4Demetra:
                    _mapId = 2619;
                    _bossMapId = 199;
                    _bossVNum = 2504;
                    _bossX = 19;
                    _bossY = 19;
                    _sourcePortalX = 102;
                    _sourcePortalY = 15;
                    _destPortalX = 42;
                    _destPortalY = 11;
                    _bossMove = true;
                    break;
            }

#if DEBUG
            _raidTime = 1800;
#endif

            //Run once to load everything in place
            refreshRaid(_raidTime);

            ServerManager.Instance.Act4RaidStart = DateTime.Now;

            while (_raidTime > 0)
            {
                _raidTime -= _interval;
                Thread.Sleep(_interval * 1000);
                refreshRaid(_raidTime);
            }

            endRaid();
        }

        private void endRaid()
        {
            foreach (Family fam in ServerManager.Instance.FamilyList.GetAllItems())
            {
                if (fam.Act4Raid != null)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(fam.Act4Raid, EventActionType.DISPOSEMAP, null));
                    fam.Act4Raid = null;
                }
                if (fam.Act4RaidBossMap != null)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(fam.Act4RaidBossMap, EventActionType.DISPOSEMAP, null));
                    fam.Act4RaidBossMap = null;
                }
            }
            MapInstance bitoren = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(134));

            foreach (Portal portal in bitoren.Portals.ToList().Where(s => s.Type.Equals(10) || s.Type.Equals(11)))
            {
                portal.IsDisabled = true;
                bitoren.Broadcast(portal.GenerateGp());
                bitoren.Portals.Remove(portal);
            }

            bitoren.Portals.RemoveAll(s => s.Type.Equals(10));
            bitoren.Portals.RemoveAll(s => s.Type.Equals(11));
            switch (_faction)
            {
                case 1:
                    ServerManager.Instance.Act4AngelStat.Mode = 0;
                    ServerManager.Instance.Act4AngelStat.IsMorcos = false;
                    ServerManager.Instance.Act4AngelStat.IsHatus = false;
                    ServerManager.Instance.Act4AngelStat.IsCalvina = false;
                    ServerManager.Instance.Act4AngelStat.IsBerios = false;
                    break;

                case 2:
                    ServerManager.Instance.Act4DemonStat.Mode = 0;
                    ServerManager.Instance.Act4DemonStat.IsMorcos = false;
                    ServerManager.Instance.Act4DemonStat.IsHatus = false;
                    ServerManager.Instance.Act4DemonStat.IsCalvina = false;
                    ServerManager.Instance.Act4DemonStat.IsBerios = false;
                    break;
            }

            ServerManager.Instance.StartedEvents.Remove(EventType.Act4Raid);

            foreach (MapMonster monster in Act4Raid.Guardians)
            {
                bitoren.Broadcast(StaticPacketHelper.Out(UserType.Monster, monster.MapMonsterId));
                bitoren.RemoveMonster(monster);
            }

            Act4Raid.Guardians.Clear();
        }

        private void openRaid(Family fam)
        {
            fam.Act4RaidBossMap.OnCharacterDiscoveringMapEvents.Add(new Tuple<EventContainer, List<long>>(new EventContainer(fam.Act4RaidBossMap, EventActionType.STARTACT4RAIDWAVES, new List<long>()), new List<long>()));
            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
                new EventContainer(fam.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int, short>(_bossVNum, 1046, 10, 20000, 20001, 0)),
                new EventContainer(fam.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int, short>(_bossVNum, 1244, 10, 5, 6, 0))
            };
            if (_raidType.Equals(MapInstanceType.Act4Demetra))
            {
                onDeathEvents.Add(new EventContainer(fam.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int, short>(_bossVNum, 2395, 3, 1, 2, 0)));
                onDeathEvents.Add(new EventContainer(fam.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int, short>(_bossVNum, 2396, 5, 1, 2, 0)));
                onDeathEvents.Add(new EventContainer(fam.Act4RaidBossMap, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int, short>(_bossVNum, 2397, 10, 1, 2, 0)));

                fam.Act4RaidBossMap.OnCharacterDiscoveringMapEvents.Add(new Tuple<EventContainer, List<long>>(new EventContainer(fam.Act4RaidBossMap, EventActionType.SPAWNMONSTER, new MonsterToSummon(621, fam.Act4RaidBossMap.Map.GetRandomPosition(), null, move: true, hasDelay: 30)), new List<long>()));
                fam.Act4RaidBossMap.OnCharacterDiscoveringMapEvents.Add(new Tuple<EventContainer, List<long>>(new EventContainer(fam.Act4RaidBossMap, EventActionType.SPAWNMONSTER, new MonsterToSummon(622, fam.Act4RaidBossMap.Map.GetRandomPosition(), null, move: true, hasDelay: 205)), new List<long>()));
                fam.Act4RaidBossMap.OnCharacterDiscoveringMapEvents.Add(new Tuple<EventContainer, List<long>>(new EventContainer(fam.Act4RaidBossMap, EventActionType.SPAWNMONSTER, new MonsterToSummon(623, fam.Act4RaidBossMap.Map.GetRandomPosition(), null, move: true, hasDelay: 380)), new List<long>()));
            }
            onDeathEvents.Add(new EventContainer(fam.Act4RaidBossMap, EventActionType.SCRIPTEND, (byte)1));
            onDeathEvents.Add(new EventContainer(fam.Act4Raid, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(fam.Act4Raid.Portals.Find(s => s.SourceX == _sourcePortalX && s.SourceY == _sourcePortalY && !s.IsDisabled).PortalId, PortalType.Closed)));
            MonsterToSummon bossMob = new MonsterToSummon(_bossVNum, new MapCell { X = _bossX, Y = _bossY }, null, _bossMove)
            {
                DeathEvents = onDeathEvents
            };
            EventHelper.Instance.RunEvent(new EventContainer(fam.Act4RaidBossMap, EventActionType.SPAWNMONSTER, bossMob));
            EventHelper.Instance.RunEvent(new EventContainer(fam.Act4Raid, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT4RAID_OPEN"), 0)));

            //Observable.Timer(TimeSpan.FromSeconds(90)).Subscribe(o =>
            //{
            //TODO: Summon Monsters
            //});
        }

        private void refreshRaid(int remaining)
        {
            Parallel.ForEach(ServerManager.Instance.FamilyList.GetAllItems(), fam =>
            {
                if (fam.Act4Raid == null)
                {
                    fam.Act4Raid = ServerManager.GenerateMapInstance(_mapId, _raidType, new InstanceBag(), true);
                }
                if (fam.Act4RaidBossMap == null)
                {
                    fam.Act4RaidBossMap = ServerManager.GenerateMapInstance(_bossMapId, _raidType, new InstanceBag());
                }
                if (remaining <= 1800 && !fam.Act4Raid.Portals.Any(s => s.DestinationMapInstanceId.Equals(fam.Act4RaidBossMap.MapInstanceId)))
                {
                    fam.Act4Raid.CreatePortal(new Portal
                    {
                        DestinationMapInstanceId = fam.Act4RaidBossMap.MapInstanceId,
                        DestinationX = _destPortalX,
                        DestinationY = _destPortalY,
                        SourceX = _sourcePortalX,
                        SourceY = _sourcePortalY,
                    });
                    openRaid(fam);
                }

                if (fam.Act4RaidBossMap.Monsters.Find(s => s.MonsterVNum == _bossVNum && s.CurrentHp / s.MaxHp < 0.5) != null
                && fam.Act4Raid.Portals.Find(s => s.SourceX == _sourcePortalX && s.SourceY == _sourcePortalY && !s.IsDisabled) is Portal portal)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(fam.Act4Raid, EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(fam.Act4Raid.Portals.Find(s => s.SourceX == _sourcePortalX && s.SourceY == _sourcePortalY && !s.IsDisabled).PortalId, PortalType.Closed)));
                    fam.Act4Raid.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PORTAL_CLOSED"), 0));
                    fam.Act4RaidBossMap.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PORTAL_CLOSED"), 0));
                }
            });
        }

        #endregion
    }
}