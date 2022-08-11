using ApotheGSF.Models;
using System.ComponentModel.DataAnnotations;
using ApotheGSF.Clases;

namespace ApotheGSF.ViewModels
{
    public class FacturaViewModel
    {
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        [Display(Name = "SubTotal: ")]
        public float SubTotal { get; set; }
        [Display(Name = "Total: ")]
        public float Total { get; set; }
        public List<MedicamentosDetalle> MedicamentosDetalle { get; set; }
        [Display(Name = "Fecha de Creación: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado Por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Ultima Modificaión: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado Por: ")]
        public string? ModificadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }

        public FacturaViewModel()
        {
            MedicamentosDetalle = new List<MedicamentosDetalle>();
            SubTotal = 0;
            Total = 0;
        }

        
    }
}
