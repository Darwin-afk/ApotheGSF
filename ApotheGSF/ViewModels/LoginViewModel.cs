using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario: ")]
        public string Usuario { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña: ")]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        [Display(Name = "Recordarme: ")]
        public bool? RememberMe { get; set; }
    }
}
