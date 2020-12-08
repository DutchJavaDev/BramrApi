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
        private readonly string WEBSITES_DIRECTORY = @"\var\www\";
#endif

        private readonly string INDEX_DIRECTORY = "html";
        private readonly string IMAGES_DIRECTORY = "images";

        public CommandService()
        {
            if (!Directory.Exists(WEBSITES_DIRECTORY))
                Directory.CreateDirectory(WEBSITES_DIRECTORY);
        }

        public bool CreateWebsiteDirectory(string dir)
        {
            if (Directory.Exists(Path.Combine(WEBSITES_DIRECTORY, dir))) return false;

            try
            {
                var directory = Directory.CreateDirectory(Path.Combine(WEBSITES_DIRECTORY, dir));
                
                var indexDirectory = directory.CreateSubdirectory(INDEX_DIRECTORY);
                var imageDirectory = indexDirectory.CreateSubdirectory(IMAGES_DIRECTORY);

                return directory.Exists && indexDirectory.Exists && imageDirectory.Exists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DeleteWebsiteDirectory(string name)
        {
            var directory = new DirectoryInfo(Path.Combine(WEBSITES_DIRECTORY,name));

            // Delete files
            foreach (var file in directory.GetFiles())
            {
                File.Delete(file.FullName);
            }

            // Delete directory + files
            // TODO make this recursive instead!
            foreach (var dir in directory.GetDirectories())
            {
                foreach (var file in dir.GetFiles())
                {
                    File.Delete(file.FullName);
                }

                Directory.Delete(dir.FullName);
            }

            Directory.Delete(Path.Combine(WEBSITES_DIRECTORY, name));
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
