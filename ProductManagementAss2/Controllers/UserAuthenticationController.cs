using Microsoft.AspNetCore.Mvc;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;

namespace ProductManagementAss2.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthentication _authservice;
        public UserAuthenticationController(IUserAuthentication authservice)
        {
           _authservice = authservice;
        }

        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (ModelState.IsValid) {
                
                model.Role = "user";
               await _authservice.RegisterAsync(model);
              
                return RedirectToAction(nameof(Login)); 
            }
            return View(model);

        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login(LoginModel model)
        {
            if(!ModelState.IsValid) { 
            
            return View(model);
            }
            var result= await _authservice.LoginAsync(model);
            if (result.StatusCode == 1)
            {
                return Redirect("/Home/Index");
            }
            else
            {
              
                return RedirectToAction(nameof(Login));
            }
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authservice.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}
