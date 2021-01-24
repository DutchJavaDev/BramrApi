using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BramrApi.Data;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IDatabase Database;
        private readonly UserManager<IdentityUser> UserManager;

        public AccountController(UserManager<IdentityUser> userManager, IDatabase database)
        {
            UserManager = userManager;
            Database = database;
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<List<string>> GetUserInfo()
        {
            List<string> UserInfo = new List<string>();

            var user = await UserManager.FindByIdAsync(GetIdentity());
            if(user != null)
            {
                try 
                {
                    var profile = Database.GetModelByUserName(user.UserName);
                    UserInfo.Add(user.UserName);
                    UserInfo.Add($"Joined Bramr on: {profile.CreationDate}");
#if DEBUG
                    UserInfo.Add($"https://localhost:44372/cv/{user.UserName}");
                    UserInfo.Add($"https://localhost:44372/portfolio/{user.UserName}");
#else
                    UserInfo.Add($"https://www.bramr.tech/cv/{user.UserName}");
                    UserInfo.Add($"https://www.bramr.tech/portfolio/{user.UserName}");
#endif
                    UserInfo.Add(user.Email);
                    UserInfo.Add(profile.HasCv.ToString());
                    UserInfo.Add(profile.HasPortfolio.ToString());

                    return UserInfo;
                }
                catch 
                {
                    // why clear it ?
                    UserInfo.Clear();
                    return UserInfo; 
                }
            }
            return UserInfo;
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
