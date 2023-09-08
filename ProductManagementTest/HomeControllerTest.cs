using Microsoft.AspNetCore.Mvc;
using ProductManagementAss2.Controllers;


namespace ProductManagementTest
{
    public class HomeControllerTest
    {
        [Fact]
        public void TestHome()
        {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.NotNull(result);
        }
    }
}
