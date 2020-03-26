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
    [PacketHeader("gop")]
    public class CharacterOptionPacket
    {
        #region Properties

        public bool IsActive { get; set; }

        public CharacterOption Option { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            CharacterOptionPacket packetDefinition = new CharacterOptionPacket();
            if (Enum.TryParse(packetSplit[2], out CharacterOption option))
            {
                packetDefinition.Option = option;
                packetDefinition.IsActive = packetSplit[3] == "1";
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(CharacterOptionPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character == null)
            {
                return;
            }

            switch (Option)
            {
                case CharacterOption.BuffBlocked:
                    Session.Character.BuffBlocked = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.BuffBlocked
                            ? "BUFF_BLOCKED"
                            : "BUFF_UNLOCKED"), 0));
                    break;

                case CharacterOption.EmoticonsBlocked:
                    Session.Character.EmoticonsBlocked = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.EmoticonsBlocked
                            ? "EMO_BLOCKED"
                            : "EMO_UNLOCKED"), 0));
                    break;

                case CharacterOption.ExchangeBlocked:
                    Session.Character.ExchangeBlocked = !IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.ExchangeBlocked
                            ? "EXCHANGE_BLOCKED"
                            : "EXCHANGE_UNLOCKED"), 0));
                    break;

                case CharacterOption.FriendRequestBlocked:
                    Session.Character.FriendRequestBlocked = !IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.FriendRequestBlocked
                            ? "FRIEND_REQ_BLOCKED"
                            : "FRIEND_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupRequestBlocked:
                    Session.Character.GroupRequestBlocked = !IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.GroupRequestBlocked
                            ? "GROUP_REQ_BLOCKED"
                            : "GROUP_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.PetAutoRelive:
                    Session.Character.IsPetAutoRelive = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.IsPetAutoRelive
                        ? "PET_AUTO_RELIVE_ENABLED"
                        : "PET_AUTO_RELIVE_DISABLED"), 0));
                    break;

                case CharacterOption.PartnerAutoRelive:
                    Session.Character.IsPartnerAutoRelive = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.IsPartnerAutoRelive
                        ? "PARTNER_AUTO_RELIVE_ENABLED"
                        : "PARTNER_AUTO_RELIVE_DISABLED"), 0));
                    break;

                case CharacterOption.HeroChatBlocked:
                    Session.Character.HeroChatBlocked = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.HeroChatBlocked
                            ? "HERO_CHAT_BLOCKED"
                            : "HERO_CHAT_UNLOCKED"), 0));
                    break;

                case CharacterOption.HpBlocked:
                    Session.Character.HpBlocked = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.HpBlocked ? "HP_BLOCKED" : "HP_UNLOCKED"),
                        0));
                    break;

                case CharacterOption.MinilandInviteBlocked:
                    Session.Character.MinilandInviteBlocked = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.MinilandInviteBlocked
                            ? "MINI_INV_BLOCKED"
                            : "MINI_INV_UNLOCKED"), 0));
                    break;

                case CharacterOption.MouseAimLock:
                    Session.Character.MouseAimLock = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.MouseAimLock
                            ? "MOUSE_LOCKED"
                            : "MOUSE_UNLOCKED"), 0));
                    break;

                case CharacterOption.QuickGetUp:
                    Session.Character.QuickGetUp = IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.QuickGetUp
                            ? "QUICK_GET_UP_ENABLED"
                            : "QUICK_GET_UP_DISABLED"), 0));
                    break;

                case CharacterOption.WhisperBlocked:
                    Session.Character.WhisperBlocked = !IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.WhisperBlocked
                            ? "WHISPER_BLOCKED"
                            : "WHISPER_UNLOCKED"), 0));
                    break;

                case CharacterOption.FamilyRequestBlocked:
                    Session.Character.FamilyRequestBlocked = !IsActive;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.FamilyRequestBlocked
                            ? "FAMILY_REQ_LOCKED"
                            : "FAMILY_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupSharing:
                    Group grp = ServerManager.Instance.Groups.Find(
                        g => g != null && g.IsMemberOfGroup(Session.Character.CharacterId));
                    if (grp == null)
                    {
                        return;
                    }

                    if (!grp.IsLeader(Session))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_MASTER"), 0));
                        return;
                    }

                    if (!IsActive)
                    {
                        grp.SharingMode = 1;

                        Session.CurrentMapInstance?.Broadcast(Session,
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING"), 0),
                            ReceiverType.Group);
                    }
                    else
                    {
                        grp.SharingMode = 0;

                        Session.CurrentMapInstance?.Broadcast(Session,
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING_BY_ORDER"), 0),
                            ReceiverType.Group);
                    }
                    break;
            }

            Session.SendPacket(Session.Character.GenerateStat());
        }

        #endregion
    }
}
