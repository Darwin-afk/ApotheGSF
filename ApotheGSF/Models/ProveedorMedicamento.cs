using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.Models
{
    public class ProveedorMedicamento
    {
        [Key]
        public int ProveedorMedicamentoId { get; set; }
        public int ProveedorId { get; set; }
        public int MedicamentoId { get; set; }
    }
}
