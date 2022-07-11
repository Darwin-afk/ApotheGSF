using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class PerfilUsuarioViewModel
    {
		public int Id { get; set; }

		[Required(ErrorMessage = "Digite el nombre del usuario.")]
		[MaxLength(50)]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "Digite el apellido del usuario.")]
		[MaxLength(50)]
		public string Apellido { get; set; }

		public string? Foto { get; set; }

		[MaxLength(256)]
		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Digite el email.")]
		public string Email { get; set; }

		[Display(Name = "Teléfono")]
		[Required(ErrorMessage = "Digite el teléfono.")]
		public string Telefono { get; set; }
	}
}
