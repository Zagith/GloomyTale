﻿using GloomyTale.DAL.EF.Entities;
using GloomyTale.Data.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.Mapper.Mappers
{
    public class I18NShopNameMapper
    {
        #region Methods

        public static bool ToI18NShopName(I18NShopNameDto input, I18NShopName output)
        {
            if (input == null)
            {
                return false;
            }

            output.Key = input.Key;
            output.RegionType = input.RegionType;
            output.Text = input.Text;

            return true;
        }

        public static bool ToI18NShopNameDTO(I18NShopName input, I18NShopNameDto output)
        {
            if (input == null)
            {
                return false;
            }

            output.Key = input.Key;
            output.RegionType = input.RegionType;
            output.Text = input.Text;

            return true;
        }

        #endregion
    }
}