using System.ComponentModel.DataAnnotations;

namespace ManagementProduct.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username wajib diisi.")]
        [MinLength(3, ErrorMessage = "Username minimal 3 karakter.")]
        public string Username { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password wajib diisi.")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation wajib diisi.")]
        [Compare("Password", ErrorMessage = "Password confirmation tidak cocok.")]
        public string PasswordConfirmation { get; set; } = string.Empty;
    }
}
