using ActivosDerecho.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Drawing;

namespace ActivosDerecho.Controllers
{
    public class ActivosController : Controller
    {

        /// <summary>
        /// Pagina de inicio, busca en la base de datos los activos almacenados
        /// y los pasa en una lista hacia la vista para ser mostrados
        /// Buscar sin filtro en la pagina 0
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? pagina)
        {
            /*Esto es para la paginacion de la tabla
             Solo hay que instalar el paquete NuGet que se llama PagedList*/
            if (Request.HttpMethod != "GET")
            {
                pagina = 1;
            }
            int tamPagina = 20;
            int numPagina = (pagina ?? 1);
            List<Activo> lista = new Activo().GetListaActivos(Session["filtroActivos"].ToString());
            ViewBag.FiltroActual = Session["filtroActivos"].ToString();
            return View(lista.ToPagedList(numPagina, tamPagina));//Paginacion
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro">filtro que se aplica para buscar</param>
        /// <param name="btn">cual boton activó</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(String filtro = "", string btn = "")
        {
            switch (btn)
            {
                case "Exportar":
                    ExcelPackage pck = new ExcelPackage();
                    var ws = pck.Workbook.Worksheets.Add("Activos " + Session["filtroActivos"].ToString());
                    
                    ws.Cells["A1"].LoadFromDataTable(crearTabla(),true);

                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment;  filename=Activos.xlsx");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    Response.BinaryWrite(pck.GetAsByteArray());
                    Response.End();

                    break;
                default: break;
            }
            Session["filtroActivos"] = filtro;//guardo el filtro
            return RedirectToAction("Index");
        }

        public DataTable crearTabla()
        {
            DataTable tabla = new DataTable();
            DataColumn columna = new DataColumn("Placa",typeof(int));
            tabla.Columns.Add(columna);
            columna = new DataColumn("Nombre y Descripción", typeof(String));
            tabla.Columns.Add(columna);
            columna = new DataColumn("Espacio Físico", typeof(String));
            tabla.Columns.Add(columna);
            columna = new DataColumn("Encargado", typeof(String));
            tabla.Columns.Add(columna);
            columna = new DataColumn("Estado", typeof(int));
            tabla.Columns.Add(columna);
            columna = new DataColumn("Inventario Por", typeof(String));
            tabla.Columns.Add(columna);
            columna = new DataColumn("Conciliación", typeof(String));
            tabla.Columns.Add(columna);
            List<Activo> lista = new Activo().GetListaActivos(Session["filtroActivos"].ToString());
            foreach(Activo ac in lista)
            {
                DataRow row = tabla.NewRow();
                row["Placa"] = ac.placa;
                row["Nombre y Descripción"] = ac.nombreDescripcion;
                row["Espacio Físico"] = ac.espacioFisico;
                row["Encargado"] = ac.encargado;
                row["Estado"] = ac.estado;
                row["Inventario Por"] = ac.inventarioPor;
                row["Conciliación"] = ac.conciliacion;
                tabla.Rows.Add(row);
            }
            return tabla;
        }


        /// <summary>
        /// Para agregar un nuevo activo en el sistema
        /// </summary>
        /// <returns></returns>
        public ActionResult Agregar()
        {
            Boolean activo = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (activo)
            {
                MembershipUser actual = Membership.GetUser();
                if (actual.UserName.Equals("Administrador"))
                {
                    //Aqui
                    return View();
                }
                else
                {
                    return RedirectToAction("ErrorPermisos", "Home");
                }
            }
            else
            {
                return RedirectToAction("ErrorPermisos", "Home");
            }
        }

        /// <summary>
        /// Post del método de agregar
        /// </summary>
        /// <param name="ac">Modelo lleno por el usuario</param>
        /// <param name="btn">Contiene cuál botón causó el post</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Agregar(Activo ac, string btn)
        {
            switch (btn)
            {//para saber cual botón causó el post
                case "Guardar":
                    if (ModelState.IsValid)
                    {//si el modelo es valido entonces agrego
                        Boolean resultado = ac.AgregarNuevoActivo(ac);
                        Session["filtroActivos"] = "";//guardo el filtro
                        return RedirectToAction("Index");//vuelvo al inicio
                    }
                    //si llega aquí hay algo mal, se retorna el modelo para llenar
                    return View(ac);
                case "Cancelar":
                    Session["filtroActivos"] = "";//guardo el filtro
                    return RedirectToAction("Index");//vuelvo al inicio
                default:
                    Session["filtroActivos"] = "";//guardo el filtro
                    return RedirectToAction("Index");//vuelvo al inicio
            }
        }

        /// <summary>
        /// Se edita un activo del sistema
        /// </summary>
        /// <param name="id">Contiene el Guid del activo para ser consultado</param>
        /// <returns></returns>
        public ActionResult Editar(String id = "")
        {
            Boolean activo = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (activo)
            {
                MembershipUser actual = Membership.GetUser();
                if (actual.UserName.Equals("Administrador"))
                {
                    //Aqui
                    Activo ac2 = new Activo().ConsultarActivo(id);
                    //luego de consultar el activo solo basta enviarlo a la vista por parámetro
                    //y esta llenará los datos en los campos
                    return View(ac2);
                }
                else
                {
                    return RedirectToAction("ErrorPermisos", "Home");
                }
            }
            else
            {
                return RedirectToAction("ErrorPermisos", "Home");
            }
        }

        /// <summary>
        /// Post de la acción editar activo
        /// </summary>
        /// <param name="ac">Modelo de datos modificado por el usuario</param>
        /// <param name="btn">Contiene cuál botón causó el post</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Editar(Activo ac, string btn)
        {
            switch (btn)
            {//cual boton
                case "Actualizar":
                    if (ModelState.IsValid)
                    {//si el modelo es valido
                        //modifico
                        Boolean resultado = ac.ActualizarActivo(ac);
                        Session["filtroActivos"] = "";//guardo el filtro
                        return RedirectToAction("Index", "Activos");
                    }
                    //si llegue aqui hay algo mal, retorno el modelo a la vista
                    return View(ac);
                case "Cancelar":
                    Session["filtroActivos"] = "";//guardo el filtro
                    return RedirectToAction("Index");//vuelvo al inicio
                default:
                    Session["filtroActivos"] = "";//guardo el filtro
                    return RedirectToAction("Index");//vuelvo al inicio
            }
        }

        /// <summary>
        /// Eliminación de un activo
        /// </summary>
        /// <param name="id">Contiene el Guid de un activo para guardarlo en session</param>
        /// <param name="placa">La placa del activo que se va eliminar</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Eliminar(String id = "", String placa = "")
        {
            Boolean activo = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (activo)
            {
                MembershipUser actual = Membership.GetUser();
                if (actual.UserName.Equals("Administrador"))
                {
                    //Aqui
                    ViewBag.placa = placa;//para imprimirlo en la vista
                    //guardo el identificador para la eliminacion posterior
                    Session["id"] = id;
                    return View();//retorno la vista para aceptar o cancelar
                }
                else
                {
                    return RedirectToAction("ErrorPermisos", "Home");
                }
            }
            else
            {
                return RedirectToAction("ErrorPermisos", "Home");
            }

        }

        /// <summary>
        /// Post de la accion de eliminar activo
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Eliminar(String btn)
        {
            switch (btn)
            {
                case "Aceptar":
                    //anteriormente ya había almacenado el id del activo en la variable de session
                    String id = Session["id"].ToString();
                    Boolean resultado = new Activo().EliminarActivo(id);
                    Session["filtroActivos"] = "";//guardo el filtro
                    return RedirectToAction("Index");
                case "Cancelar":
                    Session["filtroActivos"] = "";//guardo el filtro
                    return RedirectToAction("Index");
            }
            return View();
        }


        /*public ActionResult LeerArchivo()
        {
            ActivosTableAdapter ac = new ActivosTableAdapter();
            FileInfo archivo = new FileInfo(@"C:\Users\Informática\Downloads\activos.xlsx");
            using (ExcelPackage package = new ExcelPackage(archivo))
            {
                ExcelWorkbook workBook = package.Workbook;
                if (workBook != null)
                {
                    if (workBook.Worksheets.Count > 0)
                    {
                        // Get the first worksheet
                        ExcelWorksheet currentWorksheet = workBook.Worksheets.First();
                        for (int row = 852; row <= 2532; ++row)
                        {
                            String placa = (currentWorksheet.Cells[row, 2].Value != null) ? currentWorksheet.Cells[row, 2].Value.ToString() : @"N/A";
                            String nombre = (currentWorksheet.Cells[row, 3].Value != null) ? currentWorksheet.Cells[row, 3].Value.ToString() : @"N/A";
                            String espacio = (currentWorksheet.Cells[row, 4].Value != null) ? currentWorksheet.Cells[row, 4].Value.ToString() : @"N/A";
                            String encargado = (currentWorksheet.Cells[row, 5].Value != null) ? currentWorksheet.Cells[row, 5].Value.ToString() : @"N/A";
                            String estado = (currentWorksheet.Cells[row, 6].Value != null) ? currentWorksheet.Cells[row, 6].Value.ToString() : @"N/A";
                            int est = (estado.Equals("bueno", StringComparison.InvariantCultureIgnoreCase)) ? 0 : 1;
                            String inventario = (currentWorksheet.Cells[row, 7].Value != null) ? currentWorksheet.Cells[row, 7].Value.ToString() : @"N/A";
                            ac.Insert(Guid.NewGuid(), Convert.ToInt32(placa), nombre, espacio, encargado, est, inventario, "No");
                        }
                    }
                }
            }
            return View();
        }*/

    }
}
