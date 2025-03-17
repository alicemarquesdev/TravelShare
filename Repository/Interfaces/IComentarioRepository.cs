using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    // Interface que define os métodos para manipulação de comentários
    public interface IComentarioRepository
    {
        // Método assíncrono que busca um comentário pelo seu identificador
        // 'id' é o identificador único do comentário
        // Retorna o comentário correspondente ao id fornecido, ou null se não encontrado
        Task<ComentarioModel?> BuscarComentarioPorIdAsync(string id);

        // Método assíncrono que busca todos os comentários de um post específico
        // 'postId' é o identificador do post para o qual os comentários devem ser buscados
        // Retorna uma lista de comentários associados ao post
        Task<List<ComentarioModel>> BuscarTodosOsComentariosDoPostAsync(string postId);

        // Método assíncrono que adiciona um novo comentário
        // 'comentario' contém os dados do novo comentário a ser adicionado
        // Retorna uma tarefa que pode ser aguardada, sem retorno explícito
        Task AddComentarioAsync(ComentarioModel comentario);

        // Método assíncrono que deleta um comentário
        // 'id' é o identificador único do comentário a ser deletado
        // Retorna 'true' se o comentário foi deletado com sucesso, ou 'false' caso contrário
        Task<bool> DeletarComentarioAsync(string id);
    }
}
