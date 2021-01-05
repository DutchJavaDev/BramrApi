using BramrApi.Data;
using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IDatabase Database;
        private readonly UserManager<IdentityUser> UserManager;
        private readonly SignInManager<IdentityUser> SignInManager;
        private readonly ISMTP MailClient;

        public PasswordController(UserManager<IdentityUser> UserManager, SignInManager<IdentityUser> SignInManager, IDatabase Database, ISMTP mailClient)
        {
            this.UserManager = UserManager;
            this.SignInManager = SignInManager;
            this.Database = Database;
            MailClient = mailClient;
        }

        /// <summary>
        /// Method waarmee het huidige wachtwoord wordt aangepast met het nieuwe wachtwoord
        /// </summary>
        /// <param name="json">Json string die meegestuurd wordt vanaf BramrSite</param>
        /// <returns>Stuurt een ApiResponse terug om aan de client kant te weten hoe het aan de api kant verlopen is</returns>
        [HttpPost("change")]
        [Authorize]
        public async Task<ApiResponse> ChangePassword([FromBody] string json)
        {
            List<string> passwords = JsonConvert.DeserializeObject<List<string>>(json);

            var user = await UserManager.FindByIdAsync(GetIdentity());
            if(user != null)
            {
                var checkPassword = await SignInManager.CheckPasswordSignInAsync(user, passwords[0], false);
                if (checkPassword.Succeeded)
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(user);
                    var result = await UserManager.ResetPasswordAsync(user, token, passwords[1]);
                    if (result.Succeeded)
                    {
                        MailClient.SendPasswordChangedEmail(user.Email, user.UserName);
                        return ApiResponse.Oke("Password succesfully changed.");
                    }
                    else
                    {
                        return ApiResponse.Error("Couldn't change password.");
                    }
                }
                else
                {
                    return ApiResponse.Error("Current password is incorrect.");
                }
            }
            else
            {
                return ApiResponse.Error("Can't find user.");
            }
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
