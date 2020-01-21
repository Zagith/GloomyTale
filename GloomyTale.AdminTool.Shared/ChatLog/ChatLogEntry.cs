using OpenNos.Domain.AdminTool;
using System;

namespace GloomyTale.AdminTool.Shared.ChatLog
{
    [Serializable]
    public class ChatLogEntry
    {
        public string Sender { get; set; }

        public long? SenderId { get; set; }

        public string Receiver { get; set; }

        public long? ReceiverId { get; set; }

        public ChatLogType MessageType { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString() => $"[{Timestamp}]<{MessageType}> {Sender}({SenderId})->{Receiver}({ReceiverId}) > {Message}";
    }
}
