using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementAss2.Models.View
{
    public class ApplicationUser:IdentityUser

    {
        [Required]
        public string FirstName { get;  set; }
        [Required]
        public string LastName { get;  set; }        
    }
}
