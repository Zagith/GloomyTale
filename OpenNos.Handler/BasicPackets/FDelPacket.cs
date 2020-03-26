using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("fdel")]
    public class FDelPacket
    {
        #region Properties

        public long CharacterId { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 3)
            {
                return;
            }
            FDelPacket packetDefinition = new FDelPacket();
            if (long.TryParse(packetSplit[2], out long charId))
            {
                packetDefinition.CharacterId = charId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(FDelPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
#warning TODO IsSpouse unique check
            if (Session.Character.CharacterRelations.Any(s => s.RelatedCharacterId == CharacterId && s.RelationType == CharacterRelationType.Spouse))
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("CANT_DELETE_COUPLE")}");
                return;
            }
            Session.Character.DeleteRelation(CharacterId, CharacterRelationType.Friend);
            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_DELETED")));
        }

        #endregion
    }
}
