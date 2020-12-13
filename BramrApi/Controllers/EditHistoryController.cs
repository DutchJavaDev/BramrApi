using BramrApi.Data;
using BramrApi.Database.Data;
using BramrApi.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("get/{location}")]
        [Authorize]
        public async Task<HistoryModel> GetEdit(int location)
        {
            var user = await userManager.FindByIdAsync(GetIdentity());
            return Database.GetHistoryModel(user.UserName, location);
        }

        [HttpPost("post")]
        [Authorize]
        public async Task<ApiResponse> PostEdit([FromBody] HistoryModel CurrentEdit)
        {
            var user = await userManager.FindByIdAsync(GetIdentity());

            if (user == null)
            {
                return ApiResponse.Error("User not found!");
            }

            await Database.AddOrUpdateModel(new HistoryModel
            {
                UserName = user.UserName,
                Location = CurrentEdit.Location,
                DesignElement = CurrentEdit.DesignElement,
                EditType = CurrentEdit.EditType,
                Edit = CurrentEdit.Edit
            });

            return ApiResponse.Oke("Change added to database");
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<ApiResponse> DeleteAllEdits()
        {
            var user = await userManager.FindByIdAsync(GetIdentity());

            if (user == null)
            {
                return ApiResponse.Error("User not found!");
            }

            await Database.DeleteAllHistoryModelsByUsername(user.UserName);

            return ApiResponse.Oke("History succesfully deleted");
        }

        [HttpDelete("deletefrom/{location}")]
        [Authorize]
        public async Task<ApiResponse> DeleteAllEditsFrom(int location)
        {
            var user = await userManager.FindByIdAsync(GetIdentity());

            if (user == null)
            {
                return ApiResponse.Error("User not found!");
            }

            await Database.DeleteAllHistoryModelsFromLocationByUsername(user.UserName, location);

            return ApiResponse.Oke("History succesfully deleted");
        }

        [NonAction]
        private string GetIdentity()
        {
            return User.Claims.Where(i => i.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
        }
    }
}
