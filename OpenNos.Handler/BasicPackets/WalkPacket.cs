using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("walk")]
    public class WalkPacket
    {
        #region Properties

        public short Speed { get; set; }

        public short Unknown { get; set; }

        public short XCoordinate { get; set; }

        public short YCoordinate { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 6)
            {
                return;
            }
            WalkPacket packetDefinition = new WalkPacket();
            if (short.TryParse(packetSplit[2], out short x)
                && short.TryParse(packetSplit[3], out short y)
                && short.TryParse(packetSplit[4], out short unknown)
                && short.TryParse(packetSplit[5], out short speed))
            {
                packetDefinition.XCoordinate = x;
                packetDefinition.YCoordinate = y;
                packetDefinition.Unknown = unknown;
                packetDefinition.Speed = speed;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(WalkPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character.CanMove())
            {
                if (Session.Character.MeditationDictionary.Count != 0)
                {
                    Session.Character.MeditationDictionary.Clear();
                }

                double currentRunningSeconds =
                    (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                int distance =
                    Map.GetDistance(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                        new MapCell { X = XCoordinate, Y = YCoordinate });

                if (Session.HasCurrentMapInstance
                    && !Session.CurrentMapInstance.Map.IsBlockedZone(XCoordinate, YCoordinate)
                    && !Session.Character.IsChangingMapInstance && !Session.Character.HasShopOpened)
                {
                    Session.Character.PyjamaDead = false;
                    if (!Session.Character.InvisibleGm)
                    {
                        Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.Move(UserType.Player,
                            Session.Character.CharacterId, XCoordinate, YCoordinate,
                            Session.Character.Speed));
                    }
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.WalkDisposable?.Dispose();
                    walk();
                    int interval = 100 - Session.Character.Speed * 5 + 100 > 0 ? 100 - Session.Character.Speed * 5 + 100 : 0;
                    Session.Character.WalkDisposable = Observable.Interval(TimeSpan.FromMilliseconds(interval)).Subscribe(obs =>
                    {
                        walk();
                    });
                    void walk()
                    {
                        MapCell nextCell = Map.GetNextStep(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY }, new MapCell { X = XCoordinate, Y = YCoordinate }, 1);

                        Session.Character.GetDir(Session.Character.PositionX, Session.Character.PositionY, nextCell.X, nextCell.Y);

                        if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                        {
                            Session.Character.MapX = nextCell.X;
                            Session.Character.MapY = nextCell.Y;
                        }

                        Session.Character.PositionX = nextCell.X;
                        Session.Character.PositionY = nextCell.Y;

                        Session.Character.LastMove = DateTime.Now;

                        if (Session.Character.IsVehicled)
                        {
                            Session.Character.Mates.Where(s => s.IsTeamMember || s.IsTemporalMate).ToList().ForEach(s =>
                            {
                                s.PositionX = Session.Character.PositionX;
                                s.PositionY = Session.Character.PositionY;
                            });
                        }

                        if (Session.Character.LastMonsterAggro.AddSeconds(5) > DateTime.Now)
                        {
                            Session.Character.UpdateBushFire();
                        }

                        Session.CurrentMapInstance?.OnAreaEntryEvents
                            ?.Where(s => s.InZone(Session.Character.PositionX, Session.Character.PositionY)).ToList()
                            .ForEach(e => e.Events.ForEach(evt => EventHelper.Instance.RunEvent(evt)));
                        Session.CurrentMapInstance?.OnAreaEntryEvents?.RemoveAll(s =>
                            s.InZone(Session.Character.PositionX, Session.Character.PositionY));

                        Session.CurrentMapInstance?.OnMoveOnMapEvents?.ForEach(e => EventHelper.Instance.RunEvent(e));
                        Session.CurrentMapInstance?.OnMoveOnMapEvents?.RemoveAll(s => s != null);

                        if (Session.Character.PositionX == XCoordinate && Session.Character.PositionY == YCoordinate)
                        {
                            Session.Character.WalkDisposable?.Dispose();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
