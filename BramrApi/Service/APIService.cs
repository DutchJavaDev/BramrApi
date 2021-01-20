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

        //Alle standaard namen voor de directories van de gebruiker
        private readonly string DEFAULT_INDEX_PATH = @"Static\default.html";
        private readonly string INDEX_FILE_NAME = "index.html";
        private readonly string CV_DIRECTORY = "cv";
        private readonly string PORTFOLIO_DIRECTORY = "portfolio";
        private readonly string INDEX_DIRECTORY = "html";
        private readonly string IMAGES_DIRECTORY = "images";

        private readonly string DEFAULT_INDEX_CONTENT;
        private readonly string PLACE_HOLDER_MESSAGE = "[MESSAGE]";
        
        public APIService()
        {
#if !DEBUG
            Directory.SetCurrentDirectory(@"\");
#endif
            if (!Directory.Exists(WEBSITES_DIRECTORY))
            {
                Directory.CreateDirectory(WEBSITES_DIRECTORY);
            }

            DEFAULT_INDEX_CONTENT = File.ReadAllText(DEFAULT_INDEX_PATH);
        }

        /// <summary>
        /// Het aanmaken van een userprofile om deze uiteindelijk op te slaan in de database.
        /// </summary>
        /// <param name="username">Username van de gebruiker</param>
        /// <returns>Stuurt terug het userprofile dat net is aangemaakt</returns>
        public UserProfile CreateUser(string username)
        {
            if (!CreateWebsiteDirectory(username))
            {
                return null;
            }

            //Creëert alle paths voor de directories van een user om deze om deze uiteindelijk op te slaan in de properties van het userprofile
            var websiteDir = Path.Combine(WEBSITES_DIRECTORY, username);
            var cvDir = Path.Combine(websiteDir, CV_DIRECTORY);
            var portfolioDir = Path.Combine(websiteDir, PORTFOLIO_DIRECTORY);
            var indexCvDir = Path.Combine(cvDir, INDEX_DIRECTORY);
            var indexPortfolioDir = Path.Combine(portfolioDir, INDEX_DIRECTORY);
            var imageCvDir = Path.Combine(indexCvDir, IMAGES_DIRECTORY);
            var imagePortfolioDir = Path.Combine(indexPortfolioDir, IMAGES_DIRECTORY);

            return new UserProfile {
                UserName = username,
                WebsiteDirectory = websiteDir,
                CvDirectory = cvDir,
                PortfolioDirectory = portfolioDir,
                IndexCvDirectory = indexCvDir,
                IndexPortfolioDirectory = indexPortfolioDir,
                ImageCvDirectory = imageCvDir,
                ImagePortfolioDirectory = imagePortfolioDir
            };
        }

        /// <summary>
        /// Deze method zorgt voor het ophalen van het html bestand voor het cv of portfolio, om deze live te zetten.
        /// </summary>
        /// <param name="username">Username van de gebruiker</param>
        /// <param name="IsCv">Boolean om te checken of de method het cv of het portfolio terug moet sturen</param>
        /// <returns>Method stuurt alles wat in het opgehaalde html bestand staat als een string.</returns>
        public async Task<string> GetIndexFor(string username, bool IsCv)
        {
            if (string.IsNullOrEmpty(username))
            {
                return DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, "Woops 404, deze pagina is niet beschikbaar.");
            }

            var websiteDir = Path.Combine(WEBSITES_DIRECTORY, username);
            var cvDir = Path.Combine(websiteDir, CV_DIRECTORY);
            var portfolioDir = Path.Combine(websiteDir, PORTFOLIO_DIRECTORY);

            if (Directory.Exists(websiteDir))
            {
                if (IsCv)
                {
                    var indexDir = Path.Combine(cvDir, INDEX_DIRECTORY);
                    return await File.ReadAllTextAsync(Path.Combine(indexDir, INDEX_FILE_NAME));
                }
                else
                {
                    var indexDir = Path.Combine(portfolioDir, INDEX_DIRECTORY);
                    return await File.ReadAllTextAsync(Path.Combine(indexDir, INDEX_FILE_NAME));
                }
            }
            else
                return DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, $"'{username}' kon niet worden gevonden, probeer het later nog eens.");
        }

        /// <summary>
        /// Creëert de volledige directory per gebruiker. 
        /// </summary>
        /// <param name="dir">De username van de gebruiker zodat deze gekoppeld wordt aan de map</param>
        /// <returns>Als alles goed verloopt stuurt de functie terug true en als de directory al bestaat of er iets fout gaat stuurt de functie terug false. Zodat de functie waar deze in wordt aangeroepen weet hoe het process is verlopen.</returns>
        public bool CreateWebsiteDirectory(string dir)
        {
            if (Directory.Exists(Path.Combine(WEBSITES_DIRECTORY, dir))) return false;

            try
            {
                var directory = Directory.CreateDirectory(Path.Combine(WEBSITES_DIRECTORY, dir));

                var cvDirectory = directory.CreateSubdirectory(CV_DIRECTORY);
                var portfolioDirectory = directory.CreateSubdirectory(PORTFOLIO_DIRECTORY);

                var indexCvDirectory = cvDirectory.CreateSubdirectory(INDEX_DIRECTORY);
                var indexPortfolioDirectory = portfolioDirectory.CreateSubdirectory(INDEX_DIRECTORY);

                var cvPath = Path.Combine(Path.Combine(Path.Combine(WEBSITES_DIRECTORY, dir), CV_DIRECTORY), INDEX_DIRECTORY);
                var portfolioPath = Path.Combine(Path.Combine(Path.Combine(WEBSITES_DIRECTORY, dir), PORTFOLIO_DIRECTORY), INDEX_DIRECTORY);

                File.WriteAllText(Path.Combine(cvPath, INDEX_FILE_NAME), DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, "Wordt aan gewerkt, probeer het later nog eens."));
                File.WriteAllText(Path.Combine(portfolioPath, INDEX_FILE_NAME), DEFAULT_INDEX_CONTENT.Replace(PLACE_HOLDER_MESSAGE, "Wordt aan gewerkt, probeer het later nog eens."));

                var imageCvDirectory = indexCvDirectory.CreateSubdirectory(IMAGES_DIRECTORY);
                var imagePortfolioDirectory = indexPortfolioDirectory.CreateSubdirectory(IMAGES_DIRECTORY);

                return directory.Exists && indexCvDirectory.Exists && indexPortfolioDirectory.Exists && imageCvDirectory.Exists && imagePortfolioDirectory.Exists;
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
                return false;
            }
        }
        
        /// <summary>
        /// Het verwijderen van de directory van een user, als het account bijvoorbeeld wordt verwijderd.
        /// </summary>
        /// <param name="name">Username van de gebruiker</param>
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
