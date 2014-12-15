using System;
using System.Collections.Generic;

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
            DateTime debutCalen = new DateTime(2014,1,1);
            return (debutCalen - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static double UnixXHeure(int nbreHeure)
        {
            return 60 * 60 * nbreHeure;
        }

        public static double UnixXJour(int nbreJours)
        {
            return 60 * 60 * 24 * nbreJours;
        }

        public static double UnixXMois(double unixTime, int nbreMois)
        {

            DateTime dateInitial = UnixTimeStampToDateTime(unixTime);
            DateTime dateFinal = dateInitial.AddMonths(nbreMois);

            return 60 * 60 * 24 * (dateFinal - dateInitial).TotalDays;
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
            DateTime dtDateTime = new DateTime(1970, 1, 1);

            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        public static string UnixTimeStampToString(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1);

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

        public static string DateFormatCalendrier(double unix)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

            return dtDateTime.AddSeconds(unix).ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        private static List<string[]> DatesTacheRecurrente(Tache tache, double start, double end, int bond, int type)
        {
            List<string[]> date = new List<string[]>();
            double tacheDebut = tache.unixDebut;
            double tacheFin = tache.unixFin;
            double differenceDebutFin = tacheFin - tacheDebut;

            if (tacheDebut < start)
            {
                if (type == 2)
                {
                    while ((tacheDebut < start))
                    {
                        tacheDebut += UnixXMois(tacheDebut, 12);
                    }
                }
                else
                {
                    if (bond == 1 || type == 1)
                    {
                        while ((tacheDebut + UnixXMois(tacheDebut, 12) < start))
                        {
                            tacheDebut += UnixXMois(tacheDebut, 12);
                        }
                    }

                    if (type == 1)
                    {
                        while (tacheDebut < start)
                        {
                            tacheDebut += UnixXMois(tacheDebut, bond);
                        }
                    }
                    else
                    {
                        if (bond == 1)
                        {
                            while ((tacheDebut + UnixXMois(tacheDebut, 1) < start))
                            {
                                tacheDebut += UnixXMois(tacheDebut, 1);
                            }
                        }
                        while (tacheDebut < start)
                        {
                            tacheDebut += UnixXJour(bond);
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

                    if (type == 2)
                    {
                        tacheDebut += UnixXMois(tacheDebut, 12);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }

                    if (type == 1)
                    {
                        tacheDebut += UnixXMois(tacheDebut, bond);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }
                    else
                    {
                        tacheDebut += UnixXJour(bond);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }
                }
            }

            return date;
        }

        private static List<Tache> TacheRecurrente(Tache tache, double start, double end, int bond, int type)
        {
            List<Tache> taches = new List<Tache>();
            double tacheDebut = tache.unixDebut;
            double tacheFin = tache.unixFin;
            double differenceDebutFin = tacheFin - tacheDebut;

            if (tacheDebut < start)
            {
                if (type == 2)
                {
                    while ((tacheDebut < start))
                    {
                        tacheDebut += UnixXMois(tacheDebut, 12);
                    }
                }
                else
                {
                    if (bond == 1 || type == 1)
                    {
                        while ((tacheDebut + UnixXMois(tacheDebut, 12) < start))
                        {
                            tacheDebut += UnixXMois(tacheDebut, 12);
                        }
                    }

                    if (type == 1)
                    {
                        while (tacheDebut < start)
                        {
                            tacheDebut += UnixXMois(tacheDebut, bond);
                        }
                    }
                    else
                    {
                        if (bond == 1)
                        {
                            while ((tacheDebut + UnixXMois(tacheDebut, 1) < start))
                            {
                                tacheDebut += UnixXMois(tacheDebut, 1);
                            }
                        }
                        while (tacheDebut < start)
                        {
                            tacheDebut += UnixXJour(bond);
                        }
                    }
                }

                tacheFin = tacheDebut + differenceDebutFin;
            }

            if (tacheDebut > start && tacheDebut < end)
            {
                while (tacheDebut < end)
                {
                    DateTime dateTache = UnixTimeStampToDateTime(tacheDebut);
                    tache.Annee = Convert.ToString(dateTache.Year);
                    tache.Mois = Convert.ToString(dateTache.Month);
                    tache.Jour = Convert.ToString(dateTache.Day);
                    tache.unixDebut = tacheDebut;
                    tache.unixFin = tacheFin;
                    taches.Add(new Tache(tache));

                    if (type == 2)
                    {
                        tacheDebut += UnixXMois(tacheDebut, 12);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }

                    if (type == 1)
                    {
                        tacheDebut += UnixXMois(tacheDebut, bond);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }
                    else
                    {
                        tacheDebut += UnixXJour(bond);
                        tacheFin = tacheDebut + differenceDebutFin;
                    }
                }
            }

            return taches;
        }

        public static List<string[]> TraitementRecurrence(Tache tache, double start, double end)
        {
            recurrence recurrence =
                        (recurrence)Enum.ToObject(typeof(recurrence), tache.Recurrence);
            List<string[]> result = null;

            switch (recurrence)
            {
                case recurrence.ChaqueJour:
                    result = DatesTacheRecurrente(tache, start, end, 1, 0);
                    break;
                case recurrence.ChaqueSemaine:
                    result = DatesTacheRecurrente(tache, start, end, 7, 0);
                    break;
                case recurrence.DeuxSemaines:
                    result = DatesTacheRecurrente(tache, start, end, 14, 0);
                    break;
                case recurrence.TroisSemaine:
                    result = DatesTacheRecurrente(tache, start, end, 21, 0);
                    break;
                case recurrence.ChaqueMois:
                    result = DatesTacheRecurrente(tache, start, end, 1, 1);
                    break;
                case recurrence.TroisMois:
                    result = DatesTacheRecurrente(tache, start, end, 3, 1);
                    break;
                case recurrence.QuatreMois:
                    result = DatesTacheRecurrente(tache, start, end, 4, 1);
                    break;
                case recurrence.ChaqueAnnee:
                    result = DatesTacheRecurrente(tache, start, end, 0, 2);
                    break;
            }

            return result;

        }

        public static List<Tache> TraitementRecurrenceTache(Tache tache, double start, double end)
        {
            recurrence recurrence =
                        (recurrence)Enum.ToObject(typeof(recurrence), tache.Recurrence);
            List<Tache> result = null;

            switch (recurrence)
            {
                case recurrence.ChaqueJour:
                    result = TacheRecurrente(tache, start, end, 1, 0);
                    break;
                case recurrence.ChaqueSemaine:
                    result = TacheRecurrente(tache, start, end, 7, 0);
                    break;
                case recurrence.DeuxSemaines:
                    result = TacheRecurrente(tache, start, end, 14, 0);
                    break;
                case recurrence.TroisSemaine:
                    result = TacheRecurrente(tache, start, end, 21, 0);
                    break;
                case recurrence.ChaqueMois:
                    result = TacheRecurrente(tache, start, end, 1, 1);
                    break;
                case recurrence.TroisMois:
                    result = TacheRecurrente(tache, start, end, 3, 1);
                    break;
                case recurrence.QuatreMois:
                    result = TacheRecurrente(tache, start, end, 4, 1);
                    break;
                case recurrence.ChaqueAnnee:
                    result = TacheRecurrente(tache, start, end, 0, 2);
                    break;
            }

            return result;

        }
    }
}

