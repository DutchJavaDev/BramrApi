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
using System.Collections.Generic;
using Newtonsoft.Json;
using BramrApi.Database.Data;
using System;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteController : ControllerBase
    {
        private readonly IDatabase Database;
        private readonly UserManager<IdentityUser> UserManager;

#if DEBUG
        private readonly string IMAGE_BASE_URL = @"https://localhost:44372/api/image/download/";
#else
        private readonly string IMAGE_BASE_URL = @"https://bramr.tech/api/image/download/";
#endif

        public WebsiteController(UserManager<IdentityUser> userManager, IDatabase database)
        {
            UserManager = userManager;
            Database = database;
        }

        [HttpPost("uploadcv")]
        [Authorize]
        public async Task<ApiResponse> UploadCV([FromBody] string DesignElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            if (user != null)
            {
                return await UploadTemplateToDatabase(user, DesignElements, 15, 1, true);
            }

            return ApiResponse.Error("Can't find user");
        }

        [HttpPost("uploadportfolio")]
        [Authorize]
        public async Task<ApiResponse> UploadPortfolio([FromBody] string DesignElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            if (user != null)
            {
                return await UploadTemplateToDatabase(user, DesignElements, 0, 0, false);
            }

            return ApiResponse.Error("Can't find user");
        }

        [HttpGet("get")]
        [Authorize]
        public async Task<List<object>> GetLiveSite()
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            if (user != null)
            {
                return Database.GetAllDesignElementsByUsername(user.UserName);
            }

            return new List<object>();
        }

        [NonAction]
        private async Task<ApiResponse> UploadTemplateToDatabase(IdentityUser user, string DesignElements, int TextAmount, int ImageAmount, bool IsCV)
        {
            try
            {
                string path = Database.GetModelByUserName(user.UserName).WebsiteDirectory;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                await Database.DeleteAllTextAndImageModelsByUsername(user.UserName);

                List<object> AllModels = JsonConvert.DeserializeObject<List<object>>(DesignElements);

                for (int i = 0; i < TextAmount; i++)
                {
                    TextModel textmodel = JsonConvert.DeserializeObject<TextModel>(AllModels[i].ToString());
                    await Database.AddOrUpdateModel(new TextModel
                    {
                        UserName = user.UserName,
                        Location = i,
                        Text = textmodel.Text,
                        TextColor = textmodel.TextColor,
                        BackgroundColor = textmodel.BackgroundColor,
                        Bold = textmodel.Bold,
                        Italic = textmodel.Italic,
                        Underlined = textmodel.Underlined,
                        Strikedthrough = textmodel.Strikedthrough,
                        TextAllignment = textmodel.TextAllignment,
                        Fontsize = textmodel.Fontsize
                    });
                }
                for (int i = TextAmount; i < ImageAmount + TextAmount; i++)
                {
                    ImageModel imagemodel = JsonConvert.DeserializeObject<ImageModel>(AllModels[i].ToString());
                    await Database.AddOrUpdateModel(new ImageModel
                    {
                        UserName = user.UserName,
                        FileUri = imagemodel.FileUri,
                        Location = i,
                        Width = imagemodel.Width,
                        Height = imagemodel.Height,
                        Alt = imagemodel.Alt,
                        Border = imagemodel.Border,
                        FloatSet = imagemodel.FloatSet,
                        Opacity = imagemodel.Opacity,
                        ObjectFitSet = imagemodel.ObjectFitSet,
                        Padding = imagemodel.Padding
                    });
                }

                return CreateHTML(user, IsCV);
            }
            catch (Exception e)
            {
                return ApiResponse.Error(e.ToString());
            }
        }

        [NonAction]
        private ApiResponse CreateHTML(IdentityUser user, bool IsCV)
        {
            try
            {
                string template;
                if (IsCV)
                {
                    template = System.IO.File.ReadAllText(@"Templates\cv_template.html");
                }
                else
                {
                    template = System.IO.File.ReadAllText(@"Templates\portfolio_template.html");
                }
                UserProfile userProfile = Database.GetModelByUserName(user.UserName);
                List<TextModel> AllTextModels = Database.GetAllTextModelsByUsername(user.UserName);
                List<ImageModel> AllImageModels = Database.GetAllImageModelsByUsername(user.UserName);

                for (int i = 0; i < AllTextModels.Count; i++)
                {
                    var textmodel = AllTextModels[i];
                    string html = $"<p style=\"color:{textmodel.TextColor}; background-color:{textmodel.BackgroundColor}; font-size:{textmodel.Fontsize / 5}vh; text-align:{(textmodel.TextAllignment == "0" ? "left" : textmodel.TextAllignment/* == "1" ? "center" : "right"*/)}\">{(textmodel.Bold ? "<b>" : "")}{(textmodel.Italic ? "<i>" : "")}{(textmodel.Underlined ? "<u>" : "")}{(textmodel.Strikedthrough ? "<s>" : "")}{textmodel.Text}{(textmodel.Bold ? "</b>" : "")}{(textmodel.Italic ? "</i>" : "")}{(textmodel.Underlined ? "</u>" : "")}{(textmodel.Strikedthrough ? "</s>" : "")}</p>";
                    template = template.Replace($"[**{i}**]", html);
                }
                for (int i = AllTextModels.Count; i < AllImageModels.Count + AllTextModels.Count; i++)
                {
                    var index = i - AllTextModels.Count;

                    var imagemodel = AllImageModels[index];
                    var imagePath = Database.GetFileModelByUri(imagemodel.FileUri).FilePath;
                    string html = $"<img src=\"{IMAGE_BASE_URL}{imagemodel.FileUri}\" alt=\"{imagemodel.Alt}\" style=\"float:{(imagemodel.FloatSet == "0" ? "none" : imagemodel.FloatSet)}; opacity:{imagemodel.Opacity.ToString().Replace(",", ".")}; width:{imagemodel.Width}%; height:{imagemodel.Height}px; padding:{imagemodel.Padding}px; border;{imagemodel.Border}px solid black; object-fit:{(imagemodel.ObjectFitSet == "0" ? "cover" : imagemodel.ObjectFitSet)};\"/>";
                    template = template.Replace($"[**{i}**]", html);
                }

                System.IO.File.WriteAllText(Path.Combine(userProfile.WebsiteDirectory, "index.html"), template);

                return ApiResponse.Oke("File uploaded");
            }
            catch (Exception e)
            {
                return ApiResponse.Error(e.ToString());
            }
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
