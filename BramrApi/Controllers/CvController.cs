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

        /// <summary>
        /// In de url om de functie aan te roepen geef je de username van de gebruiker mee en via de functie GetIndexFor haalt de api dan het juiste html bestand op.
        /// </summary>
        /// <param name="username">Naam van gebruiker</param>
        /// <returns>Html bestand van het cv van de gebruiker.</returns>
        [HttpGet("{username}")]
        public async Task<IActionResult> GetSite(string username)
        {
            return Content(await command.GetIndexFor(username, true), "text/html", Encoding.UTF8);
        }
    }
}
