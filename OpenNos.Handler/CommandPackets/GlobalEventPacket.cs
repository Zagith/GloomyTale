﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.Master.Library.Client;
using System;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$GlobalEvent", PassNonParseablePacket = true, Authority = AuthorityType.GA)]
    public class GlobalEventPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public EventType EventType { get; set; }

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
                GlobalEventPacket packetDefinition = new GlobalEventPacket();
                if (Enum.TryParse(packetSplit[2], out EventType type))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.EventType = type;
                }
                packetDefinition.ExecuteHandler(sess);
                LogHelper.Instance.InsertCommandLog(sess.Character.CharacterId, packet, sess.IpAddress);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(GlobalEventPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$GlobalEvent EVENTTYPE";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(),
                    $"[GlobalEvent]EventType: {EventType.ToString()}");

                CommunicationServiceClient.Instance.RunGlobalEvent(EventType);
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}