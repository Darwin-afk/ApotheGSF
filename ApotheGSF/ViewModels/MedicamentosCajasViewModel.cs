using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.ViewModels
{
    public class MedicamentosCajasViewModel
    {
        [Display(Name = "Código: ")]
        public int CodigoCaja { get; set; }
        [Display(Name = "Código del Medicamento: ")]
        public int CodigoMedicamento { get; set; }
        [Display(Name = "Cantidad de Unidades: ")]
        public int CantidadUnidad { get; set; }
        [Required(ErrorMessage = "Digite la fecha de adquisición")]
        [Display(Name = "Fecha de Adquisición: ")]
        public DateTime FechaAdquirido { get; set; }
        [Required(ErrorMessage = "Digite la fecha de vencimiento")]
        [Display(Name = "Fecha de Vencimiento: ")]
        public DateTime FechaVencimiento { get; set; }
        [Display(Name = "Detallada? ")]
        public bool Detallada { get; set; }
        public bool Inactivo { get; set; }
        [Display(Name = "Medicamento: ")]
        public string? NombreMedicamento { get; set; }
        [Required(ErrorMessage = "Digite una cantidad")]
        [Display(Name = "Lotes: ")]
        public int Cajas { get; set; }

        [Display(Name = "Fecha de Creación: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Última Modificación: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado por: ")]
        public string? ModificadoNombreUsuario { get; set; }
    }
}
