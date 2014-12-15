using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;

namespace InTime.Controllers
{
    public class CalendrierController : Controller
    {
        public ActionResult Index(int annee = 0, int mois = 0, int jour = 0)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    ViewBag.annee = annee;
                    ViewBag.mois = mois;
                    ViewBag.jour = jour;

                    return View();
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
            }
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

        public JsonResult Taches(double start, double end)
        {
            var lstTache = new List<Tache>();

            try
            {
                const string queryString = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut>=@DateDebut AND DateFin<=@DateFin) OR Recurrence > 0);";
                List<SqlParameter> param = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", Cookie.ObtenirCookie(User.Identity.Name)),
                        new SqlParameter("@DateDebut", start),
                        new SqlParameter("@DateFin", end)
                    };

                SqlDataReader reader = RequeteSql.Select(queryString, param);
                while (reader.Read())
                {
                    Object[] values = new Object[reader.FieldCount];
                    reader.GetValues(values);
                    var tache = ObtenirTache(values);
                    lstTache.Add(tache);
                }
                reader.Close();
            }
            catch
            {
                return Json(null);
            }

            var rows = new List<object>();
            UrlHelper urlH = new UrlHelper(ControllerContext.RequestContext);

            foreach (Tache tache in lstTache)
            {
                if (tache.Recurrence != (int)TraitementDate.recurrence.Aucune)
                {
                    List<string[]> result = TraitementDate.TraitementRecurrence(tache, start, end);
                    if (result != null)
                    {
                        foreach (string[] str in result)
                        {
                            string url = urlH.Action("Index", "ConsulterTache", new { @id = str[3], dep = str[1], fn = str[2] });
                            rows.Add(new { title = str[0], start = str[1], end = str[2], url = url, id = str[3], backgroundColor = tache.PriorityColor });
                        }
                    }
                }
                else
                {
                    string url = urlH.Action("Index", "ConsulterTache", new { @id = tache.IdTache });
                    rows.Add(new { title = tache.NomTache, start = TraitementDate.DateFormatCalendrier(tache.unixDebut),
                        end = TraitementDate.DateFormatCalendrier(tache.unixFin), url = url, backgroundColor = tache.PriorityColor });
                }
            }

            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        private Tache ObtenirTache(Object[] values)
        {
            var tache = new Tache()
            {
                IdTache = Convert.ToInt32(values[Tache.columnIdTache]),
                NomTache = Convert.ToString(values[Tache.columnNomTache]),
                unixDebut = Convert.ToDouble(values[Tache.columnDateDeb]),
                unixFin = Convert.ToDouble(values[Tache.columnDateFin]),
                Recurrence = Convert.ToInt32(values[Tache.columnRec]),
                PriorityColor = Convert.ToString(values[Tache.columnColor])
            };

            return tache;
        }
    }
}
