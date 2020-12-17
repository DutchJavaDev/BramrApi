using System;
using System.IO;
using System.Threading.Tasks;
using BramrApi.Database.Data;
using BramrApi.Service.Interfaces;

namespace BramrApi.Service
{
    public class APIService : IAPICommand
    {
#if DEBUG
        private readonly string WEBSITES_DIRECTORY = @$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\websites\";
#else
        private readonly string WEBSITES_DIRECTORY = @"\var\www\";
#endif

        private readonly string DEFAULT_INDEX_PATH = @"Static\default.html";
        private readonly string INDEX_FILE_NAME = "index.html";
        private readonly string INDEX_DIRECTORY = "html";
        private readonly string IMAGES_DIRECTORY = "images";

        private readonly string DEFAULT_INDEX_CONTENT;
        private readonly string PLACE_HOLDER_MESSAGE = "[MESSAGE]";

        public APIService()
        {
            if (!Directory.Exists(WEBSITES_DIRECTORY))
                Directory.CreateDirectory(WEBSITES_DIRECTORY);

            DEFAULT_INDEX_CONTENT = File.ReadAllText(DEFAULT_INDEX_PATH);
        }

        public UserProfile CreateUser(string username)
        {
            if (!CreateWebsiteDirectory(username))
            {
                return null;
            }

            var websiteDir = Path.Combine(WEBSITES_DIRECTORY, username);
            var indexDir = Path.Combine(websiteDir, INDEX_DIRECTORY);
            var imageDir = Path.Combine(indexDir, IMAGES_DIRECTORY);

            return new UserProfile {
                UserName = username,
                WebsiteDirectory = indexDir,
                ImageDirectory = imageDir
            };
        }

        public async Task<string> GetIndexFor(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, "Woops 404, deze pagina is niet beschikbaar.");
            }

            var websiteDir = Path.Combine(WEBSITES_DIRECTORY, username);

            if (Directory.Exists(websiteDir))
            {
                var indexDir = Path.Combine(websiteDir, INDEX_DIRECTORY);

                return await File.ReadAllTextAsync(Path.Combine(indexDir, INDEX_FILE_NAME));
            }
            else
                return DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, $"'{username}' kon niet worden gevonden, probeer het later nog eens.");
        }

        public bool CreateWebsiteDirectory(string dir)
        {
            if (Directory.Exists(Path.Combine(WEBSITES_DIRECTORY, dir))) return false;

            try
            {
                var directory = Directory.CreateDirectory(Path.Combine(WEBSITES_DIRECTORY, dir));
                
                var indexDirectory = directory.CreateSubdirectory(INDEX_DIRECTORY);

                var path = Path.Combine(Path.Combine(WEBSITES_DIRECTORY, dir), INDEX_DIRECTORY);

                File.WriteAllText(Path.Combine(path, INDEX_FILE_NAME),DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, "Wordt aan gewerkt, probeer het later nog eens."));

                var imageDirectory = indexDirectory.CreateSubdirectory(IMAGES_DIRECTORY);

                return directory.Exists && indexDirectory.Exists && imageDirectory.Exists;
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
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

    }
}
