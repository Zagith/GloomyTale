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

using System;

namespace OpenNos.Data
{
    [Serializable]
    public class DropDTO
    {
        #region Properties

        public int Amount { get; set; }

        public int DropChance { get; set; }

        public short DropId { get; set; }

        public short ItemVNum { get; set; }

        public short? MapTypeId { get; set; }

        public short? MonsterVNum { get; set; }

        public bool IsLevelPenalty { get; set; }

        #endregion
    }
}