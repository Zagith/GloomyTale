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
    [PacketHeader("fins")]
    public class FInsPacket
    {
        #region Properties

        public long CharacterId { get; set; }

        public byte Type { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            FInsPacket packetDefinition = new FInsPacket();
            if (byte.TryParse(packetSplit[2], out byte type) && long.TryParse(packetSplit[3], out long charId))
            {
                packetDefinition.Type = type;
                packetDefinition.CharacterId = charId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(FInsPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            long characterId = CharacterId;
            if (Session.Character.CharacterId == CharacterId)
            {
                return;
            }
            if (Session.CurrentMapInstance?.MapInstanceType != MapInstanceType.BaseMapInstance)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("IMPOSSIBLE_TO_USE")));
                return;
            }
            
            ClientSession otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
            if (otherSession != null)
            {
                if (Session.Character.Timespace != null || otherSession.Character.Timespace != null)
                {
                    return;
                }
                if (!Session.Character.IsFriendlistFull())
                {
                    if (!Session.Character.IsFriendOfCharacter(characterId) && (Type == 1 || Type == 2)
                     || !Session.Character.IsMarried && (Type == 34 || Type == 69))
                    {
                        if (!Session.Character.IsBlockedByCharacter(characterId))
                        {
                            if (!Session.Character.IsBlockingCharacter(characterId))
                            {
                                if (otherSession.Character.MarryRequestCharacters.GetAllItems()
                                       .Contains(Session.Character.CharacterId))
                                {
                                    switch (Type)
                                    {
                                        case 34:
                                            Session.Character.DeleteRelation(characterId, CharacterRelationType.Friend);
                                            Session.Character.AddRelation(characterId, CharacterRelationType.Spouse);
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("MARRIAGE_ACCEPT")}");
                                            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("MARRIAGE_ACCEPT_SHOUT"), Session.Character.Name, otherSession.Character.Name), 0));
                                            break;

                                        case 69:
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("MARRIAGE_REJECTED")}");
                                            //ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MARRIAGE_REJECT_SHOUT"), 1));
                                            break;
                                    }
                                }
                                if (otherSession.Character.FriendRequestCharacters.GetAllItems()
                                    .Contains(Session.Character.CharacterId))
                                {
                                    switch (Type)
                                    {
                                        case 1:
                                            Session.Character.AddRelation(characterId, CharacterRelationType.Friend);
                                            Session.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                            break;

                                        case 2:
                                            otherSession.SendPacket(
                                                $"info {Language.Instance.GetMessageFromKey("FRIEND_REJECTED")}");
                                            break;

                                        default:
                                            if (Session.Character.IsFriendlistFull())
                                            {
                                                Session.SendPacket(
                                                    $"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                                otherSession.SendPacket(
                                                    $"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                            }
                                            break;
                                    }
                                }
                                else if (Type != 34 && Type != 69)
                                {
                                    if (otherSession.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                                    {
                                        return;
                                    }

                                    if (otherSession.Character.FriendRequestBlocked)
                                    {
                                        Session.SendPacket(
                                            $"info {Language.Instance.GetMessageFromKey("FRIEND_REJECTED")}");
                                        return;
                                    }

                                    otherSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#fins^1^{Session.Character.CharacterId} #fins^2^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("FRIEND_ADD"), Session.Character.Name)}"));
                                    Session.Character.FriendRequestCharacters.Add(characterId);
                                }
                            }
                            else
                            {
                                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKING")}");
                            }
                        }
                        else
                        {
                            Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")}");
                        }
                    }
                    else
                    {
                        Session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_FRIEND")}");
                    }
                }
                else
                {
                    Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                }
            }
        }

        #endregion
    }
}
