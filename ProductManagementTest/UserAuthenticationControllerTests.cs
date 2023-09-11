
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementAss2.Controllers;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.DTO;

namespace ProductManagementTest
{
    public class UserAuthenticationControllerTests
    {
        private static Mock<IUserAuthentication> CreateMockUserAuthentication()
        {
            return new Mock<IUserAuthentication>();
        }


        [Fact]
        public void TestLoginAction()
        {
            var mockAuthService = CreateMockUserAuthentication();
            var controller = new UserAuthenticationController(mockAuthService.Object);
            var result = controller.Login() as ViewResult;
            Assert.NotNull(result);
        }
        [Fact]
        public void TestRegisterAction()
        {
            var mockAuthService = CreateMockUserAuthentication();
            var controller = new UserAuthenticationController(mockAuthService.Object);
            var result = controller.Registration() as ViewResult;
            Assert.NotNull(result);

        }
        [Fact]
        public async Task Register_InvalidModel_ReturnsView()
        {
            // Arrange
            var mockAuthService = new Mock<IUserAuthentication>();
            var controller = new UserAuthenticationController(mockAuthService.Object);

            // Create an invalid RegistrationModel (e.g., Password and PasswordConfirm don't match)
            var registerModel = new RegistrationModel
            {
                FirstName = "Test",
                LastName = "User",
                Username = "Test@gmail.com",
                Email = "Test@gmail.com",
                Password = "Test#1234",
                PasswordConfirm = "DifferentPassword",
                Role = "user",
            };

            // Manually add an error to ModelState
            controller.ModelState.AddModelError("PasswordConfirm", "Passwords do not match");

            mockAuthService.Setup(service => service.RegisterAsync(It.IsAny<RegistrationModel>()))
                .ReturnsAsync(new Status { StatusCode = 0 });

            // Act
            var result = await controller.Registration(registerModel) as ViewResult;

            // Assert
            Assert.Null(result?.ViewName); // Check that it returns the default view
            Assert.False(controller.ModelState.IsValid); // Check that the ModelState is invalid
            Assert.Contains(controller.ModelState.Values, v => v.Errors.Count > 0); // Check for errors in ModelState
        }
        [Fact]
        public async Task Register_ValidModel_ReturnsView()
        {
            // Arrange
            var mockAuthService = new Mock<IUserAuthentication>();
            var controller = new UserAuthenticationController(mockAuthService.Object);

            // Create a valid RegistrationModel
            var registerModel = new RegistrationModel
            {
                FirstName = "Test",
                LastName = "User",
                Username = "Test@gmail.com",
                Email = "Test@gmail.com",
                Password = "Test#1234",
                PasswordConfirm = "Test#1234",
                Role = "user",
            };

            mockAuthService.Setup(service => service.RegisterAsync(It.IsAny<RegistrationModel>()))
                .ReturnsAsync(new Status { StatusCode = 1 }); // Assuming a successful registration

            var result = await controller.Registration(registerModel) as RedirectToActionResult;

            // Assert
            Assert.Equal("Login", result?.ActionName); 
          
        }




        [Fact]
        public async Task Login_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var mockAuthService = CreateMockUserAuthentication();
            var controller = new UserAuthenticationController(mockAuthService.Object);
            var validModel = new LoginModel()
            {

                Username = "validUsername",
                Password = "validPassword",
            };

            // Act
            mockAuthService.Setup(authService => authService.LoginAsync(validModel))
            .ReturnsAsync(new Status { StatusCode = 1 });

            // Act
            var result = await controller.Login(validModel) as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("/Home/Index", result.Url);
        }
        [Fact]
        public async Task Login_InValidModel_RedirectsToLogin()
        {
            // Arrange
            var mockAuthService = CreateMockUserAuthentication();
            var controller = new UserAuthenticationController(mockAuthService.Object);
            var InvalidModel = new LoginModel()
            {
                Username = "invalidUsername",
                Password = "invalidPassword",
            };
            mockAuthService.Setup(authService => authService.LoginAsync(InvalidModel))
            .ReturnsAsync(new Status { StatusCode = 0 });

            // Act
            var result = await controller.Login(InvalidModel) as RedirectToActionResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }
        //Logout
        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            // Arrange
            var mockAccountService = new Mock<IUserAuthentication>();
            var controller = new UserAuthenticationController(mockAccountService.Object);
            var result = await controller.Logout() as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
           
        }
    }
}

