﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)
using GloomyTale.Core;

namespace GloomyTale.GameObject.Packets.ClientPackets
{
    [PacketHeader("glmk")]
    public class CreateFamilyPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        #endregion
    }
}