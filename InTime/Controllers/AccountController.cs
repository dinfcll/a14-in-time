using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using InTime.Filters;
using InTime.Models;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace InTime.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                SqlConnection con = null;
                try
                {
                    con = RequeteSql.ConnexionBD(con);
                    int id = RequeteSql.RechercheID(con, model.UserName);

                    InTime.Models.Cookie.CreationCookie(model.UserName, Convert.ToString(id), InTime.Models.Cookie.Heure);
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
                return RedirectToLocal(returnUrl);
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            ModelState.AddModelError("", "Le nom d'utilisateur ou mot de passe fourni est incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

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

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Tentative d'inscription de l'utilisateur
                try
                {
                    if (!UtilisateurPresent(model.UserName, model.Password))
                    {
                        ModelState.AddModelError("", "Votre nom d'utilisateur ou votre courriel n'est pas unique");
                    }
                    else
                    {
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { model.Nom, model.Prenom, model.Email });
                        WebSecurity.Login(model.UserName, model.Password);
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        private bool UtilisateurPresent(string NomUtilisateur, string adresseCourriel)
        {
            String query = "SELECT * FROM UserProfile";
            SqlDataReader reader = RequeteSql.Select(query, null);

            while (reader.Read())
            {
                Object[] values = new Object[reader.FieldCount];
                int fieldCounts = reader.GetValues(values);
                var account = new RegisterModel()
                {
                    UserName = Convert.ToString(values[1]),
                    Email = Convert.ToString(values[4])
                };

                if (account.UserName.ToLower() == NomUtilisateur.ToLower() ||
                    account.Email.ToLower() == adresseCourriel.ToLower())
                {
                    return false;
                }
            }

            return true;
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Dissocier uniquement le compte si l'utilisateur actuellement connecté est le propriétaire
            if (ownerAccount == User.Identity.Name)
            {
                // Utiliser une transaction pour empêcher l'utilisateur de supprimer ses dernières informations d'identification de connexion
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

        //
        // GET: /Account/Manage

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

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword va lever une exception plutôt que de renvoyer la valeur False dans certains scénarios de défaillance.
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
                // L'utilisateur n'a pas de mot de passe local. Veuillez donc supprimer les erreurs de validation provoquées par un
                // champ OldPassword manquant
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

            // Si nous sommes arrivés là, quelque chose a échoué, réafficher le formulaire
            return View(model);
        }

        public ActionResult Renseignements()
        {
            if (User.Identity.IsAuthenticated)
            {
                RegisterModel userProfile = null;

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
                    userProfile = new RegisterModel()
                    {
                        Nom = Convert.ToString(values[2]),
                        Prenom = Convert.ToString(values[3]),
                        Email = Convert.ToString(values[4])
                    };
                }
                if (userProfile == null)
                {
                    return HttpNotFound();
                }
                ViewData["utilisateur"] = userProfile;


                return View();
            }
            else
            {
                return View(UrlErreur.Authentification);
            }
        }

        [HttpPost]
        public ActionResult Renseignements(RegisterModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.Remove("Password");
                ModelState.Remove("Username");
                if (!ModelState.IsValid)
                {
                    RegisterModel userProfile = null;
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
                        userProfile = new RegisterModel()
                        {
                            Nom = Convert.ToString(values[2]),
                            Prenom = Convert.ToString(values[3]),
                            Email = Convert.ToString(values[4])
                        };
                    }

                    if (userProfile == null)
                    {
                        return HttpNotFound();
                    }

                    ViewData["utilisateur"] = userProfile;
                    return View();
                }
                else
                {

                    if (ModifRenseig(model, Int32.Parse(Cookie.ObtenirCookie(User.Identity.Name))))
                    {
                        ViewBag.Message = RequeteSql.Message.Reussi;
                    }
                    else
                    {
                        ViewBag.Message = RequeteSql.Message.Echec;
                    }

                    RegisterModel userProfile = new RegisterModel()
                    {
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        Email = model.Email
                    };

                    ViewData["utilisateur"] = userProfile;

                    return View();
                }
            }
            else
            {
                return View(UrlErreur.Authentification);
            }
        }

        private bool ModifRenseig(RegisterModel model, int UserId)
        {
            string SqlUpdate = "UPDATE UserProfile Set Nom = @Nom, Prenom = @Prenom, Email = @Email WHERE UserId = @Id;";

            List<SqlParameter> Parametres = new List<SqlParameter>
            {
                new SqlParameter("@Nom", model.Nom),
                new SqlParameter("@Prenom", model.Prenom),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@Id", UserId)
            };

            return RequeteSql.ExecuteQuery(SqlUpdate, Parametres);
        }

        #region Applications auxiliaires
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
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
        #endregion
    }
}
