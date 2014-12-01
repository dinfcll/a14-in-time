using System;
using System.Web.Mvc;
using InTime.Filters;
using InTime.Models;
using InTime.Controllers;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InTime;
using System.Security.Principal;
using System.Threading;

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
        [TestMethod]
        public void AccesGererSansEtreConnecter()
        {
            //Given
            GererController controller = new GererController();
            //When
            ViewResult result = controller.GererForm() as ViewResult;
            //Then  
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesManageSansEtreConnecter()
        {
            //Given
            LocalPasswordModel model = new LocalPasswordModel();
            AccountController controller = new AccountController();
            //When
            model.OldPassword = "abc123";
            model.NewPassword = "abc456";
            model.ConfirmPassword = "abc456";
            ViewResult result = controller.Manage(model) as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesRenseignementsSansEtreConnecter()
        {
            //Given
            RegisterModel model = new RegisterModel();
            AccountController controller = new AccountController();
            //When
            ViewResult result = controller.Renseignements(model) as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesAjouterTacheSansEtreConnecter()
        {
            //Given
            Tache model = new Tache();
            AjouterTacheController controller = new AjouterTacheController();
            //When
            model.Annee = "2015";
            model.Mois = "10";
            model.Jour = "15";
            model.Description = "UnitTest";
            model.HDebut = "10";
            model.mDebut = "30";
            model.HFin = "14";
            model.mFin = "30";
            model.IdTache = 0;
            model.NomTache = "UnitTest5";
            model.Recurrence = 0;
            model.Lieu = "G-264";
            model.HRappel = "0";
            model.mRappel = "00";
            model.UserId = 0;

            ViewResult result = controller.Index(model) as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesCalendrierSansEtreConnecter()
        {
            //Given
            RegisterModel model = new RegisterModel();
            CalendrierController controller = new CalendrierController();
            //When
            ViewResult result = controller.Index() as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesHistoriqueSansEtreConnecter()
        {
            //Given
            ConsulterTacheController controller = new ConsulterTacheController();
            //When
            ViewResult result = controller.Historique(null,null,null,null,null) as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesIndexSansEtreConnecter()
        {
            //Given
            ConsulterTacheController controller = new ConsulterTacheController();
            //When
            ViewResult result = controller.Index(2,null,null) as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesModificationSansEtreConnecter()
        {
            //Given
            ConsulterTacheController controller = new ConsulterTacheController();
            //When
            ViewResult result = controller.ModifTache(2,null,null,null) as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesTachesSansEtreConnecter()
        {
            //Given
            ConsulterTacheController controller = new ConsulterTacheController();
            //When
            ViewResult result = controller.Taches("") as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
        [TestMethod]
        public void AccesGererCompteSansEtreConnecter()
        {
            //Given
            GererCompteController controller = new GererCompteController();
            //When
            ViewResult result = controller.Index() as ViewResult;
            //Then
            Assert.AreEqual("~/Views/ErreurAuthentification.cshtml", result.ViewName);
        }
    }
}
