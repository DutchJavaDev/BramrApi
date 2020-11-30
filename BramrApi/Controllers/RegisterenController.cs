using BramrApi.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterenController : ControllerBase
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly static PasswordOptions PasswordOptions = new PasswordOptions()
        {
            RequiredLength = 8,
            RequiredUniqueChars = 4,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true,
        };

        public RegisterenController(UserManager<IdentityUser> umanager)
        {
            UserManager = umanager;
        }

        [HttpGet("Account")]
        public async Task<object> RegisterAccount([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (await UserManager.FindByEmailAsync(model.Email) != null)
                {
                    // user exists
                    // return message saying so
                    return ""; 
                }

                var password = GenerateRandomPassword();

                var result = await UserManager.CreateAsync(new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email.Split("@")[0],
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                }, password);

                if (result.Succeeded)
                {

                }
                else
                {
                    
                }
                
                return Ok();
            }

            return NotFound();
        }

        [NonAction]
        private string GenerateRandomPassword()
        {
            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            var rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (PasswordOptions.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (PasswordOptions.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (PasswordOptions.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (PasswordOptions.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < PasswordOptions.RequiredLength
                || chars.Distinct().Count() < PasswordOptions.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

    }
}
