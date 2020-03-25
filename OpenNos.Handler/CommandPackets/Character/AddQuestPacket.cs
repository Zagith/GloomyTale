using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System.Linq;

namespace OpenNos.Handler.CommandPackets.Character
{
    [PacketHeader("$AddQuest", PassNonParseablePacket = true, Authority = AuthorityType.GA)]
    public class AddQuestPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public short QuestId { get; set; }

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
                AddQuestPacket packetDefinition = new AddQuestPacket();
                if (short.TryParse(packetSplit[2], out short questId))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.QuestId = questId;
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(AddQuestPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$AddQuest <QuestId>";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(),
                                       $"[AddQuest]QuestId: {QuestId}");

                if (ServerManager.Instance.Quests.Any(q => q.QuestId == QuestId))
                {
                    session.Character.AddQuest(QuestId, false);
                    return;
                }

                session.SendPacket(session.Character.GenerateSay("This Quest doesn't exist", 11));
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}
