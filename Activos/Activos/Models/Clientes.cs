using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Activos.Models
{
    [MetadataType(typeof(tbClientesMetaData))]

    public partial class tbClientes
    {
    }
    public class tbClientesMetaData
    {
        [Display(Name = "Id")]
        public int clie_Id { get; set; }

        [Display(Name = "Código de la compañia")]
        [Required(ErrorMessage = " es obligatorio")]
        public string clie_CodigoCompania { get; set; }

        [Display(Name = "Nombre del cliente")]
        [Required(ErrorMessage = " es obligatorio")]
        public string clie_NombreCliente { get; set; }

        [Display(Name = "RTN del cliente")]
        [Required(ErrorMessage = " es obligatorio")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{6}$", ErrorMessage = "Debe tener el formato 9999-9999-999999 y contener solo números")]

        public string clie_RTNCliente { get; set; }

        [Display(Name = "Lugar")]
        [Required(ErrorMessage = " es obligatorio")]
        public string clie_Lugar { get; set; }
        [Display(Name = "Responsable")]
        [Required(ErrorMessage = " es obligatorio")]
        public string clie_Responsale { get; set; }

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = " es obligatorio")]
        [RegularExpression(@"^[0-9]{4}\-[0-9]{4}$", ErrorMessage = " debe poseer el formato (9999-9999)")]
        public string clie_Telefono { get; set; }


    }
}