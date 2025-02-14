using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class PostModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Legenda")]
        [StringLength(250, ErrorMessage = "A legenda deve ter no máximo 250 caracteres.")]
        public string? Legenda { get; set; }

        [BsonElement("Localizacao")]
        public string? Localizacao { get; set; }

        [BsonElement("ImagemPost")]
        [Required(ErrorMessage = "Insira uma imagem.")]
        public required List<string> ImagemPost { get; set; }

        [BsonElement("DataCriacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("DataAtualizacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DataAtualizacao { get; set; }

        [BsonElement("UsuarioId")]
        public required string UsuarioId { get; set; }

        public List<string> Curtidas { get; set; } = new List<string>();

        [BsonIgnore]
        public UsuarioModel? Usuario { get; set; }

        [BsonIgnore]
        public List<ComentarioModel> Comentarios { get; set; } = new List<ComentarioModel>();
    }
}