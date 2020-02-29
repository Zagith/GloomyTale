using GloomyTale.Core;
using GloomyTale.GameObject;
using GloomyTale.GameObject.Networking;
using GloomyTale.Handler;
using GloomyTale.NetworkManager.Cryptography;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GloomyTale.World
{
    public class WorldServer : TcpServer
    {
        private readonly SessionManager _sessionManager = new SessionManager(typeof(BasicPacketHandler), true);

        public WorldServer(IPAddress address, int port) : base(address, port)
        {
        }

        protected override TcpSession CreateSession()
        {
            var infos = new NetworkInformations();
            var tmp = new WorldServerSession(this, new WorldEncrypter(infos), new WorldDecrypter(infos), infos, _sessionManager);
            _sessionManager.AddSession(tmp);
            return tmp;
        }

        protected override void OnConnected(TcpSession session)
        {

            Logger.Log.Info($"Connected : {(session.Socket.RemoteEndPoint as IPEndPoint).Address}");
        }

        protected override void OnStarted()
        {
            Logger.Log.Info("[TCP-SERVER] Started");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"TCP server caught an error with code {error}");
            ServerManager.Instance.Shutdown();
            Stop();
        }

        protected override void OnDisconnected(TcpSession session)
        {
        }
    }
}
