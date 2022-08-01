using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Facturas
    {
        [Key]
        public int Codigo { get; set; }
       // [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public float SubTotal { get; set; }
      //  [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public float Total { get; set; }

        [Column("Creado", TypeName = "datetime")]
        public DateTime? Creado { get; set; }
        public string? CreadoNombreUsuario { get; set; }
        public DateTime? Modificado { get; set; }
        public string? ModificadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }

        public Facturas()
        {
            FacturasMedicamentosCajas = new HashSet<FacturaMedicamentosCajas>();
        }
        
        public ICollection<FacturaMedicamentosCajas> FacturasMedicamentosCajas { get; set; }
    }
}
