using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using ProductManagementAss2.Models.DTO;
using ProductManagementAss2.Models.View;
using System.Security.Claims;
//using System.Web.Mvc;

namespace ProductManagementAss2.Data.Repository
{
   
    public class UserAuthentication : IUserAuthentication
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthentication(UserManager<IdentityUser> userManager,
             SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;

        }
      
        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.Username);
            if(user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid Username";
                return status;

            }
            if(!await userManager.CheckPasswordAsync(user,model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Invalid Password";
                return status;
            }
            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.rememberme, false);
            if(!signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName)
                };

                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role,userRole));
                }
                status.StatusCode = 1;
                status.Message = "Logged in Successfully";
                return status;

            }
            else if(signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User Locked out";
             

            }
            else
            {
                status.StatusCode = 1;
                status.Message = "Error on logging in";
               
            }
            return status;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegisterAsync(RegistrationModel model)
        {
            var status = new Status();
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            var existingName = await userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exist";
                return status;
            }
            if (existingName != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exist";
                return status;
            }
           
            var user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName=model.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));


            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;


          
        }

    }
}
