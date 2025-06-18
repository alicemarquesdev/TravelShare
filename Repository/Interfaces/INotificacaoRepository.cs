using TravelShare.Enums;
using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    // Interface que define os métodos para manipulação de notificações
    public interface INotificacaoRepository
    {
        // Método assíncrono que busca uma notificação específica por seu identificador
        // 'id' é o identificador único da notificação
        // Retorna a notificação correspondente ao id fornecido, ou null se não encontrado
        Task<NotificacaoModel?> BuscarNotificacaoPorIdAsync(string id);

        // Busca a notificacao por comentarioId
        Task<NotificacaoModel?> BuscarNotificacaoPorComentarioId(string comentarioId);

        // Método assíncrono que busca todas as notificações de um usuário específico
        // 'usuarioId' é o identificador do usuário cujas notificações serão buscadas
        // Retorna uma lista de notificações associadas ao usuário
        Task<List<NotificacaoModel>> BuscarTodasAsNotificacoesDoUsuarioAsync(string usuarioId);

        // Método assíncrono que adiciona uma nova notificação
        // 'notificacao' contém os dados da nova notificação a ser adicionada
        // Retorna uma tarefa que pode ser aguardada, sem retorno explícito
        Task AddNotificacaoAsync(NotificacaoModel notificacao);

        // Método assíncrono que remove uma notificação com base no destino, origem, tipo de notificação e postId (se aplicável)
        // 'usuarioDestino' é o destinatário da notificação
        // 'usuarioOrigem' é o usuário que gerou a notificação
        // 'notificacao' é o tipo de notificação a ser removida (definido pelo enum NotificacaoEnum)
        // 'postId' é o identificador do post relacionado à notificação, se aplicável
        // Retorna 'true' se a notificação foi removida com sucesso, caso contrário, retorna 'false'
        Task<bool> RemoverNotificacaoAsync(string usuarioDestinoId, string usuarioOrigemId, NotificacaoEnum notificacao, string? postId);

        // Método assíncrono que remove uma notificação associada a um comentário
        // 'comentarioId' é o identificador do comentário associado à notificação
        // Retorna 'true' se a notificação foi removida com sucesso, caso contrário, retorna 'false'
        Task<bool> RemoverNotificacaoPorComentarioAsync(string comentarioId);
    }
}
