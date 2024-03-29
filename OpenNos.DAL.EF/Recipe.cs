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

using System.Collections.Generic;

namespace OpenNos.DAL.EF
{
    public class Recipe
    {
        #region Instantiation

        public Recipe()
        {
            RecipeItem = new HashSet<RecipeItem>();
            RecipeList = new HashSet<RecipeList>();
        }

        #endregion

        #region Properties

        public short Amount { get; set; }

        public virtual Item Item { get; set; }

        public short ItemVNum { get; set; }

        public short RecipeId { get; set; }

        public short Rare { get; set; }

        public byte Upgrade { get; set; }

        public virtual ICollection<RecipeItem> RecipeItem { get; set; }

        public virtual ICollection<RecipeList> RecipeList { get; set; }

        #endregion
    }
}