using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using TravelShare.Helper;

namespace TravelShare.Models
{
    // Model necessária para criar usuário
    public class UsuarioModel
    {
        // Identificador único do usuário no MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Nome do usuário
        [BsonElement("Nome")]
        [Required(ErrorMessage = "Digite seu nome.")]
        [StringLength(30, ErrorMessage = "O nome deve ter no máximo 30 caracteres.")]
        [RegularExpression(@"^(?!\s)(?!.*\s\s)([A-Za-zÀ-ÿ]+(?:\s[A-Za-zÀ-ÿ]+)*)(?<!\s)$",
            ErrorMessage = "O nome não pode ter espaços consecutivos, nem no começo ou no final, e deve conter apenas letras e espaços.")]
        public required string Nome { get; set; }

        // Nome de usuário único na plataforma
        [BsonElement("Username")]
        [Required(ErrorMessage = "Digite o username.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O Username deve ter no máximo 20 caracteres e no mínimo 3.")]
        [RegularExpression(@"^(?![_.])([a-z]+(?:[._][a-z]+)*)(?<![_.])$",
            ErrorMessage = "O Username deve conter apenas letras minúsculas, pontos e sublinhados. Não pode ter dois pontos ou dois underlines consecutivos, nem ponto ou underline no início ou no final.")]
        public required string Username { get; set; }

        // Endereço de e-mail do usuário
        [BsonElement("Email")]
        [Required(ErrorMessage = "Digite o email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
        public required string Email { get; set; }

        // Senha do usuário
        [BsonElement("Senha")]
        [Required(ErrorMessage = "Digite a senha.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "A senha deve ter no mínimo 8 caracteres e no máximo 20, incluindo pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
        public required string Senha { get; set; }

        // Confirmação da senha (não armazenada no banco)
        [BsonIgnore]
        [Required(ErrorMessage = "Digite a senha novamente.")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não são iguais.")]
        public string? SenhaConfirmacao { get; set; }

        // Data de nascimento do usuário
        [BsonElement("DataNascimento")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required(ErrorMessage = "Digite a data de nascimento.")]
        public required DateTime DataNascimento { get; set; }

        // Cidade onde o usuário nasceu
        [BsonElement("CidadeDeNascimento")]
        [Required(ErrorMessage = "Digite a cidade onde nasceu.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9.,!?()'""\s-]+$", ErrorMessage = "Contém caracteres inválidos")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public required string CidadeDeNascimento { get; set; }

        // Caminho da imagem de perfil do usuário
        [BsonElement("ImagemPerfil")]
        public string ImagemPerfil { get; set; } = "~/image/profile-img.jpg";

        // Biografia do usuário
        [BsonElement("Bio")]
        [StringLength(300, ErrorMessage = "A bio deve ter no máximo 300 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9.,!?()'""\s-]+$", ErrorMessage = "Contém caracteres inválidos")]
        public string? Bio { get; set; }

        // Data de registro do usuário na plataforma
        [BsonElement("DataRegistro")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

        // Lista de usuários que este usuário está seguindo
        public List<string> Seguindo { get; set; } = new List<string>();

        // Lista de seguidores do usuário
        public List<string> Seguidores { get; set; } = new List<string>();

        // Lista de cidades visitadas pelo usuário
        public List<string> CidadesVisitadas { get; set; } = new List<string>();

        // Verifica se a senha informada corresponde à senha armazenada (hash)
        public bool SenhaValida(string senha)
        {
            return Senha == senha.GerarHash();
        }

        // Define a senha do usuário como hash
        public void SetSenhaHash()
        {
            Senha = Senha.GerarHash();
        }

        // Define uma nova senha para o usuário
        public void SetNovaSenha(string novaSenha)
        {
            Senha = novaSenha.GerarHash();
        }

        // Gera uma nova senha aleatória para o usuário
        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            return novaSenha;
        }
    }
}
