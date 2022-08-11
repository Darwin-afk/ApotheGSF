using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Facturas
    {
        [Key]
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        // [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "SubTotal: ")]
        public float SubTotal { get; set; }
        //  [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Total: ")]
        public float Total { get; set; }

        [Column("Creado", TypeName = "datetime")]
        [Display(Name = "Fecha de Creacion: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Última Modificación: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado Por: ")]
        public string? ModificadoNombreUsuario { get; set; }
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
