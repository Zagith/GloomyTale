using Grpc.Core;
using GloomyTale.Core;
using System.Threading.Tasks;

namespace GloomyTale.Communication.RPC
{
    public interface ICommunicationServiceFactory
    {
        Task<ICommunicationService> CreateService(string ipAddress, int port);
    }

    public class GRpcCommunicationServiceFactory : ICommunicationServiceFactory
    {
        public async Task<ICommunicationService> CreateService(string ipAddress, int port)
        {
            var channel = new Channel(ipAddress, port, ChannelCredentials.Insecure);
            Logger.Log.Info($"[MASTER_AUTH] Connecting to {ipAddress}:{port}");
            await channel.ConnectAsync();
            Logger.Log.Info("[MASTER_AUTH] connected !");
            return new MasterCommunicator(new Master.MasterClient(channel));
        }
    }
}
