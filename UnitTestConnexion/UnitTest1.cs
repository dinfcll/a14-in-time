using System.Web.Mvc;
using InTime.Models;
using InTime.Controllers;
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
            RegisterModel model = new RegisterModel
            {
                Nom = "Unit",
                Prenom = "Test",
                Email = "Unit@test.com",
                UserName = "UT",
                Password = "abc1234",
                ConfirmPassword = "abc1234",
                Categorie = "Travail",
                TypeConnec = "Dummy"
            };
            //When
            var result = Account.Register(model) as RedirectToRouteResult;
            //Then
            Assert.IsTrue(result.RouteValues["action"].Equals("Index"));
        }
        [TestMethod]
        public void AccesGererSansEtreConnecté()
        {
            //Given
            GererController controller = new GererController();
            //When
            ViewResult result = controller.GererForm() as ViewResult;
            //Then  
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesManageSansEtreConnecté()
        {
            //Given
            LocalPasswordModel model = new LocalPasswordModel
            {
                OldPassword = "abc123",
                NewPassword = "abc456",
                ConfirmPassword = "abc456"
            };
            AccountController controller = new AccountController();
            //When
            ViewResult result = controller.Manage(model) as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesRenseignementsSansEtreConnecté()
        {
            //Given
            RegisterModel model = new RegisterModel();
            AccountController controller = new AccountController();
            //When
            ViewResult result = controller.Renseignements(model) as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesAjouterTacheSansEtreConnecté()
        {
            //Given
            Tache model = new Tache
            {
                Annee = "2015",
                Mois = "10",
                Jour = "15",
                Description = "UnitTest",
                HDebut = "10",
                mDebut = "30",
                HFin = "14",
                mFin = "30",
                IdTache = 0,
                NomTache = "UnitTest5",
                Recurrence = 0,
                Lieu = "G-264",
                HRappel = "0",
                mRappel = "00",
                UserId = 0
            };
            AjouterTacheController controller = new AjouterTacheController();
            //When

            ViewResult result = controller.Index(model) as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesCalendrierSansEtreConnecté()
        {
            //Given
            RegisterModel model = new RegisterModel();
            CalendrierController controller = new CalendrierController();
            //When
            ViewResult result = controller.Index() as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesHistoriqueSansEtreConnecté()
        {
            //Given
            HistoriqueController controller = new HistoriqueController();
            //When
            ViewResult result = controller.Historique(null,null,null,null,null) as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesIndexSansEtreConnecté()
        {
            //Given
            int idTache = 2;
            ConsulterTacheController controller = new ConsulterTacheController();
            //When
            ViewResult result = controller.Index(idTache,null,null) as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesModificationSansEtreConnecté()
        {
            //Given
            ModifierTacheController controller = new ModifierTacheController();
            //When
            ViewResult result = controller.Modification(2, null, null, null) as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesTachesSansEtreConnecté()
        {
            //Given
            ConsulterTacheController controller = new ConsulterTacheController();
            //When
            ViewResult result = controller.Taches() as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesGererCompteSansEtreConnecté()
        {
            //Given
            GererCompteController controller = new GererCompteController();
            //When
            ViewResult result = controller.Index() as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
        [TestMethod]
        public void AccesTacheFormSansEtreConnecté()
        {
            //Given
            TacheController controller = new TacheController();
            //When
            ViewResult result = controller.TacheForm() as ViewResult;
            //Then
            Assert.AreEqual(UrlErreur.ErreurGeneral, result.ViewName);
        }
    }
}
