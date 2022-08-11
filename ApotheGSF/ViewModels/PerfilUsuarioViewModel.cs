using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class PerfilUsuarioViewModel
    {
		[Display(Name = "Código: ")]
		public int Codigo { get; set; }

		[Required(ErrorMessage = "Digite el nombre del usuario.")]
		[MaxLength(50)]
		[Display(Name = "Nombre: ")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "Digite el apellido del usuario.")]
		[MaxLength(50)]
		[Display(Name = "Apellido: ")]
		public string Apellido { get; set; }
		[Display(Name = "Foto: ")]
		public string? Foto { get; set; }

		[MaxLength(256)]
		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Digite el email.")]
		[Display(Name = "Email: ")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Digite el teléfono.")]
		[Display(Name = "Teléfono: ")]
		public string Telefono { get; set; }
	}
}
