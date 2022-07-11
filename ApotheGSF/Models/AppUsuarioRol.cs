using Microsoft.AspNetCore.Identity;

namespace ApotheGSF.Models
{
    public class AppUsuarioRol : IdentityUserRole<int>
    {
        public AppUsuario Usuario { get; set; }
        public AppRol Rol { get; set; }
    }
}