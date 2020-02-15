using GloomyTale.Core;
using GloomyTale.Domain;

namespace GloomyTale.GameObject.CommandPackets
{
    [PacketHeader("$AddQuest", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.GA })]
    public class AddQuestPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short QuestId { get; set; }

        public static string ReturnHelp() => "$AddQuest <QuestId>";

        #endregion
    }
}
