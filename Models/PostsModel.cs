using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class PostsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Legenda")]
        public string? Legenda { get; set; }

        [BsonElement("Localizacao")]
        public string? Localizacao { get; set; }

        [BsonElement("FotoPost")]
        [Required(ErrorMessage = "Insira uma imagem.")]
        public List<string> FotoPost { get; set; } = new List<string>();

        [BsonElement("DataCriacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("DataAtualizacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DataAtualizacao { get; set; }

        [BsonElement("UsuarioId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioId { get; set; }

        public List<string> Curtidas { get; set; } = new List<string>();

        [BsonIgnore]
        public UsuarioModel? Usuario { get; set; }

        [BsonIgnore]
        public List<ComentarioModel?> Comentarios { get; set; } = new List<ComentarioModel?>();
    }
}