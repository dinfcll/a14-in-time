using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;
using System.Globalization;

namespace InTime.Controllers
{
    public class ConsulterTacheController : Controller
    {
        CultureInfo culture = new CultureInfo("fr-CA");


        public ActionResult Index()
        {
            //TODO :
            //Aller chercher l'information dans la base de données 

            if (User.Identity.IsAuthenticated)
            {
                var tache = new Tache()
                {
                    m_strNomTache = "Test",
                    m_strDescTache = "C'est un test",
                    m_annee = "2015",
                    m_mois = "11",
                    m_jour = "22",
                    m_debHeure = "9",
                    m_debMin = "0",
                    m_finHeure = "17",
                    m_finMin = "30",
                    m_rappelHeure = "0",
                    m_rappelMin = "30",
                    m_strLieu = "Bureau",
                };
                ViewData["Tache"] = tache;

                DateTime DateDebut = new DateTime(
                    Convert.ToInt32(tache.m_annee), Convert.ToInt32(tache.m_mois), Convert.ToInt32(tache.m_jour),
                    Convert.ToInt32(tache.m_debHeure), Convert.ToInt32(tache.m_debMin), 0
                    );
                ViewBag.DateDebut = DateDebut.ToString(culture);

                DateTime DateFin = new DateTime(
                    Convert.ToInt32(tache.m_annee), Convert.ToInt32(tache.m_mois), Convert.ToInt32(tache.m_jour),
                    Convert.ToInt32(tache.m_finHeure), Convert.ToInt32(tache.m_finMin), 0
                    );
                ViewBag.DateFin = DateFin.ToString(culture);

                TimeSpan tsRappel = new TimeSpan(
                    Convert.ToInt32(tache.m_rappelHeure), Convert.ToInt32(tache.m_rappelMin), 0
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
                return View("~/Views/ErreurAuthentification.cshtml");
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
