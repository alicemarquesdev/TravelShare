using TravelShare.Models;

namespace TravelShare.Repository.Interfaces
{
    // Interface que define os métodos para alteração e redefinição de senha
    public interface IAlteracaoSenhaRepository
    {
        // Método assíncrono que altera a senha de um usuário com base nas informações fornecidas
        // O parâmetro 'alterarSenha' contém os dados necessários para a alteração, como a senha atual e a nova senha
        Task<bool> AlterarSenhaAsync(AlterarSenhaModel alterarSenha);

        // Método assíncrono que redefine a senha de um usuário, geralmente utilizado quando a senha é esquecida
        // 'id' é o identificador do usuário, e 'novaSenha' é a nova senha a ser atribuída
        Task<bool> RedefinirSenha(string id, string novaSenha);
    }
}
