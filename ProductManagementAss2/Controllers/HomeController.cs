using Microsoft.AspNetCore.Mvc;
using ProductManagementAss2.Models;
using System.Diagnostics;

namespace ProductManagementAss2.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}