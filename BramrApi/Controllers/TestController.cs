using BramrApi.Service.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;
using io = System.IO;
using Microsoft.AspNetCore.Identity;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<IdentityUser> UserManager;

        private readonly IDatabase database;

        private readonly IAPICommand command;

        public TestController(IDatabase database, IAPICommand command, UserManager<IdentityUser> umanager)
        {
            this.database = database;
            this.command = command;
            this.UserManager = umanager;
        }

        [HttpGet]
        public string Index()
        {
            return "Hello from the api :-), now fuckoff.";
        }


        [HttpGet("cv/{username}")]
        public async Task<IActionResult> Cv(string username)
        {
            return Content(await command.GetIndexFor(username, true),"text/html", Encoding.UTF8);
        }

        [HttpGet("exterminate")]
        public async Task<string> Delete()
        {
            var users = UserManager.Users.ToArray();

            var builder = new StringBuilder();

            for (var i = 0; i < users.Length; i++)
            {
                var user = users[i];

                if (user != null)
                {
                    var profile = database.GetModelByUserName(user.UserName);

                    if (profile != null)
                    {
                        try
                        {
                            io.Directory.Delete(profile.WebsiteDirectory, true);
                        }
                        catch (System.Exception e)
                        {
                            builder.AppendLine($"user: {user.UserName} || {e.Message}");
                        }
                    }

                    await database.DeleteModel(profile);

                    await UserManager.DeleteAsync(user);
                }
            }

            return builder.ToString();
        }
    }
}
