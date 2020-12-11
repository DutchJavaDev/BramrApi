using BramrApi.Data;
using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using BramrApi.Utils;
using System.Threading.Tasks;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IDatabase Database;

        // Provides the APIs for managing user in identity
        private readonly UserManager<IdentityUser> userManager;

        public ImageController(UserManager<IdentityUser> userManager, IDatabase Database)
        {
            this.userManager = userManager;
            this.Database = Database;
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
                    string path = Database.GetModelByUserName(user.UserName).ImageDirectory;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var imagePath = Path.Combine(path, $"{Image.FileName}.png");

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);

                        await Database.DeleteFileModelByPath(imagePath);
                    }

                    await Database.AddModel(new FileModel
                    {
                        UserName = user.UserName,
                        FilePath = Path.Combine(path, $"{Image.FileName}.png"),
                        FileName = $"{Image.FileName}",
                        FileUri = Utility.CreateFileUri()
                    });

                    using Stream FileStream = new FileStream(Path.Combine(path, $"{Image.FileName}.png"), FileMode.Create);
                    await Image.CopyToAsync(FileStream);

                    return ApiResponse.Oke("File uploaded");
                }

                return ApiResponse.Error("Can't find user");
            }

            return ApiResponse.Error("Can't find file");
        }

        [HttpGet("info/{type}")]
        [Authorize]
        public async Task<string> GetFileInfo(string type)
        {
            var user = await userManager.FindByIdAsync(GetIdentity());
            return Database.GetFileModel(user.UserName, type).FileUri;
        }

        [HttpGet("download/{uri}")]
        [AllowAnonymous]
        public IActionResult DownloadFile(string uri)
        {
            string path = Database.GetFileModelByUri(uri).FilePath;
            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }

            return NotFound();
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
