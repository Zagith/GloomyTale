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

namespace OpenNos.Core.Handling
{
    public class HandlerMethodReference
    {
        #region Instantiation

        public HandlerMethodReference(Type packetBaseParameterType)
        {
            PacketDefinitionParameterType = packetBaseParameterType;
            PacketHeaderAttribute headerAttribute = (PacketHeaderAttribute)Array.Find(PacketDefinitionParameterType.GetCustomAttributes(true), ca => ca.GetType().Equals(typeof(PacketHeaderAttribute)));
            Amount = headerAttribute?.Amount ?? 1;
            Identification = headerAttribute?.Identification;
            PassNonParseablePacket = headerAttribute?.PassNonParseablePacket ?? false;
            CharacterRequired = headerAttribute?.CharacterRequired ?? true;
            Authority = headerAttribute?.Authority ?? AuthorityType.User;
        }

        #endregion

        #region Properties

        public int Amount { get; }

        public AuthorityType Authority { get; }
        public bool CharacterRequired { get; }

        /// <summary>
        /// String identification of the Packet by Header
        /// </summary>
        public string[] Identification { get; }

        public Type PacketDefinitionParameterType { get; }

        public bool PassNonParseablePacket { get; }

        #endregion
    }
}