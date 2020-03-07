﻿using GloomyTale.Core;
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Packets.ServerPackets
{
    [PacketHeader("icon")]
    public class IconPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public VisualType VisualType { get; set; }
        [PacketIndex(1)]
        public long VisualId { get; set; }
        [PacketIndex(2)]
        public byte IconType { get; set; }
        [PacketIndex(3)]
        public short IconParameter { get; set; }
    }
}