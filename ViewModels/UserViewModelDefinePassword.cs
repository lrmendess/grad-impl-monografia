using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SCAP.ViewModels
{
    public class UserDefinePasswordViewModel
    {
        [Key]
        [HiddenInput]
        public string Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayName("Nova senha")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Compare("Password", ErrorMessage = "A nova senha e a senha de confirmação não correspondem.")]
        [DisplayName("Repetir nova senha")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [HiddenInput]
        public string Token { get; set; }
    }
}
