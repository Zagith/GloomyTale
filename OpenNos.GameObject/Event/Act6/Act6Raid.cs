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

namespace OpenNos.GameObject.Event
{
    public static class Act6Raid
    {
        #region Methods

        public static void GenerateRaid(FactionType faction)
        {
            MapInstance bitoren = null;
            switch (faction)
            {
                case FactionType.Angel:
                    {
                        bitoren = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(232));
                        bitoren.CreatePortal(new Portal
                        {
                            SourceMapId = 232,
                            SourceX = 103,
                            SourceY = 124,
                            Type = (byte)PortalType.Raid
                        });
                        ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_ZENAS_RAID_OPEN"), 0));
                    }
                    break;

                case FactionType.Demon:
                    {
                        bitoren = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(236));
                        bitoren.CreatePortal(new Portal
                        {
                            SourceMapId = 236,
                            SourceX = 130,
                            SourceY = 117,
                            Type = (byte)PortalType.Raid
                        });
                        ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_ERENIA_RAID_OPEN"), 0));
                    }
                    break;
            }

            Observable.Timer(TimeSpan.FromMinutes(60)).Subscribe(e =>
            {
                if(bitoren != null)
                {
                    Portal p = bitoren.Portals.Where(p => p.Type == (byte)PortalType.Raid).FirstOrDefault();
                    if(p != null)
                    {
                        p.IsDisabled = true;
                        bitoren.Broadcast(p.GenerateGp());
                        bitoren.Portals.Remove(p);
                    }

                    if (bitoren.Map.MapId == 232)
                    {
                        ServerManager.Instance.Act6AngelStat.Percentage = 0;
                        ServerManager.Instance.Act6AngelStat.IsBossZenas = false;
                        ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_ZENAS_RAID_CLOSED"), 0));
                    } 
                    else
                    {
                        ServerManager.Instance.Act6DemonStat.Percentage = 0;
                        ServerManager.Instance.Act6DemonStat.IsBossErenia = false;
                        ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACT6_ERENIA_RAID_CLOSED"), 0));
                    }

                    Parallel.ForEach(ServerManager.Instance.Sessions.Where(s => s.CurrentMapInstance.Map.MapTypes.Any(mt => mt.MapTypeId == (short)MapTypeEnum.Act61a || mt.MapTypeId == (short)MapTypeEnum.Act61d || mt.MapTypeId == (short)MapTypeEnum.Act61)), sess => sess.SendPacket(sess.Character.GenerateAct6()));
                }
            });
        }

        #endregion
    }
}