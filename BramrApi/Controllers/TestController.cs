using BramrApi.Service.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using io = System.IO;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDatabase database;

        private readonly IAPICommand command;

        public TestController(IDatabase database, IAPICommand command)
        {
            this.database = database;
            this.command = command;
        }

        [HttpGet]
        public async Task<string> Index()
        {
            var res = await command.Test();

            try
            {
                return await io.File.ReadAllTextAsync(res);
            }
            catch (System.Exception e)
            {
                return e.Message;
            }
        }


        [HttpGet("cv/{username}")]
        public async Task<IActionResult> Cv(string username)
        {
            return Content(await command.GetIndexFor(username, true),"text/html", Encoding.UTF8);
        }
    }
}
