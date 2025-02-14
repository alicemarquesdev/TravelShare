using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IComentarioRepository
    {
        Task<ComentarioModel> BuscarComentarioPorIdAsync(string id);

        Task<List<ComentarioModel>> BuscarTodosOsComentariosDoPostAsync(string postId);

        Task AddComentarioAsync(ComentarioModel comentario);

        Task<bool> DeletarComentarioAsync(string id);
    }
}