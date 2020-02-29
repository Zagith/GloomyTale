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
using GloomyTale.Domain;
using GloomyTale.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using GloomyTale.GameObject.Networking;

namespace GloomyTale.GameObject.Event
{
    public static class InstantBattle
    {
        #region Methods

        public static void GenerateInstantBattle()
        {
            /*ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 5), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 5), 1));
            Thread.Sleep(4 * 60 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 1), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 1), 1));
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 30), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 30), 1));
            Thread.Sleep(20 * 1000);*/
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 10), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 10), 1));
            Thread.Sleep(10 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_STARTED"), 1));
            ServerManager.Instance.IsInstantBattle = true;
            ServerManager.Instance.Sessions.Where(s => s.Character?.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance).ToList().ForEach(s => s.SendPacket($"qnaml 1 #guri^506 {Language.Instance.GetMessageFromKey("INSTANTBATTLE_QUESTION")}"));
            ServerManager.Instance.EventInWaiting = true;
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_STARTED"), 1));
            ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == false).ToList().ForEach(s => s.SendPacket("esf"));
            ServerManager.Instance.EventInWaiting = false;
            ServerManager.Instance.IsInstantBattle = false;
            IEnumerable<ClientSession> sessions = ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == true && s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);
            List<Tuple<MapInstance, byte>> maps = new List<Tuple<MapInstance, byte>>();
            MapInstance map = null;
            int i = -1;
            int level = 0;
            byte instancelevel = 1;
            foreach (ClientSession s in sessions.OrderBy(s => s.Character?.Level))
            {
                i++;
                if (s.Character.Level > 75 && level <= 75)
                {
                    i = 0;
                    instancelevel = 76;
                }
                else if (s.Character.Level > 45 && level <= 45)
                {
                    i = 0;
                    instancelevel = 46;
                }
                if (i % 50 == 0)
                {
                    map = ServerManager.GenerateMapInstance(2517, MapInstanceType.NormalInstance, new InstanceBag());
                    maps.Add(new Tuple<MapInstance, byte>(map, instancelevel));
                }
                if (map != null)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(s, map.MapInstanceId);
                }

                level = s.Character.Level;
            }
            ServerManager.Instance.Sessions.Where(s => s.Character != null).ToList().ForEach(s => s.Character.IsWaitingForEvent = false);
            ServerManager.Instance.StartedEvents.Remove(EventType.INSTANTBATTLE);
            foreach (Tuple<MapInstance, byte> mapinstance in maps)
            {
                InstantBattleTask task = new InstantBattleTask();
                Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => InstantBattleTask.Run(mapinstance));
            }
        }

        #endregion

        #region Classes

        public class InstantBattleTask
        {
            #region Methods

            public static void Run(Tuple<MapInstance, byte> mapinstance)
            {
                long maxGold = ServerManager.Instance.MaxGold;
                Thread.Sleep(10 * 1000);
                /*if (!mapinstance.Item1.Sessions.Skip(3 - 1).Any())
                {
                    mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList().ForEach(s => {
                        s.Character.RemoveBuffByBCardTypeSubType(new List<KeyValuePair<byte, byte>>()
                        {
                            new KeyValuePair<byte, byte>((byte)BCardType.CardType.SpecialActions, (byte)AdditionalTypes.SpecialActions.Hide),
                            new KeyValuePair<byte, byte>((byte)BCardType.CardType.FalconSkill, (byte)AdditionalTypes.FalconSkill.Hide),
                            new KeyValuePair<byte, byte>((byte)BCardType.CardType.FalconSkill, (byte)AdditionalTypes.FalconSkill.Ambush)
                        });
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, s.Character.MapId, s.Character.MapX, s.Character.MapY);
                    });
                }*/
                Observable.Timer(TimeSpan.FromMinutes(12)).Subscribe(X =>
                {
                    for (int d = 0; d < 180; d++)
                    {
                        if (!mapinstance.Item1.Monsters.Any(s => s.CurrentHp > 0))
                        {
                            EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SPAWNPORTAL, new Portal { SourceX = 30, SourceY = 101, DestinationMapId = 1 }));
                            mapinstance.Item1.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SUCCEEDED"), 0));
                            foreach (ClientSession cli in mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList())
                            {
                                cli.Character.GenerateFamilyXp(cli.Character.Level * 4);
                                //cli.Character.GetReputation(cli.Character.Level * 50);
                                cli.Character.Gold += cli.Character.Level * 1000;
                                cli.Character.Gold = cli.Character.Gold > maxGold ? maxGold : cli.Character.Gold;
                                cli.Character.SpAdditionPoint += cli.Character.Level * 100;

                                if (cli.Character.SpAdditionPoint > 1000000)
                                {
                                    cli.Character.SpAdditionPoint = 1000000;
                                }

                                cli.SendPacket(cli.Character.GenerateSpPoint());
                                cli.SendPacket(cli.Character.GenerateGold());
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_MONEY"), cli.Character.Level * 1000), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REPUT"), cli.Character.Level * 50), 10));
                                if (cli.Character.Family != null)
                                {
                                    cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), cli.Character.Level * 4), 10));
                                }
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_SP_POINT"), cli.Character.Level * 100), 10));
                            }
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                });

                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(15), new EventContainer(mapinstance.Item1, EventActionType.DISPOSEMAP, null));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(3), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 12), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 10), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 5), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(11), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 4), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(12), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 3), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(13), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 2), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 1), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS_REMAINING"), 30), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS_REMAINING"), 30), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_INCOMING"), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_HERE"), 0)));

                for (int wave = 0; wave < 4; wave++)
                {
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(130 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_WAVE"), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(160 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_INCOMING"), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(170 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_HERE"), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance, mapinstance.Item2, wave)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(140 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.DROPITEMS, getInstantBattleDrop(mapinstance.Item1.Map, mapinstance.Item2, wave)));
                }
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(650), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance, mapinstance.Item2, 4)));
            }


            private static IEnumerable<Tuple<short, int>> GenerateDrop(Map map, short vnum, int amount)
            {
                List<Tuple<short, int>> dropParameters = new List<Tuple<short, int>>();
                MapCell cell = map.GetRandomPosition();
                dropParameters.Add(new Tuple<short, int>(vnum, amount));
                return dropParameters;
            }

            private static List<Tuple<short, int>> getInstantBattleDrop(Map map, short instantbattletype, int wave)
            {
                List<Tuple<short, int>> dropParameters = new List<Tuple<short, int>>();
                switch (instantbattletype)
                {
                    case 1:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    30000));
                                dropParameters.AddRange(GenerateDrop(map, 5948, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1363, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 3));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1248, 1));
                                break;

                            case 1:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    40000));
                                dropParameters.AddRange(GenerateDrop(map, 5949, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 4));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1247, 1));
                                break;

                            case 2:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    60000));
                                dropParameters.AddRange(GenerateDrop(map, 5950, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1218, 1));
                                dropParameters.AddRange(GenerateDrop(map, 5369, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 5));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 3));
                                dropParameters.AddRange(GenerateDrop(map, 1246, 1));
                                break;

                            case 3:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    65000));
                                dropParameters.AddRange(GenerateDrop(map, 1218, 1));
                                dropParameters.AddRange(GenerateDrop(map, 5369, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1363, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 6));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 4));
                                break;
                        }

                        break;

                    case 46:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    30000));
                                dropParameters.AddRange(GenerateDrop(map, 5948, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1363, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 3));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1248, 1));
                                break;

                            case 1:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    40000));
                                dropParameters.AddRange(GenerateDrop(map, 5949, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 4));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 2));
                                dropParameters.AddRange(GenerateDrop(map, 1247, 1));
                                break;

                            case 2:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    60000));
                                dropParameters.AddRange(GenerateDrop(map, 5950, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1218, 1));
                                dropParameters.AddRange(GenerateDrop(map, 5369, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 5));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 5));
                                dropParameters.AddRange(GenerateDrop(map, 1246, 1));
                                break;

                            case 3:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    3000 * ServerManager.Instance.GoldRate / 4));
                                dropParameters.AddRange(GenerateDrop(map, 1218, 1));
                                dropParameters.AddRange(GenerateDrop(map, 5369, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1363, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 6));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 4));
                                break;
                        }

                        break;

                    case 76:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    30000));
                                dropParameters.AddRange(GenerateDrop(map, 5948, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1363, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 3));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1248, 1));
                                break;

                            case 1:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    40000));
                                dropParameters.AddRange(GenerateDrop(map, 5949, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 4));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 2));
                                dropParameters.AddRange(GenerateDrop(map, 1247, 1));
                                break;

                            case 2:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    60000));
                                dropParameters.AddRange(GenerateDrop(map, 5950, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1218, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 5));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 3));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 5369, 1));
                                break;

                            case 3:
                                dropParameters.AddRange(GenerateDrop(map, 1046,
                                    65000));
                                dropParameters.AddRange(GenerateDrop(map, 1218, 1));
                                dropParameters.AddRange(GenerateDrop(map, 5369, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1363, 1));
                                dropParameters.AddRange(GenerateDrop(map, 1364, 1));
                                dropParameters.AddRange(GenerateDrop(map, 2282, 6));
                                dropParameters.AddRange(GenerateDrop(map, 1030, 4));
                                break;
                        }

                        break;
                }
                return dropParameters;
            }

            private static List<MonsterToSummon> getInstantBattleMonster(Map map, Tuple<MapInstance, byte> mapinstance, short instantbattletype, int wave)
            {
                List<MonsterToSummon> summonParameters = new List<MonsterToSummon>();
                    switch (instantbattletype)
                {
                    case 1:
                        switch (wave)
                        {
                            case 0:
                                map.GenerateMonsters(85, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(89, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 1:
                                map.GenerateMonsters(168, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(824, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 2:
                                map.GenerateMonsters(826, 30, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(1286, 30, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 3:
                                map.GenerateMonsters(825, 35, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(170, 35, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 4:
                                map.GenerateMonsters(732, 1, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(85, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(168, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(826, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(825, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;
                        }

                        break;

                    case 46:
                        switch (wave)
                        {
                            case 0:
                                map.GenerateMonsters(279, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(180, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 1:
                                map.GenerateMonsters(469, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(243, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 2:
                                map.GenerateMonsters(202, 30, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(201, 30, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 3:
                                map.GenerateMonsters(257, 35, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(439, 35, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 4:
                                map.GenerateMonsters(179, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(469, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(202, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(257, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(595, 1, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;
                        }

                        break;

                    case 76:
                        switch (wave)
                        {
                            case 0:
                                map.GenerateMonsters(156, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(630, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 1:
                                map.GenerateMonsters(631, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(633, 25, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 2:
                                map.GenerateMonsters(615, 30, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(632, 30, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 3:
                                map.GenerateMonsters(616, 35, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(634, 35, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;

                            case 4:
                                map.GenerateMonsters(839, 1, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(156, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(631, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(615, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                map.GenerateMonsters(616, 20, true, new List<EventContainer>()).ToList()
                                    .ForEach(s => summonParameters.Add(s));
                                break;
                        }

                        break;
                }
                return summonParameters;
            }

            #endregion
        }

        #endregion
    }
}