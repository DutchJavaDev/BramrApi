using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BramrApi.Data;
using System.Collections.Generic;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public SignInController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("account")]
        public async Task<ApiResponse> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return ApiResponse.Error("User not found!");

                var result = await signInManager.PasswordSignInAsync(user, model.Password, true, false);

                if (result.Succeeded)
                {
                    return ApiResponse.Oke("Logged in!", data: user.Id);
                }
                else
                {
                    return ApiResponse.Error("Failed to login");
                }
            }

            return ApiResponse.Error("Not all required fields have been filled in");
        }

    }
}
