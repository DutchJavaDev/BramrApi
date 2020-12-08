using BramrApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        // Provides the APIs for managing user in identity
        private readonly UserManager<IdentityUser> userManager;

        public ImageController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ApiResponse> UploadFile([FromForm] IFormFile Image)
        {
            if (Image != null)
            {
                var user = await userManager.FindByIdAsync(GetIdentity());
                if (user != null)
                {
                    string path = @$"C:\Users\ruben\OneDrive\Bureaublad\{user.UserName}";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using Stream FileStream = new FileStream(Path.Combine(path, "profielfoto.png"), FileMode.Create);
                    await Image.CopyToAsync(FileStream);

                    return ApiResponse.Oke("File uploaded");
                }

                return ApiResponse.Error("Can't find user");
            }

            return ApiResponse.Error("Can't find file");
        }

        [HttpGet("download")]
        //[Authorize]
        public async Task<IActionResult> DownloadFile()
        {
            //var user = await userManager.FindByIdAsync(GetIdentity());
            //if (user != null)
            //{
                string path = @$"C:\Users\ruben\OneDrive\Bureaublad\Admin";
                if (Directory.Exists(path))
                {
                    return File(System.IO.File.ReadAllBytes(Path.Combine(path, "profielfoto.png")), "image/png");
                }
            //}
            return NotFound();
        }

        [HttpDelete("delete")]
        public async Task<ApiResponse> DeleteFile([FromBody] string FileName)
        {
            string path = Path.Combine(string.Empty, FileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);

                return ApiResponse.Oke("Succesfully deleted file");
            }

            return ApiResponse.Error("Can't find file");
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
