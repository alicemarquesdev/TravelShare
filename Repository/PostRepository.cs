using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<PostModel> _postsCollection;
        private readonly IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public PostRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository,
                IComentarioRepository comentarioRepository, IWebHostEnvironment hostingEnvironment)
        {
            _postsCollection = mongoContext.GetCollection<PostModel>("Posts");
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentarios");
            _usuarioRepository = usuarioRepository;
            _comentarioRepository = comentarioRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<List<PostModel>> BuscarTodosOsPostsSeguindoAsync(string id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(id);

            if (usuario == null) return new List<PostModel>();

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

        public async Task<List<PostModel>> BuscarTodosOsPostsNãoSeguindoAsync(string id)
        {
            // Obtém os usuários que o usuário não segue
            var usuarios = await _usuarioRepository.BuscarSugestoesParaSeguirAsync(id);

            var posts = new List<PostModel>();

            // Loop para buscar os posts de cada usuário
            foreach (var usuario in usuarios)
            {
                var postsDoUsuario = await BuscarTodosOsPostsDoUsuarioAsync(usuario.Id);

                // Adiciona todos os posts do usuário na lista de posts
                posts.AddRange(postsDoUsuario);
            }

            return posts;
        }

        public async Task<List<PostModel>> BuscarTodosOsPostsDoUsuarioAsync(string usuarioId)
        {
            var posts = await _postsCollection.Find(x => x.UsuarioId == usuarioId).SortByDescending(x => x.DataCriacao).ToListAsync();

            foreach (var post in posts)
            {
                // Buscar o usuário relacionado
                post.Usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);

                // Buscar os comentários relacionados ao post
                post.Comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);
            }
            return posts;
        }

        public async Task<PostModel> BuscarPostPorIdAsync(string postId)
        {
            var post = await _postsCollection.Find(x => x.Id == postId).FirstOrDefaultAsync();

            post.Usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(post.UsuarioId);

            // Buscar os comentários relacionados ao post
            post.Comentarios = await _comentarioRepository.BuscarTodosOsComentariosDoPostAsync(post.Id);

            return post;
        }

        public async Task AddPostAsync(PostModel post)
        {
            await _postsCollection.InsertOneAsync(post);
        }

        public async Task AtualizarPostAsync(PostModel post)
        {
            var filter = Builders<PostModel>.Filter.Eq(x => x.Id, post.Id);

            var update = Builders<PostModel>.Update
                       .Set(x => x.Localizacao, post.Localizacao)
                       .Set(x => x.Legenda, post.Legenda)
                       .Set(x => x.DataAtualizacao, DateTime.Now);

            var result = await _postsCollection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0) throw new Exception("Nenhum documento foi atualizado.");
        }

            public async Task<bool> DeletarPostAsync(string id)
            {
                // Passo 1: Buscar o post para obter as imagens associadas
                var post = await _postsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (post == null)
                {
                    return false; // Se o post não existir no banco de dados, não faz sentido continuar
                }

                // Passo 2: Criar uma lista para armazenar as imagens
                List<string> imagensParaDeletar = new List<string>();

                if (post.ImagemPost != null)
                {
                    imagensParaDeletar.AddRange(post.ImagemPost); // Adiciona as imagens à lista
                }

                // Passo 3: Deletar os comentários relacionados ao post
                await _comentarioCollection.DeleteManyAsync(x => x.PostId == id);

                // Passo 4: Deletar o post do banco de dados
                var deletarPost = await _postsCollection.DeleteOneAsync(x => x.Id == id);
                if (deletarPost.DeletedCount > 0)
                {
                    // Se o post foi deletado com sucesso, deletar as imagens do servidor
                    foreach (var imagem in imagensParaDeletar)
                    {
                        var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, imagem);

                        if (File.Exists(imagePath))
                        {
                            try
                            {
                                File.Delete(imagePath); // Tenta apagar a imagem do servidor
                            }
                            catch (Exception)
                            {
                                // Log de erro se falhar ao deletar a imagem
                                Console.WriteLine($"Erro ao excluir a imagem: {imagem}");
                            }
                        }
                    }

                    return true; // Sucesso na exclusão do post e das imagens
                }

                return false; // Falha na exclusão do post
            }
        }
}