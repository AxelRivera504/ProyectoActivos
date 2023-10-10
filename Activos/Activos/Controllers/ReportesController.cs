using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Activos.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Index()
        {
            try
            {
                if (Session.Count > 0)
                {
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Login");
                throw;
            }
           
        }

        public ActionResult ReporteActivos()
        {
            try
            {
                if (Session.Count > 0)
                {
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }catch (Exception)
            {
                return RedirectToAction("Index", "Login");
                throw;
            }
        }

        public ActionResult ReporteNoActivos()
        {
            try
            {
                if (Session.Count > 0)
                {
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Login");
                throw;
            }
        }
    }
}