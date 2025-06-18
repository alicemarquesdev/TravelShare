using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    // Model usada para armazenar os dados de um post
    public class PostModel
    {
        // Identificador único do post no banco de dados (MongoDB)
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Texto da legenda do post, limitado a 250 caracteres
        [BsonElement("Legenda")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9.,!?()'""\s-]+$", ErrorMessage = "Contém caracteres inválidos")]
        [StringLength(250, ErrorMessage = "A legenda deve ter no máximo 250 caracteres.")]
        public string? Legenda { get; set; }

        // Localização associada ao post (caso o usuário informe)
        [BsonElement("Localizacao")]
        public string? Localizacao { get; set; }

        // Lista de URLs das imagens associadas ao post
        [BsonElement("ImagemPost")]
        [Required(ErrorMessage = "Insira uma imagem.")]
        public List<string> ImagemPost { get; set; } = new List<string>();

        // Data de criação do post, armazenada em UTC
        [BsonElement("DataCriacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Data da última atualização do post (caso tenha sido editado)
        [BsonElement("DataAtualizacao")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DataAtualizacao { get; set; }

        // ID do usuário que criou o post
        [BsonElement("UsuarioId")]
        public required string UsuarioId { get; set; }

        // Lista de IDs dos usuários que curtiram o post
        public List<string> Curtidas { get; set; } = new List<string>();

        // Referência ao usuário que criou o post (não armazenado no banco)
        [BsonIgnore]
        public UsuarioModel? Usuario { get; set; }

        // Lista de comentários do post (não armazenado diretamente no banco)
        [BsonIgnore]
        public List<ComentarioModel> Comentarios { get; set; } = new List<ComentarioModel>();
    }
}
