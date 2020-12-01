using System.Threading.Tasks;

namespace BramrApi.Service.Interfaces
{
    public interface IServerBlockWriterService
    {
        void CreateServerBlock(string url, string alias);

        void DeleteServerBlock(string url, string alias);

        Task<bool> BlockExists(string url);
    }
}
