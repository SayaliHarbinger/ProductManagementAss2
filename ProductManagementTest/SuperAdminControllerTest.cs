using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagementAss2.Models.View;



namespace ProductManagementTest
{
    public class SuperAdminControllerTest
    {
        private readonly SuperAdminController _controller;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

        public SuperAdminControllerTest()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<IdentityUser>>(),
                new List<IUserValidator<IdentityUser>>(),
                new List<IPasswordValidator<IdentityUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<IdentityUser>>>());

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new IdentityUser
                {
                    Id = "LoggedInUserId",
                    UserName = "LoggedInUser",
                });

            _controller = new SuperAdminController(_userManagerMock.Object);
        }

        [Fact]
        public async Task Index_Role_ReturnsViewWithUsers()
        {
            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserModel>>(result.Model);
            var usersWithRoles = result.Model as List<UserModel>;
            Assert.NotNull(usersWithRoles);
            Assert.Empty(usersWithRoles);
        }
        [Fact]
        public async Task Index_AuthenticatedUser_ReturnsViewWithUsers()
        {
            // Arrange
            var loggedInUser = new ApplicationUser
            {
                Id = "LoggedInUserId",
                UserName = "LoggedInUser",
            };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(loggedInUser);

            var users = new List<ApplicationUser>
            {
              new ApplicationUser
                 {
                  Id = "UserId1",
                UserName = "User1",
            Email = "user1@example.com",
        },
        new ApplicationUser
        {
            Id = "UserId2",
            UserName = "User2",
            Email = "user2@example.com",
        },
    };

            _userManagerMock.Setup(u => u.Users).Returns(users.AsQueryable());

            var controller = new SuperAdminController(_userManagerMock.Object);
            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserModel>>(result.Model);
            var model = result.Model as List<UserModel>;

            Assert.Equal(2, model.Count);
        }
  
        [Fact]
        public async Task Create_ValidUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "email@gmail.com",
                IsUser = true,
                IsAdmin = true
            };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Create(userModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("SuperAdmin", redirectToActionResult.ControllerName);
        }
        [Fact]
        public async Task Create_User_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "email@gmail.com",
                IsUser = true,
                IsAdmin = true
            };

            var existingUser = null as ApplicationUser;
            var existingName = null as ApplicationUser;

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingUser!);
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(existingName!);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Create(userModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("SuperAdmin", redirectToActionResult.ControllerName);
        }
        [Fact]
        public async Task Create_ExistingEmail_ReturnsViewWithError()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "existing@email.com", 
                IsUser = true,
                IsAdmin = true
            };

            var existingUser = new ApplicationUser(); // This user will simulate an existing user with the same email

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

            // Act
            var result = await _controller.Create(userModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(userModel, viewResult.Model); 
            Assert.True(_controller.ModelState.ContainsKey("Email"));
            Assert.Equal("Email address is already in use.", _controller?.ModelState?["Email"].Errors[0].ErrorMessage);
        }
        [Fact]
        public async Task Edit_ValidUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "user@gmail.com",
                IsUser = true,
                IsAdmin = true
            };


            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Edit(userModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("SuperAdmin", redirectToActionResult.ControllerName);
        }
        [Fact]
        public async Task Create_ExistingUsername_ReturnsViewWithError()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "existingusername", // Assuming this username already exists
                Email = "newemail@gmail.com",
                IsUser = true,
                IsAdmin = true
            };

            var existingUser = new ApplicationUser(); // This user will simulate an existing user with the same username

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

            // Act
            var result = await _controller.Create(userModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(userModel, viewResult.Model); // Make sure the same model is passed back to the view
            Assert.True(_controller.ModelState.ContainsKey("UserName")); // Check if the ModelState contains an error for the UserName field
            Assert.Equal("UserName address is already in use.", _controller?.ModelState?["UserName"].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsView()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "email@gmail.com",
                IsUser = true,
                IsAdmin = true
            };

            _controller.ModelState.AddModelError("PropertyName", "Error Message");

            // Act
            var result = await _controller.Create(userModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(userModel, viewResult.Model); 
        }

        [Fact]
        public async Task Create_UserCreationFails_ReturnsViewWithError()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "email@gmail.com",
                IsUser = true,
                IsAdmin = true
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error Message" }));

            // Act
            var result = await _controller.Create(userModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(userModel, viewResult.Model); // Make sure the same model is passed back to the view
            Assert.True(_controller.ModelState.ContainsKey(string.Empty)); // Check if there is a generic error message in the ModelState
            Assert.Equal("Error Message", _controller?.ModelState?[string.Empty].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Edit_Invalid_User_Returns_NotFound()
        {
            // Arrange
            var userModel = new UserModel
            {
                FirstName = "Test",
                LastName = "Case",
                Username = "username",
                Email = "email",
                IsUser = true,
                IsAdmin = true
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Edit(userModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Delete_ValidEmail_ReturnsRedirectToActionResult()
        {
            // Arrange
            var email = "test@example.com";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Delete(email);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("SuperAdmin", redirectToActionResult.ControllerName);
        }
        [Fact]
        public async Task Delete_InvalidUser_ReturnsErrorView()
        {
            // Arrange
            var email = "test@example.com";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed()); 

            // Act
            var result = await _controller.Delete(email);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
        [Fact]
        public async Task Create_InvalidUser_ReturnsViewWithError()
        {
            // Arrange
            var userModel = new UserModel()
            {
                FirstName = "Test",
                LastName = "Case",
                Email = "email",
                IsUser = true,
                IsAdmin = true,
            };

            // Act
            _controller.ModelState.AddModelError("Key", "Error message");
            var result = await _controller.Create(userModel);

            // Assert
            Assert.IsType<ViewResult>(result);
        }



        [Fact]
        public async Task Delete_InvalidEmail_ReturnsNotFound()
        {
            // Arrange
            var email = "invalid@example.com";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Delete(email);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Should return a NotFoundResult
        }

        [Fact]
        public async Task Edit_ValidEmail_ReturnsViewResult()
        {
            // Arrange

            var controller = new SuperAdminController(_userManagerMock.Object);

            var email = "test@example.com";
            _userManagerMock.Setup(u => u.FindByEmailAsync(email))
                .ReturnsAsync(new ApplicationUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = email,
                    UserName = "johndoe",
                });

            // Act
            var result = await controller.Edit(email) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserModel>(result.Model);
        }

        [Fact]
        public async Task Edit_UserNotFound_ReturnsRedirectToActionResult()
        {
            // Arrange

            var controller = new SuperAdminController(_userManagerMock.Object);

            var email = "nonexistent@example.com";
            _userManagerMock.Setup(u => u.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await controller.Edit(email) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
        [Fact]
        public async Task Edit_ValidUser_NotApplicationUser_ReturnsViewResult()
        {
            // Arrange
            var controller = new SuperAdminController(_userManagerMock.Object);
            var email = "test@example.com";

            _userManagerMock.Setup(u => u.FindByEmailAsync(email))
                .ReturnsAsync(new IdentityUser()); // Not an ApplicationUser

            // Act
            var result = await controller.Edit(email) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Model); // Ensure the model is null in this case
        }
        [Fact]
        public async Task Edit_ValidUser_NotInUserRole_ReturnsViewResult()
        {
            var userModelMock = new Mock<UserModel>();
            userModelMock.SetupAllProperties(); // Allow properties to be set
            var email = "user@example.com";
            // Set up properties as needed
            userModelMock.Object.FirstName = "John";
            userModelMock.Object.LastName = "Doe";

            _userManagerMock.Setup(u => u.FindByEmailAsync(email))
                .ReturnsAsync(new ApplicationUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = email,
                    UserName = "johndoe",
                });

            _userManagerMock.Setup(u => u.IsInRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(false); 

            // Act
            var result = await _controller.Edit(email) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(userModelMock.Object.FirstName, ((UserModel)result.Model).FirstName);
            Assert.Equal(userModelMock.Object.LastName, ((UserModel)result.Model).LastName);
        }
        [Fact]
        public async Task Edit_EmailIsNullOrEmpty_ReturnsNotFoundResult()
        {
            var userModelMock = new Mock<UserModel>();
            userModelMock.SetupAllProperties();

            // Act
            var result = await _controller.Edit(new UserModel()) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Arrange
            var controller = new SuperAdminController(_userManagerMock.Object);

            // Act
            var result = controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}
