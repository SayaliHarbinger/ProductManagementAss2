using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Models.Domain;
using ProductManagementAss2.Models.DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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

            _controller = new SuperAdminController(_userManagerMock.Object, null, null);
        }

        [Fact]
        public async Task Index_AuthenticatedUser_ReturnsViewWithUsers()
        {
            

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserModel>>(result.Model);
            var usersWithRoles = result.Model as List<UserModel>;
            Assert.NotNull(usersWithRoles);
            Assert.Equal(0, usersWithRoles.Count);
        }

        [Fact]
        public async Task Create_ValidUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userModel = new UserModel();
            userModel.FirstName= "Test";
            userModel.LastName = "Case";
            userModel.Username = "username";
            userModel.Email = "email";
            userModel.IsUser= true;
            userModel.IsAdmin= true;


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
        public async Task Edit_ValidUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userModel = new UserModel();
            userModel.FirstName = "Test";
            userModel.LastName = "Case";
            userModel.Username = "username";
            userModel.Email = "email";
            userModel.IsUser = true;
            userModel.IsAdmin = true;

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
       
      
            public async Task Create_InvalidUser_ReturnsViewWithError()
            {
                // Arrange
                var userModel = new UserModel();
                userModel.FirstName = "Test";
                userModel.LastName = "Case";
                userModel.Email = "email"; // Invalid email format
                userModel.IsUser = true;
                userModel.IsAdmin = true;

                // Act
                _controller.ModelState.AddModelError("Key", "Error message");
                var result = await _controller.Create(userModel) ;

            // Assert
            Assert.IsType<ViewResult>(result);
        }

            [Fact]
        public async Task Edit_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var userModel = new UserModel();
            userModel.FirstName = "Test";
            userModel.LastName = "Case";
            userModel.Email = "email"; 
            userModel.IsUser = true;
            userModel.IsAdmin = true;

            // Act
            var result = await _controller.Edit(userModel);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Delete_InvalidEmail_ReturnsNotFound()
        {
            // Arrange
            var email = "invalid@example.com"; // An email that doesn't exist

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Delete(email);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Should return a NotFoundResult
        }
    }
}
