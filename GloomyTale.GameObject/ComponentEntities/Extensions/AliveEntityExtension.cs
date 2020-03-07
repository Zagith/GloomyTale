using GloomyTale.Domain;
using GloomyTale.GameObject.ComponentEntities.Interfaces;
using GloomyTale.GameObject.Packets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.ComponentEntities.Extensions
{
    public static class AliveEntityExtension
    {
        public static ServerDirPacket GenerateChangeDir(this IAliveEntity namedEntity)
        {
            return new ServerDirPacket
            {
                VisualType = namedEntity.VisualType,
                VisualId = namedEntity.VisualId,
                Direction = namedEntity.Direction
            };
        }

        public static RequestNpcPacket GenerateNpcReq(this IAliveEntity namedEntity, long dialog)
        {
            return new RequestNpcPacket
            {
                Type = namedEntity.VisualType,
                TargetId = namedEntity.VisualId,
                Data = dialog
            };
        }

        public static PinitSubPacket GenerateSubPinit(this INamedEntity namedEntity, int groupPosition)
        {
            return new PinitSubPacket
            {
                VisualType = namedEntity.VisualType,
                VisualId = namedEntity.VisualId,
                GroupPosition = groupPosition,
                Level = namedEntity.Level,
                Name = namedEntity.Name,
                Gender = (namedEntity as ICharacterEntity)?.Gender ?? GenderType.Male,
                Race = namedEntity.Race,
                Morph = namedEntity.Morph,
                HeroLevel = namedEntity.HeroLevel
            };
        }

        public static PidxSubPacket GenerateSubPidx(this IAliveEntity playableEntity)
        {
            return playableEntity.GenerateSubPidx(false);
        }

        public static PidxSubPacket GenerateSubPidx(this IAliveEntity playableEntity, bool isMemberOfGroup)
        {
            return new PidxSubPacket
            {
                IsGrouped = isMemberOfGroup,
                VisualId = playableEntity.VisualId
            };
        }

        public static StPacket GenerateStatInfo(this IAliveEntity aliveEntity)
        {
            return new StPacket
            {
                Type = aliveEntity.VisualType,
                VisualId = aliveEntity.VisualId,
                Level = aliveEntity.Level,
                HeroLvl = aliveEntity.HeroLevel,
                HpPercentage = (int)(aliveEntity.Hp / (float)aliveEntity.MaxHp * 100),
                MpPercentage = (int)(aliveEntity.Mp / (float)aliveEntity.MaxMp * 100),
                CurrentHp = aliveEntity.Hp,
                CurrentMp = aliveEntity.Mp,
                BuffIds = null
            };
        }
    }
}
