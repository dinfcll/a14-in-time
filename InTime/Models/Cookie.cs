using System;
using System.Web;
using System.Web.Security;

namespace InTime.Models
{
    public static class Cookie
    {
        public static void CreationCookie(string Utilisateur, string Valeur, TimeSpan TempsSurvie)
        {
            //On rajoute une valeur a la fin, car MVC cree un cookie au nom de l'utilisateur.
            HttpCookie Cookie = new HttpCookie(Utilisateur+"1", Valeur);

            if (HttpContext.Current.Request.Cookies[Utilisateur+"1"] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[Utilisateur+"1"];
                cookieOld.Expires = DateTime.Now.Add(TempsSurvie);
                cookieOld.Value = Cookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                Cookie.Expires = DateTime.Now.Add(TempsSurvie);
                HttpContext.Current.Response.Cookies.Add(Cookie);
            }
        }
        public static string ObtenirCookie(string NomCookie)
        {
            string value = "";
            HttpCookie cookie = HttpContext.Current.Request.Cookies[NomCookie+"1"];

            if (cookie != null)
            {
                value = cookie.Value;
            }
            return value;
        }
    }
}