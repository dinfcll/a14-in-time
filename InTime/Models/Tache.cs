﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace InTime.Models
{
    public class Tache
    {
        public static List<string> tempsHeure
        {
            get
            {
                return new List<string>
                {
                                    "0","1","2","3","4","5","6","7","8","9","10",
                                    "11","12","13","14","15","16","17","18","19","20","21",
                                    "22","23"
                                  };
            }
        }

        public static List<string> tempsMinutes
        {
            get
            {
                return new List<string> { "00", "15", "30", "45" };
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
        public string Reccurence { get; set; }
        public double unixDebut { get; set; }
        public double unixFin { get; set; }
    }
}

