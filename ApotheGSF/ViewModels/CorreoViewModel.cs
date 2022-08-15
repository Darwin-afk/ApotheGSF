using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class CorreoViewModel
    {
        [Display(Name = "Medicamento: ")]
        public string NombreMedicamento { get; set; }
        [Display(Name = "Código del Proveedor: ")]
        public int CodigoProveedor { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Cajas: ")]
        public int Cajas { get; set; }
    }
}
