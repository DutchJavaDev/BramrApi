using BramrApi.Database.Data;
using System.Threading.Tasks;

namespace BramrApi.Service.Interfaces
{
    public interface INginxCommand
    {
        bool CreateWebsiteDirectory(string name);

        void DeleteWebsiteDirectory(string name);

        UserProfile CreateUser(string username);

        Task<string> TestNginxConfiguration();

        Task<string> ReloadNginx();
    }
}
