using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
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
    [PacketHeader("blins")]
    public class BlInsPacket
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
            BlInsPacket packetDefinition = new BlInsPacket();
            if (long.TryParse(packetSplit[2], out long charId))
            {
                packetDefinition.CharacterId = charId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(BlInsPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            if (session.Character.CharacterId == CharacterId)
            {
                return;
            }

            if (DAOFactory.CharacterDAO.LoadById(CharacterId) is CharacterDTO character
             && DAOFactory.AccountDAO.LoadById(character.AccountId).Authority >= AuthorityType.GM)
            {
                session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_BLACKLIST_TEAM")));
                return;
            }

            session.Character.AddRelation(CharacterId, CharacterRelationType.Blocked);
            session.SendPacket(
                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_ADDED")));
            session.SendPacket(session.Character.GenerateBlinit());
        }

        #endregion
    }
}
