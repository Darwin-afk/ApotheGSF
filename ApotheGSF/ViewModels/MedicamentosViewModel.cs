using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class MedicamentosViewModel
    {
        public int Codigo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Categoria { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Sustancia { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Concentracion { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int UnidadesCaja { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int Costo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int PrecioUnidad { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Indicaciones { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Dosis { get; set; }

        public List<int>? ProveedoresId { get; set; }
        public string? NombreProveedor { get; set; }

        public bool Inactivo { get; set; }

    }
}
