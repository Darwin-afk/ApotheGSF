using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class AppUsuario : IdentityUser<int>
    {
        [Required]
        [Column("Nombre", TypeName = "varchar(50)")]
        public string Nombre { get; set; }
        [Required]
        [Column("Apellido", TypeName = "varchar(50)")]
        public string Apellido { get; set; }
        [Column("Foto", TypeName = "varchar(50)")]
        public string? Foto { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public string? Cedula { get; set; }
        public string? Direccion { get; set; }
        [NotMapped]
        [Display(Name = "Rol")]
        public string? Rol { get; set; }
        public ICollection<AppUsuarioRol> UsuariosRoles { get; set; }
        public DateTime Creado { get; set; }
        public int CreadoPorId { get; set; }
        public DateTime? Modificado { get; set; }
        public int? ModificadoPorId { get; set; }
        public bool Inactivo { get; set; }
    }
}
