using BramrApi.Service.Interfaces;
using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Threading.Tasks;
using BramrApi.Data;
using BramrApi.Utils;
using MimeKit.Utils;

namespace BramrApi.Service
{
    public class SMTPMailService : ISMTP
    {
        private readonly MailGenerator mailGen = new MailGenerator();
        private readonly MailboxAddress BRAMR_EMAIL = new MailboxAddress("Bramr hosting", "bramrinfo@gmail.com");

        public async void SendPasswordEmail(string email, string password, string username)
        {
#if DEBUG
            var message = await GetMailWithImg(email, password, username, $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\{username}.jpeg");
#else
            var message = await GetMailWithImg(email, password, username, Utility.CreatePathFromBegin(@$"usr/share/temp/{username}.jpeg"));
#endif
            SendEmail(message);
        }

        public async void SendPasswordChangedEmail(string email, string username)
        {
            var message = new MimeMessage();

            message.From.Add(BRAMR_EMAIL);

            message.To.Add(new MailboxAddress(username, email));

            message.Subject = "Your Bramr password has been changed";

            var builder = new BodyBuilder
            {
                HtmlBody = await mailGen.GeneratePasswordChangedMail(username)
            };

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public async void SendPasswordForgottenEmail(string email, string username, string token)
        {
#if DEBUG
            var link = $@"https://localhost:44309/password/recovery?Token={token}";
#else
            var link = @$"https://bramr.tech/password/recovery?Token={token}";
#endif

            var message = new MimeMessage();

            message.From.Add(BRAMR_EMAIL);

            message.To.Add(new MailboxAddress(username, email));

            message.Subject = "Password recovery";

            var builder = new BodyBuilder
            {
                HtmlBody = await mailGen.GeneratePasswordRecoveryMail(username, link)
            };

            message.Body = builder.ToMessageBody();

            SendEmail(message);
        }

        public async void SendContactMail(string recipientEmail, string recipientName, string sendersName, string sendersEmail, string message, string service)
        {
            var mail = new MimeMessage();

            mail.From.Add(BRAMR_EMAIL);

            mail.To.Add(new MailboxAddress(recipientName, recipientEmail));

            mail.Subject = $@"{sendersName} has contacten you through Bramr.";

            var builder = new BodyBuilder
            {
                HtmlBody = await mailGen.GenerateContactMail(recipientName, sendersName, sendersEmail, message)
            };

            mail.Body = builder.ToMessageBody();
        }

        private async void SendEmail(MimeMessage email)
        {
            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync("bramrinfo@gmail.com", "4*zhKqq6=Z9-#A=%");
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                Sentry.SentrySdk.CaptureException(e);
            }
        }

        private async Task<MimeMessage> GetMailWithImg(string email, string password, string username, string res)
        {
            var message = new MimeMessage();

            message.From.Add(BRAMR_EMAIL);
            
            message.To.Add(new MailboxAddress(username, email));
            
            message.Subject = "Registration on Bramr!";

            message.Body = await GetEmbeddedImage(password, username, res);

            return message;
        }

        private async Task<MimeEntity> GetEmbeddedImage(string password, string username, string res)
        {
            var builder = new BodyBuilder();

            var embeddedImage = builder.LinkedResources.Add(res);

            embeddedImage.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = await mailGen.GenerateRegistrationMail(username, password, $@"<center><img src=""cid:{embeddedImage.ContentId}""></center>");

            return builder.ToMessageBody();
        }
    }
}
