using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class AlterarSenhaModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Digite a senha atual.")]
        public required string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Digite a nova senha.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$", ErrorMessage = "A senha deve ter no minimo 8 caracteres e no maximo 20, incluindo pelo menos uma letra maiuscula, uma minuscula, um numero e um caractere especial.")]
        public required string NovaSenha { get; set; }

        [Required(ErrorMessage = "Digite a nova senha novamente.")]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não são iguais.")]
        public required string NovaSenhaConfirmacao { get; set; }

    }
}
