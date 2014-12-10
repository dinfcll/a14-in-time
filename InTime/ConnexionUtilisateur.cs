using InTime.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime
{
    public interface ConnexionUtilisateur
    {
        void CreerUsager(RegisterModel model);
        void LoginUsager(RegisterModel model);
        void Cookie(string username);
    }
}
