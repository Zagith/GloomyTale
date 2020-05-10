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
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenNos.GameObject.Networking;
using OpenNos.DAL;
using OpenNos.Data;
using System.Collections.Concurrent;
using Discord.Rest;

namespace OpenNos.GameObject.Event
{
    public static class Act6Raid
    {
        #region Methods

        public static void GenerateRaid(FactionType faction)
        {
            MapInstance bitoren = null;
            ScriptedInstanceDTO raid = null;
            switch (faction)
            {
                case FactionType.Angel:
                    {
                        raid = DAOFactory.ScriptedInstanceDAO.LoadByMap(232).FirstOrDefault();
                    }
                    break;

                case FactionType.Demon:
                    {
                        raid = DAOFactory.ScriptedInstanceDAO.LoadByMap(236).FirstOrDefault();
                    }
                    break;
            }

            if (raid != null)
            {
                ScriptedInstance siObj = new ScriptedInstance(raid);

                if (siObj != null)
                {
                    siObj.LoadGlobals();
                    ServerManager.Instance.Raids.Add(siObj);
                    bitoren = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(siObj.MapId));
                    bitoren.CreatePortal(new Portal
                    {
                        SourceMapId = siObj.MapId,
                        SourceX = siObj.PositionX,
                        SourceY = siObj.PositionY,
                        Type = (byte)PortalType.Raid
                    });
                    string message = faction == FactionType.Angel ? Language.Instance.GetMessageFromKey("ACT6_ZENAS_RAID_OPEN") : Language.Instance.GetMessageFromKey("ACT6_ERENIA_RAID_OPEN");
                    ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(message, 0));
                }
            }
            Observable.Timer(TimeSpan.FromMinutes(60)).Subscribe(e =>
            {
                if (bitoren != null)
                {
                    Portal p = bitoren.Portals.Where(p => p.Type == (byte)PortalType.Raid).FirstOrDefault();
                    if (p != null)
                    {
                        p.IsDisabled = true;
                        bitoren.Broadcast(p.GenerateGp());
                        bitoren.Portals.Remove(p);
                    }

                    if (bitoren.Map.MapId == 232)
                    {
                        ServerManager.Instance.Raids.RemoveWhere(s => s.MapId == 232, out ConcurrentBag<ScriptedInstance> tmp);
                        ServerManager.Instance.Act6AngelStat.Percentage = 0;
                        ServerManager.Instance.Act6AngelStat.IsBossZenas = false;
                        ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_ZENAS_RAID_CLOSED"), 0));
                        Observable.Timer(TimeSpan.FromMinutes(3)).Subscribe(e =>
                        {
                            Act6TS raidThread = new Act6TS();
                            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run(FactionType.Angel));
                        });
                    }
                    else
                    {
                        ServerManager.Instance.Raids.RemoveWhere(s => s.MapId == 236, out ConcurrentBag<ScriptedInstance> tmp);
                        ServerManager.Instance.Act6DemonStat.Percentage = 0;
                        ServerManager.Instance.Act6DemonStat.IsBossErenia = false;
                        ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_ERENIA_RAID_CLOSED"), 0));
                        Observable.Timer(TimeSpan.FromMinutes(3)).Subscribe(e =>
                        {
                            Act6TS raidThread = new Act6TS();
                            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run(FactionType.Demon));
                        });
                    }

                    Parallel.ForEach(ServerManager.Instance.Sessions.Where(s => s.CurrentMapInstance.Map.MapTypes.Any(mt => mt.MapTypeId == (short)MapTypeEnum.Act61a || mt.MapTypeId == (short)MapTypeEnum.Act61d || mt.MapTypeId == (short)MapTypeEnum.Act61)), sess => sess.SendPacket(sess.Character.GenerateAct6()));
                }
            });
        }

        #endregion

    }

    public class Act6TS
    {
        #region members

        public List<Tuple<MapInstance, string, int, int>> maps = new List<Tuple<MapInstance, string, int, int>>();
        public MapInstance TSMap = null;
        public FactionType TStype;

        #endregion

        public void Run(FactionType raidtype)
        {
            TStype = raidtype;
            initializeMaps();
            
            if (TSMap != null)
            {
                initializeTime();
                TSMap.IsAct6Ts = true;
                Parallel.ForEach(TSMap.Sessions, s => s.SendPacket(s.CurrentMapInstance.Clock.GetClock()));
                TSMap.Clock.StartClock();
                ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ACT6_TS_OPENED"), maps.FirstOrDefault(m => m.Item1 == TSMap).Item2), 0));
                TSMap.Clock.GetClock();
                ServerManager.GetMapInstanceByMapId(228)?.CreatePortal(new Portal
                {
                    SourceMapId = 228,
                    SourceX = 127,
                    SourceY = 117,
                    DestinationMapId = TSMap.Map.MapId,
                    DestinationMapInstanceId = TSMap.MapInstanceId,
                    DestinationX = (short)maps.FirstOrDefault(m => m.Item1 == TSMap).Item3,
                    DestinationY = (short)maps.FirstOrDefault(m => m.Item1 == TSMap).Item4,
                    Type = 12
                });
                TSMap.IsPVP = true;
                while(TSMap.Clock.SecondsRemaining > 0)
                {
                    Thread.Sleep(1000);
                }
                TSMap.Clock.GetClock();
                TSMap.IsAct6Ts = false;
                TSMap.IsPVP = false;
                Parallel.ForEach(TSMap.Sessions, s => s.SendPacket(s.CurrentMapInstance.Clock.GetClock()));
                ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_TS_CLOSED"), 0));
                Portal p = ServerManager.GetMapInstanceByMapId(228)?.Portals?.Where(p => p.DestinationX == (short)maps.FirstOrDefault(m => m.Item1 == TSMap).Item3 && p.DestinationY == (short)maps.FirstOrDefault(m => m.Item1 == TSMap).Item4).FirstOrDefault();
                if (p != null)
                {
                    p.IsDisabled = true;
                    ServerManager.GetMapInstanceByMapId(228)?.Broadcast(p.GenerateGp());
                    ServerManager.GetMapInstanceByMapId(228)?.Portals.Remove(p);
                }
            }
        }

        public void initializeTime()
        {
            TSMap.Clock = new Clock(3);
            TSMap.Clock.AddTime(900);
        }

        public void initializeMaps()
        {
            //Inferno maps
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(233), "Hells Gate 1", 145, 242));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(234), "Hells Gate 2", 410, 87));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(235), "Hells Gate 3", 406, 124));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(236), "Hells Gate 4", 116, 321));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(2604), "Hells Ruin 2", 61, 206));

            //Heaven maps
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(229), "Heaven Gate 1", 137, 54));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(230), "Heaven Gate 2", 99, 81));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(231), "Heaven Gate 3", 63, 107));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(232), "Heaven Gate 4", 118, 320));
            maps.Add(Tuple.Create(ServerManager.GetMapInstanceByMapId(2601), "Heaven Ruin 2", 217, 175));

            //Select map in relation of the end audience
            switch(TStype)
            {
                case FactionType.Demon:
                    {
                        TSMap = maps.ElementAt(ServerManager.RandomNumber(0, 4)).Item1;
                    }
                    break;

                case FactionType.Angel:
                    {
                        TSMap = maps.ElementAt(ServerManager.RandomNumber(5, 9)).Item1;
                    }
                    break;
            }
        }
    }
}