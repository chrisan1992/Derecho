using ActivosDerecho.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace ActivosDerecho.Controllers
{
    public class ContrasenasController : Controller
    {
        //
        // GET: /Contrasenas/

        /*public ActionResult Index(int? pagina = 1)
        {
            //Esto es para la paginacion de la tabla Solo hay que instalar el paquete NuGet que se llama PagedList
            if (Request.HttpMethod != "GET")
            {
                pagina = 1;
            }
            int tamPagina = 20;
            int numPagina = (pagina ?? 1);
            List<Contrasena> lista = new Contrasena().GetContraseñas(Session["filtroActivos"].ToString());
            ViewBag.FiltroActual = Session["filtroActivos"].ToString();
            return View(lista.ToPagedList(numPagina, tamPagina));//Paginacion
        }*/

        /// <summary>
        /// Guarda en la variable filtro lo que el usuario haya digitado
        /// </summary>
        /// <param name="filtro">contiene el filtro que el usuario quiere aplicar</param>
        /// <returns></returns>
        /*[HttpPost]
        public ActionResult Index(String filtro="")
        {
            //guardo el filtro que ha digitado el usuario
            Session["filtroContrasenas"] = filtro;
            return RedirectToAction("Index");
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Agregar()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="btn"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Agregar(Contrasena c, string btn)
        {
            switch (btn)
            {//para saber cual botón causó el post
                case "Guardar":
                    if (ModelState.IsValid)
                    {//si el modelo es valido entonces agrego
                        Boolean resultado = c.AgregarContrasena(c);
                        Session["filtroContrasenas"] = "";//guardo el filtro
                        return View("Exito");
                    }
                    //si llega aquí hay algo mal, se retorna el modelo para llenar
                    return View(c);
                case "Cancelar":
                    Session["filtroContrasenas"] = "";//guardo el filtro
                    return RedirectToAction("Agregar");
                default:
                    Session["filtroContrasenas"] = "";//guardo el filtro
                    return RedirectToAction("Agregar");
            }
        }

    }
}
