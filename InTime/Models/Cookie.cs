using System;
using System.Web;

namespace InTime.Models
{
    public static class Cookie
    {
        public static void CreationCookie(string utilisateur, string valeur, TimeSpan tempsSurvie)
        {
            
            //On rajoute une valeur a la fin, car MVC cree un cookie au nom de l'utilisateur.
            HttpCookie cookie = new HttpCookie(utilisateur+"1", valeur);

            if (HttpContext.Current.Request.Cookies[utilisateur+"1"] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[utilisateur+"1"];
                cookieOld.Expires = DateTime.Now.Add(tempsSurvie);
                cookieOld.Value = cookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                cookie.Expires = DateTime.Now.Add(tempsSurvie);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static TimeSpan Heure
        {
            get
            {
                return new TimeSpan(1, 0, 0);
            }
        }

        public static TimeSpan Journee
        {
            get
            {
                return new TimeSpan(24, 0, 0);
            }
        }

        public static string ObtenirCookie(string nomCookie)
        {
            string value = "";
            HttpCookie cookie = HttpContext.Current.Request.Cookies[nomCookie+"1"];

            if (cookie != null)
            {
                value = cookie.Value;
            }
            return value;
        }
    }
}

