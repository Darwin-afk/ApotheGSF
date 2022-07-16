using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class FacturaMedicamentos
    {
        [Key]
        public int FacturaMedicamentoId { get; set; }
        [ForeignKey("Facturas")]
        public int FacturaId { get; set; }
        public virtual Facturas? Facturas { get; set; }
        [ForeignKey("MedicamentosCajas")]
        public int CajaId { get; set; }
        public virtual MedicamentosCajas? MedicamentosCajas { get; set; }
        public int CantidadUnidad { get; set; }
        public float Precio { get; set; }

        
    }
}
