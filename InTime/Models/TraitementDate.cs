using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace InTime.Models
{
    public static class TraitementDate
    {
        public enum Reccurence { 
            Aucune, ChaqueJour, ChaqueSemaine, DeuxSemaines, TroisSemaine, ChaqueMois, TroisMois,QuatreMois,ChaqueAnnee
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            return dtDateTime.AddSeconds(unixTimeStamp);
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
            catch(Exception ex)
            {
                return null;
            }
        }

        private static List<string[]> VerificationDateSemaine(Tache tache, double FinMois, int Bond)
        {
            bool Changement = false;
            List<string[]> date = new List<string[]>();
            DateTime debut = UnixTimeStampToDateTime(tache.unixDebut);
            DateTime fin = UnixTimeStampToDateTime(tache.unixFin);
            DateTime debutCalendrier = UnixTimeStampToDateTime(FinMois);
            debutCalendrier = debutCalendrier.AddMonths(-1);

            if (debut.Month <= debutCalendrier.Month && debut.Year <= debutCalendrier.Year)
            {
                if (debut.Year < debutCalendrier.Year)
                {
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddDays(Bond);
                    }
                }
                if (debut.Month < debutCalendrier.Month)
                {
                    while (debut.Month < debutCalendrier.Month)
                    {
                        debut = debut.AddDays(Bond);
                    }
                }
                Changement = true;
            }
            else
            {
                if (debut.Month > debutCalendrier.Month && debut.Year < debutCalendrier.Year)
                {
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddDays(Bond);
                    }
                    while (debut.Month < debutCalendrier.Month)
                    {
                        debut = debut.AddDays(Bond);
                    }
                    Changement = true;
                }
            }

            if (Changement)
            {
                int days = DateTime.DaysInMonth(debut.Year, debut.Month);

                while (debut.Month == debutCalendrier.Month)
                {
                    string dateDebut = TraitementDate.DateFormatCalendrier(
                    debut.Year, debut.Month, debut.Day, debut.Hour, debut.Minute);
                    string dateFin = TraitementDate.DateFormatCalendrier(
                    debut.Year, debut.Month, debut.Day, fin.Hour, fin.Minute);

                    date.Add(new string[] { tache.NomTache, dateDebut, dateFin, Convert.ToString(tache.IdTache) });
                    debut = debut.AddDays(Bond);
                }
            }

            return date;
        }

        private static List<string[]> VerificationMois(Tache tache,double FinMois,int Bond)
        {
            List<string[]> date = new List<string[]>();
            DateTime debut = UnixTimeStampToDateTime(tache.unixDebut);
            DateTime fin = UnixTimeStampToDateTime(tache.unixFin);
            DateTime debutCalendrier = UnixTimeStampToDateTime(FinMois);
            debutCalendrier = debutCalendrier.AddMonths(-1);
            bool Changement = false;

            int nMonth = 0;
            if (debut.Month <= debutCalendrier.Month && debut.Year <= debutCalendrier.Year)
            {
                nMonth = debut.Month;
                while (nMonth < debutCalendrier.Month)
                {
                    nMonth += Bond;
                }

                if (nMonth == debutCalendrier.Month)
                {
                    Changement = true;
                }

                if (debut.Year != debutCalendrier.Year)
                {
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddYears(1);
                    }
                }
            }
            else
                if (debut.Month > debutCalendrier.Month && debut.Year < debutCalendrier.Year)
                {
                    nMonth = debutCalendrier.Month;
                    while (nMonth < debut.Month)
                    {
                        nMonth += Bond;
                    }

                    if (nMonth == debut.Month)
                    {
                        Changement = true;
                    }

                    if (debut.Year != debutCalendrier.Year)
                    {
                        while (debut.Year < debutCalendrier.Year)
                        {
                            debut = debut.AddYears(1);
                        }
                    }
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


        public static List<string[]> ChaqueJour(Tache tache, double start)
        {
            bool Changement = false;
            bool Jour = false;
            List<string[]> date = new List<string[]>();
            DateTime debut = UnixTimeStampToDateTime(tache.unixDebut);
            DateTime fin = UnixTimeStampToDateTime(tache.unixFin);
            DateTime debutCalendrier = UnixTimeStampToDateTime(start);
            debutCalendrier = debutCalendrier.AddMonths(-1);

            if (debut.Month <= debutCalendrier.Month && debut.Year <= debutCalendrier.Year)
            {
                if (debut.Month < debutCalendrier.Month)
                {
                    while (debut.Month < debutCalendrier.Month)
                    {
                        debut = debut.AddMonths(1);
                    }
                    Jour = true;
                }
                if (debut.Year < debutCalendrier.Year)
                {
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddYears(1);
                    }
                    Jour = true;
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
                    while (debut.Year < debutCalendrier.Year)
                    {
                        debut = debut.AddYears(1);
                    }
                    Jour = true;
                }
            }


            if (Jour)
            {
                int days = DateTime.DaysInMonth(debut.Year, debut.Month);
                for (int day = 1; day <= days; day++)
                {
                    string dateDebut = TraitementDate.DateFormatCalendrier(
                    debut.Year, debut.Month, day, debut.Hour, debut.Minute);
                    string dateFin = TraitementDate.DateFormatCalendrier(
                    debut.Year, debut.Month, day, fin.Hour, fin.Minute);

                    date.Add(new string[] { tache.NomTache, dateDebut, dateFin, Convert.ToString(tache.IdTache) });
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

        public static List<string[]> ChaqueSemaine(Tache tache, double start)
        {
            return VerificationDateSemaine(tache, start, 7);
        }


        public static List<string[]> DeuxSemaine(Tache tache, double start)
        {
            return VerificationDateSemaine(tache,start,14);
        }

        public static List<string[]> TroisSemaine(Tache tache, double start)
        {
            return VerificationDateSemaine(tache, start, 21);
        }

        public static List<string[]> ChaqueMois(Tache tache, double start)
        {
            return VerificationMois(tache, start, 1);
        }


        public static List<string[]> TroisMois(Tache tache, double start)
        {
            return VerificationMois(tache, start, 3);
        }


        public static List<string[]> QuatreMois(Tache tache, double start)
        {
            return VerificationMois(tache,start,4);
        }


        public static List<string[]> ChaqueAnnee(Tache tache, double start)
        {
            List<string[]> date = new List<string[]>();
            DateTime debut = DateDebut(tache);
            DateTime fin = DateFin(tache);
            DateTime debutCalendrier = UnixTimeStampToDateTime(start);
            debutCalendrier = debutCalendrier.AddMonths(-1);
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

