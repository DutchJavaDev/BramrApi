using System;
using System.Text;
using BramrApi.Service.Interfaces;

namespace BramrApi.Service
{
    public class ServerBlockWriterService : IServerBlockWriterService
    {
#if DEBUG
        private string CONFIG_FILE_PATH = @$"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}";
#else
        private const string CONFIG_FILE_PATH = @"/etc/nginx/sites-available/bramr.tech";
#endif
        public string LoadFile(string fileName)
        {
            return string.Empty;
        }

        public string CreateServerBlock(string url, string alias)
        {
            return string.Empty;
        }

        public string FindServerBlock(string name, string[] lines, StringBuilder trimmedFile)
        {
            return string.Empty;
        }
    }
}
