using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Model para alterar a senha do usuário.
    public class AlterarSenhaModel
    {
        // ID do usuário que está alterando a senha
        public required string Id { get; set; }

        // Senha atual do usuário (campo obrigatório)
        [Required(ErrorMessage = "Digite a senha atual.")]
        public required string SenhaAtual { get; set; }

        // Nova senha do usuário (campo obrigatório)
        // Deve conter entre 8 e 20 caracteres, incluindo pelo menos:
        // - Uma letra maiúscula
        // - Uma letra minúscula
        // - Um número
        // - Um caractere especial
        [Required(ErrorMessage = "Digite a nova senha.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "A senha deve ter no mínimo 8 caracteres e no máximo 20, incluindo pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
        public required string NovaSenha { get; set; }

        // Confirmação da nova senha (campo obrigatório)
        // Deve ser idêntica à nova senha
        [Required(ErrorMessage = "Digite a nova senha novamente.")]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não são iguais.")]
        public required string NovaSenhaConfirmacao { get; set; }
    }
}
