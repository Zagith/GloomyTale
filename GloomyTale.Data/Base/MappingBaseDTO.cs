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

using System;

namespace GloomyTale.Data
{
    [Serializable]
    public class MappingBaseDTO
    {
        #region Methods

        /// <summary>
        ///     Intializes the GameObject, will be injected by AutoMapper after Entity -&gt; GO mapping
        ///     Needs to be override in inherited GameObject.
        /// </summary>
        public virtual void Initialize()
        {
            //TODO override in GO
        }

        #endregion
    }
}