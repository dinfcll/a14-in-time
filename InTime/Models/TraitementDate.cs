using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace InTime.Models
{
    public static class TraitementDate
    {
        public enum recurrence
        {
            Aucune, ChaqueJour, ChaqueSemaine, DeuxSemaines, TroisSemaine, ChaqueMois, TroisMois, QuatreMois, ChaqueAnnee
        }

        public static double UnixXJour(int NbreJours)
        {
            return 60 * 60 * 24 * NbreJours;
        }

        public static double UnixXMois(double unixTime, int NbreMois)
        {

            DateTime DateInitial = UnixTimeStampToDateTime(unixTime);
            DateTime DateFinal = DateInitial.AddMonths(NbreMois);

            return 60 * 60 * 24 * (DateFinal - DateInitial).TotalDays;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static double DateTimeToUnixTimestamp()
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            return (dt - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);

            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        public static string UnixTimeStampToString(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);

            return dtDateTime.AddSeconds(unixTimeStamp).ToString("yyyy-MM-dd HH:mm");
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
            try
            {
                DateTime date = new DateTime(Annee, Mois, Jour, Heure, Minute, 0);
                return date.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string DateFormatCalendrier(double unix)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);

            return dtDateTime.AddSeconds(unix).ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static List<string[]> DatesTacheRecurrente(Tache tache, double start, double end, int Bond, int Type)
        {
            List<string[]> date = new List<string[]>();
            double tacheDebut = tache.unixDebut;
            double tacheFin = tache.unixFin;
            double differenceDebutFin = tacheFin - tacheDebut;

            if (tacheDebut < start)
            {
                if (Type == 2)
                {
                    while ((tacheDebut < start))
                    {
                        tacheDebut += UnixXMois(tacheDebut, 12);
                    }
                }
                else
                {
                    if (Bond == 1 || Type == 1)
                    {
                        while ((tacheDebut + UnixXMois(tacheDebut, 12) < start))
                        {
                            tacheDebut += UnixXMois(tacheDebut, 12);
                        }
                    }

                    if (Type == 1)
                    {
                        while (tacheDebut < start)
                        {
                            tacheDebut += UnixXMois(tacheDebut, Bond);
                        }
                    }
                    else
                    {
                        if (Bond == 1)
                        {
                            while ((tacheDebut + UnixXMois(tacheDebut, 1) < start))
                            {
                                tacheDebut += UnixXMois(tacheDebut, 1);
                            }
                        }
                        while (tacheDebut < start)
                        {
                            tacheDebut += UnixXJour(Bond);
                        }
                    }
                }

                tacheFin = tacheDebut + differenceDebutFin;
            }

            if (tacheDebut > start && tacheDebut < end)
            {
                while (tacheDebut < end)
                {
                    string dateDebutCalen = DateFormatCalendrier(tacheDebut);
                    string dateFinCalen = DateFormatCalendrier(tacheFin);

                    date.Add(new string[] { tache.NomTache, dateDebutCalen, dateFinCalen, Convert.ToString(tache.IdTache) });

                    if (Type == 2)
                    {
                        tacheDebut += UnixXMois(tacheDebut, 12);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }

                    if (Type == 1)
                    {
                        tacheDebut += UnixXMois(tacheDebut, Bond);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }
                    else
                    {
                        tacheDebut += UnixXJour(Bond);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }
                }
            }

            return date;
        }

        public static List<string[]> ChaqueAnnee(Tache tache, double start, int Affichage)
        {
            List<string[]> date = new List<string[]>();
            DateTime debut = DateDebut(tache);
            DateTime fin = DateFin(tache);
            DateTime debutCalendrier = UnixTimeStampToDateTime(start);
            if (Affichage == 2)
            {
                debutCalendrier = debutCalendrier.AddMonths(-1);
            }
            bool Changement = false;

            if (debut.Month == debutCalendrier.Month && debut.Year <= debutCalendrier.Year)
            {
                if (debut.Year != debutCalendrier.Year)
                {
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddYears(1);
                    }
                }

                Changement = true;
            }


            if (Changement)
            {
                string dateDebut = TraitementDate.DateFormatCalendrier(
                debut.Year, debutCalendrier.Month, debut.Day, debut.Hour, debut.Minute);
                string dateFin = TraitementDate.DateFormatCalendrier(
                debut.Year, debutCalendrier.Month, debut.Day, fin.Hour, fin.Minute);

                date.Add(new string[] { tache.NomTache, dateDebut, dateFin, Convert.ToString(tache.IdTache) });
            }

            return date;
        }
    }
}

