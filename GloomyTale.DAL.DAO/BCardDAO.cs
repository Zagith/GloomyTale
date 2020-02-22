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
using GloomyTale.Data.Enums;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;

namespace GloomyTale.DAL.DAO
{
    public class BCardDAO : MappingBaseDao<BCard, BCardDTO>, IBCardDAO
    {
        private readonly Dictionary<short, BCardDTO[]> _bcardsByCardId;
        private readonly Dictionary<short, BCardDTO[]> _bcardsByItemVnum;
        private readonly Dictionary<short, BCardDTO[]> _bcardsByNpcMonsterVnum;
        private readonly Dictionary<short, BCardDTO[]> _bcardsBySkillVnum;

        public BCardDAO(IMapper mapper) : base(mapper)
        {
            IEnumerable<BCardDTO> bcards = LoadAll();


            _bcardsByItemVnum = bcards.Where(s => s.ItemVNum != null).GroupBy(s => s.ItemVNum.Value).ToDictionary(s => s.Key, s => s.ToArray());
            _bcardsByCardId = bcards.Where(s => s.CardId != null).GroupBy(s => s.CardId.Value).ToDictionary(s => s.Key, s => s.ToArray());
            _bcardsByNpcMonsterVnum = bcards.Where(s => s.NpcMonsterVNum != null).GroupBy(s => s.NpcMonsterVNum.Value).ToDictionary(s => s.Key, s => s.ToArray());
            _bcardsBySkillVnum = bcards.Where(s => s.SkillVNum != null).GroupBy(s => s.SkillVNum.Value).ToDictionary(s => s.Key, s => s.ToArray());
        }

        #region Methods

        public void Insert(List<BCardDTO> cards)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (BCardDTO card in cards)
                    {
                        var entity = _mapper.Map<BCard>(card);
                        context.BCard.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public IEnumerable<BCardDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.BCard.ToArray().Select(_mapper.Map<BCardDTO>);
            }
        }

        public IEnumerable<BCardDTO> LoadByCardId(short cardId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                if (!_bcardsByCardId.TryGetValue(cardId, out BCardDTO[] bcards))
                {
                    return null;
                }

                return bcards;
            }
        }

        public IEnumerable<BCardDTO> LoadByItemVNum(short Vnum)
        {
            if (!_bcardsByItemVnum.TryGetValue(Vnum, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        public IEnumerable<BCardDTO> LoadByNpcMonsterVNum(short Vnum)
        {
            if (!_bcardsByNpcMonsterVnum.TryGetValue(Vnum, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        public IEnumerable<BCardDTO> LoadBySkillVNum(short Vnum)
        {
            if (!_bcardsBySkillVnum.TryGetValue(Vnum, out BCardDTO[] bcards))
            {
                return null;
            }

            return bcards;
        }

        #endregion
    }
}