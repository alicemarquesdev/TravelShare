using TravelShare.Models;

namespace TravelShare.ViewModels
{
    // ViewModel que agrupa os dados necessários para a página inicial
    public class HomeViewModel
    {
        // Objeto que contém as informações do usuário logado
        public UsuarioModel UsuarioLogado { get; set; }

        // Lista de posts que serão exibidos na página
        public List<PostModel> Posts { get; set; } = new List<PostModel>();

        // Lista de usuários sugeridos para o usuário logado seguir
        public List<UsuarioModel> UsuariosSugestoes { get; set; } = new List<UsuarioModel>();
    }
}
