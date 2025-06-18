using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Model usada para representar os dados de login do usuário
    public class LoginModel
    {
        // Campo obrigatório para email ou nome de usuário no login
        [Required(ErrorMessage = "Digite o email ou username.")]
        [RegularExpression(@"^(?:[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}|[a-zA-Z0-9._-]{3,20})$", ErrorMessage = "Digite um email válido ou um username de no máximo 20 caracteres.")]
        public required string EmailOuUsername { get; set; }

        // Campo obrigatório para senha no login
        [Required(ErrorMessage = "Digite a senha.")]
        [StringLength(20, MinimumLength = 0, ErrorMessage = "A senha tem no máximo 20 caracteres.")]
        public required string? Senha { get; set; }
    }
}
