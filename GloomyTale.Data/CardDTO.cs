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

using Mapster;
using GloomyTale.Data.Base;
using GloomyTale.Data.I18N;
using GloomyTale.Data.Interfaces;
using GloomyTale.Domain;
using System;

namespace GloomyTale.Data
{
    [Serializable]
    public class CardDTO : MappingBaseDTO, IStaticDto
    {
        #region Properties

        public BuffType BuffType { get; set; }

        public short CardId { get; set; }

        public int Delay { get; set; }

        public int Duration { get; set; }

        public int EffectId { get; set; }

        public byte Level { get; set; }

        [I18NFrom(typeof(II18NCardDto))]
        public I18NString Name { get; set; } = new I18NString();
        [AdaptMember("Name")]
        public string NameI18NKey { get; set; }

        public byte Propability { get; set; }

        public short TimeoutBuff { get; set; }

        public byte TimeoutBuffChance { get; set; }

        #endregion
    }
}