using Activos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Activos.Controllers
{
    public class UsuariosController : Controller
    {
        private ACTIVOSEntities16 db = new ACTIVOSEntities16();
        // GET: Usuarios
        public ActionResult Index()
        {
            try
            {
                if (Session.Count > 0)
                {
                    var usuario = db.UDP_tbUsuarios_ListarUsuarios().ToList();

                    List<tbUsuarios> USUARIOS = new List<tbUsuarios>();

                    foreach (var item in usuario)
                    {
                        tbUsuarios tbUsuarios = new tbUsuarios
                        {
                            usua_Id = item.usua_Id,
                            usua_UsuarioNombre = item.usua_UsuarioNombre,
                            usua_UsuarioApellido = item.usua_UsuarioApellido,
                            usua_Usuario = item.usua_Usuario,
                            usua_Estado = item.usua_Estado
                        };

                        USUARIOS.Add(tbUsuarios);
                    }

                    if (USUARIOS != null)
                    {
                        return View(USUARIOS);
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
        public ActionResult Create(string usua_UsuarioNombre, string usua_UsuarioApellido, string usua_Usuario, string usua_Contra)
        {
            try
            {
                if (Session.Count > 0)
                {
                    var resultado = db.UDP_tbusuarios_InsertarUsuario(usua_UsuarioNombre, usua_UsuarioApellido, usua_Usuario, usua_Contra, int.Parse(Session["usua_Id"].ToString())).FirstOrDefault();
                    if (resultado == 1)
                    {
                        return Json(new { success = true, correcto = true }, JsonRequestBehavior.AllowGet);
                    }
                    if(resultado == 0)
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
                    tbUsuarios tbusuario = db.tbUsuarios.Find(int.Parse(id));
                    if (tbusuario.usua_Id == 0)
                    {
                        return HttpNotFound();
                    }
                    return Json(new { success = true, Usua_Id = tbusuario.usua_Id.ToString(), usuaNombre = tbusuario.usua_UsuarioNombre.ToString(), usuaApellido = tbusuario.usua_UsuarioApellido.ToString(), Usuario = tbusuario.usua_Usuario.ToString() }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Editar(string usua_Id, string usua_UsuarioNombre, string usua_UsuarioApellido)
        {
            try
            {
                if (Session.Count > 0)
                {
                    var resultado = db.UDP_tbusuarios_EditarUsuario((int.Parse(usua_Id)),usua_UsuarioNombre, usua_UsuarioApellido, int.Parse(Session["usua_Id"].ToString())).FirstOrDefault();
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

        // GET: Empleados/Details/5
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

                    tbUsuarios tbusuario = db.tbUsuarios.Find(id);
                    tbUsuarios AS = db.tbUsuarios.Find(tbusuario.usua_UsuarioCreacion);
                    tbUsuarios AS1 = db.tbUsuarios.Find(tbusuario.usua_Modificacion);
                    Session["UsuarioCrea"] = AS.usua_UsuarioNombre + ' ' + AS.usua_UsuarioApellido;
                    Session["FechaCreUsua"] = tbusuario.usua_FechaCreacion;
                    if (tbusuario.usua_Modificacion != null && tbusuario.usua_Modificacion != 0)
                    {
                        Session["UsuarioMod"] = AS1.usua_UsuarioNombre + ' ' + AS1.usua_UsuarioApellido;
                        Session["FechaModUsua"] = tbusuario.usua_FechaModificacion;
                    }

                    if (tbusuario == null)
                    {
                        return RedirectToAction("Error404", "Home");
                    }
                    return View(tbusuario);
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

        [HttpPost]
        public ActionResult Deshabilitar(string id)
        {
            try
            {
                if (Session.Count > 0)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var resultado = db.UDP_tbUsuarios_DeshabilitarUsuarios(int.Parse(id)).FirstOrDefault();
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

        [HttpPost]
        public ActionResult Habilitar(string id)
        {
            try
            {
                if (Session.Count > 0)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var resultado = db.UDP_tbUsuarios_HabilitarUsuarios(int.Parse(id)).FirstOrDefault();
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
    }
}