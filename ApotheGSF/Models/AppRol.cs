using Microsoft.AspNetCore.Identity;

namespace ApotheGSF.Models
{
    public class AppRol : IdentityRole<int>
    {
        public int GetRolId()
        {
            return this.GetRolId();
        }

        public virtual ICollection<AppUsuarioRol> UsuariosRoles { get; set; }
    }
}
