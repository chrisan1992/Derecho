using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ActivosDerecho.Models
{
    public class IContrasena
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Nombre Requerido")]
        [StringLength(128, ErrorMessage = "Nombre no válido")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?)*", ErrorMessage = "Nombre no válido")]
        public String nombre { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Usuario Requerido")]
        [StringLength(128, ErrorMessage = "Nombre no válido")]
        [RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?)*", ErrorMessage = "Usuario no válido")]
        public String usuario { get; set; }

        /*
         * (?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$  
         * 
         * Contraseñas que contengan al menos una letra mayúscula.
         * Contraseñas que contengan al menos una letra minúscula.
         * Contraseñas que contengan al menos un número o caracter especial.
         * Contraseñas cuya longitud sea como mínimo 8 caracteres.
         * Contraseñas cuya longitud máxima no debe ser arbitrariamente limitada.
         * 
         * */

        [Display(Name = "Contraseña Usuario")]
        [Required(ErrorMessage = "Contraseña Requerida")]
        [StringLength(128, ErrorMessage = "Contraseña no válida")]
        //[RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?)*", ErrorMessage = "Contraseña no válida")]
        public String passUsuario { get; set; }


        [Display(Name = "Correo")]
        [Required(ErrorMessage = "Usuario Requerido")]
        //la expresion regular del correo va con el html5 de type="email"
        public String correo { get; set; }

        [Display(Name = "Contraseña Correo")]
        [Required(ErrorMessage = "Contraseña Requerida")]
        [StringLength(128, ErrorMessage = "Contraseña no válida")]
        //[RegularExpression(@"[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?(( |\-)[a-zA-ZÀ-ÖØ-öø-ÿ0-9]+\.?)*", ErrorMessage = "Contraseña no válida")]
        public String passCorreo { get; set; }
        
    }

    [MetadataType(typeof(IContrasena))]
    public partial class Contrasena
    {
        /*Aqui los atributos que no estan en la tabla de la base de datos*/
        [Display(Name = "Confirmar Contraseña")]
        [Compare("passUsuario", ErrorMessage = "Contraseñas no coinciden!")]
        public String passUsuarioComparar { get; set; }

        [Display(Name = "Confirmar Contraseña")]
        [Compare("passCorreo", ErrorMessage = "Contraseñas no coinciden!")]
        public String passCorreoComparar { get; set; }

        public List<Contrasena> GetContraseñas(String filtro)
        {
            List<Contrasena> lista = new List<Contrasena>();
            try
            {
                ModeloDataContext dt = new ModeloDataContext();
                var items = from a in dt.Contrasenas
                            //busqueda filtrada
                            where SqlMethods.Like(a.nombre + "", "%" + filtro + "%")
                             || SqlMethods.Like(a.correo + "", "%" + filtro + "%")
                            orderby a.nombre
                            select a;
                foreach (Contrasena c in items)
                {
                    lista.Add(c);
                }
                dt.Dispose();
            }
            catch (SqlException ex)
            {

            }
            return lista;
        }

        public Boolean AgregarContrasena(Contrasena c)
        {
            try
            {
                ModeloDataContext dt = new ModeloDataContext();
                dt.Contrasenas.InsertOnSubmit(c);
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