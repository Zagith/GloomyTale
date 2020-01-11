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
using System;

namespace GloomyTale.Data
{
    [Serializable]
    public class ShopDTO : IStaticDto
    {
        #region Properties

        public int MapNpcId { get; set; }

        public byte MenuType { get; set; }

        [I18NFrom(typeof(I18NShopNameDto))]
        public I18NString Name { get; set; } = new I18NString();
        [AdaptMember("Name")]
        public string NameI18NKey { get; set; }

        public int ShopId { get; set; }

        public byte ShopType { get; set; }

        #endregion
    }
}