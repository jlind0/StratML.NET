using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace StratML.Web.Services.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectPermanent("/swagger");
        }
        [HttpGet]
        [Authorize]
        public IActionResult IdentityName()
        {
            return Content(this.User.Identity.Name);
        }
    }
}