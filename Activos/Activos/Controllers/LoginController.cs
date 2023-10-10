using Activos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Activos.Controllers
{
    public class LoginController : Controller
    {
        private ACTIVOSEntities16 db = new ACTIVOSEntities16();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string txtUsername, string txtPassword)
        {
            try
            {
                bool usernameValido = false, passwordValida = false;

                if (txtUsername == "")
                {
                    ModelState.AddModelError("Validacion1", "Los campos (*) son obligatorios");
                    ModelState.AddModelError("UsernameError", "*");
                }
                else if (txtUsername != "")
                {
                    usernameValido = true;
                }

                if (txtPassword == "")
                {
                    ModelState.AddModelError("Validacion", "Los campos (*) son obligatorios");
                    ModelState.AddModelError("ContraseniaError", "*");
                }
                else if (txtPassword != "")
                {
                    passwordValida = true;
                }

                if (usernameValido && passwordValida)
                {
                    var result = db.UDP_IniciarSesion(txtUsername, txtPassword).ToList();

                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            Session["usua_Id"] = item.usua_Id;
                            Session["Username"] = item.usua_Usuario;
                            Session["FullName"] = item.usua_UsuarioNombre + ' ' + item.usua_UsuarioApellido;
                        }
                        Response.Write("<script>localStorage.setItem('Login', 2);</script>");
                        return RedirectToAction("Index", "Home");
                    }
                    else {          
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("Validacion", "El usuario y/o contraseña son incorrectos");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Validacion", "El usuario y/o contraseña son incorrectos");
                Response.Write("<script>localStorage.setItem('Login', 1)</script>");
                return View();
                throw;
            }
        }

        public ActionResult Logout()
        {
            Session.Clear(); // Cerrar la sesión
            Response.Write("<script>localStorage.setItem('cerrar', 1);</script>");
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult validarUsernameExiste(string username)
        {
            try
            {
                var resultado = db.UDP_tbUsuarios_VerificarUsuarios(username).FirstOrDefault();

                if (resultado == 1)
                {
                    return Json(new { usernameExiste = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { usernameExiste = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }


        [HttpPost]
        public ActionResult CambiarPass(string username, string passNueva)
        {

            var resultado2 = db.UDP_tbusuarios_CambiarPassword(username, passNueva).FirstOrDefault();

            if (resultado2 == 1)
            {
                return Json(new { passUpdate = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { passUpdate = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}