using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class CalendrierController : Controller
    {
        //
        // GET: /Calendrier/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return View("~/Views/ErreurAuthentification.cshtml");
            }
        }

    }
}
