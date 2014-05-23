using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ActivosDerecho.Models
{
    public class IActivo
    {

        [Required(ErrorMessage = "Placa Requerida")]
        [Display(Name = "Placa")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Placa no válida")]
        public int placa { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Nombre y Descripción")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?)*", ErrorMessage = "Campo no válido")]
        public String nombreDescripcion { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Espacio Físico")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?)*", ErrorMessage = "Campo no válido")]
        public String espacioFisico { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Encargado")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?)*", ErrorMessage = "Campo no válido")]
        public String encargado { get; set; }

        [Display(Name = "Estado")]
        public int estado { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Inventario Por")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?)*", ErrorMessage = "Campo no válido")]
        public String inventarioPor { get; set; }

        //[Required(ErrorMessage = "Campo Requerido")]
        [Display(Name = "Conciliación OAF")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9\/_]+\.?)*", ErrorMessage = "Campo no válido")]
        public String conciliacion { get; set; }
    }

    [MetadataType(typeof(IActivo))]
    public partial class Activo
    {
        //metodos de la clase
        public Boolean AgregarNuevoActivo(Activo ac)
        {
            try
            {
                ac._id = Guid.NewGuid();
                if (ac.conciliacion.Equals(""))
                    ac.conciliacion = "No";
                ModeloDataContext dt = new ModeloDataContext();
                dt.Activos.InsertOnSubmit(ac);
                dt.SubmitChanges();
                dt.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Activo> GetListaActivos(String filtro = "")
        {
            List<Activo> lista = new List<Activo>();
            try
            {
                ModeloDataContext dt = new ModeloDataContext();
                var items = from a in dt.Activos
                            //busqueda filtrada
                            where SqlMethods.Like(a.placa + "", "%" + filtro + "%")
                             || SqlMethods.Like(a.nombreDescripcion + "", "%" + filtro + "%")
                             || SqlMethods.Like(a.encargado + "", "%" + filtro + "%")
                             || SqlMethods.Like(a.inventarioPor + "", "%" + filtro + "%")
                             || SqlMethods.Like(a.conciliacion + "", "%" + filtro + "%")
                            orderby a.nombreDescripcion
                            select a;
                foreach (Activo ac in items)
                {
                    lista.Add(ac);
                }
                dt.Dispose();
            }
            catch (SqlException ex)
            {

            }
            return lista;
        }

        public Activo ConsultarActivo(String id)
        {
            List<Activo> lista = new List<Activo>();
            try
            {
                ModeloDataContext dt = new ModeloDataContext();
                Guid actual = new Guid(id);
                var items = dt.Activos.Where(ai => ai.id == actual);
                foreach (Activo ac in items)
                {
                    lista.Add(ac);
                }
                dt.Dispose();
            }
            catch (SqlException ex)
            {

            }
            if (lista.Count == 1)
                return lista[0];
            else
                return new Activo();
        }

        public Boolean ActualizarActivo(Activo ac)
        {
            try
            {
                ModeloDataContext dt = new ModeloDataContext();
                var items = dt.Activos.Where(ai => ai.id == ac.id);
                foreach (Activo a in items)
                {
                    a.placa = ac.placa;
                    a.nombreDescripcion = ac.nombreDescripcion;
                    a.espacioFisico = ac.espacioFisico;
                    a.encargado = ac.encargado;
                    a.estado = ac.estado;
                    a.inventarioPor = ac.inventarioPor;
                    a.conciliacion = ac.conciliacion;
                }
                dt.SubmitChanges();
                dt.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Boolean EliminarActivo(String id)
        {
            try
            {
                ModeloDataContext dt = new ModeloDataContext();
                Guid actual = new Guid(id);
                var items = dt.Activos.Where(ai => ai.id == actual);
                foreach (Activo ac in items)
                {
                    dt.Activos.DeleteOnSubmit(ac);
                }
                dt.SubmitChanges();
                dt.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}