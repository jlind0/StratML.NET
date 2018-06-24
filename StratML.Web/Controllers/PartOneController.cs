using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StratML.Web.Models;

namespace StratML.Web.Controllers
{
    public class PartOneController : Controller
    {
        protected AzureSearchUri SearchUri { get; private set; }
        public PartOneController(AzureSearchUri uri)
        {
            this.SearchUri = uri;
        }
        public IActionResult Index()
        {
            return View(this.SearchUri);
        }
    }
}