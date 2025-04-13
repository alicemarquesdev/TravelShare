using Newtonsoft.Json;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;

namespace TravelShare.Helper
{
    // A classe 'Sessao' implementa a interface ISessao e gerencia a sessão do usuário na aplicação.
    // Ela utiliza o IHttpContextAccessor para acessar e manipular as sessões armazenadas no HTTP.
    public class Sessao : ISessao
    {
        // O IHttpContextAccessor é injetado para permitir o acesso ao contexto HTTP atual.
        private readonly IHttpContextAccessor _httpContext;

        // Construtor que recebe o IHttpContextAccessor, permitindo o acesso à sessão do usuário.
        public Sessao(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        // Método que busca a sessão do usuário logado.
        // Retorna o usuário da sessão ou null se não houver sessão.
        public UsuarioModel BuscarSessaoDoUsuario()
        {
            // Recupera a sessão do usuário do contexto HTTP
            string sessaoDoUsuario = _httpContext.HttpContext?.Session.GetString("SessaoDoUsuarioLogado") ?? "";

            // Se a sessão estiver vazia ou nula, retorna null
            if (string.IsNullOrEmpty(sessaoDoUsuario))
                return null;

            // Se a sessão contiver dados, deserializa e retorna o objeto UsuarioModel
            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoDoUsuario);
        }

        // Método para criar uma nova sessão para o usuário.
        // O objeto usuário é convertido para uma string JSON e armazenado na sessão.
        public void CriarSessaoDoUsuario(UsuarioModel usuario)
        {
            // Serializa o objeto usuário para uma string JSON
            string valor = JsonConvert.SerializeObject(usuario);

            // Armazena a string JSON da sessão do usuário no contexto HTTP
            _httpContext.HttpContext.Session.SetString("SessaoDoUsuarioLogado", valor);
        }

        // Método para remover a sessão do usuário.
        // Esse método limpa a sessão armazenada no contexto HTTP.
        public void RemoverSessaoDoUsuario()
        {
            // Remove a chave "SessaoDoUsuarioLogado" da sessão
            _httpContext.HttpContext.Session.Remove("SessaoDoUsuarioLogado");
        }
    }
}
