using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class GererController : Controller
    {
        public ActionResult GererForm()
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name=="Superuser")
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
