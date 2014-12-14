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

                            Tache.PreparationPourAffichage(ref tache);
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
    }
}
