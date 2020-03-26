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
    [PacketHeader("rl")]
    public class RaidListRegisterPacket
    {
        #region Properties

        public string CharacterName { get; set; }

        public short Type { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 2)
            {
                return;
            }
            RaidListRegisterPacket packetDefinition = new RaidListRegisterPacket();
            if (true)
            {
                if (packetSplit.Length > 2 && short.TryParse(packetSplit[2], out short type))
                {
                    packetDefinition.Type = type;
                }

                if (packetSplit.Length > 3 && !string.IsNullOrEmpty(packetSplit[3]))
                {
                    packetDefinition.CharacterName = packetSplit[3];
                }

                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RaidListRegisterPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            switch (Type)
            {
                case 0: // Show the Raid List
                    if (Session.Character.Group?.IsLeader(Session) == true
                        && Session.Character.Group.GroupType != GroupType.Group
                        && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(1));
                    }
                    else if (Session.Character.Group != null && Session.Character.Group.GroupType != GroupType.Group
                             && Session.Character.Group.IsLeader(Session))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(2));
                    }
                    else if (Session.Character.Group != null)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(3));
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(0));
                    }

                    break;

                case 1: // Register a team
                    if (Session.Character.Group != null
                        && Session.Character.Group.GroupType != GroupType.Group
                        && Session.Character.Group.IsLeader(Session)
                        && !ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Add(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(1));
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("RAID_REGISTERED")));
                        ServerManager.Instance.Broadcast(Session,
                            $"qnaml 100 #rl {string.Format(Language.Instance.GetMessageFromKey("SEARCH_TEAM_MEMBERS"), Session.Character.Name, Session.Character.Group.Raid?.Label)}",
                            ReceiverType.AllExceptGroup);
                    }

                    break;

                case 2: // Cancel the team registration
                    if (Session.Character.Group != null
                        && Session.Character.Group.GroupType != GroupType.Group
                        && Session.Character.Group.IsLeader(Session)
                        && ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
                    {
                        ServerManager.Instance.GroupList.Remove(Session.Character.Group);
                        Session.SendPacket(UserInterfaceHelper.GenerateRl(2));
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("RAID_UNREGISTERED")));
                    }

                    break;

                case 3: // Become a team member
                    ClientSession targetSession = ServerManager.Instance.GetSessionByCharacterName(CharacterName);

                    if (targetSession?.Character?.Group == null)
                    {
                        return;
                    }

                    if (targetSession.Character.CharacterId == Session.Character.CharacterId)
                    {
                        return;
                    }

                    if (!targetSession.Character.Group.IsLeader(targetSession))
                    {
                        return;
                    }

                    if (!ServerManager.Instance.GroupList.Any(group => group.GroupId == targetSession.Character.Group.GroupId))
                    {
                        return;
                    }

                    targetSession.Character.GroupSentRequestCharacterIds.Add(Session.Character.CharacterId);

                    GroupJoinPacket.HandlePacket(Session, new PJoinPacket
                    {
                        RequestType = GroupRequestType.Accepted,
                        CharacterId = targetSession.Character.CharacterId
                    }.ToString());

                    break;
            }
        }

        #endregion
    }
}
