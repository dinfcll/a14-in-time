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
            UrlHelper UrlH = new UrlHelper(this.ControllerContext.RequestContext);
            foreach (Tache tache in lstTache)
            {

                if (tache.Reccurence != "Aucune")
                {
                    List<string[]> result = null;
                    switch(tache.Reccurence)
                    {
                        case "À chaque jour":
                            result = TraitementDate.ChaqueJour(tache, end);
                            break;
                        case "Chaque semaine":
                            result = TraitementDate.ChaqueSemaine(tache, end);
                            break;
                        case "Aux deux semaines":
                            result = TraitementDate.DeuxSemaine(tache, end);
                            break;
                        case "Aux trois semaines":
                            result = TraitementDate.TroisSemaine(tache, end);
                            break;
                        case "À chaque mois":
                            result = TraitementDate.ChaqueMois(tache, end);
                            break;
                        case "Aux trois mois":
                            result = TraitementDate.TroisMois(tache, end);
                            break;
                        case "Aux quatre mois":
                            result = TraitementDate.QuatreMois(tache, end);
                            break;
                        case "À chaque année":
                            result = TraitementDate.ChaqueAnnee(tache, end);
                            break;
                    }

                    if (result != null)
                    {
                        foreach(string[] str in result)
                        {
                            string urll = UrlH.Action("Index", "ConsulterTache", new { @id = str[3] });
                            rows.Add(new { title = str[0], start = str[1], end = str[2], url = urll, id=str[3] });
                        }
                    }
                }
                else
                {

                    string dateDebut = TraitementDate.DateFormatCalendrier(
                        tache.Annee, tache.Mois, tache.Jour, tache.HDebut, tache.mDebut);
                    string dateFin = TraitementDate.DateFormatCalendrier(
                       tache.Annee, tache.Mois, tache.Jour, tache.HFin, tache.mFin);

                    string urll = UrlH.Action("Index", "ConsulterTache", new { @id = tache.IdTache });

                    rows.Add(new { title = tache.NomTache, start = dateDebut, end = dateFin, url = urll });
                }
            }

            return Json(rows, JsonRequestBehavior.AllowGet);
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
                Annee = Convert.ToString(values[13]),
                Reccurence = Convert.ToString(values[14])
            };

            return tache;
        }

    }
}
