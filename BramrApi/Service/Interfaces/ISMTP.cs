using System.Threading.Tasks;
using System;

namespace BramrApi.Service.Interfaces
{
    public interface ISMTP
    {
        Task SendPasswordEmail(string email, string password);
    }
}
