using BramrApi.Database.Data;
using System.Threading.Tasks;

namespace BramrApi.Service.Interfaces
{
    public interface ICommandService
    {
        bool CreateWebsiteDirectory(string name);

        void DeleteWebsiteDirectory(string name);

        UserProfile CreateUser(string username);

        Task<bool> TestNginxConfiguration();

        void ReloadNginx();
    }
}
