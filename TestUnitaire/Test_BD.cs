using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InTime.Filters;
using InTime.Models;
using System.Data.SqlClient;

namespace TestUnitaire
{
    [TestClass]
    public class Test_BD
    {
        [TestMethod]
        public bool Connexionstring()
        {
            string ConString = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
            return true;
        }
    }
}
