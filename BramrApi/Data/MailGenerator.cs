using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace BramrApi.Data
{
    public class MailGenerator
    {
        public async Task<string> GeneratePasswordMail(string username, string password, string qrcode)
        {
            
#if DEBUG
            if (!Directory.Exists(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\"))
            {
                Directory.CreateDirectory(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\");
            }
#else
            if (!Directory.Exists(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\"))
            {
                Directory.CreateDirectory(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\");
            } 
#endif
            var Image1 = "https://picsum.photos/200/300";
            var Image2 = "https://picsum.photos/200/300";
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");

            var bindings = new Dictionary<string, object>()
            {
                {"[USERNAME]", username },
                {"[PASSWORD]", password },
                {"[QRCODE]", qrcode },
                {"[STYLE]", style },
                {"[IMAGE_SOURCE1]", Image1 },
                {"[IMAGE_SOURCE2]", Image2 }
            };

            foreach (var pair in bindings)
            {
                html = html.Replace(pair.Key, pair.Value.ToString());
            }

            return html;
        }


    }
}
