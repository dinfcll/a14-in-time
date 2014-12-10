using InTime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime
{
    public class DummyConnexion : ConnexionUtilisateur
    {
        //Cette classe vide est pour la création d'un utilisateur fictif dans le UnitTest1
       public void CreerUsager(RegisterModel model)
        {

        }

       public void LoginUsager(RegisterModel model)
        {

        }
       public void Cookie(string username)
       {

       }
    }
}

