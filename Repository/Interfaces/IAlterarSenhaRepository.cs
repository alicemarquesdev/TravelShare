using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    public interface IAlterarSenhaRepository
    {
        Task<bool> AlterarSenhaAsync(AlterarSenhaModel alterarSenha);

    }
}
