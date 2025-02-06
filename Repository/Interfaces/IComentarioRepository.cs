using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IComentarioRepository
    {
        Task<ComentarioModel> BuscarComentarioPorIdAsync(string id);

        Task<List<ComentarioModel>> BuscarTodosOsComentariosDoPostAsync(string postId);

        Task AddComentario(ComentarioModel comentario);

        Task<bool> DeletarComentario(string id);
    }
}