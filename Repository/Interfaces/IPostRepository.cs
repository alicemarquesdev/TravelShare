using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IPostRepository
    {
        Task<List<PostsModel>> BuscarTodosOsPostsSeguindoAsync(string id);

        Task<List<PostsModel>> BuscarTodosOsPostsNãoSeguindoAsync(string id);

        Task<List<PostsModel>> BuscarTodosOsPostsDoUsuarioAsync(string usuarioId);

        Task<PostsModel> BuscarPostPorIdAsync(string postId);

        Task AddPostAsync(PostsModel post);

        Task AtualizarPostAsync(PostsModel post);

        Task<bool> DeletarPostAsync(string id);
    }
}