using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Medicamentos
    {
        [Key]
        public int Codigo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [StringLength(30)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Marca { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Sustancia { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int UnidadesPorCaja { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Concentracion { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int Costo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int PrecioUnidad { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Indicaciones { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Dosis { get; set; }
        
        public Medicamentos()
        {
            ProveedoresMedicamentos = new HashSet<ProveedorMedicamentos>();
            MedicamentosCajas = new HashSet<MedicamentosCajas>();
        }
        
        public ICollection<ProveedorMedicamentos> ProveedoresMedicamentos { get; set; }
        public ICollection<MedicamentosCajas> MedicamentosCajas { get; set; }

        [Column("Creado", TypeName = "datetime")]
        public DateTime? Creado { get; set; }
        public string? CreadoNombreUsuario { get; set; }
        public DateTime? Modificado { get; set; }
        public string? ModificadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }

      

    }
}
