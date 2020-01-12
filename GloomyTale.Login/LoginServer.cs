﻿using GloomyTale.Core;
using GloomyTale.GameObject;
using GloomyTale.Handler;
using GloomyTale.NetworkManager;
using GloomyTale.NetworkManager.Cryptography;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GloomyTale.Login
{
    public class LoginServer : TcpServer
    {
        private readonly ISpamProtector _spamProtector;
        private readonly SessionManager _sessionManager = new SessionManager(typeof(LoginPacketHandler), false);

        public LoginServer(string address, int port, ISpamProtector spamProtector) : base(address, port)
        {
            _spamProtector = spamProtector;

        }

        protected override void OnConnected(TcpSession session)
        {
            var ip = (session.Socket.RemoteEndPoint) as IPEndPoint;
            if (!_spamProtector.CanConnect(ip.Address.ToString()))
            {
                session.Disconnect();
            }
        }

        protected override void OnDisconnected(TcpSession session)
        {
            base.OnDisconnected(session);
        }

        protected override TcpSession CreateSession()
        {
            var tmp = new LoginServerSession(this, new NostaleLoginEncrypter(Encoding.Default), new NostaleLoginDecrypter());
            _sessionManager.AddSession(tmp);
            return tmp;
        }

        protected override void OnStarted()
        {
            Logger.Log.Info("[TCP-SERVER] Started !");
        }

        protected override void OnError(SocketError error)
        {
            Logger.Log.Info("[TCP-SERVER] SocketError");
            Stop();
        }
    }
}
