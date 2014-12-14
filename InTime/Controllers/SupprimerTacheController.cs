using InTime.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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
                            string SqlDelete = "DELETE FROM Taches WHERE UserId=@UserId AND IdTache=@IdTache";
                            List<SqlParameter> Parametres = new List<SqlParameter>
                            {
                                new SqlParameter("@UserId",InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)),
                                new SqlParameter("@IdTache",id)
                            };

                            if (RequeteSql.ExecuteQuery(SqlDelete, Parametres))
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
