﻿using GloomyTale.Data;
using System.Collections.Generic;

namespace GloomyTale.DAL.Interface
{
    public interface IRecipeListDAO
    {
        #region Methods

        RecipeListDTO Insert(RecipeListDTO recipeList);

        IEnumerable<RecipeListDTO> LoadAll();

        RecipeListDTO LoadById(int recipeListId);

        IEnumerable<RecipeListDTO> LoadByItemVNum(short itemVNum);

        IEnumerable<RecipeListDTO> LoadByMapNpcId(int mapNpcId);

        IEnumerable<RecipeListDTO> LoadByRecipeId(short recipeId);

        void Update(RecipeListDTO recipe);

        #endregion
    }
}
