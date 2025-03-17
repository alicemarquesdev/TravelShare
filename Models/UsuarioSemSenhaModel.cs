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
        [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "O email deve ser válido e não pode conter espaços.")]
        public required string Email { get; set; }

        // Propriedade para a data de nascimento do usuário, com validação de intervalo
        [Required(ErrorMessage = "Digite a data de nascimento.")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2025", ErrorMessage = "Data de nascimento inválida.")]
        public required DateTime DataNascimento { get; set; }

        // Propriedade para a cidade de nascimento, com validação de comprimento
        [Required(ErrorMessage = "Digite a cidade onde nasceu.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public required string CidadeDeNascimento { get; set; }

        // Propriedade para a imagem de perfil do usuário, com valor padrão
        public string ImagemPerfil { get; set; } = "~/image/profile-img.jpg";

        // Propriedade opcional para a bio do usuário, com validação de comprimento
        [StringLength(300, ErrorMessage = "A Bio deve ter no máximo 300 caracteres.")]
        public string? Bio { get; set; }

        // Lista das cidades visitadas pelo usuário, iniciada como uma lista vazia
        public List<string> CidadesVisitadas { get; set; } = new List<string>();
    }
}
