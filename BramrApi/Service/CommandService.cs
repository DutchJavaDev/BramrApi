﻿using System;
using System.IO;
using System.Threading.Tasks;
using BramrApi.Service.Interfaces;

namespace BramrApi.Service
{
    public class CommandService : ICommandService
    {
#if DEBUG
        private readonly string WEBSITES_DIRECTORY = @$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\websites\";
#else
        private readonly string WEBSITES_DIRECTORY = @$"\var\www\";
#endif

        private readonly string INDEX_DIRECTORY = "html";

        public CommandService()
        {
            if (!Directory.Exists(WEBSITES_DIRECTORY))
                Directory.CreateDirectory(WEBSITES_DIRECTORY);
        }

        public bool CreateWebsiteDirectory(string name)
        {
            if (Directory.Exists(Path.Combine(WEBSITES_DIRECTORY, name))) return false;

            try
            {
                var directory = Directory.CreateDirectory(Path.Combine(WEBSITES_DIRECTORY, name));
                
                var indexDirectory = directory.CreateSubdirectory(INDEX_DIRECTORY);

                return directory.Exists && indexDirectory.Exists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async void ReloadNginx()
        {
            await ExecuteCommand();
        }

        public async Task<bool> TestNginxConfiguration()
        {
            await ExecuteCommand();
            return false;
        }

        private async Task ExecuteCommand() 
        {
            await Task.Delay(1);
        }
    }
}
