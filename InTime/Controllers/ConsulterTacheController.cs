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
                double unixDebut = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateDebut(Model));
                double unixFin = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateFin(Model));
                string SqlUpdate = "UPDATE Taches set NomTache=@NomTache,Lieu=@Lieu,Description=@Description,"
                +"DateDebut=@DateDebut,DateFin=@DateFin,HRappel=@HRappel,mRappel=@mRappel,"
                +"Reccurence=@Reccurence WHERE UserId=@UserId AND IdTache=@IdTache;";

                List<SqlParameter> listParametres = new List<SqlParameter>
                {
                    new SqlParameter("@IdTache",Model.IdTache),
                    new SqlParameter("@UserId", UserId),
                    new SqlParameter("@NomTache", Model.NomTache),
                    new SqlParameter("@Lieu", Model.Lieu),
                    new SqlParameter("@DateDebut",unixDebut),
                    new SqlParameter("@DateFin",unixFin),
                    new SqlParameter("@Description", Model.Description),
                    new SqlParameter("@HRappel", SqlDbType.VarChar) { Value = Model.HRappel ?? (object)DBNull.Value },
                    new SqlParameter("@mRappel", SqlDbType.VarChar) { Value = Model.mRappel ?? (object)DBNull.Value },
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
                        IdMin(tache);
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
                NomTache = Convert.ToString(values[2]),
                Lieu = Convert.ToString(values[3]),
                Description = Convert.ToString(values[4]),
                unixDebut = Convert.ToDouble(values[5]),
                unixFin = Convert.ToDouble(values[6]),
                HRappel = Convert.ToString(values[7]),
                mRappel = Convert.ToString(values[8]),
                Reccurence = Convert.ToInt32(values[9])
            };

            return tache;
        }

        private void InitialiseViewBags()
        {
            ViewBag.trancheMin = new SelectList(Tache.tempsMinutes);

            ViewBag.trancheHeure = new SelectList(Tache.tempsHeure);

            ViewBag.MoisAnnee = new SelectList(Tache.les_mois, "Value", "Text");

            ViewBag.Reccurence = new SelectList(Tache.options, "Value","Text");
        }

        private void InitialiseViewBag(Tache tache)
        {
            DateTime DateDebut = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);

            ViewBag.DateDebut = DateDebut.ToString(culture);
            ViewBag.DateFin = TraitementDate.UnixTimeStampToDateTime(tache.unixFin).ToString(culture);

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

            ViewBag.Recurrence = Tache.NomReccurence(tache.Reccurence);
        }


        private void IdMin(Tache tache)
        {
            switch (tache.mDebut)
            {
                case "00":
                    tache.mDebut = "1";
                    break;
                case "15":
                    tache.mDebut = "2";
                    break;
                case "30":
                    tache.mDebut = "3";
                    break;
                case "45":
                    tache.mDebut = "4";
                    break;
            }

            switch (tache.mFin)
            {
                case "00":
                    tache.mFin = "1";
                    break;
                case "15":
                    tache.mFin = "2";
                    break;
                case "30":
                    tache.mFin = "3";
                    break;
                case "45":
                    tache.mFin = "4";
                    break;
            }

            switch (tache.mRappel)
            {
                case "00":
                    tache.mRappel = "1";
                    break;
                case "15":
                    tache.mRappel = "2";
                    break;
                case "30":
                    tache.mRappel = "3";
                    break;
                case "45":
                    tache.mRappel = "4";
                    break;
            }
        }
    }
}
