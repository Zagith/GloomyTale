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
    public class RecipeListDAO : MappingBaseDao<RecipeList, RecipeListDTO>, IRecipeListDAO
    {
        public RecipeListDAO(IMapper mapper) : base(mapper)
        { }

        #region Methods

        public IEnumerable<RecipeListDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeList recipeList in context.RecipeList)
                {
                    yield return _mapper.Map<RecipeListDTO>(recipeList);
                }
            }
        }

        public RecipeListDTO LoadById(int recipeListId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RecipeListDTO>(context.RecipeList.FirstOrDefault(s => s.RecipeListId.Equals(recipeListId)));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeListDTO> LoadByItemVNum(short itemVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeList recipeList in context.RecipeList.Where(s => s.ItemVNum.Equals(itemVNum)))
                {
                    yield return _mapper.Map<RecipeListDTO>(recipeList);
                }
            }
        }

        public IEnumerable<RecipeListDTO> LoadByMapNpcId(int mapNpcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeList recipeList in context.RecipeList.Where(s => s.MapNpcId.Equals(mapNpcId)))
                {
                    yield return _mapper.Map<RecipeListDTO>(recipeList);
                }
            }
        }

        public IEnumerable<RecipeListDTO> LoadByRecipeId(short recipeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeItem recipeItem in context.RecipeItem.Where(s => s.RecipeId.Equals(recipeId)))
                {
                    yield return _mapper.Map<RecipeListDTO>(recipeItem);
                }
            }
        }

        #endregion
    }
}