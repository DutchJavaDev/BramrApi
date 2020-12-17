using BramrApi.Service.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BramrApi.Service
{
    public class SMTPMailService : ISMTP
    {
        public void SendPasswordEmail(string email, string password, string username)
        {
            try
            {
#if DEBUG
                using (LinkedResource res = new LinkedResource($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\{username}.jpeg", new ContentType("image/jpeg")))
#else
                using (LinkedResource res = new LinkedResource($@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\{username}.jpeg", new ContentType("image/jpeg")))
#endif
                {
                    MailMessage mailWithImg = GetMailWithImg(email, password, username, res);
                    CreateClient().Send(mailWithImg);
                    res.Dispose();
                }
                
            }
            catch (Exception)
            {
               
            }
        }
        public void SendPasswordChangedEmail(string email, string username)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("bramrinfo@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Uw Bramr wachtwoord is gewijzigd.";
                mail.Body = @$"<p>Beste {username}, </p> <p>Uw wachtwoord op bramr.tech is gewijzigd. Heeft u dit <b>niet</b> zelf gedaan? neem dan contact met ons op via bramrinfo@gmail.com</p><p></p><p>Met vriendelijke groet, team Bramr</p>";
                CreateClient().Send(mail);
            }
            catch (Exception)
            {
               
            }
        }

        private SmtpClient CreateClient()
            {
                return new SmtpClient("smtp.gmail.com", 587) {
                    Credentials = new NetworkCredential("bramrinfo@gmail.com", "4*zhKqq6=Z9-#A=%"),
                    EnableSsl = true
                };
            }

        private MailMessage GetMailWithImg(string email, string password,string username, LinkedResource res)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.AlternateViews.Add(GetEmbeddedImage(password, username,res));
            mail.From = new MailAddress("bramrinfo@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Uw Bramr Account!";
            return mail;
        }

        private AlternateView GetEmbeddedImage( string password, string username, LinkedResource res)// string filePath
        {
            //LinkedResource res = new LinkedResource(filePath, new ContentType("image/jpeg"));
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = $@"<p>Beste {username}, </p><p>Bedankt voor het aanmaken van uw Bramr account. Uw wachtwoord is: <b>{password}</b> </p>" + @"<img style=''width: 300px; height: 300px;'' src='cid:" + res.ContentId + @"'/>";
                
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            
            return alternateView;
        }
    }
}
