using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BramrApi.Data;

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

        public async Task<ApiResponse> Login([FromBody] LoginModel model)
        {

            if (ModelState.IsValid)
            {
                if (await userManager.FindByEmailAsync(model.Email) == null)
                    return ApiResponse.Error("User not found!");


            }

            return ApiResponse.Error("Not all required fields have been filled in");
        }

    }
}
