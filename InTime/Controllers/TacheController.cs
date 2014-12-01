using System.Web.Mvc;
using InTime.Models;


namespace InTime.Controllers
{
    public class TacheController : Controller
    {
        //
        // GET: /Tache/

        public ActionResult TacheForm()
        {
            try
            {
                if (User.Identity.IsAuthenticated && User.Identity.Name != "Superuser")
                {
                    return View();
                }
                else
                    if (User.Identity.IsAuthenticated && User.Identity.Name == "Superuser")
                    {
                        return RedirectToAction("GererForm", "Gerer");
                    }
                    else
                    {
                        return View(UrlErreur.Authentification);
                    }
            }
            catch
            {
                return View(UrlErreur.Authentification);
            }
        }

    }
}
