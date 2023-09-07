using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ProductManagementTest
{
    public class FakeSignInManager : SignInManager<IdentityUser>
    {
        public FakeSignInManager(UserManager<IdentityUser> userManager)
            : base(userManager,
                  new HttpContextAccessor(),
                  new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
                  new Mock<IAuthenticationSchemeProvider>().Object,  
                  new Mock<IUserConfirmation<IdentityUser>>().Object) 
        { }
    }
}

