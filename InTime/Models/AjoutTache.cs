using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace InTime.Models
{
    public class AjoutTache
    {
        [Required(ErrorMessage = "Obligatoire")]
        [StringLength(30)]
        public string m_strNomTache {get; set;}

        [Required(ErrorMessage = "Obligatoire")]
        [StringLength(30)]
        public string m_strLieu { get; set; }

        [Required(ErrorMessage = "Obligatoire")]
        [RegularExpression(@"^[0-9]{1-2}")]
        public string m_jour { get; set; }

        [Required(ErrorMessage = "Obligatoire")]
        public string m_mois { get; set; }
      
        [Required(ErrorMessage = "Obligatoire")]
        [RegularExpression(@"^[0-9]{4}")]
        public string m_annee { get; set; }

        [StringLength(300,ErrorMessage="Votre texte est trop long")]
        public string m_strDescTache { get; set; }

        public string m_debHeure { get; set; }
        public string m_debMin { get; set; }
        public string m_finHeure { get; set; }
        public string m_finMin { get; set; }
        public string m_rappelHeure { get; set; }
        public string m_rappelMin { get; set; }
        public DateTime m_dtDebut { get; set; }
        public DateTime m_dtFin { get; set; }
        public DateTime m_dtRappel { get; set; }
    }
}