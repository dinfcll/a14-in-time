using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;
using System.Data;
using System.Data.SqlClient;

namespace InTime.Controllers
{
    public class CalendrierController : Controller
    {
        public ActionResult Index()
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

        public JsonResult Taches(double start, double end)
        {
            var lstTache = new List<Tache>();
            try
            {
                string queryString = "SELECT * FROM Taches where UserId=@Id;";

                List<SqlParameter> param = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", InTime.Models.Cookie.ObtenirCookie(User.Identity.Name))
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
            foreach (Tache tache in lstTache)
            {
                string dateDebut = String.Format("{0}-{1}-{2}T{3}:{4}:00-05:00",
                    tache.Annee, tache.Mois, tache.Jour, tache.HDebut, tache.mDebut);
                string dateFin = String.Format("{0}-{1}-{2}T{3}:{4}:00-05:00",
                   tache.Annee, tache.Mois, tache.Jour, tache.HFin, tache.mFin);

                UrlHelper UrlH = new UrlHelper(this.ControllerContext.RequestContext);
                string urll = UrlH.Action("Index", "ConsulterTache", new { @id = tache.IdTache });
                rows.Add(new { title = tache.NomTache, start = dateDebut, end = dateFin, url = urll });
            }
            var test = rows.ToArray();

            return Json(test, JsonRequestBehavior.AllowGet);
        }


        private Tache ObtenirTache(Object[] values)
        {
            var tache = new Tache()
            {
                IdTache = Convert.ToInt32(values[0]),
                NomTache = RequeteSql.RemettreApostrophe(Convert.ToString(values[2])),
                Mois = Convert.ToString(values[5]),
                Jour = Convert.ToString(values[6]),
                HDebut = Convert.ToString(values[7]),
                HFin = Convert.ToString(values[8]),
                mDebut = Convert.ToString(values[9]),
                mFin = Convert.ToString(values[10]),
                Annee = Convert.ToString(values[13])
            };

            return tache;
        }

    }
}
