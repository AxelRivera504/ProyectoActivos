//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Activos.Models
{
    using System;
    
    public partial class UDP_tbActivos_ListarActivos_Result
    {
        public int acvo_Id { get; set; }
        public string acvo_Nombre { get; set; }
        public string acvo_NumeroSerie { get; set; }
        public int viut_Id { get; set; }
        public int viut_VidaUtil { get; set; }
        public int clie_Id { get; set; }
        public string clie_NombreCliente { get; set; }
        public string clie_CodigoCompania { get; set; }
        public string acvo_Ubicacion { get; set; }
        public System.DateTime acvo_FechaAdquisicion { get; set; }
        public System.DateTime acvo_InicioDepreciacion { get; set; }
        public decimal acvo_CostoOriginal { get; set; }
        public decimal acvo_ValorResidual { get; set; }
        public decimal acvo_CostoDespreciable { get; set; }
        public Nullable<bool> acvo_Estado { get; set; }
        public System.DateTime acvo_FechaCreacion { get; set; }
        public Nullable<System.DateTime> acvo_FechaModificacion { get; set; }
    }
}
