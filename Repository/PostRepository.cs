using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Repositório responsável pela manipulação de posts e comentários.
    // Métodos disponíveis:
    // - BuscarPostPorIdAsync(string postId): Busca um post pelo ID.
    // - BuscarTodosOsPostsSeguindoAsync(string id): Busca posts de usuários que o usuário está seguindo.
    // - BuscarTodosOsPostsNaoSeguindoAsync(string id): Busca posts de usuários que o usuário não está seguindo.
    // - BuscarTodosOsPostsDoUsuarioAsync(string usuarioId): Busca posts de um usuário específico.
    // - AddPostAsync(PostModel post): Adiciona um novo post.
    // - AtualizarPostAsync(PostModel post): Atualiza um post existente.
    // - DeletarPostAsync(string id): Deleta um post.
    public class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<PostModel> _postsCollection;
        private readonly IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ILogger<PostRepository> _logger;

        // Construtor que recebe o contexto do MongoDB, repositórios de usuários, comentários e ambiente de hospedagem
        public PostRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository,
                IComentarioRepository comentarioRepository, ILogger<PostRepository> logger)
        {
            _postsCollection = mongoContext.GetCollection<PostModel>("Posts");
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentarios");
            _usuarioRepository = usuarioRepository;
            _comentarioRepository = comentarioRepository;
            _logger = logger;
        }

        // Método para buscar um post específico pelo seu ID
        public async Task<PostModel?> BuscarPostPorIdAsync(string postId)
        {
            try
            {
                // Busca o post pelo ID
                var post = await _postsCollection.Find(x => x.Id == postId).FirstOrDefaultAsync();
                if (post == null) return null; // Retorna null se o post não for encontrado

                // Busca informações adicionais sobre o usuário e os comentários do post
                post.Usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);
                post.Comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);

                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar post por ID.");
                throw new Exception("Erro ao buscar post por ID.");
            }
        }

        // Método para buscar todos os posts de usuários que o usuário está seguindo
        public async Task<List<PostModel>> BuscarTodosOsPostsSeguindoAsync(string id)
        {
            try
            {
                // Busca o usuário 
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);
                if (usuario == null) throw new Exception("Usuário não encontrado no banco de dados");

                // Monta a lista de IDs para buscar os posts dos usuários seguidos
                var idsParaBuscar = new List<string> { id };
                if (usuario.Seguindo != null)
                {
                    idsParaBuscar.AddRange(usuario.Seguindo);
                }

                // Busca os posts de usuários que o usuário está seguindo, ordenados pela data de criação
                var posts = await _postsCollection.Find(post => idsParaBuscar.Contains(post.UsuarioId))
                                                  .SortByDescending(post => post.DataCriacao)
                                                  .ToListAsync();

                // Para cada post, busca informações adicionais, como o usuário que fez o post e os comentários
                foreach (var post in posts)
                {
                    post.Usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);
                    post.Comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);
                }

                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts de usuários seguindo.");
                throw new Exception("Erro ao buscar posts de usuários seguindo.");
            }
        }

        // Método para buscar posts de usuários que o usuário não está seguindo
        public async Task<List<PostModel>> BuscarTodosOsPostsNãoSeguindoAsync(string id)
        {
            try
            {
                // Busca o usuário 
                var usuarioDb = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);
                if (usuarioDb == null) throw new Exception("Usuário não encontrado no banco de dados");

                // Busca sugestões de usuários para seguir
                var usuarios = await _usuarioRepository.BuscarSugestoesParaSeguirAsync(id);
                var posts = new List<PostModel>();

                // Para cada usuário sugerido, busca os posts desse usuário
                foreach (var usuario in usuarios)
                {
                    var postsDoUsuario = await BuscarTodosOsPostsDoUsuarioAsync(usuario.Id);
                    posts.AddRange(postsDoUsuario);
                }

                return posts;
            }
            catch (Exception ex)
            {
                // Em caso de erro, exibe a mensagem de erro e lança a exceção
                _logger.LogError(ex, "Erro ao buscar posts de usuários não seguindo.");
                throw new Exception("Erro ao buscar posts de usuários não seguindo.");
            }
        }

        // Método para buscar todos os posts de um usuário específico
        public async Task<List<PostModel>> BuscarTodosOsPostsDoUsuarioAsync(string usuarioId)
        {
            try
            {
                // Busca o usuário 
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);
                if (usuario == null) throw new Exception("Usuário não encontrado no banco de dados");

                // Busca todos os posts do usuário, ordenados pela data de criação
                var posts = await _postsCollection.Find(x => x.UsuarioId == usuarioId)
                                                  .SortByDescending(x => x.DataCriacao)
                                                  .ToListAsync();

                // Para cada post, busca informações adicionais, como o usuário que fez o post e os comentários
                foreach (var post in posts)
                {
                    post.Usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);
                    post.Comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);
                }

                return posts;
            }
            catch (Exception ex)
            {
                // Em caso de erro, exibe a mensagem de erro e lança a exceção
                _logger.LogError(ex, "Erro ao buscar posts do usuário.");
                throw new Exception("Erro ao buscar posts do usuário.");
            }
        }

        // Método para adicionar um novo post
        public async Task AddPostAsync(PostModel post)
        {
            try
            {
                // Valida se o post não é nulo
                if (post == null) throw new ArgumentNullException(nameof(post), "O post não pode ser nulo.");

                // Insere o novo post na coleção do MongoDB
                await _postsCollection.InsertOneAsync(post);
            }
            catch (Exception ex)
            {
                // Em caso de erro, exibe a mensagem de erro e lança a exceção
                _logger.LogError(ex, "Erro ao adicionar post.");
                throw new Exception("Erro ao adicionar post.");

            }
        }

        // Método para atualizar as informações de um post existente
        public async Task AtualizarPostAsync(PostModel post)
        {
            try
            {
                // Busca o usuário 
                var postDb = await BuscarPostPorIdAsync(post.Id);
                if (postDb == null) throw new Exception("Post não encontrado no banco de dados");

                // Cria o filtro para encontrar o post pelo ID
                var filter = Builders<PostModel>.Filter.Eq(x => x.Id, post.Id);
                var update = Builders<PostModel>.Update
                           .Set(x => x.Localizacao, post.Localizacao)
                           .Set(x => x.Legenda, post.Legenda)
                           .Set(x => x.DataAtualizacao, DateTime.Now);

                // Atualiza o post no banco de dados
                var result = await _postsCollection.UpdateOneAsync(filter, update);

                // Verifica se nenhum documento foi atualizado
                if (result.ModifiedCount == 0) throw new Exception("Nenhum documento foi atualizado.");
            }
            catch (Exception ex)
            {
                // Em caso de erro, exibe a mensagem de erro e lança a exceção
                _logger.LogError(ex, "Erro ao atualizar post.");
                throw new Exception("Erro ao atualizar post.");
            }
        }

        // Método para deletar um post
        public async Task<bool> DeletarPostAsync(string id)
        {
            try
            {
                // Busca o post a ser deletado pelo ID
                var post = await BuscarPostPorIdAsync(id);
                if (post == null) throw new Exception("Post não encontrado no banco de dados");

                // Prepara a lista de imagens para deletar
                List<string> imagensParaDeletar = new List<string>();
                if (post.ImagemPost != null)
                {
                    imagensParaDeletar.AddRange(post.ImagemPost);
                }

                // Deleta os comentários do post
                await _comentarioCollection.DeleteManyAsync(x => x.PostId == id);

                // Deleta o post do banco de dados
                var deletarPost = await _postsCollection.DeleteOneAsync(x => x.Id == id);
                if (deletarPost.DeletedCount > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Em caso de erro, exibe a mensagem de erro e lança a exceção
                _logger.LogError(ex, "Erro ao deletar post.");
                throw new Exception("Erro ao deletar post.");
            }
        }
    }
}
