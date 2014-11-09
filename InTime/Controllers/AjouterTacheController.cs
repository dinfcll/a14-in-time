using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace InTime.Controllers
{
    public class AjouterTacheController : Controller
    {
        int StrToInt(string nombre)
        {
            return Convert.ToInt32(nombre);
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                InitialiseViewBags();

                return View();
            }
            else
            {
                return View(UrlErreur.Authentification);
            }
        }

        public JsonResult JourDuMois(int Year, string Month)
        {
            if (!String.IsNullOrEmpty(Month))
            {
                List<SelectListItem> jours = new List<SelectListItem>();
                if (!String.IsNullOrEmpty(Month))
                {
                    int nDays = DateTime.DaysInMonth(Year, StrToInt(Month));
                    for (int i = 1; i <= nDays; ++i)
                    {
                        jours.Add(new SelectListItem { Text = Convert.ToString(i), Value = Convert.ToString(i) });
                    }
                }

                return Json(new SelectList(jours.ToArray(), "Text", "Value"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult Index(Tache model)
        {
            if (User.Identity.IsAuthenticated)
            {
                Validations(model);

                if (!ModelState.IsValid)
                {
                    InitialiseViewBags();

                    return View("Index");
                }
                else
                {
                    var message = InsertionTache(model) ? "Reussi" : "Erreur";
                    TempData["Message"] = message;

                    return RedirectToAction("Index", "AjouterTache");
                }
            }
            else
            {
                return View(UrlErreur.Authentification);
            }
        }

        private void Validations(Tache model)
        {
            const string strValidationMotContain = "Choisir";

            if ((model.Mois == null || model.Mois.Contains(strValidationMotContain)) ||
                (model.Annee == null) ||
                (model.Jour == null || model.Jour.Contains(strValidationMotContain)))
            {
                ModelState.AddModelError("Mois", "Veuillez compléter la date correctement.");
                ModelState.AddModelError("Annee", "");
                ModelState.AddModelError("Jour", "");
            }

            if (model.HDebut == null || model.mDebut == null)
            {
                ModelState.AddModelError("dbTacheHeure", "Veuillez compléter l'heure de début correctement.");
                ModelState.AddModelError("dbTacheMinute", "");
            }

            if (model.HFin == null || model.mFin == null)
            {
                ModelState.AddModelError("finTacheHeure", "Veuillez compléter l'heure de fin correctement.");
                ModelState.AddModelError("finTacheMinute", "");
            }
            else
            {
                ValHeureFinDebut(ref model);
            }

            if (model.HRappel != null && model.mRappel == null)
            {
                ModelState.AddModelError("rapTacheHeure", "Veuillez compléter l'heure de rappel correctement.");
                ModelState.AddModelError("rapTacheMinute", "");
            }
        }

        private void ValHeureFinDebut(ref Tache model)
        {
            const string strMessageErreur = "Vos heures ne sont pas valide";

            if (model.HDebut != null && model.mDebut != null)
            {
                if (StrToInt(model.HDebut) > StrToInt(model.HFin))
                {
                    ModelState.AddModelError("", strMessageErreur);
                }
                else
                {
                    if (StrToInt(model.HDebut) == StrToInt(model.HFin) &&
                        StrToInt(model.mDebut) >= StrToInt(model.mFin))
                    {
                        ModelState.AddModelError("", strMessageErreur);
                    }
                }
            }
        }

        private bool InsertionTache(Tache Model)
        {
            try
            {
                int UserId = Int32.Parse(InTime.Models.Cookie.ObtenirCookie(User.Identity.Name));
                double unixDebut = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateDebut(Model));
                double unixFin = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateFin(Model));
                string SqlInsert = "INSERT INTO Taches (UserId,NomTache,Lieu,Description,DateDebut,DateFin,HRappel,mRappel,Reccurence)"
                    + " VALUES (@UserId,@NomTache,@Lieu,@Description,@DateDebut,@DateFin,@HRappel,@mRappel,@Reccurence);";

                List<SqlParameter> listParametres = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", UserId),
                    new SqlParameter("@NomTache", Model.NomTache),
                    new SqlParameter("@Lieu", Model.Lieu),
                    new SqlParameter("@DateDebut",unixDebut),
                    new SqlParameter("@DateFin",unixFin),
                    new SqlParameter("@Description", Model.Description),
                    new SqlParameter("@HRappel", SqlDbType.VarChar) { Value = Model.HRappel ?? (object)DBNull.Value },
                    new SqlParameter("@mRappel", SqlDbType.VarChar) { Value = Model.mRappel ?? (object)DBNull.Value },
                    new SqlParameter("@Reccurence", Model.Reccurence)
                };

                return RequeteSql.ExecuteQuery(SqlInsert, listParametres);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void InitialiseViewBags()
        {
            ViewBag.trancheMin = new SelectList(Tache.tempsMinutes);

            ViewBag.trancheHeure = new SelectList(Tache.tempsHeure);

            ViewBag.MoisAnnee = new SelectList(Tache.les_mois, "Value", "Text");

            ViewBag.Reccurence = new SelectList(Tache.options, "Value","Text");
        }
    }
}
