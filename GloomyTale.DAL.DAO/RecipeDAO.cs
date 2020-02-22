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

using GloomyTale.Core;
using GloomyTale.DAL.EF;

using GloomyTale.DAL.EF.Helpers;
using GloomyTale.DAL.Interface;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class RecipeDAO : MappingBaseDao<Recipe, RecipeDTO>, IRecipeDAO
    {
        public RecipeDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public RecipeDTO Insert(RecipeDTO recipe)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Recipe>(recipe);
                    context.Recipe.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<RecipeDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Recipe Recipe in context.Recipe)
                {
                    yield return _mapper.Map<RecipeDTO>(Recipe);
                }
            }
        }

        public RecipeDTO LoadById(short recipeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RecipeDTO>(context.Recipe.FirstOrDefault(s => s.RecipeId.Equals(recipeId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeDTO> LoadByItemVNum(short itemVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Recipe Recipe in context.Recipe.Where(s => s.ItemVNum.Equals(itemVNum)))
                {
                    yield return _mapper.Map<RecipeDTO>(Recipe);
                }
            }
        }

        public void Update(RecipeDTO recipe)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Recipe result = context.Recipe.FirstOrDefault(c => c.ItemVNum == recipe.ItemVNum);
                    if (result != null)
                    {
                        recipe.RecipeId = result.RecipeId;
                        _mapper.Map(recipe, result);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        #endregion
    }
}