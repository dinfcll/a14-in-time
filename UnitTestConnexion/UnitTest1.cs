using System;
using InTime.Filters;
using InTime.Models;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestConnexion
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConnexionString()
        {
            string Constring = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
            SqlConnection connect = new SqlConnection(Constring);
            Assert.IsNotNull(connect);
        }
    }
}
