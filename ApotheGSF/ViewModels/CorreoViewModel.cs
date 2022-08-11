using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class CorreoViewModel
    {
        [Display(Name = "Medicamento: ")]
        public string NombreMedicamento { get; set; }
        [Display(Name = "Código del Proveedor: ")]
        public int CodigoProveedor { get; set; }
        [Display(Name = "Cajas: ")]
        public int Cajas { get; set; }
    }
}
