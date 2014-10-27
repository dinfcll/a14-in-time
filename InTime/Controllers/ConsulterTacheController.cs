using System;
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
        private InTime.Models.InTime db = new InTime.Models.InTime();

        public ActionResult Taches()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var lstTache = new List<Tache>();

                    string queryString = string.Format("SELECT * FROM Taches where UserId='{0}'",
                        InTime.Models.Cookie.ObtenirCookie(User.Identity.Name));
                    SqlDataReader reader = RequeteSql.Select(queryString);

                    while (reader.Read())
                    {
                        Object[] values = new Object[reader.FieldCount];
                        int fieldCounts = reader.GetValues(values);
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
                            Annee = Convert.ToString(values[13])
                        };
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


        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
                if (User.Identity.IsAuthenticated)
                {
                    Tache tache = null;
                    try
                    {

                        string queryString = string.Format("SELECT * FROM Taches where IdTache='{0}'",
                            InTime.Models.Cookie.ObtenirCookie(User.Identity.Name));
                        SqlDataReader reader = RequeteSql.Select(queryString);

                        while (reader.Read())
                        {
                            Object[] values = new Object[reader.FieldCount];
                            int fieldCounts = reader.GetValues(values);
                            tache = new Tache()
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
                                Annee = Convert.ToString(values[13])
                            };
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }

                    if (tache == null)
                    {
                        return HttpNotFound();
                    }

                    if (tache.HFin == "24")
                        tache.HFin = "0";

                    if (tache.HDebut == "24")
                        tache.HFin = "0";

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

                    tache.HRappel = (String.IsNullOrEmpty(tache.HRappel)) ? "0" : tache.HRappel;
                    tache.mRappel = (String.IsNullOrEmpty(tache.mRappel)) ? "0" : tache.mRappel;
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
                    ViewData["Tache"] = tache;

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
    }
}
