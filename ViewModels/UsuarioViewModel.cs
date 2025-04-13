using TravelShare.Models;

namespace TravelShare.ViewModels
{
    // ViewModel utilizado para exibir dados relacionados ao perfil do usuário
    public class UsuarioViewModel
    {
        // Para armazenar o chave da API
        public string? GoogleAPIKey { get; set; } 

        // Indica se o perfil exibido é o do próprio usuário logado (true) ou de outro usuário (false)
        public bool PerfilDoUsuarioLogado { get; set; }

        // ID do usuário logado, usado para identificar de quem é o perfil
        public string? UsuarioLogadoId { get; set; }

        // Informações do usuário cujo perfil está sendo exibido (pode ser o próprio usuário ou outro)
        public UsuarioModel? UsuarioPerfil { get; set; }

        // Lista de posts associados ao usuário cujo perfil está sendo exibido
        public List<PostModel> Posts { get; set; } = new List<PostModel>();

        // Lista de usuários que seguem o usuário cujo perfil está sendo exibido
        public List<UsuarioModel> Seguidores { get; set; } = new List<UsuarioModel>();

        // Lista de usuários que o usuário cujo perfil está sendo exibido está seguindo
        public List<UsuarioModel> Seguindo { get; set; } = new List<UsuarioModel>();
    }
}
