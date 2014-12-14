using InTime.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InTime.Controllers
{
    public class ModifierTacheController : Controller
    {
        public ActionResult Modification(int? id, double? dep, double? fn, bool? Existe)
        {
            int idTache;
            try
            {
                if (id == null)
                {
                    return View(UrlErreur.ErreurGeneral);
                }
                else
                    if (User.Identity.IsAuthenticated)
                    {
                        try
                        {
                            idTache = id.GetValueOrDefault();
                            Tache tache = RequeteSql.RechercherTache(idTache);

                            if (dep != null && fn != null)
                            {
                                tache.unixDebut = Convert.ToDouble(dep);
                                tache.unixFin = Convert.ToDouble(fn);
                                tache.Description = RequeteSql.RechercheDescSupplTache(idTache, tache.unixDebut) ?? tache.Description;
                                ViewBag.Modif = true;
                            }
                            else
                            {
                                ViewBag.Modif = false;
                            }

                            Tache.PreparationPourAffichage(ref tache);
                            Tache.InitChampsTache(ref tache);
                            ViewData["Tache"] = tache;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }

                        return View();
                    }
                    else
                    {
                        return View(UrlErreur.Authentification);
                    }
            }
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

        [HttpPost]
        public ActionResult Modification(Tache Model, string modif, bool Existe)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {


                    if (!Validations(Model))
                    {
                        TempData["Modification"] = Messages.RequeteSql.Echec;
                    }
                    else
                    {
                        try
                        {
                            string SqlCommande;
                            List<SqlParameter> listParametres = new List<SqlParameter>();
                            int UserId = Int32.Parse(InTime.Models.Cookie.ObtenirCookie(User.Identity.Name));
                            double unixDebut = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateDebut(Model));
                            double unixFin = TraitementDate.DateTimeToUnixTimestamp(TraitementDate.DateFin(Model));
                            if (modif == "False")
                            {
                                SqlCommande = "UPDATE Taches set NomTache=@NomTache,Lieu=@Lieu,Description=@Description,"
                                + "DateDebut=@DateDebut,DateFin=@DateFin,HRappel=@HRappel,mRappel=@mRappel,"
                                + "recurrence=@recurrence, PriorityColor=@PriorityColor WHERE UserId=@UserId AND IdTache=@IdTache;";
                                listParametres.Add(new SqlParameter("@IdTache", Model.IdTache));
                                listParametres.Add(new SqlParameter("@UserId", UserId));
                                listParametres.Add(new SqlParameter("@NomTache", Model.NomTache));
                                listParametres.Add(new SqlParameter("@Lieu", Model.Lieu));
                                listParametres.Add(new SqlParameter("@DateDebut", unixDebut));
                                listParametres.Add(new SqlParameter("@DateFin", unixFin));
                                listParametres.Add(new SqlParameter("@Description", Model.Description));
                                listParametres.Add(new SqlParameter("@HRappel", SqlDbType.VarChar) { Value = Model.HRappel ?? (object)DBNull.Value });
                                listParametres.Add(new SqlParameter("@mRappel", SqlDbType.VarChar) { Value = Model.mRappel ?? (object)DBNull.Value });
                                listParametres.Add(new SqlParameter("@recurrence", Model.Recurrence));
                                listParametres.Add(new SqlParameter("@PriorityColor", Model.PriorityColor));
                            }
                            else
                            {
                                if (!Existe)
                                {
                                    SqlCommande = "INSERT INTO InfoSupplTacheRecurrente (IdTache,DateDebut,DateFin,Description)"
                                    + " VALUES (@IdTache,@DateDebut,@DateFin,@Description);";
                                }
                                else
                                {
                                    SqlCommande = "UPDATE InfoSupplTacheRecurrente SET Description=@Description WHERE IdTache=@IdTache "
                                        + "AND DateDebut=@DateDebut AND DateFin=@DateFin;";
                                }
                                listParametres.Add(new SqlParameter("@IdTache", Model.IdTache));
                                listParametres.Add(new SqlParameter("@DateDebut", unixDebut));
                                listParametres.Add(new SqlParameter("@DateFin", unixFin));
                                listParametres.Add(new SqlParameter("@Description", Model.Description));
                                listParametres.Add(new SqlParameter("@PriorityColor", Model.PriorityColor));
                            }
                            TempData["Modification"] = RequeteSql.ExecuteQuery(SqlCommande, listParametres) ? Messages.RequeteSql.Reussi : Messages.RequeteSql.Echec;
                        }
                        catch
                        {
                            TempData["Modification"] = Messages.RequeteSql.Echec;
                        }
                    }

                    return RedirectToAction("Taches", "ConsulterTache");
                }
                else
                {
                    return View(UrlErreur.Authentification);
                }
            }
            catch
            {
                return View(UrlErreur.ErreurGeneral);
            }
        }

        private bool Validations(Tache model)
        {
            const string strValidationMotContain = "Choisir";

            if ((model.Mois == null || model.Mois.Contains(strValidationMotContain)) ||
                (model.Annee == null) ||
                (model.Jour == null || model.Jour.Contains(strValidationMotContain)))
            {
                return false;
            }
            else
            {
                if (new DateTime(StrToInt(model.Annee), StrToInt(model.Mois), StrToInt(model.Jour)) < DateTime.Now.AddDays(-1))
                {
                    return false;
                }
            }

            if (model.HDebut == null || model.mDebut == null ||
                model.HFin == null || model.mFin == null ||
                model.HRappel != null && model.mRappel == null)
            {
                return false;
            }
            else
            {
                if (!ValHeureFinDebut(ref model))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValHeureFinDebut(ref Tache model)
        {
                if (StrToInt(model.HDebut) > StrToInt(model.HFin))
                {
                    return false;
                }
                else
                {
                    if (StrToInt(model.HDebut) == StrToInt(model.HFin) &&
                        StrToInt(model.mDebut) >= StrToInt(model.mFin))
                    {
                        return false;
                    }
                }

                return true;
        }

        int StrToInt(string nombre)
        {
            return Convert.ToInt32(nombre);
        }

    }
}
