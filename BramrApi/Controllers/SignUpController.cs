using BramrApi.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using BramrApi.Service.Interfaces;
using System.Linq;
using System.Text;
using io = System.IO;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        // Provides the APIs for managing user in identity
        private readonly UserManager<IdentityUser> UserManager;

        // Db
        private readonly IDatabase Database;

        // Server
        private readonly IAPICommand CommandService;

        // Mail
        private readonly ISMTP MailClient;

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
        public SignUpController(UserManager<IdentityUser> umanager, ISMTP mailClient, IDatabase database, IAPICommand commandService)
        {
            UserManager = umanager;
            MailClient = mailClient;
            Database = database;
            CommandService = commandService;
        }

        [HttpGet("username/exists/{name}")]
        public ApiResponse UsernameExists(string name)
        {
            var Exists = Database.UserNameExist(name);
            return ApiResponse.Oke().AddData("user_exists", Exists.ToString());
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

                if (Database.UserNameExist(model.UserName))
                { 
                    return ApiResponse.Error("Username is allready taken");
                }

                model.UserName = model.UserName.Trim();

                // Identity user class
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                };

                // Gen password
                 var password = GenerateRandomPassword();

                // Create a user with function
                var result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    user = await UserManager.FindByEmailAsync(model.Email);

                    var userprofile = CommandService.CreateUser(model.UserName);

                    if (userprofile == null)
                    {
                        // Break opperation
                        await DeleteAccount(user);

                        return ApiResponse.Error("Failed to create profile");
                    }

                    if (!CommandService.CreateWebsiteDirectory(model.UserName))
                    {
                        userprofile.Identity = user.Id;

                        await Database.AddOrUpdateModel(userprofile);

                        #region QRCode black magic by Mathijs
                        QrCodeGenerator qrGen = new QrCodeGenerator();

                        qrGen.CreateQR($"https://bramr.tech/api/test/cv/{model.UserName}", model.UserName); //api url is temp

                        MailClient.SendPasswordEmail(model.Email, password, model.UserName);
#if DEBUG
                        io.File.Delete($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\{model.UserName}.jpeg");
#else
                        io.File.Delete(@$"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\{model.UserName}.jpeg");
#endif
                        #endregion
                        //👋
                        return ApiResponse.Oke("Account has been created");
                    }
                    else
                    {
                        await DeleteAccount(user);

                        return ApiResponse.Error("CWD failed");
                    }
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

        [NonAction]
        private async Task DeleteAccount(IdentityUser user)
        {
            if (user == null) return;

            var result = await UserManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var builder = new StringBuilder();

                builder.AppendLine($"[User] {user.Email}{Environment.NewLine}");

                foreach (var error in result.Errors)
                {
                    builder.AppendLine($"[{error.Code}] '{error.Description}'");
                }

                Sentry.SentrySdk.CaptureMessage($"Identity [DeleteAccount] errors {Environment.NewLine}", Sentry.Protocol.SentryLevel.Warning);
            }
        }

        //Stole this from stackoverflow, might not use it. Ask group

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
