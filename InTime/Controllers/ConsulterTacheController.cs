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
                    m_annee = "2014",
                    m_mois = "1",
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
                    ViewBag.DateRappel = DateRappel.ToString(culture);
                }
                ViewData["Tache"] = tache;

                return View();
            }
            else
            {
                return View("~/Views/ErreurAuthentification.cshtml");
            }
        }

    }
}
