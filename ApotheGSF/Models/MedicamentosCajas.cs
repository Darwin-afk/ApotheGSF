using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class MedicamentosCajas
    {
        [Key]
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        [ForeignKey("Medicamentos")]
        [Display(Name = "Código del Medicamentos: ")]
        public int CodigoMedicamento { get; set; }
        public virtual Medicamentos? Medicamentos { get; set; }
        [Required(ErrorMessage = "Digite una cantidad")]
        [Display(Name = "Cantidad de Unidades: ")]
        public int CantidadUnidad { get; set; }
        [Required(ErrorMessage = "Digite la fecha de adquisición")]
        [Display(Name = "Fecha de Adquisición: ")]
        public DateTime FechaAdquirido { get; set; }
        [Required(ErrorMessage = "Digite la fecha de vencimiento")]
        [Display(Name = "Fecha de Vencimiento: ")]
        public DateTime FechaVencimiento { get; set; }
        public bool Detallada { get; set; }
        [Display(Name = "Fecha de Creación: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Última Modificación: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado por: ")]
        public string? ModificadoNombreUsuario { get; set; }
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
