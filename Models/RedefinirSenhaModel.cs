using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Modelo usado para redefinir a senha do usuário, em caso de esquecimento.
    // O usuário deve fornecer o email associado à conta, que será verificado se realmente existe no banco de dados.
    // Se confirmado, o usuário receberá um link de redefinição por email com uma nova senha.
    public class RedefinirSenhaModel
    {
        // Email do usuário, necessário para enviar o link de redefinição de senha
        [Required(ErrorMessage = "Digite o seu email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public required string Email { get; set; }
    }
}
