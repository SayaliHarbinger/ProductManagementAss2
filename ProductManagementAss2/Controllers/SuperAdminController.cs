using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAss2.Models.DTO;
using ProductManagementAss2.Models.View;



namespace ProductManagementAss2.Controllers
{
    public class SuperAdminController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        public SuperAdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = new List<UserModel>();
            var loggedInUser = await _userManager.GetUserAsync(User);
            foreach (var user in _userManager.Users)
            {
                if (user is ApplicationUser applicationUser && user.Id != loggedInUser.Id)
                {
                    var userViewModel = new UserModel
                    {
                        FirstName = applicationUser.FirstName,
                        LastName = applicationUser.LastName,
                        Email = applicationUser.Email,
                        Username = applicationUser.UserName,
                        IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
                        IsUser = await _userManager.IsInRoleAsync(user, "User")
                    };

                    usersWithRoles.Add(userViewModel);
                }
            }
            return View(usersWithRoles);
        }



        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                var existingName = await _userManager.FindByNameAsync(user.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email address is already in use.");
                    return View(user);
                }
                if (existingName != null)
                {
                    ModelState.AddModelError("UserName", "UserName address is already in use.");
                    return View(user);
                }
                var identityUser = new ApplicationUser
                {
                    Email = user.Email,
                    UserName = user.Username,
                    FirstName = user?.FirstName!,
                    LastName = user?.LastName!,
                    EmailConfirmed = true,
                };

                IdentityResult result = await _userManager.CreateAsync(identityUser, user?.Password);

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(identityUser, "User");


                    if (user!.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(identityUser, "Admin");
                    }

                    return RedirectToAction("Index", "SuperAdmin");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(user);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string email)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return View("Error");
            }

            return RedirectToAction("Index", "SuperAdmin");
        }
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Edit(string email)
        {

            if (email == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                UserModel userViewModel = null!;
                if (user is ApplicationUser applicationUser)
                {
                    userViewModel = new UserModel
                    {
                        FirstName = applicationUser.FirstName,
                        LastName = applicationUser.LastName,
                        Email = applicationUser.Email,
                        Username = applicationUser.UserName,
                        IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
                        IsUser = await _userManager.IsInRoleAsync(user, "User")
                    };

                }
                return View(userViewModel);
            }
            return RedirectToAction("Index");


        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> Edit(UserModel userViewModel)
        {
            if (userViewModel == null || string.IsNullOrEmpty(userViewModel.Email))
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(userViewModel.Email);
            if (user == null)
            {
                return NotFound();
            }

            if (user is ApplicationUser applicationUser)
            {
                applicationUser.FirstName = userViewModel?.FirstName!;
                applicationUser.LastName = userViewModel?.LastName!;
                applicationUser.UserName = userViewModel?.Username;
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");


            if (userViewModel.IsAdmin && !isAdmin)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else if (!userViewModel.IsAdmin && isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "SuperAdmin");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Index", "SuperAdmin");
        }


    }
}
