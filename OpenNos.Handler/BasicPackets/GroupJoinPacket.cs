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
    [PacketHeader("pjoin")]
    public class GroupJoinPacket
    {
        #region Properties

        public long CharacterId { get; set; }

        public GroupRequestType RequestType { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            GroupJoinPacket packetDefinition = new GroupJoinPacket();
            if (Enum.TryParse(packetSplit[2], out GroupRequestType requestType) && long.TryParse(packetSplit[3], out long characterId))
            {
                packetDefinition.RequestType = requestType;
                packetDefinition.CharacterId = characterId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(GroupJoinPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            bool createNewGroup = true;
            ClientSession targetSession = ServerManager.Instance.GetSessionByCharacterId(CharacterId);

            if ((targetSession == null && !RequestType.Equals(GroupRequestType.Sharing))
            || targetSession?.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance
            || ServerManager.Instance.ArenaMembers.ToList().Any(s => s.Session?.Character?.CharacterId == Session.Character.CharacterId)
            || (targetSession != null && ServerManager.Instance.ChannelId == 51 && targetSession.Character.Faction != Session.Character.Faction)
            || Session.Character.Timespace != null
            || targetSession?.Character.Timespace != null)
            {
                return;
            }

            if (RequestType.Equals(GroupRequestType.Requested)
                || RequestType.Equals(GroupRequestType.Invited))
            {
                if (CharacterId == 0)
                {
                    return;
                }

                if (ServerManager.Instance.IsCharactersGroupFull(CharacterId))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                    return;
                }

                if (ServerManager.Instance.IsCharacterMemberOfGroup(CharacterId)
                    && ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("ALREADY_IN_GROUP")));
                    return;
                }

                if (Session.Character.CharacterId != CharacterId && targetSession != null)
                {
                    if (Session.Character.IsBlockedByCharacter(CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(
                                Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                        return;
                    }

                    if (targetSession.Character.IsBlockedByCharacter(Session.Character.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(
                                Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKING")));
                        return;
                    }

                    if (targetSession.Character.GroupRequestBlocked)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_BLOCKED"),
                                0));
                    }
                    else
                    {
                        // save sent group request to current character
                        Session.Character.GroupSentRequestCharacterIds.Add(targetSession.Character.CharacterId);
                        if (Session.Character.Group == null || Session.Character.Group.GroupType == GroupType.Group)
                        {
                            if (targetSession.Character?.Group == null
                                || targetSession.Character?.Group.GroupType == GroupType.Group)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                                    string.Format(Language.Instance.GetMessageFromKey("GROUP_REQUEST"),
                                        targetSession.Character.Name)));
                                targetSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                    $"#pjoin^3^{Session.Character.CharacterId} #pjoin^4^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU"), Session.Character.Name)}"));
                            }
                            else
                            {
                                //can't invite raid member
                            }
                        }
                        else if (Session.Character.Group.IsLeader(Session))
                        {
                            targetSession.SendPacket(
                                $"qna #rd^1^{Session.Character.CharacterId}^1 {string.Format(Language.Instance.GetMessageFromKey("INVITE_RAID"), Session.Character.Name)}");
                        }
                    }
                }
            }
            else if (RequestType.Equals(GroupRequestType.Sharing))
            {
                if (Session.Character.Group != null)
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_SHARE_INFO")));
                    Session.Character.Group.Sessions
                        .Where(s => s.Character.CharacterId != Session.Character.CharacterId).ToList().ForEach(s =>
                        {
                            s.SendPacket(UserInterfaceHelper.GenerateDialog(
                                $"#pjoin^6^{Session.Character.CharacterId} #pjoin^7^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INVITED_YOU_SHARE"), Session.Character.Name)}"));
                            Session.Character.GroupSentRequestCharacterIds.Add(s.Character.CharacterId);
                        });
                }
            }
            else if (RequestType.Equals(GroupRequestType.Accepted))
            {
                if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems()
                        .Contains(Session.Character.CharacterId) == false)
                {
                    return;
                }

                try
                {
                    targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId)
                    && ServerManager.Instance.IsCharacterMemberOfGroup(CharacterId))
                {
                    // everyone is in group, return
                    return;
                }

                if (ServerManager.Instance.IsCharactersGroupFull(CharacterId)
                    || ServerManager.Instance.IsCharactersGroupFull(Session.Character.CharacterId))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                    targetSession?.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_FULL")));
                    return;
                }

                // get group and add to group
                if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                {
                    // target joins source
                    Group currentGroup =
                        ServerManager.Instance.GetGroupByCharacterId(Session.Character.CharacterId);

                    if (currentGroup != null)
                    {
                        currentGroup.JoinGroup(targetSession);
                        targetSession?.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOINED_GROUP"),
                                10));
                        createNewGroup = false;
                    }
                }
                else if (ServerManager.Instance.IsCharacterMemberOfGroup(CharacterId))
                {
                    // source joins target
                    Group currentGroup = ServerManager.Instance.GetGroupByCharacterId(CharacterId);

                    if (currentGroup != null)
                    {
                        createNewGroup = false;
                        if (currentGroup.GroupType == GroupType.Group)
                        {
                            currentGroup.JoinGroup(Session);
                        }
                        else
                        {
                            if (!currentGroup.Raid.InstanceBag.Lock)
                            {
                                if (Session.Character.Level < currentGroup.Raid?.LevelMinimum)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("LOW_LVL"), 10));
                                    return;
                                }

                                if (currentGroup.JoinGroup(Session))
                                {
                                    Session.SendPacket(
                                    Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_JOIN"),
                                        10));
                                    if (Session.Character.Level > currentGroup.Raid?.LevelMaximum)
                                    {
                                        Session.SendPacket(Session.Character.GenerateSay(
                                            Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));
                                        if (Session.Character.Level
                                            >= currentGroup.Raid?.LevelMaximum + 10 /* && AlreadySuccededToday*/)
                                        {
                                            //modal 1 ALREADY_SUCCEDED_AS_ASSISTANT
                                        }
                                    }

                                    Session.SendPacket(Session.Character.GenerateRaid(2));
                                    Session.SendPacket(Session.Character.GenerateRaid(1));
                                    currentGroup.Sessions.ForEach(s =>
                                    {
                                        s.SendPacket(currentGroup.GenerateRdlst());
                                        s.SendPacket(s.Character.GenerateSay(
                                            string.Format(Language.Instance.GetMessageFromKey("JOIN_TEAM"),
                                                Session.Character.Name), 10));
                                        s.SendPacket(s.Character.GenerateRaid(0));
                                    });
                                }
                                else
                                {
                                    if (ServerManager.Instance.RainbowBattleMembersRegistered.Count() > 0)
                                    {
                                        if (ServerManager.Instance.RainbowBattleMembersRegistered?.Where(s => s.Session == Session).Count() > 0)
                                        {
                                            return;
                                        }
                                    }
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("BA_JOIN"), 10));

                                    currentGroup.JoinGroup(Session);
                                    Session.SendPacket(Session.Character.GenerateFbt(1));
                                    currentGroup.Sessions.ForEach(s =>
                                    {
                                        s.SendPacket(currentGroup.GenerateFblst());
                                        s.SendPacket(s.Character.GenerateSay(
                                            string.Format(Language.Instance.GetMessageFromKey("JOIN_TEAM_BA"),
                                                Session.Character.Name), 10));
                                        s.SendPacket(s.Character.GenerateFbt(0));
                                    });

                                    ServerManager.Instance.RainbowBattleMembers.Add(new RainbowBattleMember
                                    {
                                        RainbowBattleType = EventType.RAINBOWBATTLE,
                                        Session = Session,
                                        GroupId = currentGroup.GroupId,
                                    });
                                }
                            }
                            else
                            {
                                Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_ALREADY_STARTED"),
                                    10));
                            }
                        }
                    }
                }

                if (createNewGroup)
                {
                    Group group = new Group
                    {
                        GroupType = GroupType.Group
                    };
                    group.JoinGroup(CharacterId);
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("GROUP_JOIN"),
                            targetSession?.Character.Name), 10));
                    group.JoinGroup(Session.Character.CharacterId);
                    ServerManager.Instance.AddGroup(group);
                    targetSession?.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_ADMIN")));

                    // set back reference to group
                    Session.Character.Group = group;
                    if (targetSession != null)
                    {
                        targetSession.Character.Group = @group;
                    }
                }

                if (Session.Character?.Group?.GroupType == GroupType.Group)
                {
                    // player join group
                    ServerManager.Instance.UpdateGroup(CharacterId);
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GeneratePidx());
                }
            }
            else if (RequestType == GroupRequestType.Declined)
            {
                if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems().Contains(Session.Character.CharacterId) == false)
                {
                    return;
                }

                targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                targetSession?.SendPacket(Session.Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("REFUSED_GROUP_REQUEST"),
                        Session.Character.Name), 10));
            }
            else if (RequestType == GroupRequestType.AcceptedShare)
            {
                if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems().Contains(Session.Character.CharacterId) == false)
                {
                    return;
                }

                targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                    string.Format(Language.Instance.GetMessageFromKey("ACCEPTED_SHARE"),
                        targetSession?.Character.Name), 0));
                if (Session.Character?.Group?.IsMemberOfGroup(CharacterId) == true && targetSession != null)
                {
                    Session.Character.SetReturnPoint(targetSession.Character.Return.DefaultMapId,
                        targetSession.Character.Return.DefaultX, targetSession.Character.Return.DefaultY);
                    targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("CHANGED_SHARE"), targetSession.Character.Name), 0));
                }
            }
            else if (RequestType == GroupRequestType.DeclinedShare)
            {
                if (targetSession?.Character.GroupSentRequestCharacterIds.GetAllItems()
                        .Contains(Session.Character.CharacterId) == false)
                {
                    return;
                }

                targetSession?.Character.GroupSentRequestCharacterIds.Remove(Session.Character.CharacterId);

                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REFUSED_SHARE"), 0));
            }
        }

        #endregion
    }
}
