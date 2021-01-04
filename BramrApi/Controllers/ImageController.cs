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
        private readonly UserManager<IdentityUser> UserManager;

        public ImageController(UserManager<IdentityUser> userManager, IDatabase Database)
        {
            this.UserManager = userManager;
            this.Database = Database;
        }

        [HttpGet("info/{type}")]
        [Authorize]
        public async Task<string> GetFileInfo(string type)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return Database.GetFileModel(user.UserName, type).FileUri;
        }

        [HttpGet("download/{uri}")]
        [AllowAnonymous]
        public IActionResult DownloadFile(string uri)
        {
            var file = Database.GetFileModelByUri(uri);

            if (file == null) return NotFound();

            if (System.IO.File.Exists(file.FilePath))
                return File(System.IO.File.ReadAllBytes(file.FilePath), $"image/png");

            return NotFound();
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ApiResponse> UploadFile([FromForm] IFormFile Image)
        {

            if (FileUploadValidator.FileIsImage(Image.OpenReadStream()))
            {
                if (Image != null)
                {

                    var identity = GetIdentity();

                    var user = await UserManager.FindByIdAsync(identity);
                    if (user != null)
                    {

                        string path = Database.GetModelByUserName(user.UserName).ImageDirectory;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var imagePath = Path.Combine(path, $"{Image.FileName}.png");

                        // You can also update the filemodel so that you wont need to delete and create a new one 
                        //await Database.DeleteFileModelByPath(imagePath);
                        var model = Database.GetFileModelByPath(imagePath);

                        if (model != null)
                        {
                            model.Identity = user.Id;
                            model.FilePath = Path.Combine(path, $"{Image.FileName}.png");
                            model.FileName = $"{Image.FileName}";
                            model.FileUri = Utility.CreateFileUri();

                            // AddModel can also update an existing model
                            await Database.AddOrUpdateModel(model);
                        }
                        else
                        {
                            await Database.AddOrUpdateModel(new FileModel
                            {
                                UserName = user.UserName,
                                FilePath = Path.Combine(path, $"{Image.FileName}.png"),
                                FileName = $"{Image.FileName}",
                                FileUri = Utility.CreateFileUri(),
                                Identity = user.Id
                            });
                        }

                        using Stream FileStream = new FileStream(Path.Combine(path, $"{Image.FileName}.png"), FileMode.Create);
                        await Image.CopyToAsync(FileStream);

                        return ApiResponse.Oke("File uploaded");
                    }
                    else
                    {
                        return ApiResponse.Error("Can't find user");
                    }

                }
                else
                    return ApiResponse.Error("Can't find file");
            }
            else
            {
                return ApiResponse.Error("Cannot validate file.");
            }
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
