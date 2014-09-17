using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;

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
                m_mois = "Janvier",
                m_jour = "15",
                m_debHeure = "9",
                m_debMin = "0",
                m_finHeure = "17",
                m_finMin = "30",
                m_rappelHeure = "",
                m_rappelMin = "",
                m_strLieu = "Bureau",
            };

            ViewData["Tache"] = tache1;

            return View();
        }

    }
}
