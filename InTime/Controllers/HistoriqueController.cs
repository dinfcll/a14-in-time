using InTime.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class HistoriqueController : Controller
    {
        public ActionResult Historique(string ChoixTemps, string FinAnn, string DebAnn, string ChoixMoisFin, string ChoixMoisDebut, bool Recurrence = false)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    int choix = 1;
                    int anneeDebut = DateTime.Now.Year;
                    int anneeFin = DateTime.Now.Year;

                    if (!String.IsNullOrEmpty(ChoixTemps))
                    {
                        if (!Int32.TryParse(ChoixTemps, out choix) || choix == 0)
                        {
                            choix = 1;
                        }
                        Int32.TryParse(FinAnn, out anneeFin);
                        Int32.TryParse(DebAnn, out anneeDebut);
                    }
                    ViewBag.anneeDebut = anneeDebut;
                    ViewBag.anneeFin = anneeFin;
                    ViewBag.Taches = TraitementChoixHistorique(choix, FinAnn, DebAnn, ChoixMoisFin, ChoixMoisDebut, Recurrence);

                    return View();
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

        private List<Tache> TraitementChoixHistorique(int Choix, string FinAnn, string DebAnn, string ChoixMoisFin, string ChoixMoisDebut, bool Recurrence)
        {
            string select = "";
            var lstTache = new List<Tache>();
            List<SqlParameter> listParametres = new List<SqlParameter>();
            double tacheRecDebut = 0;
            double tacheRecFin = 0;

            switch (Choix)
            {
                case 0:
                    break;
                case 1:
                    tacheRecDebut = TraitementDate.DateTimeToUnixTimestamp(DateTime.Now.AddMonths(-3));
                    tacheRecFin = TraitementDate.DateTimeToUnixTimestamp(DateTime.Now);
                    if (!Recurrence)
                    {
                        select = "SELECT * FROM Taches where UserId=@Id AND DateDebut>@TroisMoisArriere AND DateDebut < @Maintenant AND Recurrence = 0;";
                    }
                    else
                    {
                        select = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut>@TroisMoisArriere AND DateDebut < @Maintenant AND Recurrence = 0) OR Recurrence > 0);";
                    }
                    listParametres.Add(new SqlParameter("@Id",Cookie.ObtenirCookie(User.Identity.Name)));
                    listParametres.Add(new SqlParameter("@TroisMoisArriere", tacheRecDebut));
                    listParametres.Add(new SqlParameter("@Maintenant", tacheRecFin));
                    break;
                case 2:
                    try
                    {
                        DateTime date1 = new DateTime(Convert.ToInt32(DebAnn), Convert.ToInt32(ChoixMoisDebut), 1);
                        DateTime date2 = new DateTime(Convert.ToInt32(FinAnn), Convert.ToInt32(ChoixMoisFin), 1);
                        date2 = date2.AddMonths(1);
                        date2 = date2.AddDays(-1);
                        tacheRecDebut = TraitementDate.DateTimeToUnixTimestamp(date1);
                        tacheRecFin = TraitementDate.DateTimeToUnixTimestamp(date2);

                        if (!Recurrence)
                        {
                            select = "SELECT * FROM Taches where UserId=@Id AND DateDebut >= @Date1 AND DateDebut <= @Date2 AND Recurrence = 0;";
                        }
                        else
                        {
                            select = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut >= @Date1 AND DateDebut <= @Date2 AND Recurrence = 0) OR Recurrence > 0);";
                        }
                        listParametres.Add(new SqlParameter("@Id", Cookie.ObtenirCookie(User.Identity.Name)));
                        listParametres.Add(new SqlParameter("@Date1", tacheRecDebut));
                        listParametres.Add(new SqlParameter("@Date2", tacheRecFin));
                    }
                    catch
                    {
                        select = "";
                    }
                    break;
                case 3:
                    tacheRecDebut = TraitementDate.DebutCalendrier();
                    tacheRecFin = TraitementDate.DateTimeToUnixTimestamp();
                    if (!Recurrence)
                    {
                        select = "SELECT * FROM Taches where UserId=@Id AND DateDebut < @Maintenant AND Recurrence = 0;";
                    }
                    else
                    {
                        select = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut < @Maintenant AND Recurrence = 0) OR Recurrence > 0);";
                    }
                    listParametres.Add(new SqlParameter("@Id", Cookie.ObtenirCookie(User.Identity.Name)));
                    listParametres.Add(new SqlParameter("@Maintenant", tacheRecFin));
                    break;
                default:
                    select = "";
                    break;
            }

            try
            {
                if (!String.IsNullOrEmpty(select))
                {
                    SqlDataReader reader = RequeteSql.Select(select, listParametres);
                    while (reader.Read())
                    {
                        Object[] values = new Object[reader.FieldCount];
                        reader.GetValues(values);
                        var tache = Tache.ObtenirTache(values);
                        if (tache.Recurrence == (int)TraitementDate.recurrence.Aucune)
                        {
                            DateTime dateTache = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);
                            tache.Annee = Convert.ToString(dateTache.Year);
                            tache.Mois = Convert.ToString(dateTache.Month);
                            tache.Jour = Convert.ToString(dateTache.Day);
                            lstTache.Add(tache);
                        }
                        else
                        {
                            List<Tache> result = TraitementDate.TraitementRecurrenceTache(tache, tacheRecDebut, tacheRecFin);
                            if (result != null)
                            {
                                foreach (Tache tacheRec in result)
                                {
                                    string resultat = RequeteSql.RechercheDescSupplTache(tacheRec.IdTache, tacheRec.unixDebut);
                                    if (!String.IsNullOrEmpty(resultat))
                                    {
                                        tache.Description = resultat;
                                    }
                                }
                                lstTache.AddRange(result);
                            }
                        }
                    }
                    reader.Close();
                }
            }
            catch
            {
                lstTache = new List<Tache>();
            }

            return lstTache;
        }

    }
}
