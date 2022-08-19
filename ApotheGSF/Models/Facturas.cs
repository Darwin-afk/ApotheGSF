using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Facturas
    {
        [Key]
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "SubTotal: ")]
        public float SubTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Total: ")]
        public float Total { get; set; }

        [Column("Creado", TypeName = "datetime")]
        [Display(Name = "Fecha de Creacion: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }

        public Facturas()
        {
            FacturasMedicamentosCajas = new HashSet<FacturaMedicamentosCajas>();
        }
        
        public ICollection<FacturaMedicamentosCajas> FacturasMedicamentosCajas { get; set; }

        [NotMapped]
        public int Posicion { get; set; }
    }
}
