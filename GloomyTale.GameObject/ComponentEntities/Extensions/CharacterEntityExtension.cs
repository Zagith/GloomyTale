using GloomyTale.GameObject.ComponentEntities.Interfaces;
using GloomyTale.GameObject.Packets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.ComponentEntities.Extensions
{
    public static class CharacterEntityExtension
    {
        public static GoldPacket GenerateGold(this ICharacterEntity characterEntity)
        {
            return new GoldPacket { Gold = characterEntity.Gold };
        }
    }
}
