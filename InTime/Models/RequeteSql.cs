﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InTime.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace InTime.Models
{
    public static class RequeteSql
    {
        public const string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=EQUIPE-02;Integrated Security=True";

        public static SqlConnection ConnexionBD(SqlConnection con)
        {
            con = new SqlConnection(connectionString);
            con.Open();

            return con;
        }

        public static int RechercheID(SqlConnection con,string NomUtilisateur)
        {
            string SqlrId = string.Format("SELECT * FROM UserProfile where UserName='{0}'", NomUtilisateur);
            SqlCommand cmdId = new SqlCommand(SqlrId, con);
            int id = (Int32)cmdId.ExecuteScalar();

            return id;
        }

        public static SqlDataReader Select (string Query)
        {
            SqlConnection con = null;
            try
            {
                con = RequeteSql.ConnexionBD(con);
                SqlCommand cmdQuery = new SqlCommand(Query, con);
                SqlDataReader reader = cmdQuery.ExecuteReader();

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static bool ExecuteQuery(string Query)
        {
            SqlConnection con = null;
            try
            {
                con = RequeteSql.ConnexionBD(con);
                SqlCommand cmd = new SqlCommand(Query, con);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string EnleverApostrophe(string texte)
        {
            string Text = "";
            char apostrophe = Convert.ToChar(39);
            char doubleApostrophe = Convert.ToChar(34);

            foreach (char ch in texte)
            {
                if (ch == apostrophe)
                {

                    Text += doubleApostrophe;
                }
                else
                {
                    Text += ch;
                }
            }

            return Text;
        }


        public static string RemettreApostrophe(string texte)
        {
            string Text = "";
            char apostrophe = Convert.ToChar(39);
            char doubleApostrophe = Convert.ToChar(34);

            foreach (char ch in texte)
            {
                if (ch == doubleApostrophe)
                {

                    Text += Convert.ToChar(apostrophe);
                }
                else
                {
                    Text += ch;
                }
            }

            return Text;
        }
    }
}
