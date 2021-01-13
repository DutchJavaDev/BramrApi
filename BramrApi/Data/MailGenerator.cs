using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace BramrApi.Data
{
    public class MailGenerator
    {
        public async Task<string> GenerateRegistrationMail(string username, string password, string qrcode)
        {

            var Image1 = "https://i.ibb.co/pngDhMF/logo-size-BRAMR-mail.png";
            var Image2 = "https://i.ibb.co/x3wSP73/Homepage-achtergrond-mail.png";
#if DEBUG
            if (!Directory.Exists(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\"))
            {
                Directory.CreateDirectory(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\");
            }
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_Registration.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#else
            if (!Directory.Exists(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\"))
            {
                Directory.CreateDirectory(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\");
            } 
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\MailTemplate_Registration.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\MailTemplate.css");
#endif

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
        public async Task<string> GeneratePasswordRecoveryMail(string username, string link) 
        {
            var Image1 = "https://i.ibb.co/pngDhMF/logo-size-BRAMR-mail.png";
            var Image2 = "https://i.ibb.co/x3wSP73/Homepage-achtergrond-mail.png";
#if DEBUG
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_PasswordRecovery.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#else
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_PasswordRecovery.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#endif
            var bindings = new Dictionary<string, object>()
            {
                {"[USERNAME]", username },
                {"[LINK]", link },
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
        public async Task<string> GeneratePasswordChangedMail(string username)
        {
            var Image1 = "https://i.ibb.co/pngDhMF/logo-size-BRAMR-mail.png";
            var Image2 = "https://i.ibb.co/x3wSP73/Homepage-achtergrond-mail.png";
#if DEBUG
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_PasswordChanged.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#else
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_PasswordChanged.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#endif
            var bindings = new Dictionary<string, object>()
            {
                {"[USERNAME]", username },
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
        public async Task<string> GenerateContactMail(string username,string sendersName,string sendersEmail, string message)
        {
            var Image1 = "https://i.ibb.co/pngDhMF/logo-size-BRAMR-mail.png";
            var Image2 = "https://i.ibb.co/x3wSP73/Homepage-achtergrond-mail.png";
#if DEBUG
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_Contact.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#else
            var html = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate_Contact.html");
            var style = await File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\MailTemplate.css");
#endif
            var bindings = new Dictionary<string, object>()
            {
                {"[USERNAME]", username },
                {"[STYLE]", style },
                {"[IMAGE_SOURCE1]", Image1 },
                {"[IMAGE_SOURCE2]", Image2 },
                {"[MESSAGE]", message },
                {"[SENDERS_NAME]", sendersName },
                {"[SENDERS_EMAIL]", sendersEmail }
            };

            foreach (var pair in bindings)
            {
                html = html.Replace(pair.Key, pair.Value.ToString());
            }
            return html;

        }

    }
}
