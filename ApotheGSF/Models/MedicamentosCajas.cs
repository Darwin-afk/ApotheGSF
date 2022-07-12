using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class MedicamentosCajas
    {
        [Key]
        public int CajaId { get; set; }
        [ForeignKey("Medicamentos")]
        public virtual Medicamentos? MedicamentosId { get; set; }
        [NotMapped]
        public string? NombreMedicamentos { get; set; }
        public DateTime FechaAdquirido { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Detallada { get; set; }

        public MedicamentosCajas()
        {
            FacturaMedicamentos = new HashSet<FacturaMedicamentos>();
        }

        public ICollection<FacturaMedicamentos> FacturaMedicamentos { get; set; } 
    }
}
