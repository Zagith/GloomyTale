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

using Microsoft.EntityFrameworkCore;
using GloomyTale.Core;
using System;

namespace GloomyTale.DAL.EF.Helpers
{
    public interface IOpenNosContextFactory
    {
        OpenNosContext CreateContext();
    }

    public static class DataAccessHelper
    {
        #region Members

        private static IOpenNosContextFactory _contextFactory;

        #endregion

        #region Methods

        /// <summary>
        /// Creates new instance of database context.
        /// </summary>
        public static OpenNosContext CreateContext() => _contextFactory.CreateContext();

        public static bool Initialize(IOpenNosContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            using (OpenNosContext context = CreateContext())
            {
                try
                {
                    context.Database.GetDbConnection().Open();
                    Logger.Log.Info(Language.Instance.GetMessageFromKey("DATABASE_INITIALIZED"));
                }
                catch (Exception ex)
                {
                    Logger.Log.LogEventError("DATABASE_INITIALIZATION", "Database Error", ex);
                    Logger.Log.LogEventError("DATABASE_INITIALIZATION", Language.Instance.GetMessageFromKey("DATABASE_NOT_UPTODATE"));
                    return false;
                }
                return true;
            }
        }

        #endregion
    }
}