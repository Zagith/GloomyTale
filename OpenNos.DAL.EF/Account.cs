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

using OpenNos.DAL.EF.Entities;
using OpenNos.Domain;
using OpenNos.Domain.I18N;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class Account
    {
        #region Instantiation

        public Account()
        {
            Character = new HashSet<Character>();
            GeneralLog = new HashSet<GeneralLog>();
            PenaltyLog = new HashSet<PenaltyLog>();
            MultiAccountException = new HashSet<MultiAccountException>();
        }

        #endregion

        #region Properties

        public long AccountId { get; set; }

        public AuthorityType Authority { get; set; }

        public virtual ICollection<Character> Character { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        public virtual ICollection<GeneralLog> GeneralLog { get; set; }

        public ICollection<MultiAccountException> MultiAccountException { get; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        public virtual ICollection<PenaltyLog> PenaltyLog { get; set; }

        public string ReferrerId { get; set; }

        public string ReferToken { get; set; }

        [MaxLength(45)]
        public string RegistrationIP { get; set; }

        [MaxLength(32)]
        public string VerificationToken { get; set; }

        public bool DailyRewardSent { get; set; }

        public RegionType Language { get; set; }

        [MaxLength(255)]
        public string AdminName { get; set; }

        [MaxLength(32)]
        public string TotpSecret { get; set; }
        #endregion
    }
}