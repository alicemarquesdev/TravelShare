using TravelShare.Models;

namespace TravelShare.Helper.Interfaces
{ 
    // Declara os métodos necessários para gerenciar a sessão do usuário.
    public interface ISessao
    {
        // Método para buscar a sessão do usuário atual.
        // Retorna um objeto de usuário que representa a sessão ativa.
        UsuarioModel BuscarSessaoDoUsuario();

        // Método para criar a sessão do usuário.
        // Recebe um objeto de usuário para armazenar a sessão.
        void CriarSessaoDoUsuario(UsuarioModel usuario);

        // Método para remover a sessão do usuário.
        // Limpa a sessão do usuário atual.
        void RemoverSessaoDoUsuario();
    }
}
