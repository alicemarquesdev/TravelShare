namespace TravelShare.Repository.Interfaces
{
    public interface ICurtidaRepository
    {
        Task<bool> BuscarCurtidaExistenteAsync(string postId, string usuarioId);

        Task<bool> AddCurtidaAsync(string postId, string usuarioId);

        Task<bool> RemoveCurtidaAsync(string postId, string usuarioId);
    }
}