using InTime.Models;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class GererCompteController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
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
