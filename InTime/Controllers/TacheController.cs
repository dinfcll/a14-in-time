using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace InTime.Controllers
{
    public class TacheController : Controller
    {
        //
        // GET: /Tache/

        public ActionResult TacheForm()
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name!= "Superuser")
            {
                ViewBag.User = User.Identity.Name;
                return View();
            }
            else
                if (User.Identity.IsAuthenticated && User.Identity.Name == "Superuser")
                {
                    return RedirectToAction("GererForm", "Gerer");
                }
                else
                {
                    return View("~/Views/ErreurAuthentification.cshtml");
                }

        }

    }
}
