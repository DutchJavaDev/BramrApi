using System.Threading.Tasks;

namespace BramrApi.Service.Interfaces
{
    public interface ICommandService
    {
        bool CreateWebsiteDirectory(string name);

        void DeleteWebsiteDirectory(string name);

        Task<bool> TestNginxConfiguration();

        void ReloadNginx();
    }
}
