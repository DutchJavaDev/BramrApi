using System.Threading.Tasks;
using System;

namespace BramrApi.Service.Interfaces
{
    public interface ISMTP
    {
        void SendPasswordEmail(string email, string password, string username);
    }
}
