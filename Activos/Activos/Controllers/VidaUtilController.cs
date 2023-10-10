using Activos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Activos.Controllers
{
    public class VidaUtilController : Controller
    {
        private ACTIVOSEntities16 db = new ACTIVOSEntities16();
        // GET: VidaUtil
        public ActionResult Index()
        {
            try
            {
                if (Session.Count > 0)
                {
                    var Vida = db.UDP_tbVidaUtil_ListarVidaUtil().ToList();

                    List<tbVidaUtil> VIDAUTIL = new List<tbVidaUtil>();

                    foreach (var item in Vida)
                    {
                        tbVidaUtil tbvidautil = new tbVidaUtil
                        {
                            viut_Id = item.viut_Id,
                            viut_Objeto = item.viut_Objeto,
                            viut_Descripcion = item.viut_Descripcion,
                            viut_VidaUtil = item.viut_VidaUtil,
                            viut_Estado = item.viut_Estado
                        };

                        VIDAUTIL.Add(tbvidautil);
                    }

                    if (VIDAUTIL != null)
                    {
                        return View(VIDAUTIL);
                    }
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Login");
                throw;
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(string Objeto, string Descripcion, string VidaUtil)
        {
            try
            {
                string var1 = Objeto.Trim();
                string var2 = Descripcion.Trim();
                string var3 = VidaUtil.Trim();
                if (Session.Count > 0)
                {
                    var resultado = db.UDP_tbVidaUtil_Insertar(var1, var2, int.Parse(var3),int.Parse(Session["usua_Id"].ToString())).FirstOrDefault();
                    if (resultado == 1)
                    {
                        return Json(new { success = true, correcto = 1 }, JsonRequestBehavior.AllowGet);
                    }
                    if (resultado == 2)
                    {
                        return Json(new { success = false, correcto = 2 }, JsonRequestBehavior.AllowGet);
                    }
                    if (resultado == 2)
                    {
                        return Json(new { success = false, correcto = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        public ActionResult CargarDatosEditar(string id)
        {
            try
            {
                if (Session.Count > 0)
                {

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tbVidaUtil tbvidautil = db.tbVidaUtil.Find(int.Parse(id));
                    if (tbvidautil.viut_Id == 0)
                    {
                        return HttpNotFound();
                    }
                    return Json(new { success = true, viut_Id = tbvidautil.viut_Id.ToString(), Descripcion = tbvidautil.viut_Descripcion.ToString(), vidaUtil = tbvidautil.viut_VidaUtil.ToString(), Objeto = tbvidautil.viut_Objeto.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error500", "Home");
                throw;
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Editar(string Objeto, string Descripcion, string VidaUtil, string VidaUtilId)
        {
            try
            {
                if (Session.Count > 0)
                {
                    var resultado = db.UDP_tbVidaUtil_Editar(int.Parse(VidaUtilId), Objeto, Descripcion,int.Parse(VidaUtil), int.Parse(Session["usua_Id"].ToString())).FirstOrDefault();
                    if (resultado == 1)
                    {
                        return Json(new { success = true, correcto = true }, JsonRequestBehavior.AllowGet);
                    }
                    if (resultado == 0)
                    {
                        return Json(new { success = false, correcto = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        public ActionResult Details(int? id)
        {
            try
            {
                if (Session.Count > 0)
                {
                    if (id == null)
                    {
                        return RedirectToAction("Error500", "Home");
                    }

                    tbVidaUtil tbvidautil = db.tbVidaUtil.Find(id);
                    tbUsuarios tbusuario = db.tbUsuarios.Find(id);
                    tbUsuarios AS = db.tbUsuarios.Find(tbvidautil.usua_UsuarioCreacion);
                    tbUsuarios AS1 = db.tbUsuarios.Find(tbvidautil.usua_Modificacion);
                    Session["UsuarioCrea"] = AS.usua_UsuarioNombre + ' ' + AS.usua_UsuarioApellido;
                    Session["FechaCreUsua"] = tbvidautil.viut_FechaCreacion;                 
                    if (tbvidautil.usua_Modificacion != null && tbvidautil.usua_Modificacion != 0)
                    {
                        Session["UsuarioMod"] = AS1.usua_UsuarioNombre + ' ' + AS1.usua_UsuarioApellido;
                        Session["FechaModUsua"] = tbvidautil.viut_FechaModificacion;
                    }
                   

                    if (tbvidautil == null)
                    {
                        return RedirectToAction("Error404", "Home");
                    }
                    return View(tbvidautil);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }
    }
}