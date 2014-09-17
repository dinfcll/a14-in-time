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

        public ActionResult Index()
        {
            //TODO :
            //Aller chercher l'information dans la base de donnee

            var tache1 = new AjoutTache()
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

            DateTime DateDebut = new DateTime(Convert.ToInt32(tache1.m_annee), Convert.ToInt32(tache1.m_mois), Convert.ToInt32(tache1.m_jour),
                Convert.ToInt32(tache1.m_debHeure), Convert.ToInt32(tache1.m_debMin), 0);
            ViewBag.DateDebut = DateDebut.ToString(CultureInfo.CreateSpecificCulture("fr-CA"));

            DateTime DateFin = new DateTime(Convert.ToInt32(tache1.m_annee), Convert.ToInt32(tache1.m_mois), Convert.ToInt32(tache1.m_jour),
                Convert.ToInt32(tache1.m_finHeure), Convert.ToInt32(tache1.m_finMin), 0);
            ViewBag.DateFin = DateFin.ToString(CultureInfo.CreateSpecificCulture("fr-CA"));

            TimeSpan tsRappel = new TimeSpan(Convert.ToInt32(tache1.m_rappelHeure),Convert.ToInt32(tache1.m_rappelMin),0);
            DateTime DateRappel = DateDebut.Subtract(tsRappel);

            if (DateRappel == DateDebut)
            {
                ViewBag.DateRappel = "Aucun";
            }
            else
            {
                ViewBag.DateRappel = DateRappel.ToString(CultureInfo.CreateSpecificCulture("fr-CA"));
            }
            ViewData["Tache"] = tache1;

            return View();
        }

    }
}
