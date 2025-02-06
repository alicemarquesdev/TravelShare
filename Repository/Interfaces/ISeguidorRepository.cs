using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface ISeguidorRepository
    {
        // CRUD Seguidor
        Task<List<UsuarioModel>> BuscarTodosSeguidoresAsync(string usuarioId);

        Task<UsuarioModel> BuscarSeguidorAsync(string usuarioId, string seguidorId);

        Task<bool> RemoverSeguidorAsync(string usuarioId, string seguidorId);

        // CRUD Seguindo
        Task<List<UsuarioModel>> BuscarTodosSeguindoAsync(string usuarioId);

        Task<UsuarioModel> BuscarSeguindoAsync(string usuarioId, string seguindoId);

        Task SeguirUsuarioAsync(string usuarioId, string seguindoId);

        Task DeseguirUsuarioAsync(string usuarioId, string seguindoId);
    }
}