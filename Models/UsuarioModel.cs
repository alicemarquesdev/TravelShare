using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using TravelShare.Helper;

namespace TravelShare.Models

{
    public class UsuarioModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Nome")]
        [Required(ErrorMessage = "Digite seu nome.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        [RegularExpression(@"^(?!\s)(?!.*\s\s)([A-Za-zÀ-ÿ]+(?:\s[A-Za-zÀ-ÿ]+)*)(?<!\s)$", ErrorMessage = "O nome não pode ter espaços consecutivos, nem no começo ou no final, e deve conter apenas letras e espaços.")]
        public required string Nome { get; set; }

        [BsonElement("Username")]
        [Required(ErrorMessage = "Digite o username.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O Username deve ter no máximo 20 caracteres e no minimo 3.")]
        [RegularExpression(@"^(?![_.])([a-z]+(?:[._][a-z]+)*)(?<![_.])$", ErrorMessage = "O Username deve conter apenas letras minúsculas, pontos e sublinhados. Não pode ter dois pontos ou dois underlines consecutivos, nem ponto ou underline no início ou no final.")]
        public required string Username { get; set; }

        [BsonElement("Email")]
        [Required(ErrorMessage = "Digite o email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
        [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "O email deve ser válido e não pode conter espaços.")]
        public required string Email { get; set; }

        [BsonElement("Senha")]
        [Required(ErrorMessage = "Digite a senha.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$", ErrorMessage = "A senha deve ter no minimo 8 caracteres e no maximo 20, incluindo pelo menos uma letra maiuscula, uma minuscula, um numero e um caractere especial.")]
        public required string Senha { get; set; }

        [BsonIgnore]
        [Required(ErrorMessage = "Digite a senha novamente.")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não são iguais.")]
        public string? SenhaConfirmacao { get; set; }

        [BsonElement("DataNascimento")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required(ErrorMessage = "Digite a data de nascimento.")]
        [Range(typeof(DateTime), "01/01/1920", "01/01/2010", ErrorMessage = "Data de nascimento inválida.")]
        public required DateTime DataNascimento { get; set; }

        [BsonElement("CidadeDeNascimento ")]
        [Required(ErrorMessage = "Digite a cidade onde nasceu.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public required string CidadeDeNascimento { get; set; }

        [BsonElement("ImagemPerfil")]
        public string ImagemPerfil { get; set; } = "~/image/profile-img.jpg";

        [BsonElement("Bio")]
        [StringLength(30, ErrorMessage = "A bio deve ter no máximo 300 caracteres.")]
        public string? Bio { get; set; }

        [BsonElement("DataRegistro")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

        public List<string> Seguindo { get; set; } = new List<string>();

        public List<string> Seguidores { get; set; } = new List<string>();

        public List<string> CidadesVisitadas { get; set; } = new List<string>();

        public bool SenhaValida(string senha)
        {
            return Senha == senha.GerarHash();
        }

        public void SetSenhaHash()
        {
            Senha = Senha.GerarHash();
        }

        public void SetNovaSenha(string novaSenha)
        {
            Senha = novaSenha.GerarHash();
        }

        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            return novaSenha;
        }

    }
}