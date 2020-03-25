﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$Shutdown", PassNonParseablePacket = true, Authority = AuthorityType.SGM)]
    public class ShutdownPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                ShutdownPacket packetDefinition = new ShutdownPacket();
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ShutdownPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Shutdown ";

        private void ExecuteHandler(ClientSession session)
        {
            Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(), "[Shutdown]");

            if (ServerManager.Instance.TaskShutdown != null)
            {
                ServerManager.Instance.ShutdownStop = true;
                ServerManager.Instance.TaskShutdown = null;
            }
            else
            {
                ServerManager.Instance.TaskShutdown = ServerManager.Instance.ShutdownTaskAsync();
                ServerManager.Instance.TaskShutdown.Start();
            }
        }

        #endregion
    }
}