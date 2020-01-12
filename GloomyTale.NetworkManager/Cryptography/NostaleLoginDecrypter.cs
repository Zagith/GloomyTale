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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GloomyTale.NetworkManager.Cryptography
{
    public class NostaleLoginDecrypter : IDecrypter
    {
        public string Decode(ReadOnlySpan<byte> bytesBuffer)
        {
            var decryptedPacket = new StringBuilder();
            foreach (byte character in bytesBuffer)
            {
                decryptedPacket.Append(character > 14
                    ? Convert.ToChar(character - 0xF ^ 0xC3)
                    : Convert.ToChar(0x100 - (0xF - character) ^ 195));
            }

            return decryptedPacket.ToString();
        }
    }
}