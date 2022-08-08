using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class MedicamentosCajas
    {
        [Key]
        public int Codigo { get; set; }
        [ForeignKey("Medicamentos")]
        public int CodigoMedicamento { get; set; }
        public virtual Medicamentos? Medicamentos { get; set; }
        public int CantidadUnidad { get; set; }
        public DateTime FechaAdquirido { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Detallada { get; set; }
        public bool Inactivo { get; set; }

        public MedicamentosCajas()
        {
            FacturaMedicamentos = new HashSet<FacturaMedicamentosCajas>();
        }

        public ICollection<FacturaMedicamentosCajas> FacturaMedicamentos { get; set; }

        [NotMapped]
        public int NumeroCaja { get; set; }
    }
}
