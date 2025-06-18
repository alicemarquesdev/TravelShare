using MongoDB.Driver;
using System.Net.NetworkInformation;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Classe responsável pela manipulação dos comentários de um post de um usuário.
    // Métodos disponíveis:
    // - BuscarComentarioPorIdAsync(string id): Busca um comentário específico pelo seu ID.
    // - BuscarTodosOsComentariosDoPostAsync(string postId): Busca todos os comentários de um post.
    // - AddComentarioAsync(ComentarioModel comentario): Adiciona um novo comentário ao banco de dados.
    // - DeletarComentarioAsync(string id): Deleta um comentário pelo seu ID.
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<ComentarioRepository> _logger;

        public ComentarioRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository, ILogger<ComentarioRepository> logger)
        {
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentarios");
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // Busca um comentário pelo seu ID.
        public async Task<ComentarioModel?> BuscarComentarioPorIdAsync(string id)
        {
            try
            {
                return await _comentarioCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar comentário.");
                throw new Exception("Erro ao buscar comentário.");
            }
        }

        // Busca todos os comentários de um post específico.
        public async Task<List<ComentarioModel>> BuscarTodosOsComentariosDoPostAsync(string postId)
        {
            try
            {
                var comentarios = await _comentarioCollection.Find(x => x.PostId == postId).ToListAsync();

                if (comentarios == null)
                {
                    throw new NullReferenceException("Comentarios não encontrados no banco de dados, retornou null.");
                }

                // Se não houver comentários, retorna uma lista vazia.
                if (!comentarios.Any()) return new List<ComentarioModel>();

                // Extrai os IDs dos usuários que fizeram os comentários.
                var usuarioIds = comentarios.Select(c => c.UsuarioId).Distinct().ToList();

                // Busca todos os usuários de uma vez usando o repositório de usuários.
                var usuarios = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuarioIds);

                // Cria um dicionário de usuários para associar aos comentários.
                var usuarioDictionary = usuarios.ToDictionary(u => u.Id, u => u);

                // Associa os usuários aos comentários.
                foreach (var comentario in comentarios)
                {
                    if (usuarioDictionary.ContainsKey(comentario.UsuarioId))
                    {
                        var usuario = usuarioDictionary[comentario.UsuarioId];
                        comentario.UsuarioUsername = usuario.Username;
                    }
                }

                return comentarios;
            }
            catch (Exception ex)
            {
                // Trata qualquer erro que possa ocorrer durante a busca e lança a exceção.
                _logger.LogError(ex, "Erro ao buscar comentários do post.");
                throw new Exception("Erro ao buscar comentários do post.");
            }
        }

        // Adiciona um novo comentário ao banco de dados.
        public async Task AddComentarioAsync(ComentarioModel comentario)
        {
            try
            {
                if (comentario == null)
                {
                    throw new ArgumentNullException(nameof(comentario), "Comentário não pode ser nulo.");
                }

                // Insere o comentário na coleção de comentários.
                await _comentarioCollection.InsertOneAsync(comentario);
            }
            catch (Exception ex)
            {
                // Trata qualquer erro que possa ocorrer durante a inserção e lança a exceção.
                _logger.LogError(ex, "Erro ao adicionar comentário.");
                throw new Exception("Erro ao adicionar comentário.");
            }
        }

        // Deleta um comentário pelo seu ID.
        public async Task<bool> DeletarComentarioAsync(string id)
        {
            try
            {
                var comentario = await _comentarioCollection.FindAsync(x => x.Id == id);

                // Se não houver comentário, retorna exceção.
                if (comentario == null)
                {
                    throw new Exception("Comentário não encontrado no banco de dados");
                }

                // Deleta o comentário com base no ID.
                var deletarComentario = await _comentarioCollection.DeleteOneAsync(x => x.Id == id);

                // Retorna verdadeiro se o comentário foi deletado, caso contrário, retorna falso.
                return deletarComentario.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // Trata qualquer erro que possa ocorrer durante a exclusão e lança a exceção.
                _logger.LogError(ex, "Erro ao deletar comentário.");
                throw new Exception("Erro ao deletar comentário");
            }
        }
    }
}
