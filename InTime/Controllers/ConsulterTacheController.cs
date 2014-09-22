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


        public string TempsRappel(DateTime rappel)
        {
            string strPhrase = "Il vous reste ";

            if (rappel < DateTime.Now)
                return "La date de rappel est dépassée.";
            else
            {
                TimeSpan tsTempsRestant = rappel - DateTime.Now;
                if (tsTempsRestant.Days > 365)
                {
                    int nAnnee = (tsTempsRestant.Days / 365);
                    strPhrase += String.Format("{0} {1} ", nAnnee, nAnnee == 1 ? "an" : "ans");
                }

                if (tsTempsRestant.Days > 30)
                {
                    int nMois = (tsTempsRestant.Days / 30);
                    strPhrase += String.Format("{0} mois ", nMois);
                }

                if (tsTempsRestant.Days > 0)
                {
                    int nJours = tsTempsRestant.Days;
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

                if (tsTempsRestant.Seconds > 0)
                {
                    int nSecondes = tsTempsRestant.Seconds;
                    strPhrase += String.Format("{0} {1} ", nSecondes, nSecondes == 1 ? "seconde": "secondes");
                }

                return strPhrase;
            }
        }

        public ActionResult Index()
        {
            //TODO :
            //Aller chercher l'information dans la base de données 

            var tache = new Tache()
            {
                m_strNomTache = "Test",
                m_strDescTache = "C'est un test",
                m_annee = "2015",
                m_mois = "10",
                m_jour = "15",
                m_debHeure = "9",
                m_debMin = "0",
                m_finHeure = "17",
                m_finMin = "30",
                m_rappelHeure = "0",
                m_rappelMin = "30",
                m_strLieu = "Bureau",
            };

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

            TimeSpan tsRappel = new TimeSpan(Convert.ToInt32(tache.m_rappelHeure),Convert.ToInt32(tache.m_rappelMin),0);
            DateTime DateRappel = DateDebut.Subtract(tsRappel);
            string strTempsRappel = TempsRappel(DateRappel);

            if (DateRappel == DateDebut)
            {
                ViewBag.DateRappel = "Aucun";
            }
            else
            {
                ViewBag.DateRappel = strTempsRappel;
            }
            ViewData["Tache"] = tache;

            return View();
        }

    }
}
