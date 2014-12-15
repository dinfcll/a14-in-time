using InTime.Controllers;

namespace InTime.Models
{
    public class RealConnexion : ConnexionUtilisateur
    {
        public void CreerUsager(RegisterModel model)
        {
            WebMatrix.WebData.WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { model.Nom, model.Prenom, model.Email, model.Categorie });
        }

        public void LoginUsager(RegisterModel model)
        {
            WebMatrix.WebData.WebSecurity.Login(model.UserName, model.Password);
        }

        public void Cookie(string username)
        {
            AccountController account = new AccountController();
            account.CookieNomUtilisateur(username);
        }
    }
}
