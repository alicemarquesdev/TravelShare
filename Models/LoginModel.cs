using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Digite o email ou username.")]
        public required string EmailOuUsername { get; set; }

        [Required(ErrorMessage = "Digite a senha.")]
        public required string? Senha { get; set; }
    }
}