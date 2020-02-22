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
    public class RollGeneratedItemDAO : MappingBaseDao<RollGeneratedItem, RollGeneratedItemDTO>, IRollGeneratedItemDAO
    {
        public RollGeneratedItemDAO(IMapper mapper) : base(mapper)
        {
            IEnumerable<RollGeneratedItemDTO> bcards = LoadAll();


            _rollItems = bcards.GroupBy(s => s.OriginalItemVNum).ToDictionary(s => s.Key, s => s.ToArray());
        }

        #region Methods

        private readonly Dictionary<short, RollGeneratedItemDTO[]> _rollItems;

        public IEnumerable<RollGeneratedItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.RollGeneratedItem.ToArray().Select(_mapper.Map<RollGeneratedItemDTO>);
            }
        }

        public IEnumerable<RollGeneratedItemDTO> LoadByItemVNum(short vnum)
        {
            if (!_rollItems.TryGetValue(vnum, out RollGeneratedItemDTO[] rollItems))
            {
                return null;
            }

            return rollItems.Select(_mapper.Map<RollGeneratedItemDTO>);
        }

        #endregion
    }
}