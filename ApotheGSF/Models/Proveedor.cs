using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Proveedor
    {
        [Key]
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public ICollection<ProveedorMedicamento> ProveedoresMedicamentos { get; set; }
    }
}
