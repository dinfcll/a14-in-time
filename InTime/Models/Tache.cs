using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;


namespace InTime.Models
{
    public class Tache
    {
        public static List<string> tempsHeure
        {
            get
            {
                List<string> Heures = new List<string>{
                                    "00","01","02","03","04","05","06","07","08","09","10",
                                    "11","12","13","14","15","16","17","18","19","20","21",
                                    "22","23"
                                  };
                return Heures;
            }
        }

        public static List<string> tempsMinutes
        {
            get
            {
                List<string> minutes = new List<string>{ "00", "15", "30", "45" };
                return minutes;
            }
        }

        public static List<string> options
        {
            get
            {
                List<string> option = new List<string>{
                                   "Aucune","À chaque jour", "Chaque semaine", "Au deux semaines", " Au trois semaines",
                                   "À chaque mois", "Au trois mois", "Au quatre mois", "À chaque année"
                                  };
                return option;
            }
        }



        public int IdTache { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Nom de la tâche")]
        [Required(ErrorMessage = "Vous devez donner un nom à votre tâchem")]
        [StringLength(50, ErrorMessage = "Le nom de votre tâche est trop longm")]
        public string NomTache { get; set; }

        [Display(Name = "Nom du lieu")]
        [Required(ErrorMessage = "Vous devez donner un nom de lieu à votre tâchem")]
        [StringLength(50, ErrorMessage = "Votre nom de lieu est trop long.")]
        public string Lieu { get; set; }

        [Display(Name = "Jour")]
        public string Jour { get; set; }

        [Display(Name = "Mois")]
        public string Mois { get; set; }

        [Display(Name = "Année")]
        public string Annee { get; set; }

        [StringLength(500, ErrorMessage = "La description de la tâche est trop long.")]
        public string Description { get; set; }

        public string HDebut { get; set; }
        public string mDebut { get; set; }
        public string HFin { get; set; }
        public string mFin { get; set; }
        public string HRappel { get; set; }
        public string mRappel { get; set; }
        public string Reccurence { get; set; }
    }
}

