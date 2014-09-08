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

        [HttpPost]
        public ActionResult Index(AjoutTache model)
        {
            string[] moisValid = { "Janvier","Février","Mars","Avril","Mai","Juin","Juillet",
                                     "Août","Septembre","Octobre","Novembre","Décembre"};

            if (ModelState.IsValid)
            {
                try
                {
                    int nAnne = Convert.ToInt32(model.m_annee);
                    int nJour = Convert.ToInt32(model.m_jour);
                    string strMois = model.m_mois;
                }
                catch (Exception ex)
                {
                    return View();
                }





            }

            return View();
        }


        //Action qui se produit suite \ l'appuie du bouton enregistrer
        private void InsertionTache(string m_strNomTache,string m_strLieu,string m_strDescTache,DateTime m_dtHeure,DateTime m_dtRappel)
        {

        }

    }
}
