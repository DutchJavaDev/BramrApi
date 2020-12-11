using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditHistoryController : ControllerBase
    {
        // Provides the API with methods for the database
        private readonly IDatabase Database;

        // Provides the API with managing user in identity
        private readonly UserManager<IdentityUser> userManager;

        public EditHistoryController(UserManager<IdentityUser> userManager, IDatabase Database)
        {
            this.userManager = userManager;
            this.Database = Database;
        }



        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
