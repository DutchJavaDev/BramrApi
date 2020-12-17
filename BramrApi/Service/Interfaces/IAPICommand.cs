using BramrApi.Database.Data;
using System.Threading.Tasks;

namespace BramrApi.Service.Interfaces
{
    public interface IAPICommand
    {
        bool CreateWebsiteDirectory(string name);

        void DeleteWebsiteDirectory(string name);

        Task<string> GetIndexFor(string username);

        UserProfile CreateUser(string username);
    }
}
