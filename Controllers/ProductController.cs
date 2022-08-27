using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Controllers
{
    public class ProductController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {   
            return View();
        }

        public IActionResult ProductsList()
        {
            return View();
        }
    }
}
