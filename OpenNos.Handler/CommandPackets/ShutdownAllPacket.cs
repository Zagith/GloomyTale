﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$ShutdownAll", PassNonParseablePacket = true, Authority = AuthorityType.SGM)]
    public class ShutdownAllPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                ShutdownAllPacket packetDefinition = new ShutdownAllPacket();
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ShutdownAllPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$ShutdownAll ";

        private void ExecuteHandler(ClientSession session)
        {
            Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(), "[ShutdownAll]");

            CommunicationServiceClient.Instance.Shutdown(ServerManager.Instance.ServerGroup);
            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}