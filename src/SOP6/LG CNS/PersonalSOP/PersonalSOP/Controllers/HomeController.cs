using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSOP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string id)
        {
            //ViewBag.Title = "Home Page";
            
            return View();
        }
        
        public ActionResult Find(string param1, string param2)
        {
            string result = "param1 :- " + param1 + " param2 :- " + param2;

            return Content(result);
        }
    }
}
