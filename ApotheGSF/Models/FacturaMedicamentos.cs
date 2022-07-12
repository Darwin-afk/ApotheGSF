using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class FacturaMedicamentos
    {
        [Key]
        public int FacturaMedicamentoId { get; set; }
        [ForeignKey("Facturas")]
        public virtual Facturas? FacturaId { get; set; }
        [ForeignKey("Medicamentos")]
        public virtual Medicamentos? CajaId { get; set; }
        public int CantidadUnidad { get; set; }
        public float Precio { get; set; }

        
    }
}
