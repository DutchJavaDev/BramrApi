using BramrApi.Service.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using BramrApi.Data;

namespace BramrApi.Service
{
    public class SMTPMailService : ISMTP
    {
        readonly MailGenerator mailGen = new MailGenerator();
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
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
            }
        }
        public void SendPasswordChangedEmail(string email, string username)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress("bramrinfo@gmail.com")
                };
                mail.To.Add(email);
                mail.Subject = "Your Bramr password has been changed";
                mail.Body = mailGen.GeneratePasswordChangedMail(username).Result;
                CreateClient().Send(mail);
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
            }
        }
        public void SendPasswordForgottenEmail(string email, string username, string token)
        {
            try
            {
#if DEBUG
                var link = $@"https://localhost:44309/password/recovery?Token={token}";
#else
                var link = @$"https://bramr.tech/password/recovery?Token={token}";
#endif

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("bramrinfo@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Password recovery.";
                mail.Body = mailGen.GeneratePasswordRecoveryMail(username, link).Result;
                CreateClient().Send(mail);
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
            }
        }
        public void SendContactMail(string recipientEmail, string recipientName, string sendersName, string sendersEmail, string message, string service)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    IsBodyHtml = true,
                    From = new MailAddress("bramrinfo@gmail.com")
                };
                mail.To.Add(recipientEmail);
                mail.Subject = $@"{sendersName} has contacten you through Bramr.";
                mail.Body = mailGen.GenerateContactMail(recipientName, sendersName, sendersEmail, message).Result;
                CreateClient().Send(mail);
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
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
            MailMessage mail = new MailMessage
            {
                IsBodyHtml = true
            };
            mail.AlternateViews.Add(GetEmbeddedImage(password, username,res).Result);
            mail.From = new MailAddress("bramrinfo@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Registration on Bramr!";
            return mail;
        }

        

        private async Task<AlternateView> GetEmbeddedImage(string password, string username, LinkedResource res)
        {
            res.ContentId = Guid.NewGuid().ToString();
            var htmlBody = await mailGen.GenerateRegistrationMail(username,password, @"<img src='cid:" + res.ContentId + @"'/>") ;
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            
            return alternateView;
        }
    }
}
