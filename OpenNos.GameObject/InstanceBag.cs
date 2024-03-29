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

using System.Collections.Concurrent;

namespace OpenNos.GameObject
{
    public class InstanceBag
    {
        #region Instantiation

        public InstanceBag()
        {
            Clock = new Clock(1);
            DeadList = new ConcurrentBag<long>();
            ButtonLocker = new Locker();
            MonsterLocker = new Locker();
            StatueCounter = new Locker();
        }

        #endregion

        #region Properties

        public Locker ButtonLocker { get; set; }

        public Clock Clock { get; set; }

        public int Combo { get; set; }

        public long CreatorId { get; set; }

        public ConcurrentBag<long> DeadList { get; set; }

        public byte EndState { get; set; }

        public short Lives { get; set; }

        public bool Lock { get; set; }

        public Locker MonsterLocker { get; set; }

        public int MonstersKilled { get; set; }

        public int NpcsKilled { get; set; }

        public int Point { get; set; }

        public int RoomsVisited { get; set; }

        public int LaurenaRound { get; set; }

        public Locker StatueCounter { get; set; }
        #endregion

        #region Methods

        public string GenerateScore() => $"rnsc {Point}";

        #endregion
    }
}