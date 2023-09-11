using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.DTO;
using ProductManagementAss2.Models.View;

namespace ProductManagementTest
{
    public class ServiceUserAuthenticationTest
    {
        private static Mock<UserManager<IdentityUser>> SetupUserManagerMock()
        {
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
               Mock.Of<IUserStore<IdentityUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<IdentityUser>>(),
                new List<IUserValidator<IdentityUser>>(),
                new List<IPasswordValidator<IdentityUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<IdentityUser>>>()
            );
          
            return userManagerMock;
        }

        private static Mock<SignInManager<IdentityUser>> SetupSignInManagerMock(Mock<UserManager<IdentityUser>> userManagerMock)
        {
            var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var signInManager = new Mock<SignInManager<IdentityUser>>(
                userManagerMock.Object,
                new HttpContextAccessor(),
                userClaimsPrincipalFactory.Object,
                null,
                null,
                null,
                null);

            signInManager.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                         .ReturnsAsync(SignInResult.Success);

            return signInManager;
        }
        private static Mock<RoleManager<IdentityRole>> SetupRoleManagerMock()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleValidators = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };
            var lookupNormalizer = Mock.Of<ILookupNormalizer>(ln => ln.NormalizeName(It.IsAny<string>()) == "normalizedRoleName");

            var roleManager = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                roleValidators,
                lookupNormalizer,
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object
            );

            roleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            return roleManager;
        }




        [Fact]
        public async Task LoginAsync_InvalidUserName_ReturnsInvalidUsernameStatus()
        {
            var userManagerMock = SetupUserManagerMock();
            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync((IdentityUser?)null);

            var signInManager = SetupSignInManagerMock(userManagerMock);
            var roleManager=SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            var loginModel = new LoginModel
            {
                Username = "nonexistentuser",
                Password = "password123",
                rememberme = false
            };

            // Act
            var result = await userAuthentication.LoginAsync(loginModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("Invalid Username", result.Message);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsInvalidPasswordStatus()
        {
            var userManagerMock = SetupUserManagerMock();
            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync(new IdentityUser());

            userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                           .ReturnsAsync(false);
            var signInManager = SetupSignInManagerMock(userManagerMock);
            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            var loginModel = new LoginModel
            {
                Username = "existinguser",
                Password = "invalidpassword",
                rememberme = false
            };
            var result = await userAuthentication.LoginAsync(loginModel);
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("Invalid Password", result.Message);
        }
        [Fact]
        public async Task LoginAsync_SuccessfulLogin_ReturnsLoggedInStatus()
        {
            var userManagerMock = SetupUserManagerMock();

            var user = new IdentityUser
            {
                UserName = "existinguser",
                Email = "existinguser@example.com"
            };

            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync(user);

            userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>()))
                       .ReturnsAsync(true);
            var signInManager = SetupSignInManagerMock(userManagerMock);
            signInManager.Setup(s => s.PasswordSignInAsync(user.UserName, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
             .ReturnsAsync(SignInResult.Success);
            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            var userRoles = new List<string> { "role1", "role2" };
            userManagerMock.Setup(u => u.GetRolesAsync(user))
                       .ReturnsAsync(userRoles);


            var loginModel = new LoginModel
            {
                Username = "existinguser",
                Password = "validpassword",
                rememberme = false
            };

            // Act
            var result = await userAuthentication.LoginAsync(loginModel);

            // Assert
            Assert.Equal(1, result.StatusCode);
            Assert.Equal("Logged in Successfully", result.Message);
        }

        [Fact]
        public async Task LoginAsync_UserLockedOut_ReturnsUserLockedOutStatus()
        {
            var userManagerMock = SetupUserManagerMock();
            var user = new IdentityUser
            {
                UserName = "existinguser",
                Email = "existinguser@example.com"
            };
            var signInManager = SetupSignInManagerMock(userManagerMock);
            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync(user);

            userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>()))
                       .ReturnsAsync(true);

            signInManager.Setup(s => s.PasswordSignInAsync(user.UserName, It.IsAny<string>(), false, false))
                        .ReturnsAsync(SignInResult.LockedOut);
            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            var loginModel = new LoginModel
            {
                Username = "existinguser",
                Password = "validpassword",
                rememberme = false
            };

            // Act
            var result = await userAuthentication.LoginAsync(loginModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("User Locked out", result.Message);
        }
        [Fact]
        public async Task LoginAsync_ErrorInLogin_ReturnsErrorStatus()
        {
            // Arrange
            var userManagerMock = SetupUserManagerMock();

            var user = new IdentityUser
            {
                UserName = "existinguser",
                Email = "existinguser@example.com"
            };

            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync(user);

            userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>()))
                           .ReturnsAsync(true);

            var signInManager = SetupSignInManagerMock(userManagerMock);
            signInManager.Setup(s => s.PasswordSignInAsync(user.UserName, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                         .ReturnsAsync(SignInResult.Failed);

            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);
            var loginModel = new LoginModel
            {
                Username = "existinguser",
                Password = "validpassword",
                rememberme = false
            };

            // Act
            var result = await userAuthentication.LoginAsync(loginModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("Error on logging in", result.Message);
        }
        [Fact]
        public async Task RegisterAsync_UserExistsByEmail_ReturnsUserExistsStatus()
        {
            var userManagerMock = SetupUserManagerMock();
            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                          .ReturnsAsync(new IdentityUser()); 

            var registrationModel = new RegistrationModel
            {
                Email = "existinguser@example.com",
                Username = "newuser",
                Password = "validpassword",
                FirstName = "John",
                LastName = "Doe",
                Role = "UserRole"
            };
            var signInManager = SetupSignInManagerMock(userManagerMock);
            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            // Act
            var result = await userAuthentication.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("User already exist", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_UserExistsByUsername_ReturnsUserExistsStatus()
        {
            var userManagerMock = SetupUserManagerMock();
            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                          .ReturnsAsync((IdentityUser)null);
            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync(new IdentityUser()); // Simulating existing user by username

            var registrationModel = new RegistrationModel
            {
                Email = "newuser@example.com",
                Username = "existinguser",
                Password = "validpassword",
                FirstName = "John",
                LastName = "Doe",
                Role = "UserRole"
            };
            var signInManager = SetupSignInManagerMock(userManagerMock);
            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            // Act
            var result = await userAuthentication.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("User already exist", result.Message);
        }


        [Fact]
        public async Task RegisterAsync_UserCreationFailed_ReturnsUserCreationFailedStatus()
        {
            var userManagerMock = SetupUserManagerMock();
            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                          .ReturnsAsync((IdentityUser)null); // Simulating non-existing user by email
            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                          .ReturnsAsync((IdentityUser)null); // Simulating non-existing user by username
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                          .ReturnsAsync(IdentityResult.Failed()); // Simulating user creation failure

            var registrationModel = new RegistrationModel
            {
                Email = "newuser@example.com",
                Username = "newuser",
                Password = "validpassword",
                FirstName = "John",
                LastName = "Doe",
                Role = "UserRole"
            };
            var signInManager = SetupSignInManagerMock(userManagerMock);
            var roleManager = SetupRoleManagerMock();
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManager.Object);

            // Act
            var result = await userAuthentication.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(0, result.StatusCode);
            Assert.Equal("User creation failed", result.Message);
        }
    
        [Fact]
        public async Task RegisterAsyncRoleExist_SuccessfulRegistration_ReturnsSuccessStatus()
        {
            // Arrange
            var userManagerMock = SetupUserManagerMock();
            var roleManagerMock = SetupRoleManagerMock();

            var registrationModel = new RegistrationModel
            {
                Email = "newuser@example.com",
                Username = "newuser",
                Password = "validpassword",
                FirstName = "John",
                LastName = "Doe",
                Role = "User"
            };

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync((ApplicationUser)null);

            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync((ApplicationUser)null);
            roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                           .ReturnsAsync(IdentityResult.Success);
            var signInManager = SetupSignInManagerMock(userManagerMock);
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManagerMock.Object);

            // Act
            var result = await userAuthentication.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(1, result.StatusCode);
            Assert.Equal("You have registered successfully", result.Message);

            roleManagerMock.Verify(r => r.RoleExistsAsync("User"), Times.Once);
            roleManagerMock.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Never); 
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
        }
        [Fact]
        public async Task RegisterAsyncRoleNotExist_SuccessfulRegistration_ReturnsSuccessStatus()
        {
            // Arrange
            var userManagerMock = SetupUserManagerMock();
            var roleManagerMock = SetupRoleManagerMock();

            var registrationModel = new RegistrationModel
            {
                Email = "newuser@example.com",
                Username = "newuser",
                Password = "validpassword",
                FirstName = "John",
                LastName = "Doe",
                Role = "User"
            };

            userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync((ApplicationUser)null);

            userManagerMock.Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                           .ReturnsAsync((ApplicationUser)null);

            // RoleExistsAsync returns false this time
            roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                           .ReturnsAsync(IdentityResult.Success);

            var signInManager = SetupSignInManagerMock(userManagerMock);
            var userAuthentication = new UserAuthentication(userManagerMock.Object, signInManager.Object, roleManagerMock.Object);

            var result = await userAuthentication.RegisterAsync(registrationModel);

            // Assert
            Assert.Equal(1, result.StatusCode);
            Assert.Equal("You have registered successfully", result.Message);

            roleManagerMock.Verify(r => r.RoleExistsAsync("User"), Times.Once);
            roleManagerMock.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
            userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
        }


    }

}


