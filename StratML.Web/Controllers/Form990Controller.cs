using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StratML.Web.Controllers
{
    public class Form990Controller : Controller
    {
        protected Uri ServicesURI { get; private set; }
        public Form990Controller(Uri servicesUri)
        {
            this.ServicesURI = servicesUri;
        }
        public IActionResult Index()
        {
            return View(this.ServicesURI);
        }
    }
}