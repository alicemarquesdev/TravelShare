using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IPostRepository
    {
        Task<List<PostModel>> BuscarTodosOsPostsSeguindoAsync(string id);

        Task<List<PostModel>> BuscarTodosOsPostsNãoSeguindoAsync(string id);

        Task<List<PostModel>> BuscarTodosOsPostsDoUsuarioAsync(string usuarioId);

        Task<PostModel> BuscarPostPorIdAsync(string postId);

        Task AddPostAsync(PostModel post);

        Task AtualizarPostAsync(PostModel post);

        Task<bool> DeletarPostAsync(string id);
    }
}