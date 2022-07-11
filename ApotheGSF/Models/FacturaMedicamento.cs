using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.Models
{
    public class FacturaMedicamento
    {
        [Key]
        public int FacturaMedicamentoId { get; set; }
        public int FacturaId { get; set; }
        public int MedicamentoId { get; set; }
    }
}
