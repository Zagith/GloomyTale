﻿/*
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

using OpenNos.Data;
using OpenNos.Domain;
using System;
using System.Collections.Generic;

namespace OpenNos.GameObject.Battle
{
    public class HitRequest
    {
        #region Instantiation

        public HitRequest(TargetHitType targetHitType, ClientSession session, Mate mate, NpcMonsterSkill skill)
        {
            HitTimestamp = DateTime.Now;
            Mate = mate;
            Skill = skill?.Skill;
            TargetHitType = targetHitType;
            Session = session;
            SkillBCards = skill?.Skill.BCards ?? new List<BCard>();
            SkillEffect = skill?.Skill.Effect ?? 0;
        }

        public HitRequest(TargetHitType targetHitType, MapMonster monster, NpcMonsterSkill skill, bool showTargetAnimation = false)
        {
            HitTimestamp = DateTime.Now;
            Monster = monster;
            Skill = skill?.Skill;
            TargetHitType = targetHitType;
            SkillBCards = skill?.Skill.BCards ?? new List<BCard>();
            SkillEffect = skill.Skill.Effect;
            ShowTargetHitAnimation = showTargetAnimation;
        }

        public HitRequest(TargetHitType targetHitType, ClientSession session, Skill skill, short? skillEffect = null, short? mapX = null, short? mapY = null, ComboDTO skillCombo = null, bool showTargetAnimation = false, List<BCard> skillBCards = null)
        {
            HitTimestamp = DateTime.Now;
            Session = session;
            Skill = skill;
            TargetHitType = targetHitType;
            SkillEffect = skillEffect ?? skill.Effect;
            ShowTargetHitAnimation = showTargetAnimation;

            if (mapX.HasValue)
            {
                MapX = mapX.Value;
            }

            if (mapY.HasValue)
            {
                MapY = mapY.Value;
            }


            if (skillCombo != null)
            {
                SkillCombo = skillCombo;
            }

            if (skillBCards != null)
            {
                SkillBCards = skillBCards;
            }
            else
            {
                SkillBCards = skill?.BCards ?? new List<BCard>();
            }
        }

        #endregion

        #region Properties

        public DateTime HitTimestamp { get; set; }

        public Mate Mate { get; set; }

        public MapMonster Monster { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public ClientSession Session { get; set; }

        /// <summary>
        /// Some AOE Skills need to show additional SU packet for Animation
        /// </summary>
        public bool ShowTargetHitAnimation { get; set; }

        public Skill Skill { get; set; }

        public List<BCard> SkillBCards { get; set; }

        public ComboDTO SkillCombo { get; set; }

        public short SkillEffect { get; set; }

        public NpcMonsterSkill NpcMonsterSkill { get; set; }

        public TargetHitType TargetHitType { get; set; }

        #endregion
    }
}