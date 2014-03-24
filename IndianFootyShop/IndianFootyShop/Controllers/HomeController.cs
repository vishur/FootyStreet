using FootyStreet.Business.Administration.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndianFootyShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ProductDetails()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SearchResults()
        {
            ViewBag.Message = "Your contact page.";
            IAdministrative administrativeProcessor = DependencyResolver.Current.GetService<IAdministrative>();
           // administrativeProcessor.insert();
            administrativeProcessor.insert();
            return View();
        }

        public ActionResult OrderDetails()
        {
            ViewBag.Message = "Your Order Details";

            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Message = "Home";

            return View();
        }

    }
}
