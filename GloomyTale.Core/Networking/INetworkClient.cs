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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenNos.Core
{
    public interface INetworkClient
    {
        /// <summary>
        /// </summary>
        long ClientId { get; }

        // todo urgent rework
        int SessionId { get; set; }

        /// <summary>
        /// </summary>
        string IpAddress { get; }

        bool IsConnected { get; }

        bool IsDisposing { get; set; }
        event EventHandler<string> PacketReceived;

        Task ClearLowPriorityQueueAsync();

        void SendPacket(string packet);
        Task SendPacketAsync(string packet);
        void SendPacketFormat(string packet, params object[] param);
        Task SendPacketAsyncFormat(string packet, params object[] param);
        void SendPackets(IEnumerable<string> packets);
        Task SendPacketsAsync(IEnumerable<string> packets);

        void DisconnectClient();
    }
}