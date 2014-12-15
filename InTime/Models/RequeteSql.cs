using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace InTime.Models
{
    public static class RequeteSql
    {
        public const string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=EQUIPE-02;Integrated Security=True";

        public static SqlConnection ConnexionBD()
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            return con;
        }

        public static int RechercheID(SqlConnection con, string username)
        {
            const string sqlrId = "SELECT * FROM UserProfile where UserName=@NomUtilisateur;";

            SqlCommand cmdId = new SqlCommand(sqlrId, con);
            List<SqlParameter> parametres = new List<SqlParameter>
            {
                new SqlParameter("@NomUtilisateur",username)
            };
            cmdId.Parameters.AddRange(parametres.ToArray<SqlParameter>());

            return (Int32)cmdId.ExecuteScalar();
        }

        public static SqlDataReader Select(string query, List<SqlParameter> parametres)
        {
            try
            {
                SqlConnection con = ConnexionBD();
                SqlCommand cmdQuery = new SqlCommand(query, con);

                if (parametres != null)
                {
                    cmdQuery.Parameters.AddRange(parametres.ToArray<SqlParameter>());
                }
                SqlDataReader reader = cmdQuery.ExecuteReader();

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static bool ExecuteQuery(string query, List<SqlParameter> parametres)
        {
            try
            {
                SqlConnection con = ConnexionBD();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddRange(parametres.ToArray<SqlParameter>());
                cmd.ExecuteNonQuery();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static String RechercheDescSupplTache(int id, double debut)
        {
            const string queryString = "SELECT * FROM InfoSupplTacheRecurrente WHERE IdTache=@Id AND DateDebut=@Debut";
            List<SqlParameter> parametres = new List<SqlParameter>
                            {
                                new SqlParameter("@Id", id),
                                new SqlParameter("@Debut", debut)
                            };
            SqlDataReader reader = Select(queryString, parametres);
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
            const string queryString = "SELECT * FROM Taches where IdTache=@Id";
            List<SqlParameter> parametre = new List<SqlParameter>
                        {
                            new SqlParameter("@Id", id)
                        };

            SqlDataReader reader = Select(queryString, parametre);
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
