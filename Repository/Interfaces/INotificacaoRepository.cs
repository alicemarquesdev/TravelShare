using TravelShare.Enums;
using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface INotificacaoRepository
    {
        Task<List<NotificacaoModel>> BuscarTodasAsNotificacoesDoUsuarioAsync(string usuarioId);

        Task<NotificacaoModel> BuscarNotificacaoPorIdAsync(string id);

        Task AddNotificacaoAsync(NotificacaoModel notificacao);

        Task<bool> RemoverNotificacaoAsync(string usuarioDestino, string usuarioOrigem, NotificacaoEnum notificacao, string? postId);

        Task<bool> RemoverNotificacaoPorComentarioAsync(string comentarioId);
    }
}
