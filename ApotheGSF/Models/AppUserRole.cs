using Microsoft.AspNetCore.Identity;

namespace ApotheGSF.Models
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUsuario User { get; set; }
        public AppRole Rol { get; set; }
    }
}
