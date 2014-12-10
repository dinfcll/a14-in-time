using InTime.Models;
using InTime.Controllers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime
{
    public class RealConnexion : ConnexionUtilisateur
    {
        public void CreerUsager(RegisterModel model)
        {
            WebMatrix.WebData.WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { model.Nom, model.Prenom, model.Email });
        }

        public void LoginUsager(RegisterModel model)
        {
            WebMatrix.WebData.WebSecurity.Login(model.UserName, model.Password);
        }

        public void Cookie(string username)
        {
           AccountController Account = new AccountController();
           Account.CookieNomUtilisateur(username);
        }
    }
}
