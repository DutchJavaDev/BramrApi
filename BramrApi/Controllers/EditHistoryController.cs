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
        private readonly UserManager<IdentityUser> UserManager;

        public EditHistoryController(UserManager<IdentityUser> UserManager, IDatabase Database)
        {
            this.UserManager = UserManager;
            this.Database = Database;
        }

        /// <summary>
        /// Haalt het HistoryModel op met de meegestuurde locatie.
        /// </summary>
        /// <param name="location">Locatie in de editgeschiedenis van de gebruiker</param>
        /// <returns>Stuurt het opgehaalde HistoryModel terug.</returns>
        [HttpGet("get/{location}")]
        [Authorize]
        public async Task<HistoryModel> GetEdit(int location)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());
            return Database.GetHistoryModel(user.UserName, location);
        }

        /// <summary>
        /// Als iets geëdit wordt wordt die edit opgestuurd naar de api in de vorm van een historymodel die dan weer wordt opgeslagen in de database.
        /// </summary>
        /// <param name="CurrentEdit">De edit die zojuist heeft plaatsgevonden en dus opgestuurd werdt naar de api</param>
        /// <returns>Stuurt een ApiResponse terug zodat na het process de client kant ook weer hoe het process verlopen is.</returns>
        [HttpPost("post")]
        [Authorize]
        public async Task<ApiResponse> PostEdit([FromBody] HistoryModel CurrentEdit)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());

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

        /// <summary>
        /// Verwijderd alle edits van een gebruiker die in de database staan.
        /// </summary>
        /// <returns>Stuurt een ApiResponse terug zodat na het process de client kant ook weer hoe het process verlopen is.</returns>
        [HttpDelete("delete")]
        [Authorize]
        public async Task<ApiResponse> DeleteAllEdits()
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());

            if (user == null)
            {
                return ApiResponse.Error("User not found!");
            }

            await Database.DeleteAllHistoryModelsByUsername(user.UserName);

            return ApiResponse.Oke("History succesfully deleted");
        }

        /// <summary>
        /// Verwijderd alle edits van een gebruiker vanaf een bepaald punt die in de database staan, omdat de gebruiker een aantal edit heeft teruggedraaid en daarna weer iets heeft aangepast.
        /// </summary>
        /// <returns>Stuurt een ApiResponse terug zodat na het process de client kant ook weer hoe het process verlopen is.</returns>
        [HttpDelete("deletefrom/{location}")]
        [Authorize]
        public async Task<ApiResponse> DeleteAllEditsFrom(int location)
        {
            var user = await UserManager.FindByIdAsync(GetIdentity());

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
