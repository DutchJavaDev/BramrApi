using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BramrApi.Data;
using BramrApi.Utils;
using Microsoft.AspNetCore.Authorization;
using BramrApi.Service.Interfaces;
using BramrApi.Database.Data;
using System.Linq;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SignInController : ControllerBase
    {
        // Provides the APIs for managing user in identity
        private readonly UserManager<IdentityUser> UserManager;

        // Provides the APIs for managing user logins identity
        private readonly SignInManager<IdentityUser> SignInManager;

        private readonly IDatabase Database;

        public SignInController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IDatabase database)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Database = database;
        }

        [HttpGet("verify/jwt")]
        [Authorize]
        public async Task<IActionResult> JWTValidation()
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());

            if (user == null)
                return NotFound();

            return Ok();
        }

        /// <summary>
        /// SignUp endpoint, expects a LoginModel in json form, the [FromBody] attribute will try and convert it to a LoginModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        [AllowAnonymous]
        public async Task<ApiResponse> Index([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // First check if the user can be found
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                    return ApiResponse.Error("User not found!");

                // Try a password signin
                var result = await SignInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    // TODO add roles when we diceide to use them
                    var token = IdentityConfig.GenerateJWT(user, userRoles: null);

                    if (Database.UserNameExist(user.UserName))
                    {
                        // get profile
                        var profile = Database.GetModelByIdentity<UserProfile>(user.Id);

                        return ApiResponse.Oke("Success")
                                .AddData("jwt_token", token)
                                .AddData("username", profile.UserName);
                    }
                    else
                    {
                        return ApiResponse.Error("Could not find profile");
                    }
                }
                else
                {
                    return ApiResponse.Error("Failed to login");
                }
            }

            return ApiResponse.Error("Not all required fields have been filled in");
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
