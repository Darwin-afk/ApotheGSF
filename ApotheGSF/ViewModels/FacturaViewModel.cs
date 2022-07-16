using ApotheGSF.Models;
using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class FacturaViewModel
    {
        public int Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public string? Estado { get; set; }
        public List<FacturaMedicamentosCajas> Medicamentos { get; set; }
        public DateTime? Creado { get; set; }
        public int? CreadoId { get; set; }
        public DateTime? Modificado { get; set; }
        public int? ModificadoId { get; set; }
    }
}
