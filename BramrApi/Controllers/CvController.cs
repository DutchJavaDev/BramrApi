using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BramrApi.Controllers
{
    [Route("cv")]
    [ApiController]
    public class CvController : ControllerBase
    {
        private readonly IAPICommand command;

        public CvController(IDatabase database, IAPICommand command)
        {
            this.command = command;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetSite(string username)
        {
            return Content(await command.GetIndexFor(username, true), "text/html", Encoding.UTF8);
        }
    }
}
