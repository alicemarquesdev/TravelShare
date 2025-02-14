using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class ComentarioModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Comentario")]
        [Required(ErrorMessage = "Comentário vazio.")]
        [StringLength(100, ErrorMessage = "O comentário deve ter no máximo 100 caracteres.")]
        public string Comentario { get; set; }

        [BsonElement("DataCriacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("PostId")]
        public string PostId { get; set; }

        [BsonElement("UsuarioId")]
        public string UsuarioId { get; set; }

        [BsonElement("UsuarioUsername")]
        public string UsuarioUsername { get; set; }
    }
}