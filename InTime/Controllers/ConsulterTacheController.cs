using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web.Mvc;
using InTime.Models;
using System.Globalization;
using System.Data.SqlClient;


namespace InTime.Controllers
{
    public class ConsulterTacheController : Controller
    {
        CultureInfo culture = new CultureInfo("fr-CA");

        public ActionResult Taches()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        var lstTache = new List<Tache>();
                        double DateAuj = TraitementDate.DateTimeToUnixTimestamp();
                        string queryString = "SELECT * FROM Taches where UserId=@Id AND (DateDebut>=@DateDebut OR Recurrence >= 0)";
                        List<SqlParameter> Parametres = new List<SqlParameter>
                    {
                        new SqlParameter("@Id",InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)),
                        new SqlParameter("@DateDebut", DateAuj)
                    };


                        SqlDataReader reader = RequeteSql.Select(queryString, Parametres);
                        while (reader.Read())
                        {
                            Object[] values = new Object[reader.FieldCount];
                            reader.GetValues(values);
                            var tache = Tache.ObtenirTache(values);
                            DateTime DateTache = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);
                            tache.Annee = Convert.ToString(DateTache.Year);
                            tache.Mois = Convert.ToString(DateTache.Month);
                            tache.Jour = Convert.ToString(DateTache.Day);
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
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

        public ActionResult ModifTache(int? id, double? dep, double? fn, bool? Existe)
        {
            int idTache;
            try
            {
                if (id == null)
                {
                    return View(UrlErreur.ErreurGeneral);
                }
                else
                    if (User.Identity.IsAuthenticated)
                    {
                        try
                        {
                            idTache = id.GetValueOrDefault();
                            Tache tache = RequeteSql.RechercherTache(idTache);

                            if (dep != null && fn != null)
                            {
                                tache.unixDebut = Convert.ToDouble(dep);
                                tache.unixFin = Convert.ToDouble(fn);
                                tache.Description = RequeteSql.RechercheDescSupplTache(idTache, tache.unixDebut) ?? tache.Description;
                                ViewBag.Modif = true;
                            }
                            else
                            {
                                ViewBag.Modif = false;
                            }

                            PreparationPourAffichage(ref tache);
                            InitialiseViewBags();
                            Tache.InitChampsTache(ref tache);
                            ViewData["Tache"] = tache;
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
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

        [HttpPost]
        public ActionResult Modification(Tache Model, string modif, bool Existe)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        string SqlCommande;
                        List<SqlParameter> listParametres = new List<SqlParameter>();
                        int UserId = Int32.Parse(InTime.Models.Cookie.ObtenirCookie(User.Identity.Name));
                        double unixDebut = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateDebut(Model));
                        double unixFin = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateFin(Model));
                        if (modif == "False")
                        {
                            SqlCommande = "UPDATE Taches set NomTache=@NomTache,Lieu=@Lieu,Description=@Description,"
                            + "DateDebut=@DateDebut,DateFin=@DateFin,HRappel=@HRappel,mRappel=@mRappel,"
                            + "recurrence=@recurrence, PriorityColor=@PriorityColor WHERE UserId=@UserId AND IdTache=@IdTache;";
                            listParametres.Add(new SqlParameter("@IdTache", Model.IdTache));
                            listParametres.Add(new SqlParameter("@UserId", UserId));
                            listParametres.Add(new SqlParameter("@NomTache", Model.NomTache));
                            listParametres.Add(new SqlParameter("@Lieu", Model.Lieu));
                            listParametres.Add(new SqlParameter("@DateDebut", unixDebut));
                            listParametres.Add(new SqlParameter("@DateFin", unixFin));
                            listParametres.Add(new SqlParameter("@Description", Model.Description));
                            listParametres.Add(new SqlParameter("@HRappel", SqlDbType.VarChar) { Value = Model.HRappel ?? (object)DBNull.Value });
                            listParametres.Add(new SqlParameter("@mRappel", SqlDbType.VarChar) { Value = Model.mRappel ?? (object)DBNull.Value });
                            listParametres.Add(new SqlParameter("@recurrence", Model.Recurrence));
                            listParametres.Add(new SqlParameter("@PriorityColor", Model.PriorityColor));
                        }
                        else
                        {
                            if (!Existe)
                            {
                                SqlCommande = "INSERT INTO InfoSupplTacheRecurrente (IdTache,DateDebut,DateFin,Description)"
                                + " VALUES (@IdTache,@DateDebut,@DateFin,@Description);";
                            }
                            else
                            {
                                SqlCommande = "UPDATE InfoSupplTacheRecurrente SET Description=@Description WHERE IdTache=@IdTache "
                                    + "AND DateDebut=@DateDebut AND DateFin=@DateFin;";
                            }
                            listParametres.Add(new SqlParameter("@IdTache", Model.IdTache));
                            listParametres.Add(new SqlParameter("@DateDebut", unixDebut));
                            listParametres.Add(new SqlParameter("@DateFin", unixFin));
                            listParametres.Add(new SqlParameter("@Description", Model.Description));
                            listParametres.Add(new SqlParameter("@PriorityColor", Model.PriorityColor));
                        }
                        var message = RequeteSql.ExecuteQuery(SqlCommande, listParametres) ? "Modif" : "Echec";
                        TempData["Modification"] = message;

                        DateTime date = TraitementDate.UnixTimeStampToDateTime(unixDebut);

                        return RedirectToAction("Taches", "ConsulterTache");
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
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

        public ActionResult Index(int? id, DateTime? dep, DateTime? fn, double deb = 0, double fin = 0)
        {
            try
            {
                int idTache;
                if (id == null)
                {
                    return View(UrlErreur.ErreurGeneral);
                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        try
                        {
                            idTache = id.GetValueOrDefault();
                            Tache tache = RequeteSql.RechercherTache(idTache);

                            if (dep != null && fn != null)
                            {
                                tache.unixDebut = TraitementDate.DateTimeToUnixTimestamp(Convert.ToDateTime(dep));
                                tache.unixFin = TraitementDate.DateTimeToUnixTimestamp(Convert.ToDateTime(fn));
                                ViewBag.Modif = true;
                            }
                            else
                            {
                                if (deb > 0 && fin > 0)
                                {
                                    tache.unixDebut = deb;
                                    tache.unixFin = fin;
                                }
                                ViewBag.Modif = false;
                            }

                            string result = RequeteSql.RechercheDescSupplTache(idTache, tache.unixDebut);
                            if (!String.IsNullOrEmpty(result))
                            {
                                ViewBag.Existe = true;
                                tache.Description = result;
                            }

                            PreparationPourAffichage(ref tache);
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
            }
            catch
            {
                return View(UrlErreur.ErreurGeneral);
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

        private void InitialiseViewBags()
        {
            ViewBag.trancheMin = new SelectList(Tache.tempsMinutes);

            ViewBag.trancheHeure = new SelectList(Tache.tempsHeure);

            ViewBag.MoisAnnee = new SelectList(Tache.les_mois, "Value", "Text");

            ViewBag.recurrence = new SelectList(Tache.options, "Value", "Text");
        }

        private void PreparationPourAffichage(ref Tache tache)
        {
            DateTime DateDebut = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);
            tache.Annee = Convert.ToString(DateDebut.Year);
            tache.Mois = Convert.ToString(DateDebut.Month - 1);
            tache.Jour = Convert.ToString(DateDebut.Day);

            tache.HRappel = (String.IsNullOrEmpty(tache.HRappel)) ? "00" : tache.HRappel;
            tache.mRappel = (String.IsNullOrEmpty(tache.mRappel)) ? "00" : tache.mRappel;
            TimeSpan tsRappel = new TimeSpan(
                Convert.ToInt32(tache.HRappel), Convert.ToInt32(tache.mRappel), 0
                );
            DateTime DateRappel = DateDebut.Subtract(tsRappel);

            if (DateRappel == DateDebut)
            {
                tache.DateRappelCalendrier = "Aucun";
            }
            else
            {
                tache.DateRappelCalendrier = TempsRappel(DateRappel);
            }

            tache.RecurrenceAffichage = Tache.Nomrecurrence(tache.Recurrence);
        }
    }
}
