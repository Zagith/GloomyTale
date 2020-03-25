﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$Restart", PassNonParseablePacket = true, Authority = AuthorityType.SGM)]
    public class RestartPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                RestartPacket packetDefinition = new RestartPacket();
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RestartPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Restart ";

        private void ExecuteHandler(ClientSession session)
        {
            Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(), "[Restart]");

            if (ServerManager.Instance.TaskShutdown != null)
            {
                ServerManager.Instance.ShutdownStop = true;
                ServerManager.Instance.TaskShutdown = null;
            }
            else
            {
                ServerManager.Instance.IsReboot = true;
                ServerManager.Instance.TaskShutdown = ServerManager.Instance.ShutdownTaskAsync();
                ServerManager.Instance.TaskShutdown.Start();
            }
        }

        #endregion
    }
}