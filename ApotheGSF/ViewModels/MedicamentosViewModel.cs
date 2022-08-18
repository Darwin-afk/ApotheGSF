using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class MedicamentosViewModel
    {
        [Display(Name = "Código: ")]
        public int Codigo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
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
        [Display(Name = "Unidades: ")]
        public int UnidadesCaja { get; set; }
        [Display(Name = "Lotes: ")]
        public int Cajas { get; set; }
        public bool? Inactivo { get; set; }

        [Display(Name = "Fecha de Creación: ")]
        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Última Modificación: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado por: ")]
        public string? ModificadoNombreUsuario { get; set; }

    }
}
