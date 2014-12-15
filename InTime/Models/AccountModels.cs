using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.WebPages.Html;

namespace InTime.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base(RequeteSql.connectionString)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire.")]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; }
        

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Mémoriser le mot de passe ?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire.")]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe ")]
        [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Le prenom est obligatoire.")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'adresse courriel est obligatoire.")]
        [RegularExpression(@".*@.*")]
        public string Email { get; set; }
        public string Categorie { get; set; }
        public string TypeConnec{ get; set; }

        public static List<SelectListItem> ChoixCategories
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Travail", Value = "Travail"},
                    new SelectListItem { Text = "École", Value = "École"},
                    new SelectListItem { Text = "Général", Value = "Général"}
                };
            }
        }

        public static int ColumnUsername
        {
            get
            {
                return 1;
            }
        }

        public static int ColumnNom
        {
            get
            {
                return 2;
            }
        }

        public static int ColumnPrenom
        {
            get
            {
                return 3;
            }
        }

        public static int ColumnCourriel
        {
            get
            {
                return 4;
            }
        }

        public static int ColumnCategorie
        {
            get
            {
                return 5;
            }
        }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
