using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;

namespace InTime.Controllers
{
    public class CalendrierController : Controller
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
                return View(UrlErreur.Authentification);
            }
        }

        public JsonResult Taches(double start, double end)
        {
            var lstTache = new List<Tache>();
            try
            {
                string queryString = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut>=@DateDebut AND DateFin<=@DateFin) OR Recurrence > 0);";
                List<SqlParameter> param = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)),
                        new SqlParameter("@DateDebut", start),
                        new SqlParameter("@DateFin", end)
                    };

                SqlDataReader reader = RequeteSql.Select(queryString,param);
                while (reader.Read())
                {
                    Object[] values = new Object[reader.FieldCount];
                    reader.GetValues(values);
                    var tache = ObtenirTache(values);
                    lstTache.Add(tache);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                return Json(null);
            }

            var rows = new List<object>();
            UrlHelper UrlH = new UrlHelper(this.ControllerContext.RequestContext);

            foreach (Tache tache in lstTache)
            {
                TraitementDate.recurrence recurrence =
                        (TraitementDate.recurrence)Enum.ToObject(typeof(TraitementDate.recurrence), tache.Recurrence);
                if (recurrence != TraitementDate.recurrence.Aucune)
                {
                    List<string[]> result = null;

                    switch (recurrence)
                    {
                        case TraitementDate.recurrence.ChaqueJour:
                            result = TraitementDate.ChaqueJour(tache, end);
                            break;
                        case TraitementDate.recurrence.ChaqueSemaine:
                            result = TraitementDate.ChaqueSemaine(tache, end);
                            break;
                        case TraitementDate.recurrence.DeuxSemaines:
                            result = TraitementDate.DeuxSemaine(tache, end);
                            break;
                        case TraitementDate.recurrence.TroisSemaine:
                            result = TraitementDate.TroisSemaine(tache, end);
                            break;
                        case TraitementDate.recurrence.ChaqueMois:
                            result = TraitementDate.ChaqueMois(tache, end);
                            break;
                        case TraitementDate.recurrence.TroisMois:
                            result = TraitementDate.TroisMois(tache, end);
                            break;
                        case TraitementDate.recurrence.QuatreMois:
                            result = TraitementDate.QuatreMois(tache, end);
                            break;
                        case TraitementDate.recurrence.ChaqueAnnee:
                            result = TraitementDate.ChaqueAnnee(tache, end);
                            break;
                    }

                    if (result != null)
                    {
                        foreach(string[] str in result)
                        {
                            string url = UrlH.Action("Index", "ConsulterTache", new { @id = str[3], dep = str[1], fn = str[2] });
                            rows.Add(new { title = str[0], start = str[1], end = str[2], url = url, id=str[3] });
                        }
                    }
                }
                else
                {
                    string url = UrlH.Action("Index", "ConsulterTache", new { @id = tache.IdTache });
                    rows.Add(new { title = tache.NomTache, start = tache.unixDebut, end = tache.unixFin, url = url });
                }
            }

            return Json(rows, JsonRequestBehavior.AllowGet);
        }


        private Tache ObtenirTache(Object[] values)
        {
            var tache = new Tache()
            {
                IdTache = Convert.ToInt32(values[0]),
                NomTache = Convert.ToString(values[2]),
                unixDebut = Convert.ToDouble(values[5]),
                unixFin = Convert.ToDouble(values[6]),
                Recurrence = Convert.ToInt32(values[9])
            };

            return tache;
        }

    }
}
