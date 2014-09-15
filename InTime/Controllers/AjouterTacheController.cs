using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;

namespace InTime.Controllers
{
    public class AjouterTacheController : Controller
    {
        public List<SelectListItem> Les_Mois ()
        {
            List<SelectListItem> mois = new List<SelectListItem>();
            mois.Add(new SelectListItem { Text = "Janvier", Value = "1" });
            mois.Add(new SelectListItem { Text = "Février", Value = "2" });
            mois.Add(new SelectListItem { Text = "Mars", Value = "3" });
            mois.Add(new SelectListItem { Text = "Avril", Value = "4" });
            mois.Add(new SelectListItem { Text = "Mai", Value = "5" });
            mois.Add(new SelectListItem { Text = "Juin", Value = "6" });
            mois.Add(new SelectListItem { Text = "Juillet", Value = "7" });
            mois.Add(new SelectListItem { Text = "Aout", Value = "8" });
            mois.Add(new SelectListItem { Text = "Septembre", Value = "9" });
            mois.Add(new SelectListItem { Text = "Octobre", Value = "10" });
            mois.Add(new SelectListItem { Text = "Novembre", Value = "11" });
            mois.Add(new SelectListItem { Text = "Décembre", Value = "12" });
            return mois;
        }


        public ActionResult Index(string Min,string Heure)
        {
            var trancheMin = new List<string>();
            string[] tempsMin = { "0", "15", "30", "45", "60" };
            trancheMin.AddRange(tempsMin);
            ViewBag.trancheMin = new SelectList(trancheMin);

            var trancheHeure = new List<string>();
            for (int i = 1; i < 25; ++i)
                trancheHeure.Add(Convert.ToString(i));
            ViewBag.trancheHeure = new SelectList(trancheHeure);


            ViewBag.MoisAnnee = new SelectList(Les_Mois(), "Value", "Text");

            return View();
        }


        //Retourne une liste contenant des annees.
        public JsonResult AnneeList(string Id)
        {
            List<SelectListItem> annee = new List<SelectListItem>();
            if (!String.IsNullOrEmpty(Id))
            {
                int nYear = DateTime.Now.Year;
                for (int i = nYear; i <= nYear + 2; ++i)
                {
                    //annee.Add(new District { StateName = "Bihar", DistrictName = Convert.ToString(i)});
                    annee.Add(new SelectListItem { Text = Convert.ToString(i), Value = Convert.ToString(i) });
                }
            }


            return Json(new SelectList(annee.ToArray(), "Text", "Value"), JsonRequestBehavior.AllowGet);
        }


        //Retourne une liste contenant les jours du mois et de l'annee selectionne
        public JsonResult JourList(string Year, string Month)
        {
            List<SelectListItem> jours = new List<SelectListItem>();
            if (!String.IsNullOrEmpty(Month))
            {
                int nDays = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month));
                for (int i = 1; i <= nDays; ++i)
                {
                    //annee.Add(new District { StateName = "Bihar", DistrictName = Convert.ToString(i)});
                    jours.Add(new SelectListItem { Text = Convert.ToString(i), Value = Convert.ToString(i) });
                }
            }

            return Json(new SelectList(jours.ToArray(), "Text", "Value"), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Index(AjoutTache model)
        {

            //Validations
            if ((model.m_mois == null || model.m_mois.Contains("Choisir"))||
                (model.m_annee == null || model.m_annee.Contains("Choisir")) ||
                (model.m_jour == null || model.m_jour.Contains("Choisir")))
            {
                ModelState.AddModelError("Mois", "Veuillez completer la date correctement.");
                ModelState.AddModelError("Annee", "");
                ModelState.AddModelError("Jour", "");
            }

            if (model.m_debHeure == null || model.m_debMin == null)
            {
                ModelState.AddModelError("dbTacheHeure", "Veuillez completer l'heure de début correctement.");
                ModelState.AddModelError("dbTacheMinute", "");
            }

            if (model.m_finHeure == null || model.m_finMin == null)
            {
                ModelState.AddModelError("finTacheHeure", "Veuillez completer l'heure de fin correctement.");
                ModelState.AddModelError("finTacheMinute", "");
            }

            if (model.m_rappelHeure != null && model.m_rappelMin == null)
            {
                ModelState.AddModelError("rapTacheHeure", "Veuillez completer l'heure de rappel correctement.");
                ModelState.AddModelError("rapTacheMinute", "");
            }


            //Traitement si le formulaire n'est pas bien rempli.
            if (!ModelState.IsValid)
            {
                var trancheMin = new List<string>();
                string[] tempsMin = { "0", "15", "30", "45", "60" };
                trancheMin.AddRange(tempsMin);
                ViewBag.trancheMin = new SelectList(trancheMin);

                var trancheHeure = new List<string>();
                for (int i = 0; i < 25; ++i)
                    trancheHeure.Add(Convert.ToString(i));
                ViewBag.trancheHeure = new SelectList(trancheHeure);

                ViewBag.MoisAnnee = new SelectList(Les_Mois(), "Value", "Text");

                return View("Index");
            }

            return RedirectToAction("Index", "AjouterTache");
        }


        //Action qui se produit suite \ l'appuie du bouton enregistrer
        private void InsertionTache(string m_strNomTache,string m_strLieu,string m_strDescTache,DateTime m_dtHeure,DateTime m_dtRappel)
        {

        }

    }
}
