using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Activos.Models
{
    [MetadataType(typeof(tbActivosMetaData))]

    public partial class tbActivos
    {
    }
    public class tbActivosMetaData
    {
        [Display(Name = "Id")]
        public int acvo_Id { get; set; }

        [Display(Name = "Mombre del activo")]
        [Required(ErrorMessage = " es obligatorio")]
        public string acvo_Nombre { get; set; }

        [Display(Name = "Numero de serie")]
        [Required(ErrorMessage = " es obligatorio")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Debe contener solo números")]
        public string acvo_NumeroSerie { get; set; }

        [Display(Name = "Vida Util")]
        [Required(ErrorMessage = " es obligatorio")]
        public int viut_Id { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = " es obligatorio")]
        public int clie_Id { get; set; }

        [Display(Name = "Ubicación")]
        [Required(ErrorMessage = " es obligatorio")]
        public string acvo_Ubicacion { get; set; }

        [Display(Name = "Decha de adquisición")]
        [Required(ErrorMessage = " es obligatorio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime acvo_FechaAdquisicion { get; set; }

        [Display(Name = "Costo original")]
        [Required(ErrorMessage = " es obligatorio")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Debe contener solo números")]
        public string acvo_CostoOriginal { get; set; }
    }
}