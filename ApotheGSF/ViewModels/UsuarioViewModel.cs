﻿using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class UsuarioViewModel
    {
		[Display(Name = "ID")]
		public int Id { get; set; }

		[Required(ErrorMessage = "Digite el nombre del usuario.")]
		[MaxLength(50)]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "Digite el apellido del usuario.")]
		[MaxLength(50)]
		public string Apellido { get; set; }

		[Required(ErrorMessage = "Digite el usuario.")]
		[MaxLength(30)]
		public string Usuario { get; set; }

		[Required(ErrorMessage = "La Contraseña no puede estar en blanco.")]
		[MaxLength(30, ErrorMessage = "La cantidad máxima es de 30 caracteres.")]
		[Display(Name = "Contraseña")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage = "La Confirmación de la Contraseña no puede estar en blanco.")]
		[Display(Name = "Confirmar")]
		[MaxLength(30, ErrorMessage = "La cantidad máxima es de 30 caracteres.")]
		[Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
		[DataType(DataType.Password)]
		public string ConfirmarPassword { get; set; }

		[Display(Name = "Rol")]
		[Required(ErrorMessage = "Debe elegir un rol.")]
		public string Rol { get; set; }

		[Display(Name = "Fecha de nacimiento")]
		public DateTime FechaNacimiento { get; set; }

		[Display(Name = "Cédula")]
		public string Cedula { get; set; }

        [Required(ErrorMessage = "Digite el correo electrónico del usuario.")]
		[MaxLength(100)]
		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		public string Telefono { get; set; }
        public string Direccion { get; set; }

        [Display(Name = "Creado Por")]
		public string? CreadPor { get; set; }

		[Display(Name = "Modificado Por")]
		public string? ModificadoPor { get; set; }

		[Display(Name = "Creado")]
		public DateTime? Creado { get; set; }
		[Display(Name = "Última Modificación")]
		public DateTime? Modificado { get; set; }
        public bool? Inactivo { get; set; }
    }
}
