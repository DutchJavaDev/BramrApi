using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BramrApi.Data;
using BramrApi.Utils;
using Microsoft.AspNetCore.Authorization;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SignInController : ControllerBase
    {
        // Provides the APIs for managing user in identity
        private readonly UserManager<IdentityUser> userManager;

        // Provides the APIs for managing user logins identity
        private readonly SignInManager<IdentityUser> signInManager;

        public SignInController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    return ApiResponse.Error("User not found!");

                // Try a password signin
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    // TODO add roles when we diceide to use them
                    var jwt = IdentityConfig.GenerateJWT(user, userRoles: null);

                    return ApiResponse.Oke("Sucess", data: new { user.Id, jwt });
                }
                else
                {
                    return ApiResponse.Error("Failed to login", data: new { result });
                }
            }

            return ApiResponse.Error("Not all required fields have been filled in");
        }

    }
}
