namespace GloomyTale.Communication.RPC
{
    public interface ICommunicationClientFactory
    {
        ICommunicationClient CreateClient(string ip, int port);
    }
}
