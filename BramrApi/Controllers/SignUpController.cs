using BramrApi.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        // Provides the APIs for managing user in identity
        private readonly UserManager<IdentityUser> UserManager;

        // Options for generating a password, might not use this either
        private readonly static PasswordOptions PasswordOptions = new PasswordOptions()
        {
            RequiredLength = 8,
            RequiredUniqueChars = 4,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true,
        };

        // Constructor
        public SignUpController(UserManager<IdentityUser> umanager)
        {
            UserManager = umanager;
        }

        /// <summary>
        /// Function/Endpoint in this controller for creating a new user with identitty
        /// </summary>
        /// <param name="model">Class with required fields needed to create a account</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<ApiResponse> Index([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if there is a identity user with this email
                if (await UserManager.FindByEmailAsync(model.Email) != null)
                {

                    // There is, goodbeye 👋
                    return ApiResponse.Error("This email has already been taken"); 
                }

                // Identity user class
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email.Split("@")[0],
                };

                // Create a user with function
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //👋
                    return ApiResponse.Oke("Account has been created");
                }
                else
                {
                    // failed to create user
                    // send errors for easy debuggin, remove later !REMEMBER lol
                    var list = new List<string>();

                    foreach (var error in result.Errors)
                    {
                        list.Add($"{error.Code}".Trim());
                    }

                    //👋
                    return ApiResponse.Error("Could not compleed action", errors: list); 
                }
            }

            //👋
            // Model is invalid
            return ApiResponse.Error("Not all required fields have been filled in"); 
        }

        // Stole this from stackoverflow, might not use it. Ask group
        //[NonAction]
        //private string GenerateRandomPassword()
        //{
        //    string[] randomChars = new[] {
        //    "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        //    "abcdefghijkmnopqrstuvwxyz",    // lowercase
        //    "0123456789",                   // digits
        //    "!@$?_-"                        // non-alphanumeric
        //};

        //    var rand = new Random(Environment.TickCount);
        //    List<char> chars = new List<char>();

        //    if (PasswordOptions.RequireUppercase)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[0][rand.Next(0, randomChars[0].Length)]);

        //    if (PasswordOptions.RequireLowercase)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[1][rand.Next(0, randomChars[1].Length)]);

        //    if (PasswordOptions.RequireDigit)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[2][rand.Next(0, randomChars[2].Length)]);

        //    if (PasswordOptions.RequireNonAlphanumeric)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[3][rand.Next(0, randomChars[3].Length)]);

        //    for (int i = chars.Count; i < PasswordOptions.RequiredLength
        //        || chars.Distinct().Count() < PasswordOptions.RequiredUniqueChars; i++)
        //    {
        //        string rcs = randomChars[rand.Next(0, randomChars.Length)];
        //        chars.Insert(rand.Next(0, chars.Count),
        //            rcs[rand.Next(0, rcs.Length)]);
        //    }

        //    return new string(chars.ToArray());
        //}

    }
}
