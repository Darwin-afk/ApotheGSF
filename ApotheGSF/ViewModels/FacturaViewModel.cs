using ApotheGSF.Models;
using System.ComponentModel.DataAnnotations;
using ApotheGSF.Clases;

namespace ApotheGSF.ViewModels
{
    public class FacturaViewModel
    {
        public int Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public string? Estado { get; set; }
        public List<MedicamentosDetalle> MedicamentosDetalle { get; set; }
        public DateTime? Creado { get; set; }
        public int? CreadoId { get; set; }
        public DateTime? Modificado { get; set; }
        public int? ModificadoId { get; set; }
        public bool? Inactivo { get; set; }
        public int MedicamentoId { get; set; }
        public int TipoCantidad { get; set; }
        public int Cantidad { get; set; }

        public FacturaViewModel()
        {
            MedicamentosDetalle = new List<MedicamentosDetalle>();
            MedicamentoId = 0;
            TipoCantidad = 1;
            Cantidad = 0;
        }

        
    }
}
