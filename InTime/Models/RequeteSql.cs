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

        public static int RechercheID(SqlConnection con, string username)
        {
            string SqlrId = "SELECT * FROM UserProfile where UserName=@NomUtilisateur;";

            SqlCommand cmdId = new SqlCommand(SqlrId, con);
            List<SqlParameter> Parametres = new List<SqlParameter>
            {
                new SqlParameter("@NomUtilisateur",username)
            };

            if (Parametres != null)
            {
                cmdId.Parameters.AddRange(Parametres.ToArray<SqlParameter>());
            }

            return (Int32)cmdId.ExecuteScalar();
        }

        public static SqlDataReader Select(string Query, List<SqlParameter> Parametres)
        {
            SqlConnection con = null;
            try
            {
                con = RequeteSql.ConnexionBD(con);
                SqlCommand cmdQuery = new SqlCommand(Query, con);

                if (Parametres != null)
                {
                    cmdQuery.Parameters.AddRange(Parametres.ToArray<SqlParameter>());
                }
                SqlDataReader reader = cmdQuery.ExecuteReader();

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static bool ExecuteQuery(string Query, List<SqlParameter> Parametres)
        {
            SqlConnection con = null;
            try
            {
                con = RequeteSql.ConnexionBD(con);
                SqlCommand cmd = new SqlCommand(Query, con);
                cmd.Parameters.AddRange(Parametres.ToArray<SqlParameter>());
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static String RechercheDescSupplTache(int id, double Debut)
        {
            string queryString = "SELECT * FROM InfoSupplTacheRecurrente WHERE IdTache=@Id AND DateDebut=@Debut";
            List<SqlParameter> Parametres = new List<SqlParameter>
                            {
                                new SqlParameter("@Id", id),
                                new SqlParameter("@Debut", Debut)
                            };
            SqlDataReader reader = RequeteSql.Select(queryString, Parametres);
            while (reader.Read())
            {
                Object[] values = new Object[reader.FieldCount];
                reader.GetValues(values);

                return Convert.ToString(values[4]);
            }

            return null;
        }


        public static Tache RechercherTache(int id)
        {
            string queryString = "SELECT * FROM Taches where IdTache=@Id";
            List<SqlParameter> Parametre = new List<SqlParameter>
                        {
                            new SqlParameter("@Id", id)
                        };

            SqlDataReader reader = RequeteSql.Select(queryString, Parametre);
            while (reader.Read())
            {
                Object[] values = new Object[reader.FieldCount];
                reader.GetValues(values);

                return Tache.ObtenirTache(values);
            }

            return null;
        }
    }
}

