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

using OpenNos.Domain;
using System;
using System.Linq;

namespace OpenNos.Core
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class PacketHeaderAttribute : Attribute
    {
        #region Instantiation

        public PacketHeaderAttribute(int amount = 1, params string[] identification)
        {
            Identification = identification.Select(t => t.ToLower()).ToArray();
            Amount = amount;
        }

        public PacketHeaderAttribute(params string[] identification) => Identification = identification.Select(t => t.ToLower()).ToArray();

        #endregion

        #region Properties

        /// <summary>
        /// Amount of required packets
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// Permission to handle the packet
        /// </summary>
        public AuthorityType Authority { get; set; }

        /// <summary>
        /// String identification of the Packet
        /// </summary>
        public string[] Identification { get; set; }

        /// <summary>
        /// Specifies if packet needs character to execute handling
        /// </summary>
        public bool CharacterRequired { get; set; }

        /// <summary>
        /// Pass the packet to handler method even if the serialization has failed.
        /// </summary>
        public bool PassNonParseablePacket { get; set; }

        #endregion
    }
}