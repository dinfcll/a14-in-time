using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace InTime.Models
{
    public class AjoutTache
    {
        [Required(ErrorMessage = "Vous devez donner un nom à votre tâche")]
        [StringLength(30)]
        public string m_strNomTache {get; set;}

        [Required(ErrorMessage = "Vous devez donner un nom de lieu à votre tâche")]
        [StringLength(30)]
        public string m_strLieu { get; set; }


        public string m_jour { get; set; }

        public string m_mois { get; set; }
      
        public string m_annee { get; set; }

        [StringLength(300, ErrorMessage = "La description de la tâche est trop long")]
        public string m_strDescTache { get; set; }

        public string m_debHeure { get; set; }
        public string m_debMin { get; set; }
        public string m_finHeure { get; set; }
        public string m_finMin { get; set; }
        public string m_rappelHeure { get; set; }
        public string m_rappelMin { get; set; }
    }
}