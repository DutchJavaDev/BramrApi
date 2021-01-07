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
        private readonly ISMTP MailClient;
#if DEBUG
        private readonly string IMAGE_BASE_URL = @"https://localhost:44372/api/image/download/";
#else
        private readonly string IMAGE_BASE_URL = @"https://bramr.tech/api/image/download/";
#endif

        public WebsiteController(UserManager<IdentityUser> userManager, IDatabase database, ISMTP MailClient)
        {
            UserManager = userManager;
            Database = database;
            this.MailClient = MailClient;
        }

        [HttpGet("get")]
        [Authorize]
        public async Task<List<object>> GetLiveSite()
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return user == null ? 
                new List<object>() : Database.GetAllDesignElementsByUsername(user.UserName);
        }

        [HttpPost("uploadcv")]
        [Authorize]
        public async Task<ApiResponse> UploadCV([FromBody] string DesignElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return user == null ? 
                ApiResponse.Error("Can't find user") : await UploadTemplateToDatabase(user, DesignElements, 15, 1, true);
        }

        [HttpPost("uploadportfolio")]
        [Authorize]
        public async Task<ApiResponse> UploadPortfolio([FromBody] string DesignElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return user == null ? 
                ApiResponse.Error("Can't find user") : await UploadTemplateToDatabase(user, DesignElements, 23, 4, false);
        }

        [HttpPost("contact")]
        public async Task<ApiResponse> Contact([FromForm] ContactFormModel model)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(model.recipientUsername);
                if (user != null)
                {
                    MailClient.SendContactMail(user.Email, user.UserName, model.sendersName, model.sendersEmail, model.message,model.service);
                    return ApiResponse.Oke();
                }
                else
                {
                    return ApiResponse.Error();
                }
            }
            catch (Exception)
            {
                return ApiResponse.Error();
            }

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

                await Database.DeleteAllDesignModelsByUsernameAndType(user.UserName, IsCV);

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
                        Fontsize = textmodel.Fontsize,
                        TemplateType = textmodel.TemplateType
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
                        Padding = imagemodel.Padding,
                        TemplateType = imagemodel.TemplateType
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
                    string html = $"<p style=\"color:{textmodel.TextColor}; background-color:{textmodel.BackgroundColor}; font-size:{textmodel.Fontsize}rem; text-align:{(textmodel.TextAllignment == "0" ? "left" : textmodel.TextAllignment/* == "1" ? "center" : "right"*/)}\">{(textmodel.Bold ? "<b>" : "")}{(textmodel.Italic ? "<i>" : "")}{(textmodel.Underlined ? "<u>" : "")}{(textmodel.Strikedthrough ? "<s>" : "")}{textmodel.Text}{(textmodel.Bold ? "</b>" : "")}{(textmodel.Italic ? "</i>" : "")}{(textmodel.Underlined ? "</u>" : "")}{(textmodel.Strikedthrough ? "</s>" : "")}</p>";
                    template = template.Replace($"[**{i}**]", html);
                }
                for (int i = AllTextModels.Count; i < AllImageModels.Count + AllTextModels.Count; i++)
                {
                    var index = i - AllTextModels.Count;
                    var imagemodel = AllImageModels[index];

                    var file = Database.GetFileModelByUri(imagemodel.FileUri);

                    if (file == null) continue;

                    var imagePath = file.FilePath;

                    string html = $"<img src=\"{IMAGE_BASE_URL}{imagemodel.FileUri}\" alt=\"{imagemodel.Alt}\" style=\"float:{(imagemodel.FloatSet == "0" ? "none" : imagemodel.FloatSet)}; opacity:{imagemodel.Opacity.ToString().Replace(",", ".")}; width:{imagemodel.Width}%; height:{imagemodel.Height}px; padding:{imagemodel.Padding}px; border;{imagemodel.Border}px solid black; object-fit:{(imagemodel.ObjectFitSet == "0" ? "cover" : imagemodel.ObjectFitSet)};\"/>";
                    template = template.Replace($"[**{i}**]", html);
                }

                if (IsCV)
                {
                    System.IO.File.WriteAllText(Path.Combine(userProfile.IndexCvDirectory, "index.html"), template);
                }
                else
                {
                    System.IO.File.WriteAllText(Path.Combine(userProfile.IndexPortfolioDirectory, "index.html"), template);
                }

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
