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
    using System.Collections.Generic;
    
    public partial class tbClientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbClientes()
        {
            this.tbActivos = new HashSet<tbActivos>();
        }
    
        public int clie_Id { get; set; }
        public string clie_CodigoCompania { get; set; }
        public string clie_NombreCliente { get; set; }
        public string clie_RTNCliente { get; set; }
        public string clie_Lugar { get; set; }
        public string clie_Responsale { get; set; }
        public string clie_Telefono { get; set; }
        public int usua_UsuarioCreacion { get; set; }
        public System.DateTime clie_FechaCreacion { get; set; }
        public Nullable<int> usua_Modificacion { get; set; }
        public Nullable<System.DateTime> clie_FechaModificacion { get; set; }
        public Nullable<bool> clie_Estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbActivos> tbActivos { get; set; }
    }
}
