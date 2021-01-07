using System.Threading.Tasks;
using System;

namespace BramrApi.Service.Interfaces
{
    public interface ISMTP
    {
        void SendPasswordEmail(string email, string password, string username);
        void SendPasswordChangedEmail(string email, string username);
        void SendPasswordForgottenEmail(string email, string username, string apiCall);
        public void SendContactMail(string recipientEmail, string recipientName, string sendersName, string sendersEmail, string message, string service);
    }
}
