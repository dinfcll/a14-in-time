using InTime.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class SupprimerTacheController : Controller
    {
        public ActionResult Index(int? id)
        {
            try
            {
                if (id == null)
                {
                    return View(UrlErreur.ErreurGeneral);
                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        try
                        {
                            const string sqlDelete = "DELETE FROM Taches WHERE UserId=@UserId AND IdTache=@IdTache";
                            List<SqlParameter> parametres = new List<SqlParameter>
                            {
                                new SqlParameter("@UserId",Cookie.ObtenirCookie(User.Identity.Name)),
                                new SqlParameter("@IdTache",id)
                            };

                            if (RequeteSql.ExecuteQuery(sqlDelete, parametres))
                            {
                                TempData["Suppression"] = Messages.RequeteSql.Reussi;
                            }
                            else
                            {
                                TempData["Suppression"] = Messages.RequeteSql.Echec;
                            }                       
                        }
                        catch
                        {
                            TempData["Suppression"] = Messages.RequeteSql.Echec;
                        }

                        return RedirectToAction("Taches", "ConsulterTache");
                    }
                    else
                    {
                        return View(UrlErreur.Authentification);
                    }
                }
            }
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

    }
}
