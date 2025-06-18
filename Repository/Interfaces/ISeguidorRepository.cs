using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{   
    // Interface que define os métodos para manipulação de Seguidores e Seguindo
    public interface ISeguidorRepository
    {
        // CRUD Seguidor

        // Método assíncrono que busca um seguidor específico de um usuário
        // 'usuarioId' é o identificador do usuário
        // 'seguidorId' é o identificador do seguidor a ser buscado
        // Retorna o usuário que segue o usuário especificado, ou null se não encontrado
        Task<UsuarioModel?> BuscarSeguidorAsync(string usuarioId, string seguidorId);

        // Método assíncrono que busca todos os seguidores de um usuário específico
        // 'usuarioId' é o identificador do usuário cujos seguidores serão buscados
        // Retorna uma lista de usuários que são seguidores do usuário identificado
        Task<List<UsuarioModel>> BuscarTodosSeguidoresAsync(string usuarioId);

        // Método assíncrono que remove um seguidor específico de um usuário
        // 'usuarioId' é o identificador do usuário
        // 'seguidorId' é o identificador do seguidor a ser removido
        // Retorna 'true' se o seguidor foi removido com sucesso, caso contrário, retorna 'false'
        Task<bool> RemoverSeguidorAsync(string usuarioId, string seguidorId);

        // CRUD Seguindo

        // Método assíncrono que busca um usuário específico que o usuário está seguindo
        // 'usuarioId' é o identificador do usuário
        // 'seguindoId' é o identificador do usuário que está sendo seguido
        // Retorna o usuário que está sendo seguido, ou null se não encontrado
        Task<UsuarioModel?> BuscarSeguindoAsync(string usuarioId, string seguindoId);

        // Método assíncrono que busca todos os usuários que o usuário especificado está seguindo
        // 'usuarioId' é o identificador do usuário cujos seguidos serão buscados
        // Retorna uma lista de usuários que o usuário está seguindo
        Task<List<UsuarioModel>> BuscarTodosSeguindoAsync(string usuarioId);

        // Método assíncrono que faz com que um usuário comece a seguir outro
        // 'usuarioId' é o identificador do usuário que irá seguir outro usuário
        // 'seguindoId' é o identificador do usuário que será seguido
        // Não retorna valor, mas executa a operação de seguir
        Task SeguirUsuarioAsync(string usuarioId, string seguindoId);

        // Método assíncrono que faz com que um usuário pare de seguir outro
        // 'usuarioId' é o identificador do usuário que irá deixar de seguir
        // 'seguindoId' é o identificador do usuário que será deixado de seguir
        // Não retorna valor, mas executa a operação de deixar de seguir
        Task DeseguirUsuarioAsync(string usuarioId, string seguindoId);
    }
}
