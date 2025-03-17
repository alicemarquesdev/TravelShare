using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TravelShare.Enums;

namespace TravelShare.Models
{
    // Model usada para armazenar as notificações no sistema
    public class NotificacaoModel
    {
        // Identificador único da notificação no banco de dados (MongoDB)
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // ID do usuário que receberá a notificação
        [BsonElement("usuarioDestino")]
        public required string UsuarioDestinoId { get; set; }

        // ID do usuário que gerou a notificação
        [BsonElement("usuarioOrigem")]
        public required string UsuarioOrigemId { get; set; }

        // Tipo de notificação gerada (ex: novo seguidor, comentário, curtida)
        [BsonElement("notificacao")]
        public NotificacaoEnum Notificacao { get; set; }

        // Data e horário em que a notificação foi criada
        [BsonElement("dataCriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // ID do comentário associado à notificação (se aplicável)
        [BsonElement("ComentarioId")]
        public string? ComentarioId { get; set; }

        // ID do post associado à notificação (se aplicável)
        [BsonElement("PostId")]
        public string? PostId { get; set; }

        // Objeto do usuário que gerou a notificação (não armazenado no banco)
        [BsonIgnore]
        public UsuarioModel? UsuarioOrigemModel { get; set; }

        // Objeto do post associado à notificação (não armazenado no banco)
        [BsonIgnore]
        public PostModel? Post { get; set; }
    }
}
