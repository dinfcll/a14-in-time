using InTime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime
{
    public class RealConnexion : ConnexionUtilisateur
    {
        void CreerUsager(RegisterModel model)
        {
            WebMatrix.WebData.WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { model.Nom, model.Prenom, model.Email });
        }

        void LoginUsager(RegisterModel model)
        {
            WebMatrix.WebData.WebSecurity.Login(model.UserName, model.Password);
        }
    }
}