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
using BramrApi.Utils;
using System;
using io = System.IO;

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

        [HttpGet("get/{IsCV}")]
        [Authorize]
        public async Task<List<object>> GetLiveSite(bool IsCV)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return user == null ? 
                new List<object>() : IsCV ? Database.GetAllDesignElementsByUsername(user.UserName, "Cv") : Database.GetAllDesignElementsByUsername(user.UserName, "Portfolio");
        }

        [HttpPost("uploadcv")]
        [Authorize]
        public async Task<ApiResponse> UploadCV([FromBody] string DesignElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return user == null ? 
                ApiResponse.Error("Can't find user") : await UploadTemplateToDatabase(user, DesignElements, 37, 1, true);
        }

        [HttpPost("uploadportfolio")]
        [Authorize]
        public async Task<ApiResponse> UploadPortfolio([FromBody] string DesignElements)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return user == null ? 
                ApiResponse.Error("Can't find user") : await UploadTemplateToDatabase(user, DesignElements, 24, 4, false);
        }

        [HttpPost("contact")]
        public async Task<ContentResult> Contact([FromForm] ContactFormModel model)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(model.recipientUsername);
#if DEBUG
                var html = await io.File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\ContactFormResponse.html");
#else
                //var html = await io.File.ReadAllTextAsync(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\ContactFormResponse.html");
                var html = await io.File.ReadAllTextAsync(Utility.CreatePathFromBegin($@"usr/share/temp/ContactFormResponse.html"));
#endif
                if (user != null)
                {

                    html = html.Replace("[MESSAGE1]", "Message successfully delivered.");
                    html = html.Replace("[MESSAGE2]", "The person you were trying to reach has been send an email with your message.");
                    
                    MailClient.SendContactMail(user.Email, user.UserName, model.sendersName, model.sendersEmail, model.message,model.service);
                    return Content(html,"text/html");
                }
                else
                {
                    html = html.Replace("[MESSAGE1]", "Message failed to deliver.");
                    html = html.Replace("[MESSAGE2]", "Something went wrong, please try again.");
                    return Content(html, "text/html");
                }
            }
            catch (Exception)
            {

                return Content("<b>Something went terribly wrong please notify our developer team by sending an email to Bramrinfo@gmail.com </b>", "text/html");
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
                        FontSize = textmodel.FontSize,
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

                return await CreateHTML(user, IsCV, AllModels);
            }
            catch (Exception e)
            {
                return ApiResponse.Error(e.ToString());
            }
        }

        [NonAction]
        private async Task<ApiResponse> CreateHTML(IdentityUser user, bool IsCV, List<object> AllModels)
        {
            try
            {
                UserProfile userProfile = Database.GetModelByUserName(user.UserName);
                if (IsCV)
                {
                    List<TextModel> AllTextModels = Database.GetAllTextModelsByUsername(user.UserName, "Cv");
                    List<ImageModel> AllImageModels = Database.GetAllImageModelsByUsername(user.UserName, "Cv");
                    string template = System.IO.File.ReadAllText(@"Templates/cv_template.html");

                    for (int i = 0; i < AllTextModels.Count; i++)
                    {
                        var textmodel = AllTextModels[i];
                        string html = $"<p style=\"{(textmodel.TextColor != null ? $"color :{textmodel.TextColor};" : string.Empty)} {(textmodel.BackgroundColor != null ? $"background-color :{textmodel.BackgroundColor};" : string.Empty)} font-size:{textmodel.FontSize}rem; text-align:{(textmodel.TextAllignment == "0" ? "left;" : textmodel.TextAllignment == "1" ? "center;" : "right;")} {(textmodel.Shadow ? $"text-shadow: 1px 1px 5px {(textmodel.TextColor != string.Empty ? textmodel.TextColor : "#000000")};" : string.Empty)} {(textmodel.FontWeight != 0 ? $"font-weight:{textmodel.FontWeight};" : string.Empty)} {(textmodel.Font != string.Empty && textmodel.Font != null ? $"font-family:'{textmodel.Font}', sans-serif;" : string.Empty)}\">{(textmodel.Bold ? "<b>" : string.Empty)}{(textmodel.Italic ? "<i>" : string.Empty)}{(textmodel.Underlined ? "<u>" : string.Empty)}{(textmodel.Strikedthrough ? "<s>" : string.Empty)}{textmodel.Text}{(textmodel.Bold ? "</b>" : string.Empty)}{(textmodel.Italic ? "</i>" : string.Empty)}{(textmodel.Underlined ? "</u>" : string.Empty)}{(textmodel.Strikedthrough ? "</s>" : string.Empty)}</p>";
                        template = template.Replace($"[**{i}**]", html);
                    }
                    for (int i = AllTextModels.Count; i < AllImageModels.Count + AllTextModels.Count; i++)
                    {
                        var imagemodel = AllImageModels[i - AllTextModels.Count];
                        string html = $"<img src=\"{(imagemodel.FileUri == null || imagemodel.FileUri == string.Empty ? "" : IMAGE_BASE_URL + imagemodel.FileUri)}\" alt=\"{imagemodel.Alt}\" style=\"float:{(imagemodel.FloatSet == "0" ? "none" : imagemodel.FloatSet == "1" ? "left" : "right")}; opacity:{imagemodel.Opacity.ToString().Replace(",", ".")}; width:{imagemodel.Width}%; height:{imagemodel.Height}px; padding:{imagemodel.Padding}px; border;{imagemodel.Border}px solid black; object-fit:{(imagemodel.ObjectFitSet == "0" ? "cover" : imagemodel.ObjectFitSet == "1" ? "fill" : imagemodel.ObjectFitSet == "2" ? "contain" : "none")};\"/>";
                        template = template.Replace($"[**{i}**]", html);
                    }
                    for (int i = AllImageModels.Count + AllTextModels.Count; i < AllModels.Count; i++)
                    {
                        var skillamount = AllModels[i];
                        template = template.Replace($"[**{i}**]", skillamount.ToString());
                    }

                    System.IO.File.WriteAllText(Path.Combine(userProfile.IndexCvDirectory, "index.html"), template);
                    userProfile.HasCv = IsCV;
                }
                else
                {
                    List<TextModel> AllTextModels = Database.GetAllTextModelsByUsername(user.UserName, "Portfolio");
                    List<ImageModel> AllImageModels = Database.GetAllImageModelsByUsername(user.UserName, "Portfolio");
                    string template = System.IO.File.ReadAllText(@"Templates/portfolio_template.html");

                    for (int i = 0; i < AllTextModels.Count; i++)
                    {
                        var textmodel = AllTextModels[i];
                        string html = $"style=\"{(textmodel.TextColor != null ? $"color :{textmodel.TextColor};" : string.Empty)} {(textmodel.BackgroundColor != null ? $"background-color :{textmodel.BackgroundColor};" : string.Empty)} font-size:{textmodel.FontSize}rem; text-align:{(textmodel.TextAllignment == "0" ? "left;" : textmodel.TextAllignment == "1" ? "center;" : "right;")} {(textmodel.Shadow ? $"text-shadow: 1px 1px 5px {(textmodel.TextColor != string.Empty ? textmodel.TextColor : "#000000")};" : string.Empty)} {(textmodel.FontWeight != 0 ? $"font-weight:{textmodel.FontWeight};" : string.Empty)} {(textmodel.Font != string.Empty && textmodel.Font != null ? $"font-family:'{textmodel.Font}', sans-serif;" : string.Empty)}\">{(textmodel.Bold ? "<b>" : string.Empty)}{(textmodel.Italic ? "<i>" : string.Empty)}{(textmodel.Underlined ? "<u>" : string.Empty)}{(textmodel.Strikedthrough ? "<s>" : string.Empty)}{textmodel.Text}{(textmodel.Bold ? "</b>" : string.Empty)}{(textmodel.Italic ? "</i>" : string.Empty)}{(textmodel.Underlined ? "</u>" : string.Empty)}{(textmodel.Strikedthrough ? "</s>" : string.Empty)}";
                        template = template.Replace($"[**{i}**]", html).Replace($"[**{i}-2**]", textmodel.Text);
                    }
                    for (int i = AllTextModels.Count; i < AllImageModels.Count + AllTextModels.Count; i++)
                    {
                        var imagemodel = AllImageModels[i - AllTextModels.Count];
                        string html = $"<img src=\"{(imagemodel.FileUri == null || imagemodel.FileUri == string.Empty ? "" : IMAGE_BASE_URL + imagemodel.FileUri)}\" alt=\"{imagemodel.Alt}\" style=\"float:{(imagemodel.FloatSet == "0" ? "none" : imagemodel.FloatSet == "1" ? "left" : "right")}; opacity:{imagemodel.Opacity.ToString().Replace(",", ".")}; width:{imagemodel.Width}%; height:{imagemodel.Height}px; padding:{imagemodel.Padding}px; border;{imagemodel.Border}px solid black; object-fit:{(imagemodel.ObjectFitSet == "0" ? "cover" : imagemodel.ObjectFitSet == "1" ? "fill" : imagemodel.ObjectFitSet == "2" ? "contain" : "none")};\"/>";
                        template = template.Replace($"[**{i}**]", html);
                    }
                    template = template.Replace($"[**28**]", user.UserName);

                    System.IO.File.WriteAllText(Path.Combine(userProfile.IndexPortfolioDirectory, "index.html"), template);
                    userProfile.HasPortfolio = !IsCV;
                }

                await Database.AddOrUpdateModel(userProfile);

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
