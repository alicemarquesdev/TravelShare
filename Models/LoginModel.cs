using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Model usada para representar os dados de login do usuário
    public class LoginModel
    {
        // Campo obrigatório para email ou nome de usuário no login
        [Required(ErrorMessage = "Digite o email ou username.")]
        public required string EmailOuUsername { get; set; }

        // Campo obrigatório para senha no login
        [Required(ErrorMessage = "Digite a senha.")]
        public required string? Senha { get; set; }
    }
}
