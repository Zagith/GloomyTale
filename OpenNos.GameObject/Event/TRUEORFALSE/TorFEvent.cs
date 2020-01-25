using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.GameObject.Event.TRUEORFALSE
{
    public static class TorFEvent
    {


        public static MapInstance _map  { get; set; }
        public static MapMonster True { get; set; }
        public static MapMonster False { get; set; }
        public static bool answer { get; set; }

        public static void GenerateTorF()
        {
            _map = ServerManager.GenerateMapInstance(2566, MapInstanceType.EventGameInstance, new InstanceBag());
            TorFEventThread torfThread = new TorFEventThread();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => torfThread.Run());
        }

    }

    public class TorFEventThread
    {
        public void Run()
        {
            ServerManager.Shout("True or False event is starting!");
            Thread.Sleep(5 * 1000);
            ServerManager.Shout("PREPARE YOURSELF!!");
            ServerManager.Instance.EventInWaiting = true;

            #region map preparing

            //Mob representing FALSE
            TorFEvent.False = new MapMonster
            {
                MonsterVNum = 2039,
                MapX = 29,
                MapY = 10,
                MapId = TorFEvent._map.Map.MapId,
                Position = 2,
                IsMoving = false,
                MapMonsterId = TorFEvent._map.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = false,
                Name = "FALSE",
                IsTarget = false,
                NoAggresiveIcon = true
            };
            TorFEvent.False.Initialize(TorFEvent._map);
            TorFEvent._map.AddMonster(TorFEvent.False);
            GenerateRange(TorFEvent.False);

            //Mob representing true
            TorFEvent.True = new MapMonster
            {
                MonsterVNum = 679,
                MapX = 6,
                MapY = 10,
                MapId = TorFEvent._map.Map.MapId,
                Position = 2,
                IsMoving = false,
                MapMonsterId = TorFEvent._map.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = false,
                Name = "TRUE",
                IsTarget = false,
                NoAggresiveIcon = true
            };
            TorFEvent.True.Initialize(TorFEvent._map);
            TorFEvent._map.AddMonster(TorFEvent.True);
            GenerateRange(TorFEvent.True);

            #endregion

            Thread.Sleep(20 * 1000);
            ServerManager.Shout("True or False event STARTED!");
            Portal p = ServerManager.GetMapInstanceByMapId(129).Portals.Where(p => p.DestinationMapId == 2566).FirstOrDefault();
            p.DestinationMapInstanceId = TorFEvent._map.MapInstanceId;
            EventHelper.Instance.RunEvent( new EventContainer(ServerManager.GetMapInstanceByMapId(129), EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(p.PortalId, PortalType.Open)));
            Thread.Sleep(20 * 1000);
            ServerManager.Shout("20 SECS REMAINED TO JOIN TorF EVENT! HURRY UP!!");
            Thread.Sleep(20 * 1000);
            ServerManager.Shout("True or False event started. Good luck challangers!");
            EventHelper.Instance.RunEvent( new EventContainer(ServerManager.GetMapInstanceByMapId(129), EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(p.PortalId, PortalType.Closed)));

            byte wave = 0;
            while (TorFEvent._map.Sessions.Count() > 0 && wave < 5)
            {
                switch (wave)
                {
                    case 0:
                        {
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Let's start with 1st round!!", 0)));
                            Thread.Sleep(5 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Is Bunny a TDC? Time to choose: 20 secs", 0)));
                            Thread.Sleep(10 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 secs reamining! Make your choise!!", 0)));
                            Thread.Sleep(10 * 1000);
                            TorFEvent.answer = true;
                        }
                        break;

                    case 1:
                        {
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Let's start with 2nd round!!", 0)));
                            Thread.Sleep(5 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Is Akaysen a Game Admin? Time to choose: 20 secs", 0)));
                            Thread.Sleep(10 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 secs reamining! Make your choise!!", 0)));
                            Thread.Sleep(10 * 1000);
                            TorFEvent.answer = false;
                        }
                        break;

                    case 2:
                        {
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Let's start with 3rd round!!", 0)));
                            Thread.Sleep(5 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Is Zagith a programmer? Time to choose: 20 secs", 0)));
                            Thread.Sleep(10 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 secs reamining! Make your choise!!", 0)));
                            Thread.Sleep(10 * 1000);
                            TorFEvent.answer = true;
                        }
                        break;

                    case 3:
                        {
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Let's start with 4th round!!", 0)));
                            Thread.Sleep(5 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Is Antwen italian? Time to choose: 20 secs", 0)));
                            Thread.Sleep(10 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 secs reamining! Make your choise!!", 0)));
                            Thread.Sleep(10 * 1000);
                            TorFEvent.answer = true;
                        }
                        break;

                    case 4:
                        {
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Let's start with LAST round!!", 0)));
                            Thread.Sleep(5 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("Is CarlosC turkish? Time to choose: 20 secs", 0)));
                            Thread.Sleep(10 * 1000);
                            EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 secs reamining! Make your choise!!", 0)));
                            Thread.Sleep(10 * 1000);
                            TorFEvent.answer = false;
                        }
                        break;
                }
                EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("AND THE ANSWER IS...", 0)));
                Thread.Sleep(3 * 1000);
                KickLosers();
                wave++;
            }

            if (TorFEvent._map.Sessions.Count() > 0)
            {
                foreach (ClientSession s in TorFEvent._map.Sessions)
                {
                    s.Character.GenerateFamilyXp(s.Character.Level * 4);
                    s.Character.Gold += s.Character.Level * 1000;
                    s.Character.Gold = s.Character.Gold > ServerManager.Instance.Configuration.MaxGold ? ServerManager.Instance.Configuration.MaxGold : s.Character.Gold;
                    s.Character.SpAdditionPoint += s.Character.Level * 100;

                    if (s.Character.SpAdditionPoint > 1000000)
                    {
                        s.Character.SpAdditionPoint = 1000000;
                    }

                    s.SendPacket(s.Character.GenerateSpPoint());
                    s.SendPacket(s.Character.GenerateGold());
                    s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_MONEY"), s.Character.Level * 1000), 10));
                    s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REPUT"), s.Character.Level * 50), 10));
                    if (s.Character.Family != null)
                    {
                        s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), s.Character.Level * 4), 10));
                    }
                    s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_SP_POINT"), s.Character.Level * 100), 10));
                    s.SendPacket(s.Character.GenerateSay("CONGRATS! YOU WON TorF!!", 10));
                }

                Thread.Sleep(5 * 1000);
                foreach (ClientSession s in TorFEvent._map.Sessions)
                    ServerManager.Instance.ChangeMap(s.Character.CharacterId, 129, 65, 134);
            }

            EndEvent();
        }

        public static void EndEvent()
        {
            EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.DISPOSEMAP, null));
            ServerManager.Instance.StartedEvents.Remove(EventType.TorF);
        }

        public static void GenerateRange(MapMonster m)
        {
            MapMonster SxUp = new MapMonster
            {
                MonsterVNum = m.Name.Equals("FALSE") ? (short)(132) : (short)(477),
                MapX = (short)(m.MapX - 3),
                MapY = (short)(m.MapY - 3),
                MapId = TorFEvent._map.Map.MapId,
                Position = 2,
                IsMoving = false,
                MapMonsterId = TorFEvent._map.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = false,
                Name = m.Name.Equals("FALSE") ? "NO" : "YES",
                IsTarget = false,
                NoAggresiveIcon = true
            };
            SxUp.Initialize(TorFEvent._map);
            TorFEvent._map.AddMonster(SxUp);

            MapMonster DxUp = new MapMonster
            {
                MonsterVNum = m.Name.Equals("FALSE") ? (short)(132) : (short)(477),
                MapX = (short)(m.MapX + 3),
                MapY = (short)(m.MapY - 3),
                MapId = TorFEvent._map.Map.MapId,
                Position = 2,
                IsMoving = false,
                MapMonsterId = TorFEvent._map.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = false,
                Name = m.Name.Equals("FALSE") ? "NO" : "YES",
                IsTarget = false,
                NoAggresiveIcon = true
            };
            DxUp.Initialize(TorFEvent._map);
            TorFEvent._map.AddMonster(DxUp);

            MapMonster SxDwn = new MapMonster
            {
                MonsterVNum = m.Name.Equals("FALSE") ? (short)(132) : (short)(477),
                MapX = (short)(m.MapX - 3),
                MapY = (short)(m.MapY + 3),
                MapId = TorFEvent._map.Map.MapId,
                Position = 2,
                IsMoving = false,
                MapMonsterId = TorFEvent._map.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = false,
                Name = m.Name.Equals("FALSE") ? "NO" : "YES",
                IsTarget = false,
                NoAggresiveIcon = true
            };
            SxDwn.Initialize(TorFEvent._map);
            TorFEvent._map.AddMonster(SxDwn);

            MapMonster DxDwn = new MapMonster
            {
                MonsterVNum = m.Name.Equals("FALSE") ? (short)(132) : (short)(477),
                MapX = (short)(m.MapX + 3),
                MapY = (short)(m.MapY + 3),
                MapId = TorFEvent._map.Map.MapId,
                Position = 2,
                IsMoving = false,
                MapMonsterId = TorFEvent._map.GetNextMonsterId(),
                ShouldRespawn = false,
                IsHostile = false,
                Name = m.Name.Equals("FALSE") ? "NO" : "YES",
                IsTarget = false,
                NoAggresiveIcon = true
            };
            DxDwn.Initialize(TorFEvent._map);
            TorFEvent._map.AddMonster(DxDwn);
        }

        public static void KickLosers()
        {
            if (TorFEvent.answer)
            {
                foreach (ClientSession s in TorFEvent._map.Sessions)
                    if (!TorFEvent._map.GetCharactersInRange(TorFEvent.True.MapX, TorFEvent.True.MapY, 3).Contains(s.Character))
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, 129, 65, 134);
                    else
                        s.Character.TeleportOnMap(17, 26);
                EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("TRUE", 0)));
                Thread.Sleep(3 * 1000);
            }
            else
            {
                foreach (ClientSession s in TorFEvent._map.Sessions)
                    if (!TorFEvent._map.GetCharactersInRange(TorFEvent.False.MapX, TorFEvent.False.MapY, 3).Contains(s.Character))
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, 129, 65, 134);
                    else
                        s.Character.TeleportOnMap(17, 26);
                EventHelper.Instance.RunEvent( new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("FALSE", 0)));
                Thread.Sleep(3 * 1000);
            }

        }

    }
}
