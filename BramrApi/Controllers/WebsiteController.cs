using BramrApi.Data;
using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Components.Rendering;
using System.Web;
using System.Text;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteController : ControllerBase
    {
        private readonly IDatabase Database;
        private readonly UserManager<IdentityUser> userManager;

        public WebsiteController(UserManager<IdentityUser> userManager, IDatabase Database)
        {
            this.userManager = userManager;
            this.Database = Database;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ApiResponse> UploadFile([FromBody] string list)
        {
            var user = await userManager.FindByIdAsync(GetIdentity());
            if (user != null)
            {

                string path = Database.GetModelByUserName(user.UserName).WebsiteDirectory;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(path, "list.txt");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.File.WriteAllText(filePath, list);
                Something();

                return ApiResponse.Oke("File uploaded");
            }

            return ApiResponse.Error("Can't find user");
        }

        [NonAction]
        public void Something()
        {
            bool italic = true;
            var html = $"<html>{(italic ? "<i>" : "")}oi{(italic ? "</i>" : "")}</html>";
            System.IO.File.WriteAllText(@"C:\Users\ruben\OneDrive\Bureaublad\oi.html", html);
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
