using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Model usada para armazenar os dados do comentário
    public class ComentarioModel
    {
        // Identificador único do comentário no MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Conteúdo do comentário (campo obrigatório, máximo 100 caracteres)
        [BsonElement("Comentario")]
        [Required(ErrorMessage = "Comentário vazio.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9.,!?()'""\s-]+$", ErrorMessage = "Contém caracteres inválidos")]
        [StringLength(100, ErrorMessage = "O comentário deve ter no máximo 100 caracteres.")]
        public required string Comentario { get; set; }

        // Data de criação do comentário, armazenada em UTC
        [BsonElement("DataCriacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // ID do post ao qual o comentário pertence
        [BsonElement("PostId")]
        public required string PostId { get; set; }

        // ID do usuário que fez o comentário
        [BsonElement("UsuarioId")]
        public required string UsuarioId { get; set; }

        // Nome de usuário do autor do comentário
        [BsonElement("UsuarioUsername")]
        public required string UsuarioUsername { get; set; }
    }
}
