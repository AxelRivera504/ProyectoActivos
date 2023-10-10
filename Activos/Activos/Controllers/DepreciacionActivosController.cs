using Activos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace Activos.Controllers
{
    public class DepreciacionActivosController : Controller
    {
        private ACTIVOSEntities16 db = new ACTIVOSEntities16();
        // GET: DepreciacionActivos
        public ActionResult Index()
        {
            try
            {
                if (Session.Count > 0)
                {
                    var activodepre = db.UDP_tbActivos_ListarActivos().ToList();

                    List<tbActivos> ACTIVOS = new List<tbActivos>();
                    
                    foreach (var item in activodepre)
                    {
                        DateTime dateValue1 = Convert.ToDateTime(item.acvo_FechaAdquisicion);
                        DateTime dateValue2 = Convert.ToDateTime(item.acvo_InicioDepreciacion);
                        tbActivos tbactivos = new tbActivos
                        {
                            acvo_Id = item.acvo_Id,
                            acvo_Nombre = item.acvo_Nombre,
                            acvo_NumeroSerie = item.acvo_NumeroSerie,
                            viut_Id = item.viut_Id,
                            clie_Id = item.clie_Id,
                            acvo_FechaAdquisicion = dateValue1,
                            acvo_InicioDepreciacion = dateValue2,
                            acvo_CostoOriginal = item.acvo_CostoOriginal,
                            acvo_ValorResidual = item.acvo_ValorResidual,
                            acvo_CostoDespreciable = item.acvo_CostoDespreciable,
                            acvo_Estado = item.acvo_Estado
                        };

                        ACTIVOS.Add(tbactivos);
                    }

                    if (ACTIVOS != null)
                    {
                        return View(ACTIVOS);
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

        public ActionResult Create()
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
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "acvo_Nombre,acvo_NumeroSerie,viut_Id,clie_Id,acvo_Ubicacion,acvo_FechaAdquisicion,acvo_CostoOriginal")] tbActivos tbactivo)
        {
            try
            {
                if (Session.Count > 0)
                {
                    ModelState.Remove("acvo_ValorResidual");
                    ModelState.Remove("acvo_CostoDespreciable");
                    ModelState.Remove("acvo_InicioDepreciacion");
                    ModelState.Remove("usua_UsuarioCreacion");
                    ModelState.Remove("acvo_FechaCreacion");
                    ModelState.Remove("usua_Modificacion");
                    ModelState.Remove("acvo_FechaModificacion");
                    ModelState.Remove("acvo_Estado");

                    tbactivo.usua_UsuarioCreacion = int.Parse(Session["usua_Id"].ToString());

                    if (tbactivo.clie_Id.ToString() != null && tbactivo.clie_Id.ToString() == "-1")
                    {
                        ModelState.AddModelError("clie_Id", " es obligatorio");
                    }

                    if (tbactivo.viut_Id.ToString() != null && tbactivo.viut_Id.ToString() == "-1")
                    {
                        ModelState.AddModelError("viut_Id", " es obligatorio");
                    }

                    if (ModelState.IsValid)
                    {
                        var resultado = db.UDP_tbActivos_InsertarActivos(tbactivo.acvo_Nombre, tbactivo.acvo_NumeroSerie, int.Parse(tbactivo.viut_Id.ToString()), int.Parse(tbactivo.clie_Id.ToString()), tbactivo.acvo_Ubicacion, tbactivo.acvo_FechaAdquisicion, decimal.Parse(tbactivo.acvo_CostoOriginal.ToString()), tbactivo.usua_UsuarioCreacion).FirstOrDefault();

                        if (resultado == 1)
                        {
                            Session["depreciacionIngresada"] = "si";
                            return RedirectToAction("Index", "DepreciacionActivos");
                        }
                        return RedirectToAction("Error500", "Home");
                    }

                    ViewBag.viut_Id = new SelectList(db.UDP_tbVidaUtil_ListarVidaUtil(), "viut_Id", "viut_VidaUtil"+"años", tbactivo.viut_Id);
                    ViewBag.clie_Id = new SelectList(db.UDP_tbClientes_ListarClientes(), "clie_Id", "clie_NombreCliente", tbactivo.clie_Id);
                    return View(tbactivo);
                }
                return RedirectToAction("Index", "Login");
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

                    tbActivos tbactivos = db.tbActivos.Find(id);
                    tbUsuarios AS = db.tbUsuarios.Find(tbactivos.usua_UsuarioCreacion);
                    tbUsuarios AS1 = db.tbUsuarios.Find(tbactivos.usua_Modificacion);
                    Session["UsuarioCrea"] = AS.usua_UsuarioNombre + ' ' + AS.usua_UsuarioApellido;
                    Session["FechaCreUsua"] = tbactivos.acvo_FechaCreacion;
                    if (tbactivos.usua_Modificacion != null && tbactivos.usua_Modificacion != 0)
                    {
                        Session["UsuarioMod"] = AS1.usua_UsuarioNombre + ' ' + AS1.usua_UsuarioApellido;
                        Session["FechaModUsua"] = tbactivos.acvo_FechaModificacion;
                    }

                    if (tbactivos == null)
                    {
                        return RedirectToAction("Error404", "Home");
                    }
                    return View(tbactivos);
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

        // GET: Empleados/Details/5
        public ActionResult VerficarEstado(int? id)
        {
            try
            {
                if (Session.Count > 0)
                {
                    if (id == null)
                    {
                        return RedirectToAction("Error500", "Home");
                    }

                    tbActivos tbactivos = db.tbActivos.Find(id);

                    if (tbactivos == null)
                    {
                        return RedirectToAction("Error404", "Home");
                    }

                    if (tbactivos.acvo_Estado == true)
                    {
                        return Json(new { Estado = true }, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json(new { Estado = false }, JsonRequestBehavior.AllowGet);
                    }
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


        public ActionResult Reporte()
        {
            try
            {
                if (Session.Count > 0)
                {
                    ViewBag.clie_Id = new SelectList(db.UDP_tbClientes_ListarClientes(), "clie_Id", "clie_CodigoCompania", "clie_NombreCliente");
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        [HttpPost]
        public ActionResult BajarActivo(string acvo_Id)
        {
            try
            {
                var resultado = db.UDP_tbActivos_DarDeBajaActivo(int.Parse(acvo_Id)).FirstOrDefault();

                if (resultado == 1)
                {
                    return Json(new { activoBajado = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { activoBajado = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }


        [HttpPost]
        public ActionResult VerificarTieneActivoBajados(string clie_Id)
        {
            try
            {
                var resultado = db.UDP_tbActivos_VerificarActivosCliente(int.Parse(clie_Id)).FirstOrDefault();

                if (resultado == 1)
                {
                    return Json(new { activoBajado = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { activoBajado = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        [HttpPost]
        public ActionResult VerificarTieneMActivoNOBajados(string clie_Id)
        {
            try
            {
                var resultado = db.UDP_tbActivos_VerificarActivosNoDadosBajaCliente(int.Parse(clie_Id)).FirstOrDefault();

                if (resultado == 1)
                {
                    return Json(new { activoBajado = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { activoBajado = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }


        [HttpPost]
        public ActionResult VerficarActivosTotales(string clie_Id)
        {
            try
            {
                var resultado = db.UDP_tbActivos_VerificarActivosTotalesCliente(int.Parse(clie_Id)).FirstOrDefault();

                if (resultado == 1)
                {
                    return Json(new { activoBajado = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { activoBajado = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }


        [HttpGet]
        public ActionResult GargarClientes()
        {
            try
            {
                if (Session.Count > 0)
                {
                    return Json(db.tbClientes.Select(x => new
                    {
                        clieId = x.clie_Id,
                        clieNombre = x.clie_NombreCliente,
                        clieCodigo = x.clie_CodigoCompania
                    }).ToList(), JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult CargarVidaUtil()
        {
            try
            {
                if (Session.Count > 0)
                {
                    return Json(db.tbVidaUtil.Select(x => new
                    {
                        vidaid = x.viut_Id,
                        des = x.viut_Objeto,
                        year = x.viut_VidaUtil
                    }).ToList(), JsonRequestBehavior.AllowGet);
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

        public ActionResult Edit(int? id)
        {
            try
            {
                if (Session.Count > 0)
                {
                    if (id == null || id == 0)
                    {
                        return RedirectToAction("Error500", "Home");
                    }

                    var activo = db.tbActivos.Find(id);

                    tbActivos acvo = new tbActivos
                    {
                        acvo_Id = activo.acvo_Id,
                        viut_Id = activo.viut_Id,
                        clie_Id = activo.clie_Id,
                        acvo_NumeroSerie = activo.acvo_NumeroSerie,
                        acvo_Ubicacion = activo.acvo_Ubicacion,
                        acvo_Nombre = activo.acvo_Nombre,
                        acvo_FechaAdquisicion = activo.acvo_FechaAdquisicion,
                        acvo_CostoOriginal = activo.acvo_CostoOriginal,
                    };

                    if (acvo == null)
                    {
                        return RedirectToAction("Error404", "Home");
                    }

                    ViewBag.viut_Id = new SelectList(db.tbVidaUtil, "viut_Id", "viut_VidaUtil", acvo.clie_Id);
                    ViewBag.clie_Id = new SelectList(db.tbClientes, "clie_Id", "clie_NombreCliente", acvo.viut_Id);

                    return View(acvo);
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "acvo_Id,acvo_Nombre,acvo_NumeroSerie,viut_Id,clie_Id,acvo_Ubicacion,acvo_FechaAdquisicion,acvo_CostoOriginal")] tbActivos tbactivo)
        {
            try
            {
                if (Session.Count > 0)
                {
                    ModelState.Remove("acvo_ValorResidual");
                    ModelState.Remove("acvo_CostoDespreciable");
                    ModelState.Remove("acvo_InicioDepreciacion");
                    ModelState.Remove("usua_UsuarioCreacion");
                    ModelState.Remove("acvo_FechaCreacion");
                    ModelState.Remove("usua_Modificacion");
                    ModelState.Remove("acvo_FechaModificacion");
                    ModelState.Remove("acvo_Estado");

                    tbactivo.usua_Modificacion = int.Parse(Session["usua_Id"].ToString());

                    if (ModelState.IsValid)
                    {
                        var resultado = db.UDP_tbActivos_EditarActivos(tbactivo.acvo_Id, tbactivo.acvo_Nombre, tbactivo.acvo_NumeroSerie, int.Parse(tbactivo.viut_Id.ToString()), int.Parse(tbactivo.clie_Id.ToString()), tbactivo.acvo_Ubicacion, tbactivo.acvo_FechaAdquisicion, decimal.Parse(tbactivo.acvo_CostoOriginal.ToString()), tbactivo.usua_Modificacion).FirstOrDefault();

                        if (resultado == 1)
                        {
                            Session["depreciacioneditada"] = "si";
                            return RedirectToAction("Index", "DepreciacionActivos");
                        }
                        return RedirectToAction("Error500", "Home");
                    }

                    ViewBag.viut_Id = new SelectList(db.UDP_tbVidaUtil_ListarVidaUtil(), "viut_Id", "viut_VidaUtil", tbactivo.viut_Id);
                    ViewBag.clie_Id = new SelectList(db.UDP_tbClientes_ListarClientes(), "clie_Id", "clie_NombreCliente", tbactivo.clie_Id);
                    return View(tbactivo);
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                return RedirectToAction("Error500", "Home");
                throw;
            }
        }

        [HttpPost]
        public FileResult ExportarExcel(string clie_Id)
        {
            tbClientes tbclente = db.tbClientes.Find(int.Parse(clie_Id));

            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection("data source=DESKTOP-2FTIJFF\\SQLEXPRESS; initial catalog=ACTIVOS; user id=axel2002; password=axel2002"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EXECUTE GenerarReportePorCliente @clie_Id, @acvo_Estado");
                SqlCommand cmd = new SqlCommand(sb.ToString(), cn);
                cmd.Parameters.AddWithValue("@clie_Id", int.Parse(clie_Id));
                cmd.Parameters.AddWithValue("@acvo_Estado", false);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                    da.Fill(dt);
                    
                }
            
            }

            dt.TableName = "datos";
            using (XLWorkbook libro = new XLWorkbook())
            {
                var hoja = libro.Worksheets.Add("Auxiliar Activos");
                hoja.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");


                // Combinar celdas para el título
                var titulo = hoja.Range("A1:B3");
                titulo.Merge().Value = "TIPO DEPRECIACIÓN LÍNEA RECTA";
                titulo.Style.Font.Bold = true;
                titulo.Style.Font.FontSize = 16;
                titulo.Style.Font.FontColor = XLColor.White;
                titulo.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo.Style.Font.FontName = "Arial Rounded MT Bold";

                // Ajustar el ancho de las columnas para que se ajusten al contenido
                hoja.Column(1).Width = 39;
                hoja.Column(2).Width = 39;

                // Ajustar el alto de las filas para que se ajusten al contenido
                hoja.Row(1).Height = 16;
                hoja.Row(2).Height = 16;
                hoja.Row(3).Height = 16;

                hoja.Row(6).Height = 24;
                hoja.Row(7).Height = 24;
                hoja.Row(8).Height = 24;
                hoja.Row(9).Height = 24;
                // Combinar celdas para el título
                var titulo2 = hoja.Range("A4:I4");
                titulo2.Merge().Value = "DEPRECIACION DE ACTIVOS FIJOS ";
                titulo2.Style.Font.Bold = true;
                titulo2.Style.Font.FontSize = 18;
                titulo2.Style.Font.FontColor = XLColor.White;
                titulo2.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo2.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo2.Style.Font.FontName = "Arial Rounded MT Bold";

                // Agregar todos los bordes a la celda combinada
                titulo2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo2.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo2.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo2.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Ajustar el ancho de las columnas para que se ajusten al contenido
                hoja.Column(1).Width = 39;
                hoja.Column(2).Width = 39;
                hoja.Column(3).Width = 23;
                hoja.Column(4).Width = 25;
                hoja.Column(5).Width = 25;
                hoja.Column(6).Width = 25;
                hoja.Column(7).Width = 15;
                hoja.Column(8).Width = 23;
                hoja.Column(9).Width = 30;
                hoja.Column(10).Width = 25;
                hoja.Column(11).Width = 25;
                hoja.Column(12).Width = 25;
                hoja.Column(13).Width = 25;
                hoja.Column(14).Width = 25;

                hoja.Column(15).Width = 20;
                hoja.Column(16).Width = 20;
                hoja.Column(17).Width = 20;
                hoja.Column(18).Width = 20;
                hoja.Column(19).Width = 20;
                hoja.Column(20).Width = 20;
                hoja.Column(21).Width = 20;
                hoja.Column(22).Width = 20;
                hoja.Column(23).Width = 20;
                hoja.Column(24).Width = 20;
                hoja.Column(25).Width = 20;
                hoja.Column(26).Width = 20;

                hoja.Column(27).Width = 25;
                hoja.Column(28).Width = 25;
                hoja.Column(29).Width = 25;
                // Ajustar el alto de las filas para que se ajusten al contenido
                hoja.Row(4).Height = 22;

                hoja.Cell("J4").Value = "";
                hoja.Cell("J4").Style.Font.Bold = true;
                hoja.Cell("J4").Style.Font.FontSize = 18;
                hoja.Cell("J4").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("J4").Style.Font.FontName = "Calibri";
                hoja.Cell("J4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("J4").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);

                hoja.Cell("K4").Value = "";
                hoja.Cell("K4").Style.Font.Bold = true;
                hoja.Cell("K4").Style.Font.FontSize = 18;
                hoja.Cell("K4").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("K4").Style.Font.FontName = "Calibri";
                hoja.Cell("K4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("K4").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);

                hoja.Cell("L4").Value = "";
                hoja.Cell("L4").Style.Font.Bold = true;
                hoja.Cell("L4").Style.Font.FontSize = 18;
                hoja.Cell("L4").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("L4").Style.Font.FontName = "Calibri";
                hoja.Cell("L4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("L4").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);

                hoja.Cell("M4").Value = "";
                hoja.Cell("M4").Style.Font.Bold = true;
                hoja.Cell("M4").Style.Font.FontSize = 18;
                hoja.Cell("M4").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("M4").Style.Font.FontName = "Calibri";
                hoja.Cell("M4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("M4").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);


                // Combinar celdas para el título
                var titulo3 = hoja.Range("N4:N5");
                titulo3.Merge().Value = "Dep. periodo\n en meses";
                titulo3.Style.Font.Bold = true;
                titulo3.Style.Font.FontSize = 18;
                titulo3.Style.Font.FontColor = XLColor.White;
                titulo3.Style.Fill.BackgroundColor = XLColor.FromHtml("#305496"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo3.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo3.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo3.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo3.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo3.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo3.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo3.Style.Font.FontName = "Calibri";




                hoja.Cell("A5").Value = "Descripcion Activo";
                hoja.Cell("A5").Style.Font.Bold = true;
                hoja.Cell("A5").Style.Font.FontSize = 18;
                hoja.Cell("A5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("A5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("A5").Style.Font.FontName = "Calibri";
                hoja.Cell("A5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("A5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("A5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("A5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("A5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("A5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("B5").Value = "Clase Activo";
                hoja.Cell("B5").Style.Font.Bold = true;
                hoja.Cell("B5").Style.Font.FontSize = 18;
                hoja.Cell("B5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("B5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("B5").Style.Font.FontName = "Calibri";
                hoja.Cell("B5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("B5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("B5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("B5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("B5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("B5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("C5").Value = "Ubicación y\n N°serie ";
                hoja.Cell("C5").Style.Font.Bold = true;
                hoja.Cell("C5").Style.Font.FontSize = 18;
                hoja.Cell("C5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("C5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("C5").Style.Font.FontName = "Calibri";
                hoja.Cell("C5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("C5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("C5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("C5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("C5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("C5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior




                hoja.Cell("D5").Value = "Fecha\n Adquisición";
                hoja.Cell("D5").Style.Font.Bold = true;
                hoja.Cell("D5").Style.Font.FontSize = 18;
                hoja.Cell("D5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("D5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("D5").Style.Font.FontName = "Calibri";
                hoja.Cell("D5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("D5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);


                // Agregar todos los bordes a la celda combinada
                hoja.Cell("D5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("D5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("D5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("D5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("E5").Value = "Inicio de\n la depreciacion";
                hoja.Cell("E5").Style.Font.Bold = true;
                hoja.Cell("E5").Style.Font.FontSize = 18;
                hoja.Cell("E5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("E5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("E5").Style.Font.FontName = "Calibri";
                hoja.Cell("E5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("E5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("E5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("E5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("E5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("E5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("E5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior


                hoja.Cell("F5").Value = "Costo Original";
                hoja.Cell("F5").Style.Font.Bold = true;
                hoja.Cell("F5").Style.Font.FontSize = 18;
                hoja.Cell("F5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("F5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("F5").Style.Font.FontName = "Calibri";
                hoja.Cell("F5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("F5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("F5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("F5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("F5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("F5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("F5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("G5").Value = "Vida Util";
                hoja.Cell("G5").Style.Font.Bold = true;
                hoja.Cell("G5").Style.Font.FontSize = 18;
                hoja.Cell("G5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("G5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("G5").Style.Font.FontName = "Calibri";
                hoja.Cell("G5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("G5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("G5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("G5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("G5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("G5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("G5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("H5").Value = "Valor Residual";
                hoja.Cell("H5").Style.Font.Bold = true;
                hoja.Cell("H5").Style.Font.FontSize = 18;
                hoja.Cell("H5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("H5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("H5").Style.Font.FontName = "Calibri";
                hoja.Cell("H5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("H5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("H5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("H5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("H5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("H5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("H5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("I5").Value = "Costo Depreciable";
                hoja.Cell("I5").Style.Font.Bold = true;
                hoja.Cell("I5").Style.Font.FontSize = 18;
                hoja.Cell("I5").Style.Font.FontColor = XLColor.FromHtml("#305496");
                hoja.Cell("I5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBDBDB");
                hoja.Cell("I5").Style.Font.FontName = "Calibri";
                hoja.Cell("I5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("I5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("I5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("I5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("I5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("I5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("I5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("J5").Value = "Periodo \nen Meses ";
                hoja.Cell("J5").Style.Font.Bold = true;
                hoja.Cell("J5").Style.Font.FontSize = 18;
                hoja.Cell("J5").Style.Font.FontColor = XLColor.White;
                hoja.Cell("J5").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("J5").Style.Font.FontName = "Calibri";
                hoja.Cell("J5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("J5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("J5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("J5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("J5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("J5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("J5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior




                hoja.Cell("K5").Value = "Tasa % Mensual";
                hoja.Cell("K5").Style.Font.Bold = true;
                hoja.Cell("K5").Style.Font.FontSize = 18;
                hoja.Cell("K5").Style.Font.FontColor = XLColor.White;
                hoja.Cell("K5").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("K5").Style.Font.FontName = "Calibri";
                hoja.Cell("K5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("K5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("K5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("K5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("K5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("K5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("K5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior




                hoja.Cell("L5").Value = "Periodo \nen Años";
                hoja.Cell("L5").Style.Font.Bold = true;
                hoja.Cell("L5").Style.Font.FontSize = 18;
                hoja.Cell("L5").Style.Font.FontColor = XLColor.White;
                hoja.Cell("L5").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("L5").Style.Font.FontName = "Calibri";
                hoja.Cell("L5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("L5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("L5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("L5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("L5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("L5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("L5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior




                hoja.Cell("M5").Value = "Tasa % Anual";
                hoja.Cell("M5").Style.Font.Bold = true;
                hoja.Cell("M5").Style.Font.FontSize = 18;
                hoja.Cell("M5").Style.Font.FontColor = XLColor.White;
                hoja.Cell("M5").Style.Fill.BackgroundColor = XLColor.FromHtml("#305496");
                hoja.Cell("M5").Style.Font.FontName = "Calibri";
                hoja.Cell("M5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("M5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("M5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("M5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("M5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("M5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("M5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior




                // Combinar celdas para el título
                var titulo4 = hoja.Range("O4:O5");
                titulo4.Merge().Value = "Dep.\n Enero";
                titulo4.Style.Font.Bold = true;
                titulo4.Style.Font.FontSize = 18;
                titulo4.Style.Font.FontColor = XLColor.White;
                titulo4.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo4.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo4.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo4.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo4.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo4.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo4.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo4.Style.Font.FontName = "Calibri";



                // Combinar celdas para el título
                var titulo5 = hoja.Range("P4:P5");
                titulo5.Merge().Value = "Dep.\n Febrero";
                titulo5.Style.Font.Bold = true;
                titulo5.Style.Font.FontSize = 18;
                titulo5.Style.Font.FontColor = XLColor.White;
                titulo5.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo5.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo5.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo5.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo5.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo5.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo5.Style.Font.FontName = "Calibri";



                // Combinar celdas para el título
                var titulo6 = hoja.Range("Q4:Q5");
                titulo6.Merge().Value = "Dep.\n Marzo";
                titulo6.Style.Font.Bold = true;
                titulo6.Style.Font.FontSize = 18;
                titulo6.Style.Font.FontColor = XLColor.White;
                titulo6.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo6.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo6.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo6.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo6.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo6.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo6.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo6.Style.Font.FontName = "Calibri";




                // Combinar celdas para el título
                var titulo7 = hoja.Range("R4:R5");
                titulo7.Merge().Value = "Dep.\n Abril";
                titulo7.Style.Font.Bold = true;
                titulo7.Style.Font.FontSize = 18;
                titulo7.Style.Font.FontColor = XLColor.White;
                titulo7.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo7.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo7.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo7.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo7.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo7.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo7.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo7.Style.Font.FontName = "Calibri";


                // Combinar celdas para el título
                var titulo9 = hoja.Range("S4:S5");
                titulo9.Merge().Value = "Dep.\n Mayo";
                titulo9.Style.Font.Bold = true;
                titulo9.Style.Font.FontSize = 18;
                titulo9.Style.Font.FontColor = XLColor.White;
                titulo9.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo9.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo9.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo9.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo9.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo9.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo9.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo9.Style.Font.FontName = "Calibri";



                // Combinar celdas para el título
                var titulo10 = hoja.Range("T4:T5");
                titulo10.Merge().Value = "Dep.\n Junio";
                titulo10.Style.Font.Bold = true;
                titulo10.Style.Font.FontSize = 18;
                titulo10.Style.Font.FontColor = XLColor.White;
                titulo10.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo10.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo10.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo10.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo10.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo10.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo10.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo10.Style.Font.FontName = "Calibri";



                // Combinar celdas para el título
                var titulo11 = hoja.Range("U4:U5");
                titulo11.Merge().Value = "Dep.\n Julio";
                titulo11.Style.Font.Bold = true;
                titulo11.Style.Font.FontSize = 18;
                titulo11.Style.Font.FontColor = XLColor.White;
                titulo11.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo11.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo11.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo11.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo11.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo11.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo11.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo11.Style.Font.FontName = "Calibri";

                // Combinar celdas para el título
                var titulo16 = hoja.Range("V4:V5");
                titulo16.Merge().Value = "Dep.\n Agosto";
                titulo16.Style.Font.Bold = true;
                titulo16.Style.Font.FontSize = 18;
                titulo16.Style.Font.FontColor = XLColor.White;
                titulo16.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo16.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo16.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo16.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo16.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo16.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo16.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo16.Style.Font.FontName = "Calibri";




                // Combinar celdas para el título
                var titulo12 = hoja.Range("W4:W5");
                titulo12.Merge().Value = "Dep.\n Septiembre";
                titulo12.Style.Font.Bold = true;
                titulo12.Style.Font.FontSize = 18;
                titulo12.Style.Font.FontColor = XLColor.White;
                titulo12.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo12.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo12.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo12.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo12.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo12.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo12.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo12.Style.Font.FontName = "Calibri";



                // Combinar celdas para el título
                var titulo13 = hoja.Range("X4:X5");
                titulo13.Merge().Value = "Dep.\n Octubre";
                titulo13.Style.Font.Bold = true;
                titulo13.Style.Font.FontSize = 18;
                titulo13.Style.Font.FontColor = XLColor.White;
                titulo13.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo13.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo13.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo13.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo13.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo13.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo13.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo13.Style.Font.FontName = "Calibri";





                // Combinar celdas para el título
                var titulo14 = hoja.Range("Y4:Y5");
                titulo14.Merge().Value = "Dep.\n Noviembre";
                titulo14.Style.Font.Bold = true;
                titulo14.Style.Font.FontSize = 18;
                titulo14.Style.Font.FontColor = XLColor.White;
                titulo14.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo14.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo14.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo14.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo14.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo14.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo14.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo14.Style.Font.FontName = "Calibri";



                // Combinar celdas para el título
                var titulo15 = hoja.Range("Z4:Z5");
                titulo15.Merge().Value = "Dep.\n Diciembre";
                titulo15.Style.Font.Bold = true;
                titulo15.Style.Font.FontSize = 18;
                titulo15.Style.Font.FontColor = XLColor.White;
                titulo15.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo15.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo15.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo15.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo15.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo15.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo15.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo14.Style.Font.FontName = "Calibri";


                var titulo17 = hoja.Range("AA4:AB4");
                titulo17.Merge().Value = "Depreciacion";
                titulo17.Style.Font.Bold = true;
                titulo17.Style.Font.FontSize = 18;
                titulo17.Style.Font.FontColor = XLColor.White;
                titulo17.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo17.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo17.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo17.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo17.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo17.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo17.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo17.Style.Font.FontName = "Calibri";



                hoja.Cell("AA5").Value = "Del Periodo";
                hoja.Cell("AA5").Style.Font.Bold = true;
                hoja.Cell("AA5").Style.Font.FontSize = 18;
                hoja.Cell("AA5").Style.Font.FontColor = XLColor.White;
                hoja.Cell("AA5").Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                hoja.Cell("AA5").Style.Font.FontName = "Calibri";
                hoja.Cell("AA5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("AA5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("AA5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("AA5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("AA5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("AA5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("AA5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior



                hoja.Cell("AB5").Value = "Acumulada ";
                hoja.Cell("AB5").Style.Font.Bold = true;
                hoja.Cell("AB5").Style.Font.FontSize = 18;
                hoja.Cell("AB5").Style.Font.FontColor = XLColor.White;
                hoja.Cell("AB5").Style.Fill.BackgroundColor = XLColor.FromHtml("#002060");
                hoja.Cell("AB5").Style.Font.FontName = "Calibri";
                hoja.Cell("AB5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                hoja.Cell("AB5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Habilitar el ajuste de texto automático (WordWrap)
                hoja.Cell("AB5").Style.Alignment.SetWrapText(true);

                // Agregar todos los bordes a la celda combinada
                hoja.Cell("AB5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                hoja.Cell("AB5").Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                hoja.Cell("AB5").Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                hoja.Cell("AB5").Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior


                var titulo18 = hoja.Range("AC4:AC5");
                titulo18.Merge().Value = "Valor Libros";
                titulo18.Style.Font.Bold = true;
                titulo18.Style.Font.FontSize = 18;
                titulo18.Style.Font.FontColor = XLColor.White;
                titulo18.Style.Fill.BackgroundColor = XLColor.FromHtml("#002060"); // Establecer el color de fondo

                // Alinear el contenido al centro vertical y horizontal
                titulo18.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                titulo18.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Agregar todos los bordes a la celda combinada
                titulo18.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                titulo18.Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                // Aplicar el color del borde
                titulo18.Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                titulo18.Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior

                // Aplicar el tipo de fuente "Arial Rounded MT Bold"
                titulo18.Style.Font.FontName = "Calibri";


                int filaInicio = 6;

                // Agrega el contenido de DataTable a partir de la fila especificada
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        hoja.Cell(filaInicio + i, j + 1).Value = dt.Rows[i][j].ToString();
                        hoja.Cell(filaInicio + i, j + 1).Style.Font.FontSize = 18;
                        hoja.Cell(filaInicio + i, j + 1).Style.Font.FontName = "Calibri";
                        if (j == 5 || j == 10 || j == 11 || j == 12 || j == 13 || j == 14)
                        {
                            hoja.Cell(filaInicio + i, j).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFF00");
                            hoja.Cell(filaInicio + i, j).Style.Font.Bold = true;
                        }

                        if (j == 3 || j == 4)
                        {
                            DateTime dateValue = Convert.ToDateTime(dt.Rows[i][j]);
                            hoja.Cell(filaInicio + i, j + 1).Value = dateValue.ToString("d");
                        }

                        if (j == 5 || j == 7 || j == 8 || j == 13 || (j >= 15 && j < 30))
                        {

                            double cellValue = Convert.ToDouble(dt.Rows[i][j]);
                            hoja.Cell(filaInicio + i, j + 1).Value = cellValue.ToString("#,##0.00");
                        }

                        if (j >= 15 && j < 30)
                        {
                            // Agregar todos los bordes a la celda combinada
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                            // Aplicar el color del borde
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.OutsideBorderColor = XLColor.FromHtml("#FF0000"); // Color del borde exterior
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.InsideBorderColor = XLColor.FromHtml("#FF0000"); // Color del borde interior
                        }
                        else
                        {
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borde exterior
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin; // Borde interior

                            // Aplicar el color del borde
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.OutsideBorderColor = XLColor.Black; // Color del borde exterior
                            hoja.Cell(filaInicio + i, j + 1).Style.Border.InsideBorderColor = XLColor.Black; // Color del borde interior
                        }
                        hoja.Cell(filaInicio + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                    }
                }

                //Fila vacia

                DataRow blankRow = dt.NewRow();
                dt.Rows.Add(blankRow);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");
                    if (j >= 4 && j <= 13)
                    {
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.OutsideBorderColor = XLColor.Black;
                    }
                    else if (j == 25)
                    {
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.RightBorderColor = XLColor.Black;
                    }
                    hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                }

                // Fila para calculos
                DataRow CalculoRow = dt.NewRow();
                dt.Rows.Add(CalculoRow);


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 5).Style.Font.FontName = "Calibri";

                decimal sum = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][5].ToString() == "")
                    {
                        sum += 0;
                    }
                    else
                    {
                        sum += decimal.Parse(dt.Rows[i][5].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Value = sum;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.LeftBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Font.FontName = "Calibri";


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 7).Style.Font.FontName = "Calibri";




                decimal sum1 = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][7].ToString() == "")
                    {
                        sum1 += 0;
                    }
                    else
                    {
                        sum1 += decimal.Parse(dt.Rows[i][7].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Value = sum1;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Border.LeftBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 8).Style.Font.FontName = "Calibri";


                decimal sum2 = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][8].ToString() == "")
                    {
                        sum2 += 0;
                    }
                    else
                    {
                        sum2 += decimal.Parse(dt.Rows[i][8].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Value = sum2;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Border.LeftBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 9).Style.Font.FontName = "Calibri";


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 10).Style.Font.FontName = "Calibri";


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 11).Style.Font.FontName = "Calibri";


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 12).Style.Font.FontName = "Calibri";


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 13).Style.Font.FontName = "Calibri";




                decimal sum3 = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][13].ToString() == "")
                    {
                        sum3 += 0;
                    }
                    else
                    {
                        sum3 += decimal.Parse(dt.Rows[i][13].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Value = sum3;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 14).Style.Font.FontName = "Calibri";



                decimal sumEnero = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][14].ToString() == "")
                    {
                        sumEnero += 0;
                    }
                    else
                    {
                        sumEnero += decimal.Parse(dt.Rows[i][14].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Value = sumEnero;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 15).Style.Font.FontName = "Calibri";


                decimal sumFebrero = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][15].ToString() == "")
                    {
                        sumFebrero += 0;
                    }
                    else
                    {
                        sumFebrero += decimal.Parse(dt.Rows[i][15].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Value = sumEnero;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 16).Style.Font.FontName = "Calibri";


                decimal sumMarzo = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][16].ToString() == "")
                    {
                        sumMarzo += 0;
                    }
                    else
                    {
                        sumMarzo += decimal.Parse(dt.Rows[i][16].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Value = sumMarzo;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 17).Style.Font.FontName = "Calibri";

                decimal sumAbril = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][17].ToString() == "")
                    {
                        sumAbril += 0;
                    }
                    else
                    {
                        sumAbril += decimal.Parse(dt.Rows[i][17].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Value = sumAbril;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 18).Style.Font.FontName = "Calibri";


                decimal sumMayo = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][18].ToString() == "")
                    {
                        sumMayo += 0;
                    }
                    else
                    {
                        sumMayo += decimal.Parse(dt.Rows[i][18].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Value = sumMayo;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 19).Style.Font.FontName = "Calibri";


                decimal sumJunio = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][19].ToString() == "")
                    {
                        sumJunio += 0;
                    }
                    else
                    {
                        sumJunio += decimal.Parse(dt.Rows[i][19].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Value = sumJunio;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 20).Style.Font.FontName = "Calibri";

                decimal sumJulio = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][20].ToString() == "")
                    {
                        sumJulio += 0;
                    }
                    else
                    {
                        sumJulio += decimal.Parse(dt.Rows[i][20].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Value = sumJulio;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 21).Style.Font.FontName = "Calibri";


                decimal sumAgosto = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][21].ToString() == "")
                    {
                        sumAgosto += 0;
                    }
                    else
                    {
                        sumAgosto += decimal.Parse(dt.Rows[i][21].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Value = sumAgosto;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 22).Style.Font.FontName = "Calibri";


                decimal sumSeptiembre = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][22].ToString() == "")
                    {
                        sumSeptiembre += 0;
                    }
                    else
                    {
                        sumSeptiembre += decimal.Parse(dt.Rows[i][22].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Value = sumSeptiembre;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 23).Style.Font.FontName = "Calibri";


                decimal sumOctubre = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][23].ToString() == "")
                    {
                        sumOctubre += 0;
                    }
                    else
                    {
                        sumOctubre += decimal.Parse(dt.Rows[i][23].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Value = sumOctubre;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 24).Style.Font.FontName = "Calibri";

                decimal sumNoviembre = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][24].ToString() == "")
                    {
                        sumNoviembre += 0;
                    }
                    else
                    {
                        sumNoviembre += decimal.Parse(dt.Rows[i][24].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Value = sumNoviembre;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 25).Style.Font.FontName = "Calibri";


                decimal sumDiciembre = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][25].ToString() == "")
                    {
                        sumDiciembre += 0;
                    }
                    else
                    {
                        sumDiciembre += decimal.Parse(dt.Rows[i][25].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Value = sumDiciembre;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 26).Style.Font.FontName = "Calibri";


                decimal sumPeriodo = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][26].ToString() == "")
                    {
                        sumPeriodo += 0;
                    }
                    else
                    {
                        sumPeriodo += decimal.Parse(dt.Rows[i][26].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Value = sumPeriodo;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.TopBorderColor = XLColor.Black;


                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 27).Style.Font.FontName = "Calibri";


                decimal sumAcumulada = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][27].ToString() == "")
                    {
                        sumAcumulada += 0;
                    }
                    else
                    {
                        sumAcumulada += decimal.Parse(dt.Rows[i][27].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Value = sumAcumulada;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 28).Style.Font.FontName = "Calibri";


                decimal sumValorLibro = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++) // Exclude the last (blank) row
                {
                    if (dt.Rows[i][28].ToString() == "")
                    {
                        sumValorLibro += 0;
                    }
                    else
                    {
                        sumValorLibro += decimal.Parse(dt.Rows[i][28].ToString());
                    }
                }

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Value = sumValorLibro;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.NumberFormat.Format = "#,##0.00";
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.BottomBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.RightBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.TopBorderColor = XLColor.Black;

                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Border.LeftBorderColor = XLColor.Black;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Font.Bold = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Font.Italic = true;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Font.FontSize = 18;
                hoja.Cell(filaInicio + dt.Rows.Count - 1, 29).Style.Font.FontName = "Calibri";


                for (int j = 0; j < 4; j++)
                {
                    if (j >= 0 && j < 3) {
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.TopBorderColor = XLColor.Black;

                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.BottomBorderColor = XLColor.Black;
                    }

                    if (j == 3) {
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");

                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.RightBorderColor = XLColor.Black;

                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.TopBorderColor = XLColor.Black;

                        hoja.Cell(filaInicio + dt.Rows.Count - 1, j + 1).Style.Border.BottomBorder = XLBorderStyleValues.Double;
                        hoja.Cell(filaInicio + dt.Rows.Count - 1, 6).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                   
                }


                for (int i = 0; i < 5; i++)
                {
                    DataRow newRow = dt.NewRow();
                    dt.Rows.Add(newRow);
                }

                // Set bottom border on the last empty row from columns 0 to 2
                int lastRowIndex = filaInicio + dt.Rows.Count;
                for (int col = 0; col <= 2; col++)
                {
                    hoja.Cell(lastRowIndex, col + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    hoja.Cell(lastRowIndex, col + 1).Style.Border.BottomBorderColor = XLColor.Black;
                }

                // Set styles for the entire empty row
                for (int col = 0; col <= 2; col++)
                {
                    hoja.Cell(lastRowIndex, col + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFFFF");
                    hoja.Cell(lastRowIndex, col + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Bottom);
                    hoja.Cell(lastRowIndex, col + 1).Style.Font.Bold = true;
                    hoja.Cell(lastRowIndex, col + 1).Style.Font.Italic = true;
                    hoja.Cell(lastRowIndex, col + 1).Style.Font.FontSize = 18;
                    hoja.Cell(lastRowIndex, col + 1).Style.Font.FontName = "Calibri";
                }
                using (MemoryStream stream = new MemoryStream()) {
                    libro.SaveAs(stream);
                    DateTime dateValue1 = Convert.ToDateTime(DateTime.Now.ToString());
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte - " + tbclente.clie_NombreCliente + " - " + dateValue1.ToString("d") + ".xlsx");
                }
            }
        }



        [HttpPost]
        public FileResult ExportarReporteFechas(string clie_Id, string fechainicio, string fechafin)
        {
            tbClientes tbclente = db.tbClientes.Find(int.Parse(clie_Id));

            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection("data source=DESKTOP-2FTIJFF\\SQLEXPRESS; initial catalog=ACTIVOS; user id=axel2002; password=axel2002"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EXECUTE Acti.UDP_tbActivos_ReporteFechas @clie_Id,@FechaInicio,@FechaFin");
                SqlCommand cmd = new SqlCommand(sb.ToString(), cn);
                cmd.Parameters.AddWithValue("@clie_Id", int.Parse(clie_Id));
                cmd.Parameters.AddWithValue("@FechaInicio", fechainicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechafin);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);

                }

            }

            dt.TableName = "datos";
            using (XLWorkbook libro = new XLWorkbook())
            {

                var hoja = libro.Worksheets.Add(dt);

                hoja.ColumnsUsed().AdjustToContents();
                using (MemoryStream stream = new MemoryStream())
                {
                    libro.SaveAs(stream);
                    DateTime dateValue1 = Convert.ToDateTime(DateTime.Now.ToString());
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte - " + tbclente.clie_NombreCliente + " - " + dateValue1.ToString("d") + ".xlsx");
                }
            }
        }

    }

}