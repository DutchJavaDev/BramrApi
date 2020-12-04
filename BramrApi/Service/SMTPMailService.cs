using BramrApi.Service.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BramrApi.Service
{
    public class SMTPMailService : ISMTP
    {
        private readonly MailAddress From = new MailAddress("bramrinfo@gmail.com");

        public async Task SendPasswordEmail(string email, string password)
        {
            using (var client = CreateClient())
            await client.SendMailAsync(new MailMessage(From, new MailAddress(email))
            {
                Subject = "Wachtwoord",
                Body = $"Beste heer/mevrouw,\n Uw wachtwoord is {password}"
            }); ;
        }

        private SmtpClient CreateClient()
        {
            return new SmtpClient("smtp.gmail.com", 587) {
                Credentials = new NetworkCredential("bramrinfo@gmail.com", "4*zhKqq6=Z9-#A=%"),
                EnableSsl = true
            };
        }
    }
}
