using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class Proveedores
    {
        [Key]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "RNC")]
       // [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{5})[-. ]?([0-9]{1})$", ErrorMessage = "RNC inválido")]
        public string RNC { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        // [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Teléfono inválido")]
        [Display(Name = "Teléfono 1")]
        public string Telefono1 { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        //  [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Teléfono inválido")]
        [Display(Name = "Teléfono 2")]
        public string Telefono2 { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        // [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Fax inválido")]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "TerminosdePago")]
        public string TerminosdePago { get; set; }

        [Column("Creado", TypeName = "datetime")]
        public DateTime? Creado { get; set; }
        public string? CreadoNombreUsuario { get; set; }
        public DateTime? Modificado { get; set; }
        public string? ModificadoNombreUsuario { get; set; }
        public bool? Inactivo { get; set; }

        
        public Proveedores()
        {
            ProveedoresMedicamentos = new HashSet<ProveedorMedicamentos>();
        }
        
       public ICollection<ProveedorMedicamentos> ProveedoresMedicamentos { get; set; }
        
    
    }
}
