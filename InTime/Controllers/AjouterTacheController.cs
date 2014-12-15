using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;
using System.Data;


namespace InTime.Controllers
{
    public class AjouterTacheController : Controller
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
                return View(UrlErreur.ErreurGeneral);
            }
        }

        [HttpPost]
        public ActionResult Index(Tache model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    Validations(model);

                    if (!ModelState.IsValid)
                    {
                        InitialiseDates(ref model);

                        return View("Index");
                    }
                    else
                    {
                        var message = InsertionTache(model) ? Messages.RequeteSql.Reussi : Messages.RequeteSql.Echec;
                        TempData["Message"] = message;

                        return RedirectToAction("Index", "AjouterTache");
                    }
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
            else
            {
                if (new DateTime(StrToInt(model.Annee), StrToInt(model.Mois), StrToInt(model.Jour)) < DateTime.Now.AddDays(-1))
                {
                    ModelState.AddModelError("Mois", "Vous ne pouvez pas créer une tâche avec une date inférieure à la date actuelle.");
                    ModelState.AddModelError("Annee", "");
                    ModelState.AddModelError("Jour", "");
                }
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
                int userId = Int32.Parse(Cookie.ObtenirCookie(User.Identity.Name));
                double unixDebut = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateDebut(Model));
                double unixFin = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateFin(Model));
                const string sqlInsert = "INSERT INTO Taches (UserId,NomTache,Lieu,Description,DateDebut,DateFin,HRappel,mRappel,recurrence,PriorityColor)"
                    + " VALUES (@UserId,@NomTache,@Lieu,@Description,@DateDebut,@DateFin,@HRappel,@mRappel,@recurrence,@PriorityColor);";


                List<SqlParameter> listParametres = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@NomTache", Model.NomTache),
                    new SqlParameter("@Lieu", Model.Lieu),
                    new SqlParameter("@DateDebut",unixDebut),
                    new SqlParameter("@DateFin",unixFin),
                    new SqlParameter("@Description", SqlDbType.VarChar) { Value = Model.Description ?? ""},
                    new SqlParameter("@HRappel", SqlDbType.VarChar) { Value = Model.HRappel ?? (object)DBNull.Value },
                    new SqlParameter("@mRappel", SqlDbType.VarChar) { Value = Model.mRappel ?? (object)DBNull.Value },
                    new SqlParameter("@recurrence", Model.Recurrence),
                    new SqlParameter("@PriorityColor", Model.PriorityColor)
                };

                return RequeteSql.ExecuteQuery(sqlInsert, listParametres);
            }
            catch
            {
                return false;
            }
        }

        private void InitialiseDates(ref Tache nouvTache)
        {
            if (nouvTache.Annee != null &&
                StrToInt(nouvTache.Annee) >= ValeursSpinner.ValeurMinimal &&
                StrToInt(nouvTache.Annee) <= ValeursSpinner.ValeurMaximal)
            {
                ViewBag.Annee = nouvTache.Annee;
            }

            if (nouvTache.Mois != null)
            {
                ViewBag.Mois = nouvTache.Mois;
            }

            if (nouvTache.Annee != null)
            {
                ViewBag.Jour = nouvTache.Jour;
            }
        }

        int StrToInt(string nombre)
        {
            return Convert.ToInt32(nombre);
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
    }
}
