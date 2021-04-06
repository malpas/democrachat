using System.Net;

namespace Democrachat.Kudo
{
    public interface IKudoService
    {
        void GiveKudo(int fromUserId, string toUsername, IPAddress? fromIp);
    }
}