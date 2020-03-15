using GloomyTale.Core;
using GloomyTale.GameObject;
using GloomyTale.NetworkManager.Cryptography;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.World
{
    public class NetworkInformations : INetworkClientInformation
    {
        public int SessionId { get; set; }
        public Encoding Encoding => Encoding.Default;
    }

    public class WorldServerSession : TcpSession, INetworkSession
    {
        private readonly IDecrypter _decrypter;
        private readonly IEncrypter _encrypter;
        private readonly NetworkInformations _networkClient;
        private IPEndPoint _ip;
        private SessionManager _session;
        public WorldServerSession(TcpServer server, IEncrypter encrypter, IDecrypter decrypter, NetworkInformations networkClient, SessionManager session) : base(server)
        {
            _encrypter = encrypter;
            _decrypter = decrypter;
            _networkClient = networkClient;
            _session = session;
        }

        public Encoding Encoding => Encoding.Default;

        public event EventHandler<string> PacketReceived;
        public long ClientId { get; }
        public IPAddress IpAddress => _ip.Address;

        public async Task SendPacketAsync(string packet)
        {
            await Task.Run(() => SendAsync(_encrypter.Encode(packet).ToArray()));
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            SendPacket(string.Format(packet, param));
        }

        public async Task SendPacketAsyncFormat(string packet, params object[] param)
        {
            await SendPacketAsync(string.Format(packet, param));
        }

        public void SendPackets(IEnumerable<string> packets)
        {
            foreach (var packet in packets)
            {
                SendPacket(packet);
            }
        }

        public async Task SendPacketsAsync(IEnumerable<string> packets)
        {
            foreach (var packet in packets)
            {
                await SendPacketAsync(packet);
            }
        }

        public void DisconnectClient()
        {
            Disconnect();
        }

        public bool IsDisposing { get; set; }
        public void SendPacket(string packet) => Send(_encrypter.Encode(packet).ToArray());

        public int SessionId
        {
            get => _networkClient.SessionId;
            set => _networkClient.SessionId = value;
        }

        protected override void OnConnected()
        {
            _ip = Socket.RemoteEndPoint as IPEndPoint;
        }

        protected override void OnDisconnected()
        {
            _session.RemoveSession(this);
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string buff = _decrypter.Decode(buffer.AsSpan((int)offset, (int)size));
            PacketReceived?.Invoke(this, buff);
        }

        protected override void OnError(SocketError error)
        {
            Disconnect();
        }
    }
}
