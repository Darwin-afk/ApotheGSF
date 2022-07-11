using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Factura
    {
        [Key]
        public int Codigo { get; set; }
        public ICollection<FacturaMedicamento> FacturasMedicamentos { get; set; }
    }
}
