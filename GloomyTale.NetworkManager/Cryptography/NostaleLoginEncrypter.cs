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
using System.Text;

namespace GloomyTale.NetworkManager.Cryptography
{
    public class NostaleLoginEncrypter : IEncrypter
    {
        private readonly Encoding _encoding;

        public NostaleLoginEncrypter(Encoding encoding) => _encoding = encoding;

        public ReadOnlyMemory<byte> Encode(string packet)
        {
            byte[] tmp = _encoding.GetBytes(packet);
            if (tmp.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < packet.Length; i++)
            {
                tmp[i] = Convert.ToByte(tmp[i] + 15);
            }

            tmp[tmp.Length - 1] = 25;
            return tmp;
        }
    }
}