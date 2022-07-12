using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Facturas
    {
        [Key]
        public int Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [ForeignKey("Medicamentos")]
        public virtual Medicamentos? MedicamentosId  { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public string? Estado { get; set; }

        [Column("Creado", TypeName = "datetime")]
        public DateTime? Creado { get; set; }
        public int? CreadoId { get; set; }
        public DateTime? Modificado { get; set; }
        public int? ModificadoId { get; set; }
        public bool? Inactivo { get; set; }


        [NotMapped]
        public bool IsUpdate { get; set; }

        public Facturas()
        {
            FacturasMedicamentos = new HashSet<FacturaMedicamentos>();
        }
        
        public ICollection<FacturaMedicamentos> FacturasMedicamentos { get; set; }
    }
}
