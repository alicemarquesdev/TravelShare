using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> BuscarUsuarioPorIdAsync(string id);

        Task<List<UsuarioModel>> BuscarVariosUsuariosPorIdAsync(List<string> ids);
        Task<List<UsuarioModel>> BuscarTodosOsUsuariosAsync();

        Task<List<UsuarioModel>> BuscarSugestoesParaSeguirAsync(string id);

        Task<UsuarioModel> BuscarUsuarioPorEmailOuUsernameAsync(string emailOuUsername);

        // Barra de Pesquisa.
        Task<List<UsuarioModel>> PesquisarUsuariosAsync(string termo);

        Task AddUsuarioAsync(UsuarioModel usuario);

        Task<bool> AtualizarUsuarioAsync(UsuarioSemSenhaModel usuario);

        Task<bool> DeletarUsuarioAsync(string id);


        // Senha

        Task<bool> RedefinirSenha(string id, string novaSenha);
    }
}