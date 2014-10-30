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
            return String.Format("{0}-{1}-{2}T{3}:{4}:00-05:00",
                   Annee, Mois, Jour, Heure, Minute);
        }

        public static List<string[]> dateMois(Tache tache, double start, double end)
        {
            List<string[]> date = null;

            switch(tache.Reccurence)
            {
                case "Aucune":
                    break;
                case "À chaque jour" :
                    date = ChaqueJour(tache, start);
                    break;
                case "Chaque semaine" :
                    break;
                case "Aux deux semaines" :
                    break;
                case "Aux trois semaines":
                    break;
                case "À cahque mois" :
                    break;
                case "Aux trois mois":
                    break;
                case "Aux quatre mois":
                    break;
                case "À chaque année":
                    break;
            }

            return date;
        }

        public static List<string[]> ChaqueJour(Tache tache, double start)
        {
            List<string[]> date = new List<string[]>();

            DateTime debut = DateDebut(tache);
            DateTime fin = DateFin(tache);
            DateTime debutCalendrier = UnixTimeStampToDateTime(start);
            bool Changement = false;

            if (debut.Year < debutCalendrier.Year)
            {
                while(debut.Year < debutCalendrier.Year)
                {
                     debut.AddYears(1);
                }

                Changement = true;
            }

            if (debut.Month < debutCalendrier.Month)
            {
                while (debut.Month < debutCalendrier.Month)
                {
                    debut.AddMonths(1);
                }

                Changement = true;
            }

            if (Changement)
            {
                if (debut.Day != 1)
                {
                    while (debut.Day > 1)
                    {
                        debut.AddDays(-1);
                    }
                }

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
                if (debut.Month == debutCalendrier.Month &&
                    debut.Year == debutCalendrier.Year)
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