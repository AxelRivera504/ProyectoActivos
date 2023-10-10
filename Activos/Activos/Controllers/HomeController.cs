using Activos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Activos.Controllers
{
    public class HomeController : Controller
    {
        private ACTIVOSEntities16 db = new ACTIVOSEntities16();
        public ActionResult Index()
        {
            if (Session.Count > 0)
            {
                var result = db.UDP_tbClientes_ClienteConActivosBajados().FirstOrDefault();
                Session["Cliente"] = result.clie_NombreCliente;
                Session["ClienteCompa"] = result.clie_CodigoCompania;
                Session["Cantidad"] = result.TotalActivos;

                var resul1 = db.UDP_tbClientes_ClienteConActivosNoBajados().FirstOrDefault();
                Session["Cliente1"] = resul1.clie_NombreCliente;
                Session["ClienteCompa1"] = resul1.clie_CodigoCompania;
                Session["Cantidad1"] = resul1.TotalActivos;

                var resul2 = db.UDP_tbClientes_CantidadDeClientes().FirstOrDefault();
                Session["Cliente2"] = resul2.GetValueOrDefault();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        public ActionResult DataGrafica()
        {
            if (Session.Count > 0)
            {
                var result = db.UDP_tbVidaUtil_VidaUtilMasUsadas().ToArray();
                var formattedData = result.Select(item => new
                {
                    name = item.viut_Descripcion, // Ensure 'name' is defined
                    y = float.Parse(item.Porcentaje.Replace("%", "0")), // Convert the percentage to a float
                    sliced = false,
                    selected = false
                }).ToArray();

                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else{
                return Json(new { Estado = true}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}