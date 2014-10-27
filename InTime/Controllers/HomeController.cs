using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;

namespace InTime.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Soyez toujours à l'heure pour un rendez-vous! ";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(GMail information)
        {
            if (ModelState.IsValid)
            {
                GMail mailer = new GMail(information.Subject, information.Body, true);
                mailer.Send();
                TempData["message"] = "Reussi";
                return RedirectToAction("Contact", "Home");
            }

                return View();
        }
    }
}

