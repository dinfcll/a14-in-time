using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class AjouterTacheController : Controller
    {
        //
        // GET: /AjouterTache/

        public ActionResult Index(string Min,string Heure)
        {
            var trancheMin = new List<string>();
            string[] tempsMin = { "0", "15", "30", "45", "60" };
            trancheMin.AddRange(tempsMin);

            var trancheHeure = new List<string>();
            for (int i = 1; i < 25; ++i)
                trancheHeure.Add(Convert.ToString(i));


                ViewBag.trancheMin = new SelectList(trancheMin);
            ViewBag.trancheHeure = new SelectList(trancheHeure);

            return View();
        }


        //Action qui se produit suite \ l'appuie du bouton enregistrer
        private void InsertionTache(string m_strNomTache,string m_strLieu,string m_strDescTache,DateTime m_dtHeure,DateTime m_dtRappel)
        {

        }

    }
}
