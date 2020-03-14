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

using GloomyTale.Domain;

namespace GloomyTale.GameObject.Battle
{
    public class MTListHitTarget
    {
        #region Instantiation

        public MTListHitTarget(VisualType entityType, long targetId, TargetHitType targetHitType)
        {
            EntityType = entityType;
            TargetId = targetId;
            TargetHitType = targetHitType;
        }

        #endregion

        #region Properties

        public VisualType EntityType { get; set; }

        public long TargetId { get; set; }

        public TargetHitType TargetHitType { get; set; }

        #endregion
    }
}