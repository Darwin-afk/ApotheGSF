using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Facturas
    {
        [Key]
        public int Codigo { get; set; }
        public ICollection<FacturaMedicamentos> FacturasMedicamentos { get; set; }
    }
}
