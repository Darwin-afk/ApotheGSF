using System.ComponentModel.DataAnnotations;

namespace ApotheGSF.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        [Display(Name = "Recordarme")]
        public bool? RememberMe { get; set; }
    }
}
