using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        // Método assíncrono que busca um usuário pelo ID
        // 'id' é o identificador do usuário a ser buscado
        // Retorna o modelo do usuário correspondente ao ID
        Task<UsuarioModel?> BuscarUsuarioPorIdAsync(string id);

        // Método assíncrono que busca um usuário pelo email ou username
        // 'emailOuUsername' é o email ou username do usuário a ser buscado
        // Retorna o modelo do usuário correspondente, ou null se não encontrado
        Task<UsuarioModel?> BuscarUsuarioPorEmailOuUsernameAsync(string emailOuUsername);

        // Barra de Pesquisa
        // Método assíncrono que realiza uma busca de usuários a partir de um termo de pesquisa
        // 'termo' é o termo utilizado para a pesquisa, como nome ou parte do nome
        // Retorna uma lista de usuários que correspondem ao termo
        Task<List<UsuarioModel>> PesquisarUsuariosAsync(string termo);

        // Método assíncrono que busca múltiplos usuários a partir de uma lista de IDs
        // 'ids' é uma lista de identificadores dos usuários a serem buscados
        // Retorna uma lista de modelos de usuários correspondentes aos IDs
        Task<List<UsuarioModel>> BuscarVariosUsuariosPorIdAsync(List<string> ids);

        // Método assíncrono que busca todos os usuários
        // Retorna uma lista de todos os usuários no sistema
        Task<List<UsuarioModel>> BuscarTodosOsUsuariosAsync();

        // Método assíncrono que busca sugestões de usuários para seguir com base no ID do usuário
        // 'id' é o identificador do usuário que vai receber sugestões de usuários para seguir
        // Retorna uma lista de sugestões de usuários para seguir
        Task<List<UsuarioModel>> BuscarSugestoesParaSeguirAsync(string id);

        // Método assíncrono que adiciona um novo usuário
        // 'usuario' é o modelo do usuário a ser adicionado
        // Não retorna valor, mas realiza a operação de adicionar o usuário
        Task AddUsuarioAsync(UsuarioModel usuario);

        // Método assíncrono que atualiza as informações de um usuário
        // 'usuario' é o modelo do usuário com as novas informações a serem atualizadas
        // Retorna 'true' se a atualização foi bem-sucedida, caso contrário, retorna 'false'
        Task<bool> AtualizarUsuarioAsync(UsuarioSemSenhaModel usuario);

        // Método assíncrono que deleta um usuário com base no ID
        // 'id' é o identificador do usuário a ser deletado
        // Retorna 'true' se o usuário foi deletado com sucesso, caso contrário, retorna 'false'
        Task<bool> DeletarUsuarioAsync(string id);
    }
}
