using System.ComponentModel.DataAnnotations;

namespace ProductManagementAss2.Models.DTO
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public bool rememberme { get; set; }
    }
}
