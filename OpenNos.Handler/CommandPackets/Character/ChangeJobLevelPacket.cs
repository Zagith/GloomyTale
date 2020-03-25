﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.CommandPackets.Character
{
    [PacketHeader("$JLvl", PassNonParseablePacket = true, Authority = AuthorityType.GM)]
    public class ChangeJobLevelPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public byte JobLevel { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (!(session is ClientSession sess))
            {
                return;
            }

            if (packetSplit.Length < 3)
            {
                sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                return;
            }

            ChangeJobLevelPacket packetDefinition = new ChangeJobLevelPacket();
            if (byte.TryParse(packetSplit[2], out byte jobLevel))
            {
                packetDefinition._isParsed = true;
                packetDefinition.JobLevel = jobLevel;
            }

            packetDefinition.ExecuteHandler(sess);
            LogHelper.Instance.InsertCommandLog(sess.Character.CharacterId, packet, sess.IpAddress);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ChangeJobLevelPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$JLvl JOBLEVEL";

        private void ExecuteHandler(ClientSession Session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[JLvl]JobLevel: {JobLevel}");

                if (((Session.Character.Class == 0 && JobLevel <= 20)
                     || (Session.Character.Class != 0 && JobLevel <= 255))
                     && JobLevel > 0)
                {
                    Session.Character.JobLevel = JobLevel;
                    Session.Character.JobLevelXp = 0;
                    Session.Character.ResetSkills();
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOBLEVEL_CHANGED"), 0));
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 8), Session.Character.PositionX, Session.Character.PositionY);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}