using BramrApi.Service.Interfaces;
using BramrApi.Data;
using System;
using System.Threading.Tasks;
using BramrApi.Database.Data;
using Microsoft.AspNetCore.Mvc;
using io = System.IO;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDatabase Database;
        private readonly INginxCommand CommandService;
        private readonly IServerBlockWriter ServerBlockWriter;

        public TestController(IDatabase database, INginxCommand commandService, IServerBlockWriter serverBlockWriter)
        {
            Database = database;
            CommandService = commandService;
            ServerBlockWriter = serverBlockWriter;
        }

        [HttpGet]
        public async Task<ApiResponse> Index()
        {
            ServerBlockWriter.CreateServerBlock("boris", "bmulder");

            return ApiResponse.Oke()
                   .AddData("block exist", await ServerBlockWriter.BlockExists("boris"))
                   .AddData("nginx config result", await CommandService.TestNginxConfiguration())
                   .AddData("nginx reload result", await CommandService.ReloadNginx())
                   .AddData("Lol", "yep");
        }
    }
}
