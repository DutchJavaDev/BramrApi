using BramrApi.Service.Interfaces;
using BramrApi.Data;
using System;
using System.Threading.Tasks;
using BramrApi.Database.Data;
using Microsoft.AspNetCore.Mvc;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IDatabase database;

        public TestController(IDatabase database)
        {
            this.database = database;
        }

        [HttpGet]
        public string Index() => "Hello Api :)";
    }
}
