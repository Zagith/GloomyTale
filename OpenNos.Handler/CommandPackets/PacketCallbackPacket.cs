﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$Packet", PassNonParseablePacket = true, Authority = AuthorityType.Administrator)]
    public class PacketCallbackPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public string Packet { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                string[] packetSplit = packet.Split(new[] { ' ' }, 3);
                if (packetSplit.Length < 3)
                {
                    sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                    return;
                }
                PacketCallbackPacket packetDefinition = new PacketCallbackPacket();
                if (!string.IsNullOrWhiteSpace(packetSplit[2]))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.Packet = packetSplit[2];
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(PacketCallbackPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Packet PACKET";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(),
                    $"[Packet]Packet: {Packet}");

                session.SendPacket(Packet);
                session.SendPacket(session.Character.GenerateSay(Packet, 10));
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}