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

using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IItemDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<ItemDTO> FindByName(string name);

        ItemDTO Insert(ItemDTO item);

        void Insert(IEnumerable<ItemDTO> items);

        IEnumerable<ItemDTO> LoadAll();

        ItemDTO LoadByKey(string vNum);

        ItemDTO LoadById(short vNum);

        #endregion
    }
}