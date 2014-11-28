using System;
using System.Web.Mvc;
using InTime.Filters;
using InTime.Models;
using InTime.Controllers;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InTime;

namespace UnitTestConnexion
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConnexionString()
        {
            //Given
            string Constring = @"Data Source=EQUIPE-02\SQLEXPRESS;Initial Catalog=InTime;Integrated Security=True";
            //When
            SqlConnection connect = new SqlConnection(Constring);
            //Then
            Assert.IsNotNull(connect);
        }

        [TestMethod]
        public void CreationUserValide()
        {
            //Given
            var Account = new AccountController();
            RegisterModel model = new RegisterModel();
            model.Nom = "Unit";
            model.Prenom = "Test";
            model.Email = "Unit@test.com";
            model.UserName = "UT";
            model.Password = "abc1234";
            model.ConfirmPassword = "abc1234";
            model.TypeConnec = "Dummy";
            //When
            var result = Account.Register(model) as RedirectToRouteResult;
            //Then
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }
    }
}
