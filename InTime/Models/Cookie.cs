using System;
using System.Web;
using System.Web.Security;

namespace InTime.Models
{
    public class Cookie
    {
        public static void SetCookie(string username, string value, TimeSpan expires)
        {
            //On rajoute une valeur a la fin, car MVC cree un cookie au nom de l'utilisateur.
            HttpCookie Cookie = new HttpCookie(username+"1", value);

            if (HttpContext.Current.Request.Cookies[username+"1"] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[username];
                cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = Cookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                Cookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(Cookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = "";
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key+"1"];

            if (cookie != null)
            {
                value = cookie.Value;
            }
            return value;
        }
    }
}