namespace TravelShare.Repository.Interfaces
{
    public interface ICidadesVisitadasRepository
    {
        Task<bool> VerificarCidadeVisitadaPeloUsuarioAsync(string usuarioId, string cidade);

        Task<bool> AddCidadeAsync(string usuarioId, string cidade);

        Task<bool> RemoveCidadeAsync(string usuarioId, string cidade);
    }
}