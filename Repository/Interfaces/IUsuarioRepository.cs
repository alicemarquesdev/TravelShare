using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> BuscarUsuarioPorIdAsync(string id);

        Task<List<UsuarioModel>> BuscarVariosUsuariosPorIdAsync(List<string> ids);

        Task<List<UsuarioModel>> BuscarSugestoesParaSeguir(string id);

        Task<UsuarioModel> BuscarUsuarioPorEmailOuUsername(string emailOuUsername);

        Task AddUsuarioAsync(UsuarioModel usuario);

        Task AtualizarUsuarioAsync(UsuarioSemSenhaModel usuario);

        Task<bool> DeletarUsuarioAsync(string id);
    }
}