using InTime.Models;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class GererController : Controller
    {
        //
        // GET: /Gerer/

        public ActionResult GererForm()
        {
            try
            {
                if (User.Identity.IsAuthenticated && User.Identity.Name == "Superuser")
                {
                    return View();
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
            }
            catch
            {
                return View(UrlErreur.ErreurSourceInconnu);
            }
        }

    }
}
