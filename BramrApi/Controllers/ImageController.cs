using BramrApi.Data;
using BramrApi.Service;
using BramrApi.Service.Interfaces;
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
            FileModel File = new FileModel();
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
                    using Stream FileStream = new FileStream(Path.Combine(path, $"{Image.FileName}.png"), FileMode.Create);
                    await Image.CopyToAsync(FileStream);
                    File = new FileModel() { UserName = user.UserName, FilePath = Path.Combine(path, $"{Image.FileName}.png"), FileName = $"{Image.FileName}", FileUri = await File.CreateUri() };
                    await Database.AddModel(File);

                    return ApiResponse.Oke("File uploaded");
                }

                return ApiResponse.Error("Can't find user");
            }

            return ApiResponse.Error("Can't find file");
        }

        [HttpGet("info/{type}")]
        [Authorize]
        public async Task<FileModel> GetFileInfo(string type)
        {
            var user = await userManager.FindByIdAsync(GetIdentity());
            return Database.GetFileModel(user.UserName, type);
        }

        [HttpGet("download/{uri}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(string uri)
        {
            string path = Database.GetFileModelByUri(uri).FilePath;
            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }

            return NotFound();
        }

        [HttpDelete("delete/{uri}")]
        [Authorize]
        public async Task<ApiResponse> DeleteFile(string uri)
        {
            //Get FileModel.FilePath where FileUri = uri
            string path = @$"C:\Users\ruben\OneDrive\Bureaublad\Admin";
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
