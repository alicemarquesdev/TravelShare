using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Classe responsável pela manipulação das curtidas nos posts dos usuários.
    // Métodos disponíveis:
    // - BuscarCurtidaExistenteAsync(string postId, string usuarioId): Verifica se o usuário já curtiu o post especificado.
    // - AddCurtidaAsync(string postId, string usuarioId): Adiciona uma curtida de um usuário a um post.
    // - RemoveCurtidaAsync(string postId, string usuarioId): Remove a curtida de um usuário de um post.

    public class CurtidaRepository : ICurtidaRepository
    {
        private readonly IMongoCollection<PostModel> _postsCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<CurtidaRepository> _logger;

        // Construtor que recebe o contexto do MongoDB e inicializa a coleção de posts.
        public CurtidaRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository, ILogger<CurtidaRepository> logger)
        {
            _postsCollection = mongoContext.GetCollection<PostModel>("Posts");
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // Verifica se um usuário já curtiu o post.
        public async Task<bool> BuscarCurtidaExistenteAsync(string postId, string usuarioId)
        {
            try
            {
                var curtidaExistente = await _postsCollection.Find(x => x.Id == postId && x.Curtidas.Contains(usuarioId)).FirstOrDefaultAsync();
                return curtidaExistente != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar curtida.");
                throw new Exception("Erro ao verificar curtida.");
            }
        }

        // Adiciona a curtida de um usuário a um post.
        public async Task<bool> AddCurtidaAsync(string postId, string usuarioId)
        {
            try
            {
                var post = await _postsCollection.FindAsync(x => x.Id == postId);
                if (post == null) 
                {
                    throw new Exception("Post não encontrado no banco de dados");
                }

                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);
                if (usuario == null) 
                {
                    throw new Exception("Usuario não encontrado no banco de dados");
                }

                var filter = Builders<PostModel>.Filter.Eq(x => x.Id, postId);
                var update = Builders<PostModel>.Update.AddToSet(x => x.Curtidas, usuarioId);

                // Tentamos atualizar a coleção de posts, adicionando a curtida ao post.
                var resultado = await _postsCollection.UpdateOneAsync(filter, update);
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar curtida.");
                throw new Exception("Erro ao adicionar curtida.");
            }
        }

        // Remove a curtida de um usuário de um post.
        public async Task<bool> RemoveCurtidaAsync(string postId, string usuarioId)
        {
            try
            {
                var curtidaExistente = await _postsCollection.Find(x => x.Id == postId && x.Curtidas.Contains(usuarioId)).FirstOrDefaultAsync(); 
                if (curtidaExistente == null)
                {
                    throw new Exception("Curtida não encontrado no banco de dados");
                }

                var filter = Builders<PostModel>.Filter.Eq(x => x.Id, postId);
                var update = Builders<PostModel>.Update.Pull(x => x.Curtidas, usuarioId);

                // Tentamos atualizar a coleção de posts, removendo a curtida do post.
                var resultado = await _postsCollection.UpdateOneAsync(filter, update);
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover curtida.");
                throw new Exception("Erro ao remover curtida.");
            }
        }
    }
}
