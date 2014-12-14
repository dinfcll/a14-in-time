using InTime.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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
                    int Choix = 1;
                    int anneeDebut;
                    int anneeFin;
                    anneeDebut = anneeFin = DateTime.Now.Year;

                    if (!String.IsNullOrEmpty(ChoixTemps))
                    {
                        if (!Int32.TryParse(ChoixTemps, out Choix) || Choix == 0)
                        {
                            Choix = 1;
                        }
                        Int32.TryParse(FinAnn, out anneeFin);
                        Int32.TryParse(DebAnn, out anneeDebut);
                    }
                    ViewBag.anneeDebut = anneeDebut;
                    ViewBag.anneeFin = anneeFin;
                    ViewBag.Taches = TraitementChoixHistorique(Choix, FinAnn, DebAnn, ChoixMoisFin, ChoixMoisDebut, Recurrence);

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
            string Select = "";
            var lstTache = new List<Tache>();
            List<SqlParameter> listParametres = new List<SqlParameter>();
            double TacheRecDebut = 0;
            double TacheRecFin = 0;

            switch (Choix)
            {
                case 0:
                    break;
                case 1:
                    TacheRecDebut = TraitementDate.DateTimeToUnixTimestamp(DateTime.Now.AddMonths(-3));
                    TacheRecFin = TraitementDate.DateTimeToUnixTimestamp(DateTime.Now);
                    if (!Recurrence)
                    {
                        Select = "SELECT * FROM Taches where UserId=@Id AND DateDebut>@TroisMoisArriere AND DateDebut < @Maintenant AND Recurrence = 0;";
                    }
                    else
                    {
                        Select = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut>@TroisMoisArriere AND DateDebut < @Maintenant AND Recurrence = 0) OR Recurrence > 0);";
                    }
                    listParametres.Add(new SqlParameter("@Id", InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)));
                    listParametres.Add(new SqlParameter("@TroisMoisArriere", TacheRecDebut));
                    listParametres.Add(new SqlParameter("@Maintenant", TacheRecFin));
                    break;
                case 2:
                    try
                    {
                        DateTime Date1 = new DateTime(Convert.ToInt32(DebAnn), Convert.ToInt32(ChoixMoisDebut), 1);
                        DateTime Date2 = new DateTime(Convert.ToInt32(FinAnn), Convert.ToInt32(ChoixMoisFin), 1);
                        Date2 = Date2.AddMonths(1);
                        Date2 = Date2.AddDays(-1);
                        TacheRecDebut = TraitementDate.DateTimeToUnixTimestamp(Date1);
                        TacheRecFin = TraitementDate.DateTimeToUnixTimestamp(Date2);

                        if (!Recurrence)
                        {
                            Select = "SELECT * FROM Taches where UserId=@Id AND DateDebut >= @Date1 AND DateDebut <= @Date2 AND Recurrence = 0;";
                        }
                        else
                        {
                            Select = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut >= @Date1 AND DateDebut <= @Date2 AND Recurrence = 0) OR Recurrence > 0);";
                        }
                        listParametres.Add(new SqlParameter("@Id", InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)));
                        listParametres.Add(new SqlParameter("@Date1", TacheRecDebut));
                        listParametres.Add(new SqlParameter("@Date2", TacheRecFin));
                    }
                    catch (Exception ex)
                    {
                        Select = "";
                    }
                    break;
                case 3:
                    TacheRecDebut = TraitementDate.DebutCalendrier();
                    TacheRecFin = TraitementDate.DateTimeToUnixTimestamp();
                    if (!Recurrence)
                    {
                        Select = "SELECT * FROM Taches where UserId=@Id AND DateDebut < @Maintenant AND Recurrence = 0;";
                    }
                    else
                    {
                        Select = "SELECT * FROM Taches where UserId=@Id AND ((DateDebut < @Maintenant AND Recurrence = 0) OR Recurrence > 0);";
                    }
                    listParametres.Add(new SqlParameter("@Id", InTime.Models.Cookie.ObtenirCookie(User.Identity.Name)));
                    listParametres.Add(new SqlParameter("@Maintenant", TacheRecFin));
                    break;
                default:
                    break;
            }

            try
            {
                if (!String.IsNullOrEmpty(Select))
                {
                    SqlDataReader reader = RequeteSql.Select(Select, listParametres);
                    while (reader.Read())
                    {
                        Object[] values = new Object[reader.FieldCount];
                        reader.GetValues(values);
                        var tache = Tache.ObtenirTache(values);
                        if (tache.Recurrence == (int)TraitementDate.recurrence.Aucune)
                        {
                            DateTime DateTache = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);
                            tache.Annee = Convert.ToString(DateTache.Year);
                            tache.Mois = Convert.ToString(DateTache.Month);
                            tache.Jour = Convert.ToString(DateTache.Day);
                            lstTache.Add(tache);
                        }
                        else
                        {
                            List<Tache> result = TraitementDate.TraitementRecurrenceTache(tache, TacheRecDebut, TacheRecFin);
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
