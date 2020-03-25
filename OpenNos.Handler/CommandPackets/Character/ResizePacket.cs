﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;

namespace OpenNos.Handler.CommandPackets.Character
{
    [PacketHeader("$Resize", PassNonParseablePacket = true, Authority = AuthorityType.GM)]
    public class ResizePacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public int Value { get; set; }

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
                ResizePacket packetDefinition = new ResizePacket();
                if (int.TryParse(packetSplit[2], out int value))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.Value = value;
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ResizePacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$Resize VALUE";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(), $"[Resize]Size: {Value}");

                if (Value >= 0)
                {
                    session.Character.Size = Value;
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateScal());
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