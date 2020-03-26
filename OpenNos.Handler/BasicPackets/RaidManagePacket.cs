using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("rd")]
    public class RaidManagePacket
    {
        #region Properties

        public long CharacterId { get; set; }

        public short? Parameter { get; set; }

        public short Type { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            RaidManagePacket packetDefinition = new RaidManagePacket();
            if (short.TryParse(packetSplit[2], out short type) && long.TryParse(packetSplit[3], out long characterId))
            {
                packetDefinition.Type = type;
                packetDefinition.CharacterId = characterId;
                packetDefinition.Parameter = packetSplit.Length >= 5
                    && short.TryParse(packetSplit[4], out short parameter) ? parameter : (short?)null;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RaidManagePacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.HasCurrentMapInstance)
            {
                Group grp;
                switch (Type)
                {
                    // Join Raid
                    case 1:
                        if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                        {
                            return;
                        }

                        ClientSession target = ServerManager.Instance.GetSessionByCharacterId(CharacterId);
                        if (Parameter == null && target?.Character?.Group == null && Session.Character.Group != null && Session.Character.Group.IsLeader(Session) && Session.Character.Group?.Sessions.FirstOrDefault() == Session)
                        {
                            GroupJoinPacket.HandlePacket(Session, new PJoinPacket
                            {
                                RequestType = GroupRequestType.Invited,
                                CharacterId = CharacterId
                            }.ToString());
                        }
                        else if (Session.Character.Group == null)
                        {
                            GroupJoinPacket.HandlePacket(Session, new PJoinPacket
                            {
                                RequestType = GroupRequestType.Accepted,
                                CharacterId = CharacterId
                            }.ToString());
                        }

                        break;

                    // Leave Raid
                    case 2:
                        if (Session.Character.Group == null)
                        {
                            return;
                        }

                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("LEFT_RAID")),
                                0));
                        if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                        {
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId,
                                Session.Character.MapX, Session.Character.MapY);
                        }

                        grp = Session.Character.Group;
                        Session.SendPacket(Session.Character.GenerateRaid(1, true));
                        Session.SendPacket(Session.Character.GenerateRaid(2, true));
                        if (grp != null)
                        {
                            grp.LeaveGroup(Session);
                            grp.Sessions.ForEach(s =>
                            {
                                s.SendPacket(grp.GenerateRdlst());
                                s.SendPacket(s.Character.GenerateRaid(0));
                            });
                        }
                        break;

                    // Kick from Raid
                    case 3:
                        if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                        {
                            return;
                        }

                        if (Session.Character.Group?.IsLeader(Session) == true)
                        {
                            ClientSession chartokick = ServerManager.Instance.GetSessionByCharacterId(CharacterId);
                            if (chartokick.Character?.Group == null)
                            {
                                return;
                            }

                            chartokick.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KICK_RAID"), 0));
                            grp = chartokick.Character?.Group;
                            chartokick.SendPacket(chartokick.Character?.GenerateRaid(1, true));
                            chartokick.SendPacket(chartokick.Character?.GenerateRaid(2, true));
                            grp?.LeaveGroup(chartokick);
                            grp?.Sessions.ForEach(s =>
                            {
                                s.SendPacket(grp.GenerateRdlst());
                                s.SendPacket(s.Character.GenerateRaid(0));
                            });
                        }

                        break;

                    // Disolve Raid
                    case 4:
                        if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                        {
                            return;
                        }

                        if (Session.Character.Group?.IsLeader(Session) == true)
                        {
                            grp = Session.Character.Group;

                            ClientSession[] grpmembers = new ClientSession[40];
                            grp.Sessions.CopyTo(grpmembers);
                            foreach (ClientSession targetSession in grpmembers)
                            {
                                if (targetSession != null)
                                {
                                    targetSession.SendPacket(targetSession.Character.GenerateRaid(1, true));
                                    targetSession.SendPacket(targetSession.Character.GenerateRaid(2, true));
                                    targetSession.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("RAID_DISOLVED"), 0));
                                    grp.LeaveGroup(targetSession);
                                }
                            }

                            ServerManager.Instance.GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                            ServerManager.Instance.ThreadSafeGroupList.Remove(grp.GroupId);
                        }

                        break;
                }
            }
        }

        #endregion
    }
}
