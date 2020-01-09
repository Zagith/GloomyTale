using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.GameObject.Event.TIMESPACES
{
    public static class Pts
    {
        #region Methods

        public static void GeneratePTS(int Vnum, ClientSession host)
        {
            List<ClientSession> sessions = new List<ClientSession>();
            if (host.Character.Group != null)
            {
                sessions = host.Character.Group.Sessions.Where(s => s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);
            }
            else
            {
                sessions.Add(host);
            }
            List<Tuple<MapInstance, byte>> maps = new List<Tuple<MapInstance, byte>>();
            MapInstance map = null;
            byte instancelevel = 1;
            if (Vnum == 1805)
            {
                instancelevel = 50;
            }
            if (Vnum == 1824)
            {
                instancelevel = 60;
            }
            map = ServerManager.GenerateMapInstance(2100, MapInstanceType.NormalInstance, new InstanceBag());
            maps.Add(new Tuple<MapInstance, byte>(map, instancelevel));

            if (map != null)
            {
                foreach (ClientSession s in sessions)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(s, map.MapInstanceId);
                }
            }

            foreach (Tuple<MapInstance, byte> mapinstance in maps)
            {
                PSTTask task = new PSTTask();
                Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => PSTTask.Run(mapinstance));
            }
        }

        #endregion

        #region Classes

        public class PSTTask
        {
            #region Methods

            public static void Run(Tuple<MapInstance, byte> mapinstance)
            {
                long maxGold = 10000000;// ServerManager.Instance.Configuration.MaxGold;
                Thread.Sleep(10 * 1000);
                Observable.Timer(TimeSpan.FromMinutes(2.6)).Subscribe(X =>
                {
                    for (int d = 0; d < 60; d++)
                    {
                        if (!mapinstance.Item1.Monsters.Any(s => s.CurrentHp > 0))
                        {
                            int MediaLivello = 0;
                            EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SPAWNPORTAL, new Portal { SourceX = 33, SourceY = 34, DestinationMapId = 1 }));
                            mapinstance.Item1.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PTS_SUCCEEDED"), 0));
                            foreach (ClientSession cli in mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList())
                            {
                                MediaLivello += cli.Character.Level;
                            }
                            MediaLivello = MediaLivello / mapinstance.Item1.Sessions.Count();
                            foreach (ClientSession cli in mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList())
                            {
                                cli.Character.GenerateFamilyXp(cli.Character.Level * 4);
                                //cli.Character.GetReputation(MediaLivello * 75);
                                cli.Character.SpAdditionPoint += cli.Character.Level * 100;
                                cli.Character.SpAdditionPoint = cli.Character.SpAdditionPoint > 20000 ? 20000 : cli.Character.SpAdditionPoint;
                                cli.SendPacket(cli.Character.GenerateSpPoint());
                                cli.SendPacket(cli.Character.GenerateGold());
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REPUT"), MediaLivello * 75), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), 25), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_SP_POINT"), cli.Character.Level * 100), 10));
                            }
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                });

                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(4), new EventContainer(mapinstance.Item1, EventActionType.DISPOSEMAP, null));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(1), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PTS_MINUTES_REMAINING"), 2), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(2), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PTS_MINUTES_REMAINING"), 1), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(2.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PTS_SECONDS_REMAINING"), 30), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(2.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PTS_SECONDS_REMAINING"), 30), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PTS_MONSTERS_INCOMING"), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PTS_MONSTERS_HERE"), 0)));

                for (int wave = 0; wave < 3; wave++)
                {
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10 + (wave * 60)), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance.Item2, wave)));
                }
            }


            private static List<MonsterToSummon> getInstantBattleMonster(Map map, short instantbattletype, int wave)
            {
                List<MonsterToSummon> SummonParameters = new List<MonsterToSummon>();

                switch (instantbattletype)
                {
                    case 50:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(226, 10, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(88, 10, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(62, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(244, 10, false, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(244, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(88, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(235, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(204, 10, false, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(204, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(233, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(214, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(678, 15, false, new List<EventContainer>()));
                                break;
                        }
                        break;
                    case 60:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(242, 10, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(237, 10, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(246, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(248, 10, false, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(248, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(246, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(234, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(221, 10, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(225, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(221, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(207, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(234, 15, false, new List<EventContainer>()));
                                break;
                        }
                        break;
                }
                return SummonParameters;
            }

            #endregion
        }

        #endregion
    }
}
