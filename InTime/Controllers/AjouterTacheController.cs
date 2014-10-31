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

        public List<SelectListItem> Les_Mois()
        {
            List<SelectListItem> mois = new List<SelectListItem>();
            mois.Add(new SelectListItem { Text = "Janvier", Value = "01" });
            mois.Add(new SelectListItem { Text = "Février", Value = "02" });
            mois.Add(new SelectListItem { Text = "Mars", Value = "03" });
            mois.Add(new SelectListItem { Text = "Avril", Value = "04" });
            mois.Add(new SelectListItem { Text = "Mai", Value = "05" });
            mois.Add(new SelectListItem { Text = "Juin", Value = "06" });
            mois.Add(new SelectListItem { Text = "Juillet", Value = "07" });
            mois.Add(new SelectListItem { Text = "Aout", Value = "08" });
            mois.Add(new SelectListItem { Text = "Septembre", Value = "09" });
            mois.Add(new SelectListItem { Text = "Octobre", Value = "10" });
            mois.Add(new SelectListItem { Text = "Novembre", Value = "11" });
            mois.Add(new SelectListItem { Text = "Décembre", Value = "12" });

            return mois;
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
                verificationJour(ref Model);
                string SqlInsert = "INSERT INTO Taches (UserId,NomTache,Lieu,Description,Mois,Jour,HDebut,HFin,mDebut,mFin,HRappel,mRappel,Annee,Reccurence)"
                    + " VALUES (@UserId,@NomTache,@Lieu,@Description,@Mois,@Jour,@HDebut,@HFin,@mDebut,@mFin,@HRappel,@mRappel,@Annee,@Reccurence);";

                List<SqlParameter> listParametre = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", UserId),
                    new SqlParameter("@NomTache", Model.NomTache),
                    new SqlParameter("@Lieu", Model.Lieu),
                    new SqlParameter("@Description", Model.Description),
                    new SqlParameter("@Mois", Model.Mois),
                    new SqlParameter("@Jour", Model.Jour),
                    new SqlParameter("@HDebut", Model.HDebut),
                    new SqlParameter("@HFin", Model.HFin),
                    new SqlParameter("@mDebut", Model.mDebut),
                    new SqlParameter("@mFin", Model.mFin),
                    new SqlParameter("@HRappel", SqlDbType.VarChar) { Value = Model.HRappel ?? (object)DBNull.Value },
                    new SqlParameter("@mRappel", SqlDbType.VarChar) { Value = Model.mRappel ?? (object)DBNull.Value },
                    new SqlParameter("@Annee", Model.Annee),
                    new SqlParameter("@Reccurence", Model.Reccurence)
                };

                return RequeteSql.ExecuteQuery(SqlInsert, listParametre);
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

            ViewBag.MoisAnnee = new SelectList(Les_Mois(), "Value", "Text");

            ViewBag.Reccurence = new SelectList(Tache.options);
        }

        private void verificationJour(ref Tache model)
        {
            if (Convert.ToInt32(model.Jour) < 10)
            {
                model.Jour = "0" + model.Jour;
            }
        }
    }
}
