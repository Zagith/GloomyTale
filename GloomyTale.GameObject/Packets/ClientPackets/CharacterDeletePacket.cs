﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using GloomyTale.Core;

namespace GloomyTale.GameObject
{
    [PacketHeader("Char_DEL")]
    public class CharacterDeletePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Slot { get; set; }

        [PacketIndex(1)]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"Delete Character Slot {Slot}";
        }

        #endregion
    }
}