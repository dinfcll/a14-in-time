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
        private TacheDBContext db = new TacheDBContext();

        public ActionResult Taches()
        {
            SqlConnection con = null;
            try
            {
                string cs = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
                con = new SqlConnection(cs);
                con.Open();
                //Recherche du Id de l'utilisateur connecté
                string SqlrId = string.Format("SELECT * FROM UserProfile where UserName='{0}'", User.Identity.Name);
                SqlCommand cmdId = new SqlCommand(SqlrId, con);
                int id = (Int32)cmdId.ExecuteScalar();

                var query = from o in db.Taches
                            where o.UserId == Convert.ToInt32(SqlrId)
                            select o;

                return View(query);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
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
                    Tache tache = db.Taches.Find(id);
                    if (tache == null)
                    {
                        return HttpNotFound();
                    }

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
