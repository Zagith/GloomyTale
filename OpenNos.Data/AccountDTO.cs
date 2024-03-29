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

using OpenNos.Domain;
using OpenNos.Domain.I18N;
using System;

namespace OpenNos.Data
{
    [Serializable]
    public class AccountDTO
    {
        #region Properties

        public long AccountId { get; set; }

        public AuthorityType Authority { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string ReferrerId { get; set; }

        public string ReferToken { get; set; }

        public string RegistrationIP { get; set; }

        public string VerificationToken { get; set; }

        public RegionType Language { get; set; }

        public string TotpSecret { get; set; }

        #endregion
    }
}