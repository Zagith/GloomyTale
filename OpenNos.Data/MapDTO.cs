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

using Mapster;
using OpenNos.Data.Base;
using OpenNos.Data.I18N;
using OpenNos.Data.Interfaces;
using System;

namespace OpenNos.Data
{
    [Serializable]
    public class MapDTO : IMapDTO, IStaticDto
    {
        #region Properties

        public byte[] Data { get; set; }

        public short MapId { get; set; }

        public short GridMapId { get; set; }

        public int Music { get; set; }

        [I18NFrom(typeof(II18NMapDto))]
        public I18NString Name { get; set; } = new I18NString();
        [AdaptMember("Name")]
        public string NameI18NKey { get; set; }

        public bool ShopAllowed { get; set; }

        public byte XpRate { get; set; }

        public int MeteoriteLevel { get; set; }

        public int Side { get; set; }

        public byte GoldMapRate { get; set; }

        #endregion
    }
}