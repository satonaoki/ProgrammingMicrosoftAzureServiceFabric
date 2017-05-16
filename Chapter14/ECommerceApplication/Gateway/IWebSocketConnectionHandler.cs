using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    public interface IWebSocketConnectionHandler
    {
        Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, CancellationToken cancellationToken);
    }
}
