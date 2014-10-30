using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace InTime.Models
{
    public class TraitementDate
    {
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();

            return dtDateTime;
        }

        public static DateTime DateDebut(Tache tache)
        {
            DateTime date = new DateTime(Convert.ToInt32(tache.Annee), Convert.ToInt32(tache.Mois), Convert.ToInt32(tache.Jour),
                    Convert.ToInt32(tache.HDebut), Convert.ToInt32(tache.mDebut), 0, DateTimeKind.Local);

            return date;
        }

        public static DateTime DateFin(Tache tache)
        {
            DateTime date = new DateTime(Convert.ToInt32(tache.Annee), Convert.ToInt32(tache.Mois), Convert.ToInt32(tache.Jour),
                    Convert.ToInt32(tache.HFin), Convert.ToInt32(tache.mFin), 0, DateTimeKind.Local);

            return date;
        }

        public static string DateFormatCalendrier(string Annee, string Mois, string Jour, string Heure, string Minute)
        {
            return String.Format("{0}-{1}-{2}T{3}:{4}:00-05:00",
                   Annee, Mois, Jour, Heure, Minute);
        }

        public static string DateFormatCalendrier(int Annee, int Mois, int Jour, int Heure, int Minute)
        {
            DateTime date = new DateTime(Annee, Mois, Jour, Heure, Minute, 0);

            return date.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static List<string[]> ChaqueJour(Tache tache, double start)
        {
            List<string[]> date = new List<string[]>();

            DateTime debut = DateDebut(tache);
            DateTime fin = DateFin(tache);
            DateTime debutCalendrier = UnixTimeStampToDateTime(start);
            debutCalendrier = debutCalendrier.AddMonths(-1);
            bool Changement = false;
            bool Jour = false;


            if (debut.Month <= debutCalendrier.Month && debut.Year <= debutCalendrier.Year)
            {
                if (debut.Month < debutCalendrier.Month)
                {
                    while (debut.Month < debutCalendrier.Month)
                    {
                        debut = debut.AddMonths(+1);
                    }
                    Jour = true;
                }
                if (debut.Year < debutCalendrier.Year)
                {
                    Jour = true;
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddYears(+1);
                    }
                }
                Changement = true;
            }
            else
            {
                if (debut.Month > debutCalendrier.Month && debut.Year < debutCalendrier.Year)
                {
                    while (debut.Month > debutCalendrier.Month)
                    {
                        debut = debut.AddMonths(-1);
                    }
                    while(debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddYears(1);
                    }

                    Jour = true;
                }
            }
 

            if (Jour)
            {
                int days = DateTime.DaysInMonth(debut.Year,debut.Month);
                for (int day = 1; day <= days; day++)
                {
                    string dateDebut = TraitementDate.DateFormatCalendrier(
                    debut.Year, debut.Month, day, debut.Hour, debut.Minute);
                    string dateFin = TraitementDate.DateFormatCalendrier(
                    debut.Year, debut.Month, day, fin.Hour, fin.Minute);

                    date.Add(new string[]{tache.NomTache, dateDebut, dateFin, Convert.ToString(tache.IdTache) });
                }
            }
            else
            {
                if (Changement)
                {
                    int days = DateTime.DaysInMonth(debut.Year, debut.Month);
                    for (int day = debut.Day; day <= days; day++)
                    {
                        string dateDebut = TraitementDate.DateFormatCalendrier(
                        debut.Year, debut.Month, day, debut.Hour, debut.Minute);
                        string dateFin = TraitementDate.DateFormatCalendrier(
                        debut.Year, debut.Month, day, fin.Hour, fin.Minute);

                        date.Add(new string[] { tache.NomTache, dateDebut, dateFin, Convert.ToString(tache.IdTache) });
                    }
                }
            }

            return date;
        }
    }
}