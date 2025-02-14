using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class UsuarioSemSenhaModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Digite o nome.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        [RegularExpression(@"^(?!\s)(?!.*\s\s)([A-Za-zÀ-ÿ]+(?:\s[A-Za-zÀ-ÿ]+)*)(?<!\s)$", ErrorMessage = "O nome não pode ter espaços consecutivos, nem no começo ou no final, e deve conter apenas letras e espaços.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Digite o username.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O Username deve ter no máximo 20 caracteres e no minímo 3.")]
        [RegularExpression(@"^(?![_.])([a-z]+(?:[._][a-z]+)*)(?<![_.])$", ErrorMessage = "O Username deve conter apenas letras minúsculas, pontos e sublinhados. Não pode ter dois pontos ou dois underlines consecutivos, nem ponto ou underline no início ou no final.")]
        public string? Username { get; set; }

        [BsonElement("Email")]
        [Required(ErrorMessage = "Digite o email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
        [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "O email deve ser válido e não pode conter espaços.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Digite a data de nascimento.")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2025", ErrorMessage = "Data de nascimento inválida.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Digite a cidade onde nasceu.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public required string CidadeDeNascimento { get; set; }

        public string ImagemPerfil { get; set; } = "~/image/profile-img.jpg";

        [StringLength(300, ErrorMessage = "A Bio deve ter no máximo 300 caracteres.")]
        public string? Bio { get; set; }
    }
}