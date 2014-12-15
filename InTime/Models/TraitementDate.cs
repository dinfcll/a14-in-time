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

        public static double DebutCalendrier()
        {
            DateTime DebutCalen = new DateTime(2014,1,1);
            return (DebutCalen - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static double UnixXHeure(int NbreHeure)
        {
            return 60 * 60 * NbreHeure;
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
            System.DateTime dtDateTime = new DateTime(1970, 1, 1);

            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        public static string UnixTimeStampToString(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1);

            return dtDateTime.AddSeconds(unixTimeStamp).ToString("yyyy-MM-dd HH:mm");
        }

        public static DateTime DateDebut(Tache tache)
        {
            DateTime date = new DateTime(Convert.ToInt32(tache.Annee), Convert.ToInt32(tache.Mois), Convert.ToInt32(tache.Jour),
                    Convert.ToInt32(tache.HDebut), Convert.ToInt32(tache.mDebut), 0);

            return date;
        }

        public static DateTime DateFin(Tache tache)
        {
            DateTime date = new DateTime(Convert.ToInt32(tache.Annee), Convert.ToInt32(tache.Mois), Convert.ToInt32(tache.Jour),
                    Convert.ToInt32(tache.HFin), Convert.ToInt32(tache.mFin), 0);

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
            catch 
            {
                return null;
            }
        }

        public static string DateFormatCalendrier(double unix)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);

            return dtDateTime.AddSeconds(unix).ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        private static List<string[]> DatesTacheRecurrente(Tache tache, double start, double end, int Bond, int Type)
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

        private static List<Tache> TacheRecurrente(Tache tache, double start, double end, int Bond, int Type)
        {
            List<Tache> taches = new List<Tache>();
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
                    DateTime DateTache = TraitementDate.UnixTimeStampToDateTime(tacheDebut);
                    tache.Annee = Convert.ToString(DateTache.Year);
                    tache.Mois = Convert.ToString(DateTache.Month);
                    tache.Jour = Convert.ToString(DateTache.Day);
                    tache.unixDebut = tacheDebut;
                    tache.unixFin = tacheFin;
                    taches.Add(new Tache(tache));

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

            return taches;
        }

        public static List<string[]> TraitementRecurrence(Tache tache, double start, double end)
        {
            TraitementDate.recurrence recurrence =
                        (TraitementDate.recurrence)Enum.ToObject(typeof(TraitementDate.recurrence), tache.Recurrence);
            List<string[]> result = null;

            switch (recurrence)
            {
                case TraitementDate.recurrence.ChaqueJour:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 1, 0);
                    break;
                case TraitementDate.recurrence.ChaqueSemaine:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 7, 0);
                    break;
                case TraitementDate.recurrence.DeuxSemaines:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 14, 0);
                    break;
                case TraitementDate.recurrence.TroisSemaine:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 21, 0);
                    break;
                case TraitementDate.recurrence.ChaqueMois:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 1, 1);
                    break;
                case TraitementDate.recurrence.TroisMois:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 3, 1);
                    break;
                case TraitementDate.recurrence.QuatreMois:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 4, 1);
                    break;
                case TraitementDate.recurrence.ChaqueAnnee:
                    result = TraitementDate.DatesTacheRecurrente(tache, start, end, 0, 2);
                    break;
            }

            return result;

        }

        public static List<Tache> TraitementRecurrenceTache(Tache tache, double start, double end)
        {
            TraitementDate.recurrence recurrence =
                        (TraitementDate.recurrence)Enum.ToObject(typeof(TraitementDate.recurrence), tache.Recurrence);
            List<Tache> result = null;

            switch (recurrence)
            {
                case TraitementDate.recurrence.ChaqueJour:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 1, 0);
                    break;
                case TraitementDate.recurrence.ChaqueSemaine:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 7, 0);
                    break;
                case TraitementDate.recurrence.DeuxSemaines:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 14, 0);
                    break;
                case TraitementDate.recurrence.TroisSemaine:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 21, 0);
                    break;
                case TraitementDate.recurrence.ChaqueMois:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 1, 1);
                    break;
                case TraitementDate.recurrence.TroisMois:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 3, 1);
                    break;
                case TraitementDate.recurrence.QuatreMois:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 4, 1);
                    break;
                case TraitementDate.recurrence.ChaqueAnnee:
                    result = TraitementDate.TacheRecurrente(tache, start, end, 0, 2);
                    break;
            }

            return result;

        }
    }
}

