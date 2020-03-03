/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using GloomyTale.Domain;
using GloomyTale.GameObject.Packets.ServerPackets;

namespace GloomyTale.GameObject.Helpers
{
    public static class StaticPacketHelper
    {
        #region Methods

        public static string Cancel(byte type = 0, long callerId = 0) => $"cancel {type} {callerId}";

        public static string CastOnTarget(VisualType attackerType, long attackerId, VisualType defenderType, long defenderId, short castAnimation, short castEffect, short skillVNum) => $"ct {(byte)attackerType} {attackerId} {(byte)defenderType} {defenderId} {castAnimation} {castEffect} {skillVNum}";

        public static EffectPacket GenerateEff(VisualType effectType, long callerId, int effectId)
        {
            return new EffectPacket
            {
                EffectType = effectType,
                CallerId = callerId,
                EffectId = effectId
            };
        }

        public static string In(VisualType type, short callerVNum, long callerId, short mapX, short mapY, int direction, int currentHp, int currentMp, short dialog, InRespawnType respawnType, bool isSitting, string Name, bool invisible)
        {
            switch (type)
            {
                case VisualType.Npc:
                case VisualType.Monster:
                    return $"in {(byte)type} {callerVNum} {callerId} {mapX} {mapY} {direction} {currentHp} {currentMp} {dialog} 0 0 -1 {(byte)respawnType} {(isSitting ? 1 : 0)} -1 {Name} 0 -1 0 0 0 0 0 0 0 {(invisible ? 1 : 0)}";

                case VisualType.Object:
                    return $"in 9 {callerVNum} {callerId} {mapX} {mapY} {direction} 0 0 -1";

                default:
                    return "";
            }
        }

        public static MovePacket Move(VisualType type, long callerId, short positionX, short positionY, byte speed)
        {
            return new MovePacket
            {
                MoveType = type,
                CallerId = callerId,
                PositionX = positionX,
                PositionY = positionY,
                Speed = speed
            };
        }

        public static string Out(VisualType type, long callerId) => $"out {(byte)type} {callerId}";

        public static string Say(byte type, long callerId, byte secondaryType, string message) => $"say {type} {callerId} {secondaryType} {message}";

        public static string SkillReset(int castId) => $"sr {castId}";

        public static string SkillResetWithCoolDown(int castId, int cooldown) => $"sr -10 {castId} {cooldown}";

        public static string SkillUsed(VisualType type, long callerId, byte secondaryType, long targetId, short skillVNum, short cooldown, short attackAnimation, short skillEffect, short x, short y, bool isAlive, int health, int damage, int hitmode, byte skillType) => $"su {(byte)type} {callerId} {secondaryType} {targetId} {skillVNum} {cooldown} {attackAnimation} {skillEffect} {x} {y} {(isAlive ? 1 : 0)} {health} {damage} {hitmode} {skillType}";

        public static string SkillUsed(VisualType attackerType, long attackerId, VisualType defenderType, long defenderId, short skillVNum, short cooldown, short attackAnimation, short skillEffect, short x, short y, bool isAlive, int hpPercent, int damage, int hitMode, byte skillType) => SkillUsed(attackerType, attackerId, (byte)defenderType, defenderId, skillVNum, cooldown, attackAnimation, skillEffect, x, y, isAlive, hpPercent, damage, hitMode, skillType);

        public static string Die(VisualType callerType, long callerId, VisualType targetType, long targetId) => $"die {(byte)callerType} {callerId} {(byte)targetType} {targetId}";

        public static string GenerateGet(byte pickerType, long pickerId, long itemId) => $"get {pickerType} {pickerId} {itemId} 0";

        #endregion
    }
}