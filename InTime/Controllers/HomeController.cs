using System;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;

namespace InTime.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session[User.Identity.Name]!=null)
            {
                SqlConnection con = null;
                try
                {
                    con = RequeteSql.ConnexionBD(con);
                    int id = RequeteSql.RechercheID(con, User.Identity.Name);

                    Session[User.Identity.Name] = id;
                } 
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }

            ViewBag.Message = "Soyez toujours à l'heure pour un rendez-vous! ";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(GMail information)
        {
            if (ModelState.IsValid)
            {
                GMail mailer = new GMail(information.Subject, information.Body, true);
                mailer.Send();
                TempData["message"] = RequeteSql.Message.Reussi;

                return RedirectToAction("Contact", "Home");
            }

                return View();
        }
    }
}

