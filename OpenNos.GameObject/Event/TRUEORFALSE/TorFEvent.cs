using Mapster;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Data.Base;
using OpenNos.Data.I18N;
using OpenNos.Data.Interfaces;
using OpenNos.Domain;
using OpenNos.Domain.I18N;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using ServiceStack.Text.FastMember;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Event.TRUEORFALSE
{
    public static class TorFEvent
    {


        public static MapInstance _map { get; set; }
        public static MapMonster True { get; set; }
        public static MapMonster False { get; set; }
        public static bool answer { get; set; }

        public static void GenerateTorF(short questionType)
        {
            _map = ServerManager.GenerateMapInstance(2566, MapInstanceType.EventGameInstance, new InstanceBag());
            TorFEventThread torfThread = new TorFEventThread();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => torfThread.Run(questionType));
        }

    }

    public class TorFEventThread
    {
        public List<TrueOrFalseDTO> Questions { get; set; }

        public void Run(short questionType)
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

            #region Questions preparing
            TypeAdapterConfig.GlobalSettings
                .ForDestinationType<I18NString>()
                .BeforeMapping(s => s.Clear());
            TypeAdapterConfig.GlobalSettings
                .When(s => !s.SourceType.IsAssignableFrom(s.DestinationType) && typeof(IStaticDto).IsAssignableFrom(s.DestinationType))
                .IgnoreMember((member, side) => typeof(I18NString).IsAssignableFrom(member.Type));
            TypeAdapterConfig.GlobalSettings.Default.IgnoreAttribute(typeof(I18NFromAttribute));
            var dic = new Dictionary<Type, Dictionary<string, Dictionary<RegionType, II18NDto>>>
                    {
                        {
                            typeof(I18NTorFDto),
                            DAOFactory.I18NTorFDAO.LoadAll().GroupBy(x => x.Key).ToDictionary(x => x.Key,
                                x => x.ToList().ToDictionary(o => o.RegionType, o => (II18NDto) o))
                        }
                    };
            var items = DAOFactory.TrueOrFalseDAO.LoadByType(questionType);
            var props = StaticDtoExtension.GetI18NProperties(typeof(TrueOrFalseDTO));

            var regions = Enum.GetValues(typeof(RegionType));
            var accessors = TypeAccessor.Create(typeof(TrueOrFalseDTO));
            OrderablePartitioner<TrueOrFalseDTO> itemPartitioner = Partitioner.Create(items, EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(itemPartitioner, new ParallelOptions { MaxDegreeOfParallelism = 4 }, itemDto =>
            {
                Questions.Add(itemDto);
            });
            #endregion

            Thread.Sleep(20 * 1000);
            ServerManager.Shout("True or False event STARTED!");
            Portal p = ServerManager.GetMapInstanceByMapId(129).Portals.Where(p => p.DestinationMapId == 2566).FirstOrDefault();
            p.DestinationMapInstanceId = TorFEvent._map.MapInstanceId;
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstanceByMapId(129), EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(p.PortalId, PortalType.Open)));
            Thread.Sleep(20 * 1000);
            ServerManager.Shout("20 SECS REMAINED TO JOIN TorF EVENT! HURRY UP!!");
            Thread.Sleep(20 * 1000);
            ServerManager.Shout("True or False event started. Good luck challangers!");
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstanceByMapId(129), EventActionType.CHANGEPORTALTYPE, new Tuple<int, PortalType>(p.PortalId, PortalType.Closed)));

            byte wave = 0;
            while (TorFEvent._map.Sessions.Count() > 1 && wave < Questions.Count)
            {
               
                foreach (TrueOrFalseDTO question in Questions)
                {       
                    EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg($"Let's start with {wave} round!!", 0)));
                    Thread.Sleep(5 * 1000);
                    foreach (ClientSession s in TorFEvent._map.Sessions)
                    {
                        s.SendPacket(UserInterfaceHelper.GenerateMsg($"{question.Name[s.Account.Language]}", 0));
                        s.SendPacket(s.Character.GenerateSay($"{question.Name[s.Account.Language]}", 12));
                    }
                    Thread.Sleep(10 * 1000);
                    EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("10 secs reamining! Make your choise!!", 0)));
                    Thread.Sleep(10 * 1000);
                    TorFEvent.answer = question.Answer;
                    EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("AND THE ANSWER IS...", 0)));
                    Thread.Sleep(3 * 1000);
                    KickLosers();
                    wave++;
                }
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
                    /*if (s.Character.Reputation >= (long)SideReputType.Side10)
                    {
                        s.Character.Reputation += s.Character.Level * 50;
                        s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REPUT"), s.Character.Level * 50), 10));
                    }*/
                    s.SendPacket(s.Character.GenerateSpPoint());
                    s.SendPacket(s.Character.GenerateGold());
                    s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_MONEY"), s.Character.Level * 1000), 10));
                    
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
                EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("TRUE", 0)));
                Thread.Sleep(2 * 1000);
                foreach (ClientSession s in TorFEvent._map.Sessions)
                    if (!TorFEvent._map.GetCharactersInRange(TorFEvent.True.MapX, TorFEvent.True.MapY, 3).Contains(s.Character))
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, 129, 65, 134);
                    else
                        s.Character.TeleportOnMap(17, 26);                
                Thread.Sleep(3 * 1000);
            }
            else
            {
                EventHelper.Instance.RunEvent(new EventContainer(TorFEvent._map, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg("FALSE", 0)));
                foreach (ClientSession s in TorFEvent._map.Sessions)
                    if (!TorFEvent._map.GetCharactersInRange(TorFEvent.False.MapX, TorFEvent.False.MapY, 3).Contains(s.Character))
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, 129, 65, 134);
                    else
                        s.Character.TeleportOnMap(17, 26);                
                Thread.Sleep(3 * 1000);
            }

        }

    }
}
