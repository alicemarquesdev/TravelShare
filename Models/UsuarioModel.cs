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
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [BsonElement("Username")]
        [Required(ErrorMessage = "Username é obrigatório.")]
        [StringLength(20, ErrorMessage = "O Username deve ter no máximo 20 caracteres.")]
        public string Username { get; set; }

        [BsonElement("Email")]
        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150, ErrorMessage = "O Email deve ter no máximo 150 caracteres.")]
        public string Email { get; set; }

        [BsonElement("Senha")]
        [Required(ErrorMessage = "Senha é obrigatória.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; }

        [BsonIgnore]
        [Required(ErrorMessage = "A senha é obrigatória")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não correspondem.")]
        public string SenhaConfirmacao { get; set; }

        [BsonElement("DataNascimento")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2025", ErrorMessage = "Data de nascimento inválida.")]
        public DateTime DataNascimento { get; set; }

        [BsonElement("PaisNascimento")]
        [Required(ErrorMessage = "Lugar de Nascimento é obrigátorio.")]
        [StringLength(200, ErrorMessage = "O país de nascimento deve ter no máximo 200 caracteres.")]
        public string PaisNascimento { get; set; }

        [BsonElement("FotoPerfil")]
        public string? FotoPerfil { get; set; } = "~/assets/img/profile-img.jpg";

        [BsonElement("Bio")]
        [StringLength(255, ErrorMessage = "A Bio deve ter no máximo 255 caracteres.")]
        public string? Bio { get; set; } = "";

        [BsonElement("LocalizacaoAtual")]
        public string? LocalizacaoAtual { get; set; } = "";

        [BsonElement("DataRegistro")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DataRegistro { get; set; } = DateTime.UtcNow;

        public List<string> Seguindo { get; set; } = new List<string>();

        public List<string> Seguidores { get; set; } = new List<string>();

        public List<string> PaisesVisitados { get; set; } = new List<string>();

        public bool SenhaValida(string senha)
        {
            return Senha == senha.GerarHash();
        }

        public void SetSenhaHash()
        {
            Senha = Senha.GerarHash();
        }
    }
}