namespace InTime.Models
{
    public interface ConnexionUtilisateur
    {
        void CreerUsager(RegisterModel model);
        void LoginUsager(RegisterModel model);
        void Cookie(string username);
    }
}