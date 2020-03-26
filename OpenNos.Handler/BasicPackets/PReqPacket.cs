using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("preq")]
    public class PReqPacket
    {
        #region Properties

        public short? Parameter { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 2)
            {
                return;
            }
            PReqPacket packetDefinition = new PReqPacket();
            if (short.TryParse(packetSplit[2], out short type))
            {
                packetDefinition.Parameter = type;
            }
            packetDefinition.ExecuteHandler(session as ClientSession);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(PReqPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character.IsSeal)
            {
                return;
            }

            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;

            double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;

            if (timeSpanSinceLastPortal < 4 || !Session.HasCurrentMapInstance)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                return;
            }

            if (Session.CurrentMapInstance.Portals.Concat(Session.Character.GetExtraPortal())
                    .FirstOrDefault(s => Session.Character.PositionY >= s.SourceY - 1
                        && Session.Character.PositionY <= s.SourceY + 1
                        && Session.Character.PositionX >= s.SourceX - 1
                        && Session.Character.PositionX <= s.SourceX + 1) is Portal portal)
            {
                switch (portal.Type)
                {
                    case (sbyte)PortalType.MapPortal:
                    case (sbyte)PortalType.TSNormal:
                    case (sbyte)PortalType.Open:
                    case (sbyte)PortalType.Miniland:
                    case (sbyte)PortalType.TSEnd:
                    case (sbyte)PortalType.Exit:
                    case (sbyte)PortalType.Effect:
                    case (sbyte)PortalType.ShopTeleport:
                        break;

                    case (sbyte)PortalType.Raid:
                        if (Session.Character.Group?.Raid != null)
                        {
                            if (Session.Character.Group.IsLeader(Session))
                            {
                                Session.SendPacket(
                                    $"qna #mkraid^0^275 {Language.Instance.GetMessageFromKey("RAID_START_QUESTION")}");
                            }
                            else
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("NOT_TEAM_LEADER"), 10));
                            }
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NEED_TEAM"), 10));
                        }

                        return;

                    case (sbyte)PortalType.BlueRaid:
                    case (sbyte)PortalType.DarkRaid:
                        if (!Parameter.HasValue)
                        {
                            Session.SendPacket($"qna #preq^1 {string.Format(Language.Instance.GetMessageFromKey("ACT4_RAID_ENTER"), 10000)}");
                            return;
                        }
                        else
                        {
                            if (Parameter == 1)
                            {
                                if ((int)Session.Character.Faction == portal.Type - 9 && Session.Character.Family?.Act4Raid != null)
                                {
                                    if (Session.Character.Level > 49)
                                    {
                                        if (Session.Character.Contributi >= 10000)
                                        {
                                            Session.Character.SetContributi(-10000);

                                            Session.Character.LastPortal = currentRunningSeconds;

                                            switch (Session.Character.Family.Act4Raid.MapInstanceType)
                                            {
                                                case MapInstanceType.Act4Viserion:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 97, 130);
                                                    break;

                                                case MapInstanceType.Act4Orias:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 97, 130);
                                                    break;

                                                case MapInstanceType.Act4Zanarkand:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 97, 130);
                                                    break;

                                                case MapInstanceType.Act4Demetra:
                                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                                                        Session.Character.Family.Act4Raid.MapInstanceId, 97, 130);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Session.SendPacket(
                                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_CONTRIBUTI"),
                                                    10));
                                        }
                                    }
                                    else
                                    {
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOW_LVL"),
                                                10));
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(
                                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"),
                                            10));
                                }
                            }
                        }

                        return;

                    default:
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                        return;
                }

                if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.TimeSpaceInstance
                    && Session?.Character?.Timespace != null && !Session.Character.Timespace.InstanceBag.Lock)
                {
                    if (Session.Character.CharacterId == Session.Character.Timespace.InstanceBag.CreatorId)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateDialog(
                            $"#rstart^1 rstart {Language.Instance.GetMessageFromKey("FIRST_ROOM_START")}"));
                    }

                    return;
                }

                if (Session?.CurrentMapInstance?.MapInstanceType != MapInstanceType.BaseMapInstance && portal.DestinationMapId == 134)
                {
                    if (!Parameter.HasValue)
                    {
                        Session.SendPacket($"qna #preq^1 {Language.Instance.GetMessageFromKey("ACT4_RAID_EXIT")}");
                        return;
                    }
                }

                portal.OnTraversalEvents.ForEach(e => EventHelper.Instance.RunEvent(e));
                if (portal.DestinationMapInstanceId == default)
                {
                    return;
                }

                if (ServerManager.Instance.ChannelId == 51)
                {
                    /*ScriptedInstance raid = Session.Character.Family.Act4Raid;
                    if (raid.ContributesMinimum > Session.Character.Contributi)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LOW_RAID_CONTRIBUTES"), raid.LevelMinimum), 10));
                        return;
                    }*/
                    if ((Session.Character.Faction == FactionType.Angel && portal.DestinationMapId == 131)
                        || (Session.Character.Faction == FactionType.Demon && portal.DestinationMapId == 130))
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                        return;
                    }

                    if ((portal.DestinationMapId == 130 || portal.DestinationMapId == 131)
                        && timeSpanSinceLastPortal < 15)
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                        return;
                    }
                }

#warning TODO: optimize this shit

                StaticBonusDTO vipBonus =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
                if (portal.PortalId == 583 && vipBonus == null)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"This map requires the object: " + portal.NomeOggetto + "and VIP bonus!", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side1 && portal.Side == (int)SideReputMapType.Side1)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 1", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side2 && portal.Side == (int)SideReputMapType.Side2)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 2", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side3 && portal.Side == (int)SideReputMapType.Side3)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 3", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side4 && portal.Side == (int)SideReputMapType.Side4)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 4", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side5 && portal.Side == (int)SideReputMapType.Side5)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 5", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side6 && portal.Side == (int)SideReputMapType.Side6)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 6", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side7 && portal.Side == (int)SideReputMapType.Side7)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 7", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side8 && portal.Side == (int)SideReputMapType.Side8)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 8", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side9 && portal.Side == (int)SideReputMapType.Side9)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 9", 10));
                    return;
                }
                if (Session.Character.Reputation < (long)SideReputPortalType.Side10 && portal.Side == (int)SideReputMapType.Side10)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 10", 10));
                    return;
                }
                if (portal.RequiredItem != 0)
                {
                    if (Session.Character.Inventory.CountItem(portal.RequiredItem) == 0)
                    {
                        Session.SendPacket(Session.Character.GenerateSay("This map requires the object: " + portal.NomeOggetto + "!", 10));
                        return;
                    }
                }
                Session.SendPacket(Session.CurrentMapInstance.GenerateRsfn());

                if (ServerManager.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType
                    != MapInstanceType.BaseMapInstance
                    && ServerManager.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType
                    != MapInstanceType.CaligorInstance
                    && ServerManager.GetMapInstance(portal.DestinationMapInstanceId).MapInstanceType
                    == MapInstanceType.BaseMapInstance)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                        Session.Character.MapX, Session.Character.MapY);
                }
                else if (portal.DestinationMapInstanceId == Session.Character.Miniland.MapInstanceId)
                {
                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else if (portal.DestinationMapId == 20000)
                {
                    ClientSession sess = ServerManager.Instance.Sessions.FirstOrDefault(s =>
                        s.Character.Miniland.MapInstanceId == portal.DestinationMapInstanceId);
                    if (sess != null)
                    {
                        ServerManager.Instance.JoinMiniland(Session, sess);
                    }
                }
                else
                {
                    if (ServerManager.Instance.ChannelId == 51)
                    {
                        short destinationX = portal.DestinationX;
                        short destinationY = portal.DestinationY;

                        if (portal.DestinationMapInstanceId == CaligorRaid.CaligorMapInstance?.MapInstanceId) /* Caligor Raid Map */
                        {
                            if (!Parameter.HasValue)
                            {
                                Session.SendPacket($"qna #preq^1 {Language.Instance.GetMessageFromKey("CALIGOR_RAID_ENTER")}");
                                return;
                            }
                        }
                        else if (portal.DestinationMapId == 153) /* Unknown Land */
                        {
                            if (Session.Character.MapInstance == CaligorRaid.CaligorMapInstance && !Parameter.HasValue)
                            {
                                Session.SendPacket($"qna #preq^1 {Language.Instance.GetMessageFromKey("CALIGOR_RAID_EXIT")}");
                                return;
                            }
                            else if (Session.Character.MapInstance != CaligorRaid.CaligorMapInstance)
                            {
                                if (destinationX <= 0 && destinationY <= 0)
                                {
                                    switch (Session.Character.Faction)
                                    {
                                        case FactionType.Angel:
                                            destinationX = 50;
                                            destinationY = 172;
                                            break;
                                        case FactionType.Demon:
                                            destinationX = 130;
                                            destinationY = 172;
                                            break;
                                    }
                                }
                            }
                        }

                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            portal.DestinationMapInstanceId, destinationX, destinationY);
                    }
                    else
                    {
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                    }
                }
                Session.Character.LastPortal = currentRunningSeconds;
            }
        }

        #endregion
    }
}
