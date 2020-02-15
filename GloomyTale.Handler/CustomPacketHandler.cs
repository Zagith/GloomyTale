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
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.GameObject;
using GloomyTale.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GloomyTale.Handler
{
    public class CustomPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CustomPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        // Your custom packet code written here. Put Packet Definitions in OpenNos.GameObject/Packets/CustomPackets

        #endregion
    }
}