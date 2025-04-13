using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Classe que representa um usuário sem a senha, para alterar os dados do usuario sem alterar a senha.
    public class UsuarioSemSenhaModel
    {
        // Identificador único do usuário
        public required string Id { get; set; }

        // Propriedade para o nome do usuário, com validações de tamanho, formato e espaços consecutivos
        [Required(ErrorMessage = "Digite o nome.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        [RegularExpression(@"^(?!\s)(?!.*\s\s)([A-Za-zÀ-ÿ]+(?:\s[A-Za-zÀ-ÿ]+)*)(?<!\s)$", ErrorMessage = "O nome não pode ter espaços consecutivos, nem no começo ou no final, e deve conter apenas letras e espaços.")]
        public required string Nome { get; set; }

        // Propriedade para o username do usuário, com restrições de comprimento e formato (apenas letras minúsculas, pontos e sublinhados)
        [Required(ErrorMessage = "Digite o username.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O Username deve ter no máximo 20 caracteres e no minímo 3.")]
        [RegularExpression(@"^(?![_.])([a-z]+(?:[._][a-z]+)*)(?<![_.])$", ErrorMessage = "O Username deve conter apenas letras minúsculas, pontos e sublinhados. Não pode ter dois pontos ou dois underlines consecutivos, nem ponto ou underline no início ou no final.")]
        public required string Username { get; set; }

        // Propriedade para o email, com validações de formato e comprimento
        [BsonElement("Email")]
        [Required(ErrorMessage = "Digite o email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
        public required string Email { get; set; }

        // Propriedade para a data de nascimento do usuário, com validação de intervalo
        [Required(ErrorMessage = "Digite a data de nascimento.")]
        public required DateTime DataNascimento { get; set; }

        // Propriedade para a cidade de nascimento, com validação de comprimento
        [Required(ErrorMessage = "Digite a cidade onde nasceu.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public required string CidadeDeNascimento { get; set; }

        // Propriedade para a imagem de perfil do usuário, com valor padrão
        public string ImagemPerfil { get; set; } = "~/image/profile-img.jpg";

        // Propriedade opcional para a bio do usuário, com validação de comprimento
        [StringLength(300, ErrorMessage = "A Bio deve ter no máximo 300 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9.,!?()'""\s-]+$", ErrorMessage = "Contém caracteres inválidos")]
        public string? Bio { get; set; }

        // Lista das cidades visitadas pelo usuário, iniciada como uma lista vazia
        public List<string> CidadesVisitadas { get; set; } = new List<string>();
    }
}
