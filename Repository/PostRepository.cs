using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class PostRepository : IPostRepository
    {
        private IMongoCollection<PostsModel> _postsCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly ICaminhoImagem _caminhoImagem;

        public PostRepository(MongoContext mongoContext, ICaminhoImagem caminhoImagem, IUsuarioRepository usuarioRepository, IComentarioRepository comentarioRepository)
        {
            _postsCollection = mongoContext.GetCollection<PostsModel>("Posts");
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentario");
            _usuarioRepository = usuarioRepository;
            _comentarioRepository = comentarioRepository;
            _caminhoImagem = caminhoImagem;
        }

        public async Task<List<PostsModel>> BuscarTodosOsPostsSeguindoAsync(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return new List<PostsModel>(); // Retorna vazio se o usuário não existir
            }

            // Criar uma lista de IDs contendo o próprio usuário + seguidores
            var idsParaBuscar = new List<string> { id }; // Adiciona o próprio usuário
            if (usuario.Seguindo != null)
            {
                idsParaBuscar.AddRange(usuario.Seguindo); // Adiciona os seguidores
            }

            // Busca os posts do usuário e dos seguidores
            var posts = await _postsCollection.Find(post => idsParaBuscar.Contains(post.UsuarioId))
                                              .SortByDescending(post => post.DataCriacao)
                                              .ToListAsync();

            // Associa usuários e comentários aos posts
            foreach (var post in posts)
            {
                post.Usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);
                post.Comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);
            }

            return posts;
        }

        public async Task<List<PostsModel>> BuscarTodosOsPostsDoUsuarioAsync(string usuarioId)
        {
            var posts = await _postsCollection.Find(x => x.UsuarioId == usuarioId).SortByDescending(x => x.DataCriacao).ToListAsync();

            foreach (var post in posts)
            {
                // Buscar o usuário relacionado
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);
                post.Usuario = usuario;

                // Buscar os comentários relacionados ao post
                var comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);
                post.Comentarios = comentarios;
            }

            return posts;
        }

        public async Task<PostsModel> BuscarPostPorIdAsync(string postId)
        {
            var post = await _postsCollection.Find(x => x.Id == postId).FirstOrDefaultAsync();

            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);
            post.Usuario = usuario;

            // Buscar os comentários relacionados ao post
            var comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);
            post.Comentarios = comentarios;

            return post;
        }

        public async Task AddPostAsync(PostsModel post)
        {
            await _postsCollection.InsertOneAsync(post);
        }

        public async Task AtualizarPostAsync(PostsModel post)
        {
            var filter = Builders<PostsModel>.Filter.Eq(x => x.Id, post.Id);

            var update = Builders<PostsModel>.Update
                       .Set(x => x.FotoPost, post.FotoPost)
                       .Set(x => x.Localizacao, post.Localizacao)
                       .Set(x => x.Legenda, post.Legenda)
                       .Set(x => x.DataAtualizacao, DateTime.Now);

            var result = await _postsCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0) throw new Exception("Nenhum documento foi atualizado. O ID pode não existir.");
        }

        public async Task<bool> DeletarPostAsync(string id)
        {
            var comentario = await _comentarioCollection.DeleteManyAsync(x => x.PostId == id);

            var deletarPost = await _postsCollection.DeleteOneAsync(x => x.Id == id);
            return deletarPost.DeletedCount > 0;
        }

        public async Task<List<PostsModel>> BuscarTodosOsPostsNãoSeguindoAsync(string id)
        {
            // Obtém os usuários que o usuário não segue
            var usuarios = await _usuarioRepository.BuscarSugestoesParaSeguir(id);

            var posts = new List<PostsModel>();

            // Loop para buscar os posts de cada usuário
            foreach (var usuario in usuarios)
            {
                var postsDoUsuario = await BuscarTodosOsPostsDoUsuarioAsync(usuario.Id);

                // Adiciona todos os posts do usuário na lista de posts
                posts.AddRange(postsDoUsuario);
            }

            // Embaralha os posts de forma aleatória
            Random random = new Random();
            var postsAleatorios = posts.OrderBy(x => random.Next()).ToList();

            return postsAleatorios;
        }
    }
}