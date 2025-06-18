using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    // Interface que define os métodos para manipulação de posts
    public interface IPostRepository
    {
        // Método assíncrono que busca um post específico por seu identificador
        // 'postId' é o identificador único do post a ser buscado
        // Retorna o post correspondente ao id fornecido, ou null se não encontrado
        Task<PostModel?> BuscarPostPorIdAsync(string postId);

        // Método assíncrono que busca todos os posts de usuários que o usuário específico está seguindo
        // 'id' é o identificador do usuário que está seguindo outros usuários
        // Retorna uma lista de posts dos usuários que ele está seguindo
        Task<List<PostModel>> BuscarTodosOsPostsSeguindoAsync(string id);

        // Método assíncrono que busca todos os posts de usuários que o usuário específico NÃO está seguindo
        // 'id' é o identificador do usuário que está verificando os posts de outros
        // Retorna uma lista de posts dos usuários que o usuário não está seguindo
        Task<List<PostModel>> BuscarTodosOsPostsNãoSeguindoAsync(string id);

        // Método assíncrono que busca todos os posts de um usuário específico
        // 'usuarioId' é o identificador do usuário cujos posts serão buscados
        // Retorna uma lista de posts do usuário específico
        Task<List<PostModel>> BuscarTodosOsPostsDoUsuarioAsync(string usuarioId);

        // Método assíncrono que adiciona um novo post
        // 'post' contém os dados do novo post a ser adicionado
        // Retorna uma tarefa que pode ser aguardada, sem retorno explícito
        Task AddPostAsync(PostModel post);

        // Método assíncrono que atualiza um post existente
        // 'post' contém os dados atualizados do post
        // Retorna uma tarefa que pode ser aguardada, sem retorno explícito
        Task AtualizarPostAsync(PostModel post);

        // Método assíncrono que deleta um post
        // 'id' é o identificador único do post a ser deletado
        // Retorna 'true' se o post foi deletado com sucesso, caso contrário, retorna 'false'
        Task<bool> DeletarPostAsync(string id);
    }
}
