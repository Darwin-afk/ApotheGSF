using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class FacturaMedicamentosCajas
    {
        [ForeignKey("Facturas")]
        public int CodigoFactura { get; set; }
        public virtual Facturas? Facturas { get; set; }
        [ForeignKey("MedicamentosCajas")]
        public int CodigoCaja { get; set; }
        public virtual MedicamentosCajas? MedicamentosCajas { get; set; }
        public int TipoCantidad { get; set; }//1: caja, 2: unidad
        public int CantidadUnidad { get; set; }
        public int Precio { get; set; }


    }
}

