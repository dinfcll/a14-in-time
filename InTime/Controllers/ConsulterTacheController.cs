﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InTime.Models;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;

namespace InTime.Controllers
{
    public class ConsulterTacheController : Controller
    {
        CultureInfo culture = new CultureInfo("fr-CA");

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


        public ActionResult Taches(string strMessValidation)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var lstTache = new List<Tache>();
                    string queryString = "SELECT * FROM Taches where UserId=@Id";
                    List<SqlParameter> Parametres = new List<SqlParameter>
                    {
                        new SqlParameter("@Id",InTime.Models.Cookie.ObtenirCookie(User.Identity.Name))
                    };

                    SqlDataReader reader = RequeteSql.Select(queryString,Parametres);
                    while (reader.Read())
                    {
                        Object[] values = new Object[reader.FieldCount];
                        reader.GetValues(values);
                        var tache = ObtenirTache(values);
                        lstTache.Add(tache);
                    }
                    reader.Close();
                    ViewBag.Taches = lstTache;

                    return View();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
            }
            else
            {
                return View(UrlErreur.Authentification);
            }
        }

        public ActionResult SuppTache(int? id)
        {
            try
            {
                string SqlDelete = "DELETE FROM Taches WHERE UserId=@UserId AND IdTache=@IdTache";
                List<SqlParameter> Parametres = new List<SqlParameter>
                    {
                        new SqlParameter("@UserId",InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)),
                        new SqlParameter("@IdTache",id)
                    };

                RequeteSql.ExecuteQuery(SqlDelete, Parametres);

                var message = "Reussi";
                return RedirectToAction("Taches", "ConsulterTache", new { strMessValidation = message });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        [HttpPost]
        public ActionResult Modification(Tache Model)
        {
            try
            {
                int UserId = Int32.Parse(InTime.Models.Cookie.ObtenirCookie(User.Identity.Name));
                string SqlUpdate = "UPDATE Taches set NomTache=@NomTache,Lieu=@Lieu,Description=@Desc,Mois=@Mois,Jour=@Jour,"
                +"HDebut=@HDebut,HFin=@HFin,mDebut=@mDebut,mFin=@mFin,HRappel=@HRappel,mRappel=@mRappel,Annee=@Annee,"
                +"Reccurence=@Reccurence WHERE UserId=@UserId AND IdTache=@IdTache;";
                List<SqlParameter> listParametres = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", UserId),
                    new SqlParameter("@IdTache", Model.IdTache),
                    new SqlParameter("@NomTache", Model.NomTache),
                    new SqlParameter("@Lieu", Model.Lieu),
                    new SqlParameter("@Desc", Model.Description),
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

                var message = RequeteSql.ExecuteQuery(SqlUpdate, listParametres) ? "Modif" : "Erreur";

                TempData["Modification"] = message;
                return RedirectToAction("Taches", "ConsulterTache");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public ActionResult ModifTache(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        Tache tache = null;
                        string queryString = "SELECT * FROM Taches where IdTache=@Id";
                        List<SqlParameter> Parametre = new List<SqlParameter>
                        {
                            new SqlParameter("@Id", id)
                        };

                        SqlDataReader reader = RequeteSql.Select(queryString, Parametre);
                        while (reader.Read())
                        {
                            Object[] values = new Object[reader.FieldCount];
                            reader.GetValues(values);
                            tache = ObtenirTache(values);
                        }
                        reader.Close();

                        if (tache == null)
                        {
                            return HttpNotFound();
                        }

                        InitialiseViewBags();
                        InitialiseViewBag(tache);
                        ViewData["Tache"] = tache;
                        IdRecurrence(tache);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }

                    return View("Modification");
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
        }
        private void IdRecurrence(Tache tache)
        {
            switch (tache.Reccurence)
            {
                case "Aucune":
                    tache.Reccurence = "0";
                    break;
                case "À chaque jour":
                    tache.Reccurence = "1";
                    break;
                case "Chaque semaine":
                    tache.Reccurence = "2";
                    break;
                case "Aux deux semaines":
                    tache.Reccurence = "3";
                    break;
                case "Aux trois semaines":
                    tache.Reccurence = "4";
                    break;
                case "À chaque mois":
                    tache.Reccurence = "5";
                    break;
                case "Aux trois mois":
                    tache.Reccurence = "6";
                    break;
                case "Aux quatre mois":
                    tache.Reccurence = "7";
                    break;
                case "À chaque année":
                    tache.Reccurence = "8";
                    break;
            }
        }
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        Tache tache = null;
                        string queryString = "SELECT * FROM Taches where IdTache=@Id";
                        List<SqlParameter> Parametre = new List<SqlParameter>
                        {
                            new SqlParameter("@Id", id)
                        };

                        SqlDataReader reader = RequeteSql.Select(queryString,Parametre);
                        while (reader.Read())
                        {
                            Object[] values = new Object[reader.FieldCount];
                            reader.GetValues(values);
                            tache = ObtenirTache(values);
                        }
                        reader.Close();

                        InitialiseViewBags();
                        InitialiseViewBag(tache);
                        ViewData["Tache"] = tache;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }

                    return View();
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
        }


        private string TempsRappel(DateTime rappel)
        {
            string strPhrase = "Il vous reste ";

            if (rappel < DateTime.Now)
            {
                return "La date de rappel est dépassée.";
            }
            else
            {
                TimeSpan tsTempsRestant = rappel - DateTime.Now;
                int nNombreJourRestant = tsTempsRestant.Days;
                if (nNombreJourRestant > 365)
                {
                    int nAnnee = (tsTempsRestant.Days / 365);
                    nNombreJourRestant -= (nAnnee * 365);
                    strPhrase += String.Format("{0} {1} ", nAnnee, nAnnee == 1 ? "an" : "ans");
                }

                if (nNombreJourRestant > 30)
                {
                    int nMois = (nNombreJourRestant / 30);
                    nNombreJourRestant -= (nMois * 30);
                    strPhrase += String.Format("{0} mois ", nMois);
                }

                if (nNombreJourRestant > 0)
                {
                    int nJours = nNombreJourRestant;
                    strPhrase += String.Format("{0} {1} ", nJours, nJours == 1 ? "jour" : "jours");
                }

                if (tsTempsRestant.Hours > 0)
                {
                    int nHeure = tsTempsRestant.Hours;
                    strPhrase += String.Format("{0} {1} ", nHeure, nHeure == 1 ? "heure" : "heures");
                }

                if (tsTempsRestant.Minutes > 0)
                {
                    int nMinute = tsTempsRestant.Minutes;
                    strPhrase += String.Format("{0} {1} ", nMinute, nMinute == 1 ? "minute" : "minutes");
                }

                return strPhrase + "avant le rappel.";
            }
        }


        private Tache ObtenirTache(Object[] values)
        {
            var tache = new Tache()
            {
                IdTache = Convert.ToInt32(values[0]),
                UserId = Convert.ToInt32(values[1]),
                NomTache = RequeteSql.RemettreApostrophe(Convert.ToString(values[2])),
                Lieu = RequeteSql.RemettreApostrophe(Convert.ToString(values[3])),
                Description = RequeteSql.RemettreApostrophe(Convert.ToString(values[4])),
                Mois = Convert.ToString(values[5]),
                Jour = Convert.ToString(values[6]),
                HDebut = Convert.ToString(values[7]),
                HFin = Convert.ToString(values[8]),
                mDebut = Convert.ToString(values[9]),
                mFin = Convert.ToString(values[10]),
                HRappel = Convert.ToString(values[11]),
                mRappel = Convert.ToString(values[12]),
                Annee = Convert.ToString(values[13]),
                Reccurence = Convert.ToString(values[14])
            };

            return tache;
        }

        private void InitialiseViewBags()
        {
            ViewBag.trancheMin = new SelectList(Tache.tempsMinutes);

            ViewBag.trancheHeure = new SelectList(Tache.tempsHeure);

            ViewBag.MoisAnnee = new SelectList(Les_Mois(), "Value", "Text");

            ViewBag.Reccurence = new SelectList(Tache.options);
        }

        private void InitialiseViewBag(Tache tache)
        {
            DateTime DateDebut = new DateTime(
                       Convert.ToInt32(tache.Annee), Convert.ToInt32(tache.Mois), Convert.ToInt32(tache.Jour),
                       Convert.ToInt32(tache.HDebut), Convert.ToInt32(tache.mDebut), 0
                       );
            ViewBag.DateDebut = DateDebut.ToString(culture);

            DateTime DateFin = new DateTime(
                Convert.ToInt32(tache.Annee), Convert.ToInt32(tache.Mois), Convert.ToInt32(tache.Jour),
                Convert.ToInt32(tache.HFin), Convert.ToInt32(tache.mFin), 0
                );
            ViewBag.DateFin = DateFin.ToString(culture);

            tache.HRappel = (String.IsNullOrEmpty(tache.HRappel)) ? "00" : tache.HRappel;
            tache.mRappel = (String.IsNullOrEmpty(tache.mRappel)) ? "00" : tache.mRappel;
            TimeSpan tsRappel = new TimeSpan(
                Convert.ToInt32(tache.HRappel), Convert.ToInt32(tache.mRappel), 0
                );
            DateTime DateRappel = DateDebut.Subtract(tsRappel);


            if (DateRappel == DateDebut)
            {
                ViewBag.DateRappel = "Aucun";
            }
            else
            {
                ViewBag.DateRappel = TempsRappel(DateRappel);
            }
        }
    }
}
