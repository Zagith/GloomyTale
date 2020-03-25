using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$ListAccountFamily", Authority = AuthorityType.GM)]
    public class ListAccountFamilyPacket
    {
        #region Properties

        private bool _isParsed;

        public long AccountId { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                string[] packetSplit = packet.Split(' ');
                if (packetSplit.Length < 3)
                {
                    sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                    return;
                }
                ListAccountFamilyPacket packetDefinition = new ListAccountFamilyPacket();
                if (long.TryParse(packetSplit[2], out long accId))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.AccountId = accId;
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ListAccountFamilyPacket), HandlePacket);

        public static string ReturnHelp() => "$ListAccountFamily AccountId";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                void WriteAccountInfo(AccountDTO dto)
                {
                    session.SendPacket(session.Character.GenerateSay($"AccountId: {dto.AccountId}", 13));
                    session.SendPacket(session.Character.GenerateSay($"Name: {dto.Name}", 13));
                    session.SendPacket(session.Character.GenerateSay($"E-Mail: {dto.Email}", 13));
                    session.SendPacket(session.Character.GenerateSay("----- ------- -----", 13));
                }
                session.SendPacket(session.Character.GenerateSay("----- ACCOUNTS -----", 13));
                foreach (AccountDTO acc in DAOFactory.AccountDAO.LoadFamilyById(AccountId))
                {
                    WriteAccountInfo(acc);
                }
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}
