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

using GloomyTale.Core;
using GloomyTale.Domain;
using GloomyTale.GameObject.Helpers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using GloomyTale.GameObject.Networking;
using GloomyTale.GameObject.Event;

namespace GloomyTale.GameObject
{
    public class SessionManager
    {
        #region Members

        protected ConcurrentDictionary<long, ClientSession> _sessions = new ConcurrentDictionary<long, ClientSession>();

        #endregion

        #region Instantiation

        public SessionManager(Type packetHandler, bool isWorldServer)
        {
            PacketHandler = packetHandler;
            IsWorldServer = isWorldServer;
        }

        #endregion

        #region Properties

        protected Type PacketHandler { get; }

        public bool IsWorldServer { get; set; }

        #endregion

        #region Methods

        public void AddSession(INetworkSession customClient)
        {
            Logger.Log.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + customClient.ClientId);

            ClientSession session = new ClientSession(customClient);
            session.Initialize(PacketHandler, IsWorldServer);

            if (!IsWorldServer)
            {
                return;
            }

            if (_sessions.TryAdd(customClient.ClientId, session))
            {
                return;
            }

            Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("FORCED_DISCONNECT"), customClient.ClientId));
            customClient.DisconnectClient();
            _sessions.TryRemove(customClient.ClientId, out session);
        }

        public virtual void StopServer()
        {
            _sessions.Clear();
            ServerManager.StopServer();
        }

        /*protected virtual ClientSession IntializeNewSession(INetworkClient client)
        {
            ClientSession session = new ClientSession(client);
            client.SetClientSession(session);
            return session;
        }*/

        protected void RemoveSession(INetworkSession client)
        {
            _sessions.TryRemove(client.ClientId, out ClientSession session);

            // check if session hasnt been already removed
            if (session != null)
            {
                session.IsDisposing = true;

                session.Destroy();

                if (IsWorldServer && session.HasSelectedCharacter)
                {
                    session.Character.Save();
                }

                client.DisconnectClient();
                Logger.Log.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + client.ClientId);

                // session = null;
            }
        }

        #endregion
    }
}