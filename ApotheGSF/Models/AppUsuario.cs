using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.Models
{
    public class AppUsuario : IdentityUser<int>
    {
        public ICollection<AppUsuarioRol> UserRoles { get; set; }
    }
}
