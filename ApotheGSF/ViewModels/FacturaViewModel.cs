using ApotheGSF.Models;
using System.ComponentModel.DataAnnotations;
using ApotheGSF.Clases;

namespace ApotheGSF.ViewModels
{
    public class FacturaViewModel
    {
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "SubTotal: ")]
        
        public float SubTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Total: ")]
        public float Total { get; set; }
        public List<MedicamentosDetalle> MedicamentosDetalle { get; set; }
        [Display(Name = "Fecha de Creación: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado Por: ")]
        public string? CreadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }

        public FacturaViewModel()
        {
            MedicamentosDetalle = new List<MedicamentosDetalle>();
            SubTotal = 0;
            Total = 0;
        }

        
    }
}
