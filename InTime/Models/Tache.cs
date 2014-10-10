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
        public int IdTache;
        public int UserId;


        [Display(Name = "Nom de la tâche")]
        [Required(ErrorMessage = "Vous devez donner un nom à votre tâche")]
        [StringLength(30)]
        public string NomTache { get; set; }

        [Display(Name = "Nom du lieu")]
        [Required(ErrorMessage = "Vous devez donner un nom de lieu à votre tâche")]
        [StringLength(30)]
        public string Lieu { get; set; }

        [Display(Name = "Jour")]
        public string Jour { get; set; }

        [Display(Name = "Mois")]
        public string Mois { get; set; }

        [Display(Name = "Année")]
        public string Annee { get; set; }

        [StringLength(300, ErrorMessage = "La description de la tâche est trop long")]
        public string Description { get; set; }

        public string HDebut { get; set; }
        public string mDebut { get; set; }
        public string HFin { get; set; }
        public string mFin { get; set; }
        public string HRappel { get; set; }
        public string mRappel { get; set; }
    }

    public class InTime: DbContext
    {
        public DbSet<Tache> Taches { get; set; }
    }
}

