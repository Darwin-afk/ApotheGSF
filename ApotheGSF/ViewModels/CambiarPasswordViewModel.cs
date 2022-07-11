using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class CambiarPasswordViewModel
    {
		[Required]
		[Display(Name = "ID")]
		public int Id { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }

		[Required(ErrorMessage = "La Contraseña actual no puede estar en blanco.")]
		[Display(Name = "Contraseña Actual")]
		[MaxLength(30, ErrorMessage = "La cantidad máxima es de 30 caracteres.")]
		[DataType(DataType.Password)]
		public string PasswordActual { get; set; }


		[Required(ErrorMessage = "La Contraseña no puede estar en blanco.")]
		[MaxLength(30, ErrorMessage = "La cantidad máxima es de 30 caracteres.")]
		[Display(Name = "Nueva Contraseña")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage = "La Confirmación de la Contraseña no puede estar en blanco.")]
		[Display(Name = "Confirmar Nueva Contraseña")]
		[MaxLength(30, ErrorMessage = "La cantidad máxima es de 30 caracteres.")]

		[Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
		[DataType(DataType.Password)]
		public string ConfirmarPassword { get; set; }
	}
}
