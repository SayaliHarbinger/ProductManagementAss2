using Microsoft.AspNetCore.Mvc;
using ProductManagementAss2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
