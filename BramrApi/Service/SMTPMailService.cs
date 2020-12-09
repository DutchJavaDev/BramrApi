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
        private readonly MailAddress From = new MailAddress("bramrinfo@gmail.com");

        public async Task SendPasswordEmail(string email, string password)
        {
            //try
            //{

            //    using (var client = CreateClient())
            //    {
            //        AlternateView view = AlternateView.CreateAlternateViewFromString($"<img src=\"cid:id1\"></img><img src=\"cid:id1\"></img>", null, MediaTypeNames.Text.Html);

            //        LinkedResource resource = new LinkedResource("qrCode.jpeg", MediaTypeNames.Image.Jpeg)
            //        {   
            //            ContentId = "id1",
            //        };

            //        var message = new MailMessage(From, new MailAddress(email))
            //        {

            //            Subject = "Uw Bramr Account",
            //            IsBodyHtml = true
            //        };

            //        message.AlternateViews.Add(view);

            //        Attachment att = new Attachment(@"C:\Users\mathi\Desktop\qrCode.jpeg");
            //        att.ContentDisposition.Inline = true;

            //        message.Body = String.Format(
            //"<h3>Client: Has Sent You A Screenshot</h3>" +
            //@"<img src=""cid:{0}"" />", att.ContentId);

            //        message.IsBodyHtml = true;
            //        message.Attachments.Add(att);

            //        view.LinkedResources.Add(resource);

            //        await client.SendMailAsync(message);


            //    }


            //}
            //catch (System.Exception e)
            //{


            //}
            MailMessage mailWithImg = GetMailWithImg(email, password);
            CreateClient().Send(mailWithImg); //* Set up your SMTPClient before!

          
        }
        private SmtpClient CreateClient()
            {
                return new SmtpClient("smtp.gmail.com", 587) {
                    Credentials = new NetworkCredential("bramrinfo@gmail.com", "4*zhKqq6=Z9-#A=%"),
                    EnableSsl = true
                };
            }

        private MailMessage GetMailWithImg(string email, string password)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.AlternateViews.Add(GetEmbeddedImage(@"C:\Users\mathi\Desktop\qrCode.jpeg", password));
            mail.From = new MailAddress("bramrinfo@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Uw Bramr Account!";
            return mail;
        }

        private AlternateView GetEmbeddedImage(string filePath,string password)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = $@"<p>Bedankt voor het aanmaken van uw Bramr account. Uw wachtwoord is:{password} </p>" + @"<img style=''width: 300px; height: 300px;'' src='cid:" + res.ContentId + @"'/>";
                
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
    }
}
