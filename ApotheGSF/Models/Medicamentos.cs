using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Medicamentos
    {
        [Key]
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [StringLength(30)]
        [Display(Name = "Nombre: ")]
        public string Nombre { get; set; }
        [Display(Name = "Nombre Científico: ")]
        public string NombreCientifico { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Categoria: ")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Sustancia: ")]
        public string Sustancia { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Concentración: ")]
        public string Concentracion { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Unidades por Caja: ")]
        public int UnidadesCaja { get; set; }
        [Display(Name = "Reorden: ")]
        public int Reorden { get; set; }
        [Display(Name = "Detallable: ")]
        public bool Detallable { get; set; }

        public Medicamentos()
        {
            MedicamentosCajas = new HashSet<MedicamentosCajas>();
        }
        public ICollection<MedicamentosCajas> MedicamentosCajas { get; set; }

        [Column("Creado", TypeName = "datetime")]
        [Display(Name = "Fecha de Creación: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Última Modificación: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado por: ")]
        public string? ModificadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }
        public bool? EnvioPendiente { get; set; }


    }
}
