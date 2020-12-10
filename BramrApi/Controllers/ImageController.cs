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
                    if (System.IO.File.Exists(Path.Combine(path, $"{Image.FileName}.png")))
                    {
                        System.IO.File.Delete(Path.Combine(path, $"{Image.FileName}.png"));
                        await Database.DeleteModelByPath(Path.Combine(path, $"{Image.FileName}.png"));
                    }
                    File = new FileModel() { UserName = user.UserName, FilePath = Path.Combine(path, $"{Image.FileName}.png"), FileName = $"{Image.FileName}", FileUri = File.CreateUri() };
                    await Database.AddModel(File);
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
