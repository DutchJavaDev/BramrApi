﻿using BramrApi.Service.Interfaces;
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
        MailGenerator mailGen = new MailGenerator();
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
                mail.Subject = "Your Bramr password has been changed";
                mail.Body = mailGen.GeneratePasswordChangedMail(username).Result;
                CreateClient().Send(mail);
            }
            catch (Exception)
            {
               
            }
        }
        public void SendPasswordForgottenEmail(string email, string username, string token)
        {
            try
            {
#if DEBUG
                var link = $@"https://localhost:44309/wachtwoord/vergeten?Token={token}";
#else
                var link = @$"https://bramr.tech/wachtwoord/vergeten?Token={token}";
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

            }
        }
        public void SendContactMail(string recipientEmail, string recipientName, string sendersName, string sendersEmail, string message, string service)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("bramrinfo@gmail.com");
                mail.To.Add(recipientEmail);
                mail.Subject = $@"{sendersName} has contacten you through Bramr.";
                mail.Body = @$"<p>Dear, {recipientName}</p><p>Someone has contacted you through Bramr.tech his name is {sendersName}. He is sending this mail because of: {service}, his email address is:{sendersEmail}.</p> <p>His message is:</p> <i>{message}</i>";
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
