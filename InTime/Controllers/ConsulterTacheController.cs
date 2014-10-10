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
        private InTime.Models.InTime db = new InTime.Models.InTime();

        public ActionResult Taches()
        {
            if (User.Identity.IsAuthenticated)
                {
            SqlConnection con = null;
            try
            {
                var lstTache = new List<Tache>();
                string cs = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
                con = new SqlConnection(cs);
                con.Open();
                //Recherche du Id de l'utilisateur connecté
                string SqlrId = string.Format("SELECT * FROM UserProfile where UserName='{0}'", User.Identity.Name);
                SqlCommand cmdId = new SqlCommand(SqlrId, con);
                int id = (Int32)cmdId.ExecuteScalar();

                string queryString = string.Format("SELECT * FROM Taches where UserId='{0}'", id);
                SqlCommand cmdQuery = new SqlCommand(queryString,con);
                SqlDataReader reader = cmdQuery.ExecuteReader();

                List<IDataRecord> datadb = new List<IDataRecord>();
                while(reader.Read())
                {
                    datadb.Add((IDataRecord)reader);
                    Object[] values = new Object[reader.FieldCount];
                    int fieldCounts = reader.GetValues(values);
                    var tache = new Tache()
                    {
                        IdTache = Convert.ToInt32(values[0]),
                        UserId = Convert.ToInt32(values[1]),
                        NomTache = Convert.ToString(values[2]),
                        Lieu = Convert.ToString(values[3]),
                        Description = Convert.ToString(values[4]),
                        Mois = Convert.ToString(values[5]),
                        Jour = Convert.ToString(values[6]),
                        HDebut = Convert.ToString(values[7]),
                        HFin = Convert.ToString(values[8]),
                        mDebut = Convert.ToString(values[9]),
                        mFin = Convert.ToString(values[10]),
                        HRappel = Convert.ToString(values[11]),
                        mRappel = Convert.ToString(values[12]),
                        Annee = Convert.ToString(values[13])
                    };
                    lstTache.Add(tache);
                }

                ViewBag.Taches = lstTache;
                return View();

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
            else
            {
                return View("~/Views/ErreurAuthentification.cshtml");
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
                    SqlConnection con = null;
                    Tache tache = null;
                    try
                    {
                       
                        string cs = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
                        con = new SqlConnection(cs);
                        con.Open();

                        string queryString = string.Format("SELECT * FROM Taches where IdTache='{0}'", id);
                        SqlCommand cmdQuery = new SqlCommand(queryString, con);
                        SqlDataReader reader = cmdQuery.ExecuteReader();

                        List<IDataRecord> datadb = new List<IDataRecord>();
                        while (reader.Read())
                        {
                            datadb.Add((IDataRecord)reader);
                            Object[] values = new Object[reader.FieldCount];
                            int fieldCounts = reader.GetValues(values);
                            tache = new Tache()
                            {
                                IdTache = Convert.ToInt32(values[0]),
                                UserId = Convert.ToInt32(values[1]),
                                NomTache = Convert.ToString(values[2]),
                                Lieu = Convert.ToString(values[3]),
                                Description = Convert.ToString(values[4]),
                                Mois = Convert.ToString(values[5]),
                                Jour = Convert.ToString(values[6]),
                                HDebut = Convert.ToString(values[7]),
                                HFin = Convert.ToString(values[8]),
                                mDebut = Convert.ToString(values[9]),
                                mFin = Convert.ToString(values[10]),
                                HRappel = Convert.ToString(values[11]),
                                mRappel = Convert.ToString(values[12]),
                                Annee = Convert.ToString(values[13])
                            };
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        con.Close();
                    }

                    if (tache == null)
                    {
                        return HttpNotFound();
                    }

                    if (tache.HFin == "24")
                        tache.HFin = "0";

                    if (tache.HDebut == "24")
                        tache.HFin = "0";

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
