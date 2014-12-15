using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;


namespace InTime.Controllers
{
    public class ConsulterTacheController : Controller
    {
        public ActionResult Taches()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        var lstTache = new List<Tache>();
                        double dateAuj = TraitementDate.DateTimeToUnixTimestamp();
                        const string queryString = "SELECT * FROM Taches where UserId=@Id AND (DateDebut>=@DateDebut OR Recurrence >= 0)";
                        List<SqlParameter> parametres = new List<SqlParameter>
                    {
                        new SqlParameter("@Id",Cookie.ObtenirCookie(User.Identity.Name)),
                        new SqlParameter("@DateDebut", dateAuj)
                    };


                        SqlDataReader reader = RequeteSql.Select(queryString, parametres);
                        while (reader.Read())
                        {
                            Object[] values = new Object[reader.FieldCount];
                            reader.GetValues(values);
                            var tache = Tache.ObtenirTache(values);
                            DateTime dateTache = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);
                            tache.Annee = Convert.ToString(dateTache.Year);
                            tache.Mois = Convert.ToString(dateTache.Month);
                            tache.Jour = Convert.ToString(dateTache.Day);
                            lstTache.Add(tache);
                        }
                        reader.Close();
                        ViewBag.Taches = lstTache;

                        return View();
                    }
                    catch
                    {
                        return View(UrlErreur.ErreurGeneral);
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

        public ActionResult Index(int? id, DateTime? dep, DateTime? fn,bool? Detail,bool? Historique, double deb = 0, double fin = 0)
        {
            try
            {
                
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
                            int idTache = id.GetValueOrDefault();
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

                            if (Detail != null && Detail == true)
                            {
                                ViewBag.Detail = true;
                            }
                            if (Historique != null && Historique == true)
                            {
                                ViewBag.Historique = true;
                            }
                            ViewData["Tache"] = tache;
                        }
                        catch
                        {
                            return View(UrlErreur.ErreurGeneral);
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
