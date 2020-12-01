using System.Threading.Tasks;

namespace BramrApi.Service.Interfaces
{
    interface ICommandService
    {
        bool CreateWebsiteDirectory(string name);

        Task<bool> TestNginxConfiguration();

        void ReloadNginx();
    }
}
