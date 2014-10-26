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


        public ActionResult Index(string strMessValidation)
        {

            if (User.Identity.IsAuthenticated)
            {
                var trancheMin = new List<string>();
                string[] tempsMin = { "00", "15", "30", "45" };
                trancheMin.AddRange(tempsMin);
                ViewBag.trancheMin = new SelectList(trancheMin);

                var trancheHeure = new List<string>();
                for (int i = 1; i < 25; ++i)
                {
                    trancheHeure.Add(Convert.ToString(i));
                }
                ViewBag.trancheHeure = new SelectList(trancheHeure);

                ViewBag.MoisAnnee = new SelectList(Les_Mois(), "Value", "Text");

                ViewBag.Message = strMessValidation;
                return View();
            }
            else
            {
                return View("~/Views/ErreurAuthentification.cshtml");
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
                    var trancheMin = new List<string>();
                    string[] tempsMin = { "00", "15", "30", "45" };
                    trancheMin.AddRange(tempsMin);
                    ViewBag.trancheMin = new SelectList(trancheMin);

                    var trancheHeure = new List<string>();
                    for (int i = 0; i < 25; ++i)
                        trancheHeure.Add(Convert.ToString(i));
                    ViewBag.trancheHeure = new SelectList(trancheHeure);

                    ViewBag.MoisAnnee = new SelectList(Les_Mois(), "Value", "Text");

                    return View("Index");
                }
                else
                {
                    InsertionTache(model);
                    var message = "Reussi";
                    return RedirectToAction("Index", "AjouterTache", new { strMessValidation = message });
                }
            }
            else
            {
                return View("~/Views/ErreurAuthentification.cshtml");
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
        private SqlConnection ConnexionBD(SqlConnection con)
        {
            string cs = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
            con = new SqlConnection(cs);
            con.Open();
            return con;
        }
        private int RechercheID(SqlConnection con)
        {
            //Recherche du Id de l'utilisateur connecté
            string SqlrId = string.Format("SELECT * FROM UserProfile where UserName='{0}'", User.Identity.Name);
            SqlCommand cmdId = new SqlCommand(SqlrId, con);
            int id = (Int32)cmdId.ExecuteScalar();
            return id;
        }
        private void InsertionTache(Tache Model)
        {
            SqlConnection con = null;
            try
            {
                con = ConnexionBD(con);
                int id = RechercheID(con);

                //Requête Insert
                string SqlInsert = string.Format("INSERT INTO Taches (UserId,NomTache,Lieu,Description,Mois,Jour,HDebut,HFin,mDebut,mFin,HRappel,mRappel,Annee) VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                    id,Model.NomTache, Model.Lieu, Model.Description, Model.Mois, Model.Jour, Model.HDebut, Model.HFin, Model.mDebut, Model.mFin, Model.HRappel, Model.mRappel, Model.Annee);
                SqlCommand cmd=new SqlCommand(SqlInsert,con);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
            }
        }
    }
}
