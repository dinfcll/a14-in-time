﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
