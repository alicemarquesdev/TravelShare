using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using TravelShare.Enums;

namespace TravelShare.Models
{
    public class NotificacaoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("usuarioDestino")]
        public string UsuarioDestino { get; set; } // Usuário que receberá a notificação

        [BsonElement("usuarioOrigem")]
        public string UsuarioOrigem { get; set; } // Usuário que gerou a notificação

        [BsonElement("notificacao")]
        public NotificacaoEnum Notificacao { get; set; } // Mensagem descritiva da notificação

        [BsonElement("dataCriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [BsonElement("ComentarioId")]
        public string? ComentarioId { get; set; }

        [BsonElement("PostId")]
        public string? PostId { get; set; }

        [BsonIgnore]
        public UsuarioModel? UsuarioOrigemModel { get; set; }

        [BsonIgnore]
        public PostModel? Post { get; set; }
    }
}
