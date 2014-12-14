using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using InTime.Filters;
using InTime.Models;
using System.Data.SqlClient;

namespace InTime.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                CookieNomUtilisateur(model.UserName);

                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Le nom d'utilisateur ou mot de passe fourni est incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            if (Request.Cookies[User.Identity.Name] != null)
            {
                var cookie = new HttpCookie(User.Identity.Name);
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!UtilisateurPresent(model.UserName, model.Email))
                    {
                        ModelState.AddModelError("", "Votre nom d'utilisateur ou votre courriel n'est pas unique");
                    }
                    else
                    {
                        ConnexionUtilisateur Connexion;
                        if (model.TypeConnec == null)
                        {
                            Connexion = new RealConnexion();
                        }
                        else
                        {
                            Connexion = new DummyConnexion();
                        }
                        Connexion.CreerUsager(model);
                        Connexion.LoginUsager(model);
                        Connexion.Cookie(model.UserName);

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            if (ownerAccount == User.Identity.Name)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Votre mot de passe a été modifié."
                : message == ManageMessageId.SetPasswordSuccess ? "Votre mot de passe a été défini."
                : message == ManageMessageId.RemoveLoginSuccess ? "La connexion externe a été supprimée."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            try
            {
                bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                ViewBag.HasLocalPassword = hasLocalAccount;
                ViewBag.ReturnUrl = Url.Action("Manage");
                if (hasLocalAccount)
                {
                    if (ModelState.IsValid)
                    {
                        bool changePasswordSucceeded;
                        try
                        {
                            changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                        }
                        catch (Exception)
                        {
                            changePasswordSucceeded = false;
                        }

                        if (changePasswordSucceeded)
                        {
                            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Le mot de passe actuel est incorrect ou le nouveau mot de passe n'est pas valide.");
                        }
                    }
                }
                else
                {
                    ModelState state = ModelState["OldPassword"];
                    if (state != null)
                    {
                        state.Errors.Clear();
                    }
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("", String.Format("Le compte local ne peut pas être créé. Un compte portant le même nom \"{0}\" existe peut-être déjà.", User.Identity.Name));
                        }
                    }
                }

                return View(model);
            }
            catch
            {
                return View(UrlErreur.ErreurSourceInconnu);
            }
        }

        public ActionResult Renseignements(int? Affichage)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {

                    if (!ObtenirRensUtil())
                    {
                        return HttpNotFound();
                    }

                    Messages.ChampsBloquer Aff;
                    if (Affichage != null && Affichage == 1)
                    {
                        Aff = Messages.ChampsBloquer.Oui;
                    }
                    else
                    {
                        Aff = Messages.ChampsBloquer.Non;
                    }
                    TempData["Affichage"] = Aff;

                    return View();
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
            }
            catch
            {
                return View(UrlErreur.ErreurSourceInconnu);
            }
        }

        [HttpPost]
        public ActionResult Renseignements(RegisterModel model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("Username");
                    if (!ModelState.IsValid)
                    {
                        if (!ObtenirRensUtil())
                        {
                            return HttpNotFound();
                        }

                        return View();
                    }
                    else
                    {
                        if (ModifRenseig(model, Int32.Parse(Cookie.ObtenirCookie(User.Identity.Name))))
                        {
                            TempData["Message"] = Messages.RequeteSql.Reussi;
                        }
                        else
                        {
                            TempData["Message"] = Messages.RequeteSql.Echec;
                        }
                        ViewData["utilisateur"] = model;
                        TempData["Affichage"] = Messages.ChampsBloquer.Oui;

                        return View();
                    }
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
            }
            catch
            {
                return View(UrlErreur.ErreurSourceInconnu);
            }
        }

        private bool ObtenirRensUtil()
        {
            RegisterModel profile = null;
            try
            {
                string queryString = "SELECT * FROM UserProfile where UserId=@Id;";
                List<SqlParameter> Parametres = new List<SqlParameter>
                        {
                            new SqlParameter("@Id", InTime.Models.Cookie.ObtenirCookie(User.Identity.Name))
                        };

                SqlDataReader reader = RequeteSql.Select(queryString, Parametres);
                while (reader.Read())
                {
                    Object[] values = new Object[reader.FieldCount];
                    int fieldCounts = reader.GetValues(values);
                    profile = new RegisterModel
                    {
                        Nom = Convert.ToString(values[RegisterModel.ColumnNom]),
                        Prenom = Convert.ToString(values[RegisterModel.ColumnPrenom]),
                        Email = Convert.ToString(values[RegisterModel.ColumnCourriel]),
                        Categorie = Convert.ToString(values[RegisterModel.ColumnCategorie])
                    };
                }
                ViewBag.Categorie = profile.Categorie;

            }
            catch
            {
                return false;
            }

            if (profile == null)
            {
                return false;
            }
            ViewData["utilisateur"] = profile;

            return true;
        }

        private bool ModifRenseig(RegisterModel model, int UserId)
        {
            string SqlUpdate = "UPDATE UserProfile Set Nom = @Nom, Prenom = @Prenom, Email = @Email, Categorie = @Categorie WHERE UserId = @Id;";

            List<SqlParameter> Parametres = new List<SqlParameter>
            {
                new SqlParameter("@Nom", model.Nom),
                new SqlParameter("@Prenom", model.Prenom),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@Id", UserId),
                new SqlParameter("@Categorie",model.Categorie)
            };

            return RequeteSql.ExecuteQuery(SqlUpdate, Parametres);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("TacheForm", "Tache");
            }
        }

        private bool UtilisateurPresent(string NomUtilisateur, string adresseCourriel)
        {
            String query = "SELECT * FROM UserProfile WHERE UserName=@Username OR Email=@Email;";

            List<SqlParameter> listParametres = new List<SqlParameter>
                {
                    new SqlParameter("@UserName", NomUtilisateur),
                    new SqlParameter("@Email", adresseCourriel)
                };

            SqlDataReader reader = RequeteSql.Select(query, listParametres);
            if (reader.Read())
            {
                return false;
            }

            return true;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        public void CookieNomUtilisateur(string UserName)
        {
            SqlConnection con = null;
            try
            {
                con = RequeteSql.ConnexionBD(con);
                int id = RequeteSql.RechercheID(con, UserName);
                InTime.Models.Cookie.CreationCookie(UserName, Convert.ToString(id), InTime.Models.Cookie.Journee);
            }
            catch
            {   
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Consultez http://go.microsoft.com/fwlink/?LinkID=177550 pour
            // obtenir la liste complète des codes d'état.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Le nom d'utilisateur existe déjà. Entrez un nom d'utilisateur différent.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Un nom d'utilisateur pour cette adresse de messagerie existe déjà. Entrez une adresse de messagerie différente.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Le mot de passe fourni n'est pas valide. Entrez une valeur de mot de passe valide.";

                case MembershipCreateStatus.InvalidEmail:
                    return "L'adresse de messagerie fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La réponse de récupération du mot de passe fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La question de récupération du mot de passe fournie n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Le nom d'utilisateur fourni n'est pas valide. Vérifiez la valeur et réessayez.";

                case MembershipCreateStatus.ProviderError:
                    return "Le fournisseur d'authentification a retourné une erreur. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";

                case MembershipCreateStatus.UserRejected:
                    return "La demande de création d'utilisateur a été annulée. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";

                default:
                    return "Une erreur inconnue s'est produite. Vérifiez votre entrée et réessayez. Si le problème persiste, contactez votre administrateur système.";
            }
        }
    }
}
