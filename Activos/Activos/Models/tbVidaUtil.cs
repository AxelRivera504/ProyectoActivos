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
    
    public partial class tbVidaUtil
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbVidaUtil()
        {
            this.tbActivos = new HashSet<tbActivos>();
        }
    
        public int viut_Id { get; set; }
        public string viut_Objeto { get; set; }
        public string viut_Descripcion { get; set; }
        public int viut_VidaUtil { get; set; }
        public int usua_UsuarioCreacion { get; set; }
        public System.DateTime viut_FechaCreacion { get; set; }
        public Nullable<int> usua_Modificacion { get; set; }
        public Nullable<System.DateTime> viut_FechaModificacion { get; set; }
        public Nullable<bool> viut_Estado { get; set; }
    
        public virtual tbUsuarios tbUsuarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbActivos> tbActivos { get; set; }
    }
}
