using ApotheGSF.Models;
using System.ComponentModel.DataAnnotations;
using ApotheGSF.Clases;

namespace ApotheGSF.ViewModels
{
    public class FacturaViewModel
    {
        public int Codigo { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public List<MedicamentosDetalle> MedicamentosDetalle { get; set; }
        public DateTime? Creado { get; set; }
        public string? CreadoNombreUsuario { get; set; }
        public DateTime? Modificado { get; set; }
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
