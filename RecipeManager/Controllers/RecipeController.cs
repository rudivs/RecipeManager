using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RecipeManager.Controllers
{
    public class RecipeController : Controller
    {
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
