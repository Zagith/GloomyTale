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

using GloomyTale.DAL;
using GloomyTale.Data;
using System.Collections.Generic;
using System.Linq;

namespace GloomyTale.GameObject
{
    public class Recipe : RecipeDTO
    {
        #region Properties

        public List<RecipeItemDTO> Items { get; set; }

        #endregion

        public Recipe()
        {
        }

        public Recipe(RecipeDTO input)
        {
            Amount = input.Amount;
            ItemVNum = input.ItemVNum;
            RecipeId = input.RecipeId;
            Rare = input.Rare;
            Upgrade = input.Upgrade;
        }

        #region Methods

        public void Initialize()
        {
            Items = new List<RecipeItemDTO>();
            foreach (RecipeItemDTO recipe in DAOFactory.Instance.RecipeItemDAO.LoadByRecipe(RecipeId).ToList())
            {
                Items.Add(recipe);
            }
        }

        #endregion
    }
}