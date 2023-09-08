using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.DTO;


namespace ProductManagementTest
{
    public class AuthenticationServiceTest
    {
       

        [Fact]
        public async Task LogoutAsync_SignOutCalled()
        {
            // Arrange
            var userManager = new Mock<UserManager<IdentityUser>>(
                     Mock.Of<IUserStore<IdentityUser>>(),
                     Mock.Of<IOptions<IdentityOptions>>(),
                     Mock.Of<IPasswordHasher<IdentityUser>>(),
                     new List<IUserValidator<IdentityUser>>(),
                     new List<IPasswordValidator<IdentityUser>>(),
                     Mock.Of<ILookupNormalizer>(),
                     Mock.Of<IdentityErrorDescriber>(),
                     Mock.Of<IServiceProvider>(),
                     Mock.Of<ILogger<UserManager<IdentityUser>>>());


            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManager = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null
            );

            var signInManager = new Mock<FakeSignInManager>(userManager.Object);

            var userAuthentication = new UserAuthentication(userManager.Object, signInManager.Object, roleManager.Object);

            // Act
            await userAuthentication.LogoutAsync();

            // Assert
            signInManager.Verify(s => s.SignOutAsync(), Times.Once);
        }
        [Fact]
        public async Task RegisterAsync_UserDoesNotExist_ReturnsSuccessStatus()
        {
            var userManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<IdentityUser>>(),
                new IUserValidator<IdentityUser>[0],
                new IPasswordValidator<IdentityUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<IdentityUser>>>());

            var roleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                new IRoleValidator<IdentityRole>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                Mock.Of<ILogger<RoleManager<IdentityRole>>>());
            var signInManager = new Mock<FakeSignInManager>(userManager.Object);
            var registrationService = new UserAuthentication(userManager.Object, signInManager.Object, roleManager.Object);

            var registrationModel = new RegistrationModel
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "TestPassword123",
                Role = "UserRole",
                FirstName = "John",
                LastName = "Doe"
            };

            userManager.Setup(u => u.FindByEmailAsync(registrationModel.Email)).ReturnsAsync((string email) => null);
            userManager.Setup(u => u.FindByNameAsync(registrationModel.Username)).ReturnsAsync((string userName) => null);
            userManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), registrationModel.Password)).ReturnsAsync(IdentityResult.Success);
            roleManager.Setup(rm => rm.RoleExistsAsync(registrationModel.Role)).ReturnsAsync(false);
            roleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await registrationService.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(1, result.StatusCode);
            Assert.Equal("You have registered successfully", result.Message);
        }
        [Fact]
        public async Task RegisterAsync_UserExists_ReturnsUserExistsStatus()
        {
            var userManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<IdentityUser>>(),
              
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<IdentityUser>>>());

            var roleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                new IRoleValidator<IdentityRole>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                Mock.Of<ILogger<RoleManager<IdentityRole>>>());

            var signInManager = new Mock<FakeSignInManager>(userManager.Object);
            var registrationService = new UserAuthentication(userManager.Object, signInManager.Object, roleManager.Object);

            var existingUser = new IdentityUser
            {
                UserName = "existinguser",
                Email = "existing@example.com"
            };

            var registrationModel = new RegistrationModel
            {
                Email = existingUser.Email,
                Username = existingUser.UserName,
                Password = "TestPassword123",
                Role = "UserRole",
                FirstName = "John",
                LastName = "Doe"
            };

            userManager.Setup(u => u.FindByEmailAsync(registrationModel.Email)).ReturnsAsync(existingUser);
            userManager.Setup(u => u.FindByNameAsync(registrationModel.Username)).ReturnsAsync(existingUser);

            // Act
            var result = await registrationService.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("User already exist", result.Message);
        }

    }
}
