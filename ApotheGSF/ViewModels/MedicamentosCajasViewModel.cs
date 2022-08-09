using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.ViewModels
{
    public class MedicamentosCajasViewModel
    {
        public int CodigoCaja { get; set; }
        public int CodigoMedicamento { get; set; }
        public int CantidadUnidad { get; set; }
        [Required(ErrorMessage = "Digite la fecha de adquisición")]
        public DateTime FechaAdquirido { get; set; }
        [Required(ErrorMessage = "Digite la fecha de vencimiento")]
        public DateTime FechaVencimiento { get; set; }
        public bool Detallada { get; set; }
        public bool Inactivo { get; set; }
        public string? NombreMedicamento { get; set; }
        [Required(ErrorMessage = "Digite una cantidad")]
        public int Cajas { get; set; }
    }
}
