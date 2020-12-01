using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BramrApi.Service.Interfaces;

namespace BramrApi.Service
{
    public class ServerBlockWriterService : IServerBlockWriterService
    {
#if DEBUG
        private readonly string CONFIG_FILE_PATH = @$"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\temp\config.txt";
        private readonly string TEMPLATE_FILE_PATH = @$"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\temp\template.txt";
#else
        private const string CONFIG_FILE_PATH = @"/etc/nginx/sites-available/bramr.tech";
        private const string TEMPLATE_FILE_PATH = @"/etc/nginx/sites-available/template";
#endif

        private readonly string Template;

        public ServerBlockWriterService()
        {
            Template = File.ReadAllText(TEMPLATE_FILE_PATH);
        }

        public async void CreateServerBlock(string url, string dir)
        {
            var lines = (await ReadConfig()).Split('\n');

            var serverBlock = CreateTemplate(url, dir);

            var mainBlock = new StringBuilder();

            foreach (var line in lines)
            {
                mainBlock.AppendLine(line.Trim());
            }

            WriteConfig(mainBlock.ToString().Replace("#next_block", serverBlock).Trim());
        }

        public async void DeleteServerBlock(string url, string alias)
        {
            var lines = (await ReadConfig()).Split('\n');

            var mainBlock = new StringBuilder();

            foreach (var line in lines)
            {
                mainBlock.AppendLine(line.Trim());
            }

            var serverBlock = FindServerBlock(alias, lines);

            WriteConfig(mainBlock.ToString().Replace(serverBlock, string.Empty).Trim());
        }

        public async Task<bool> BlockExists(string url) => (await ReadConfig()).Contains(url);

        private async void WriteConfig(string contents) => await File.WriteAllTextAsync(CONFIG_FILE_PATH, contents);

        private async Task<string> ReadConfig() => await File.ReadAllTextAsync(CONFIG_FILE_PATH);

        private string CreateTemplate(string url, string folder)
        {
            var builder = new StringBuilder();
            
            builder.AppendLine($@"#start_{folder}");

            foreach (var line in Template.Split('\n'))
            {
                builder.AppendLine(line.Trim());
            }

            builder.AppendLine($@"#end_{folder}");
            
            builder.AppendLine("#next_block\n");

            return builder.ToString().Replace("[URL]", url).Replace("[USER_FOLDER]", folder);
        }

        private string FindServerBlock(string name, string[] lines)
        {
            var start = $"#start_{name}";
            var end = $"#end_{name}";

            var contents = lines;
            var builder = new StringBuilder();
            var lineCount = contents.Length;
            var count = 0;
            var startLine = -1;
            var endLine = -1;

            while (count != lineCount)
            {
                if (startLine == -1 && contents[count].Contains(start))
                {
                    startLine = count;
                }

                if (startLine > 1 && endLine == -1 && contents[count].Contains(end))
                {
                    endLine = count;
                }

                if (startLine > 0 && endLine > 0)
                    break;

                count++;
            }

            for (var i = startLine; i <= endLine; i++)
            {
                builder.AppendLine(contents[i].Trim());
            }

            return builder.ToString();
        }
    }
}
