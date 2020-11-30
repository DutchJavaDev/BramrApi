using System.ComponentModel.DataAnnotations;

namespace BramrApi.Data
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
