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

        public WebsiteController(UserManager<IdentityUser> UserManager, IDatabase Database)
        {
            this.UserManager = UserManager;
            this.Database = Database;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ApiResponse> UploadCV([FromBody] string TextElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            if (user != null)
            {
                try
                {
                    string path = Database.GetModelByUserName(user.UserName).WebsiteDirectory;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    await Database.DeleteAllTextAndImageModelsByUsername(user.UserName);

                    List<object> AllModels = JsonConvert.DeserializeObject<List<object>>(TextElements);

                    for (int i = 0; i < 2; i++)
                    {
                        TextModel textmodel = JsonConvert.DeserializeObject<TextModel>(AllModels[i].ToString());
                        await Database.AddModel(new TextModel
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
                    ImageModel imagemodel = JsonConvert.DeserializeObject<ImageModel>(AllModels[2].ToString());
                    await Database.AddModel(new ImageModel
                    {
                        UserName = user.UserName,
                        FileUri = imagemodel.FileUri,
                        Location = 2,
                        Width = imagemodel.Width,
                        Height = imagemodel.Height,
                        Alt = imagemodel.Alt,
                        Border = imagemodel.Border,
                        FloatSet = imagemodel.FloatSet,
                        Opacity = imagemodel.Opacity,
                        ObjectFitSet = imagemodel.ObjectFitSet,
                        Padding = imagemodel.Padding
                    });

                    return CreateHTML(user);
                }
                catch (Exception e)
                {
                    return ApiResponse.Error(e.ToString());
                }
            }

            return ApiResponse.Error("Can't find user");
        }

        [NonAction]
        private ApiResponse CreateHTML(IdentityUser user)
        {
            try
            {
                string template = System.IO.File.ReadAllText(@"Templates\cv_template.html");
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
                    var imagemodel = AllImageModels[i];
                    var imagePath = Database.GetFileModelByUri(imagemodel.FileUri).FilePath;
                    string html = $"<img src=\"{imagePath}\" alt=\"{imagemodel.Alt}\" style=\"float:{(imagemodel.FloatSet == "0" ? "none" : imagemodel.FloatSet)}; opacity:{imagemodel.Opacity.ToString().Replace(",", ".")}; width:{imagemodel.Width}%; height:{imagemodel.Height}px; padding:{imagemodel.Padding}px; border;{imagemodel.Border}px solid black; object-fit:{(imagemodel.ObjectFitSet == "0" ? "cover" : imagemodel.ObjectFitSet)};\"/>";
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
