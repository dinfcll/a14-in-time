﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace InTime.Models
{
    public class Tache
    {
        public int IdTache { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Nom de la tâche")]
        [Required(ErrorMessage = "Vous devez donner un nom à votre tâche.")]
        [StringLength(50, ErrorMessage = "Le nom de votre tâche est trop longue.")]
        public string NomTache { get; set; }

        [Display(Name = "Nom du lieu")]
        [Required(ErrorMessage = "Vous devez donner un nom de lieu à votre tâche.")]
        [StringLength(50, ErrorMessage = "Votre nom de lieu est trop longue.")]
        public string Lieu { get; set; }

        [Display(Name = "Jour")]
        public string Jour { get; set; }

        [Display(Name = "Mois")]
        public string Mois { get; set; }

        [Display(Name = "Année")]
        public string Annee { get; set; }

        [StringLength(500, ErrorMessage = "La description de la tâche est trop longue.")]
        public string Description { get; set; }

        public string HDebut { get; set; }
        public string mDebut { get; set; }
        public string HFin { get; set; }
        public string mFin { get; set; }
        public string HRappel { get; set; }
        public string mRappel { get; set; }
        public int Recurrence { get; set; }
        public double unixDebut { get; set; }
        public double unixFin { get; set; }
        public string DateRappelCalendrier { get; set; }
        public string RecurrenceAffichage { get; set; }
        public string PriorityColor { get; set; }

        public Tache()
        {
        }

        public Tache(Tache tache)
        {
            IdTache = tache.IdTache;
            UserId = tache.UserId;
            NomTache = tache.NomTache;
            Lieu = tache.Lieu;
            Jour = tache.Jour;
            Mois = tache.Mois;
            Annee = tache.Annee;
            Description = tache.Description;
            HDebut = tache.HDebut;
            mDebut = tache.mDebut;
            HFin = tache.HFin;
            mFin = tache.mFin;
            HRappel = tache.HRappel;
            mRappel = tache.mRappel;
            Recurrence = tache.Recurrence;
            unixDebut = tache.unixDebut;
            unixFin = tache.unixFin;
            PriorityColor = tache.PriorityColor;
        }

        public static List<SelectListItem> tempsHeure
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "0", Value = "0"},
                    new SelectListItem { Text = "1", Value = "1"},
                    new SelectListItem { Text = "2", Value = "2"},
                    new SelectListItem { Text = "3", Value = "3"},
                    new SelectListItem { Text = "4", Value = "4"},
                    new SelectListItem { Text = "5", Value = "5"},
                    new SelectListItem { Text = "6", Value = "6"},
                    new SelectListItem { Text = "7", Value = "7"},
                    new SelectListItem { Text = "8", Value = "8"},
                    new SelectListItem { Text = "9", Value = "9"},
                    new SelectListItem { Text = "10", Value = "10"},
                    new SelectListItem { Text = "11", Value = "11"},
                    new SelectListItem { Text = "12", Value = "12"},
                    new SelectListItem { Text = "13", Value = "13"},
                    new SelectListItem { Text = "14", Value = "14"},
                    new SelectListItem { Text = "15", Value = "15"},
                    new SelectListItem { Text = "16", Value = "16"},
                    new SelectListItem { Text = "17", Value = "17"},
                    new SelectListItem { Text = "18", Value = "18"},
                    new SelectListItem { Text = "19", Value = "19"},
                    new SelectListItem { Text = "20", Value = "20"},
                    new SelectListItem { Text = "21", Value = "21"},
                    new SelectListItem { Text = "22", Value = "22"},
                    new SelectListItem { Text = "23", Value = "23"}
                };
            }
        }

        public static List<SelectListItem> tempsMinutes
        {
            get
            {
                return new List<SelectListItem> 
                { 
                    new SelectListItem { Text = "00", Value = "0"},
                    new SelectListItem { Text = "15", Value = "15"},
                    new SelectListItem { Text = "30", Value = "30"},
                    new SelectListItem { Text = "45", Value = "45"},
                };
            }
        }

        public static List<SelectListItem> options
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Aucune", Value = "0"},
                    new SelectListItem { Text = "À chaque jour", Value = "1"},
                    new SelectListItem { Text = "Chaque semaine", Value = "2"},
                    new SelectListItem { Text = "Aux deux semaines", Value = "3"},
                    new SelectListItem { Text = "Aux trois semaines", Value = "4"},
                    new SelectListItem { Text = "À chaque mois", Value = "5"},
                    new SelectListItem { Text = "Aux trois mois", Value = "6"},
                    new SelectListItem { Text = "Aux quatre mois", Value = "7"},
                    new SelectListItem { Text = "À chaque année", Value = "8"}
                };
            }
        }

        public static List<SelectListItem> Choix_Historique
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem {Text = "Les 3 derniers mois", Value = "1"},
                    new SelectListItem {Text = "Lapse de temps", Value = "2"},
                    new SelectListItem {Text = "Depuis le début", Value = "3"}
                };
            }
        }

        public static List<SelectListItem> les_mois
        {
            get
            {
                return new List<SelectListItem> 
                {
                    new SelectListItem { Text = "Janvier", Value = "1" },
                    new SelectListItem { Text = "Février", Value = "2" },
                    new SelectListItem { Text = "Mars", Value = "3" },
                    new SelectListItem { Text = "Avril", Value = "4" },
                    new SelectListItem { Text = "Mai", Value = "5" },
                    new SelectListItem { Text = "Juin", Value = "6" },
                    new SelectListItem { Text = "Juillet", Value = "7" },
                    new SelectListItem { Text = "Aout", Value = "8" },
                    new SelectListItem { Text = "Septembre", Value = "9" },
                    new SelectListItem { Text = "Octobre", Value = "10" },
                    new SelectListItem { Text = "Novembre", Value = "11" },
                    new SelectListItem { Text = "Décembre", Value = "12" }
                };
            }
        }

        public static string Nomrecurrence(int index)
        {
            switch (index)
            {
                case 0:
                    return "Aucune";
                case 1:
                    return "À chaque jour";
                case 2:
                    return "Chaque semaine";
                case 3:
                    return "Aux deux semaines";
                case 4:
                    return "Aux trois semaines";
                case 5:
                    return "À chaque mois";
                case 6:
                    return "Aux trois mois";
                case 7:
                    return "Aux quatre mois";
                case 8:
                    return "À chaque année";
                default:
                    return "Aucune";
            }
        }

        public static void InitChampsTache(ref Tache tache)
        {
            DateTime debut = TraitementDate.UnixTimeStampToDateTime(tache.unixDebut);
            DateTime fin = TraitementDate.UnixTimeStampToDateTime(tache.unixFin);

            tache.mDebut = InitialiseTempsMinute(Convert.ToString(debut.Minute));
            tache.mFin = InitialiseTempsMinute(Convert.ToString(fin.Minute));
            tache.mRappel = InitialiseTempsMinute(Convert.ToString(tache.mRappel));
            tache.HDebut = Convert.ToString(debut.Hour);
            tache.HFin = Convert.ToString(fin.Hour);
            tache.Jour = Convert.ToString(debut.Day);
            tache.Mois = Convert.ToString(debut.Month);
            tache.Annee = Convert.ToString(debut.Year);
        }

        private static string InitialiseTempsMinute(string Temps)
        {
            switch (Temps)
            {
                case "0":
                    return "1";
                case "15":
                    return "2";
                case "30":
                    return "3";
                case "45":
                    return "4";
                default:
                    return "1";
            }
        }
    }
}

