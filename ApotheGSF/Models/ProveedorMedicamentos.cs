using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class ProveedorMedicamentos
    {
        [ForeignKey("Proveedores")]
        public int ProveedoresId { get; set; }
        public virtual Proveedores? Proveedores { get; set; }

        [NotMapped]
        public string? NombreProveedores { get; set; }
        
        [ForeignKey("Medicamentos")]
        public int MedicamentosId { get; set; }
        public virtual Medicamentos? Medicamentos { get; set; }
        
        [NotMapped]
        public string? NombreMedicamentos { get; set; }
     
    }
}
