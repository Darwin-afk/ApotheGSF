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

		[Required(ErrorMessage = "Digite el correo electrónico del usuario.")]
		[MaxLength(100)]
		[EmailAddress]
		[Display(Name = "Email: ")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Digite el teléfono.")]
		[RegularExpression(@"^(\+1)?\s?\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})(\s?(x|([Ee]xt[.:]?\s?))[0-9]{4})?$", ErrorMessage = "Teléfono inválido")]
		[Display(Name = "Teléfono: ")]
		public string Telefono { get; set; }
	}
}
