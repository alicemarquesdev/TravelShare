using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly IUsuarioRepository _usuarioRepository;

        public ComentarioRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository)
        {
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentarios");
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ComentarioModel> BuscarComentarioPorIdAsync(string id)
        {
            var comentario = await _comentarioCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (comentario == null) throw new NullReferenceException("A referência do comentário está nula.");

            return comentario;
        }

        public async Task<List<ComentarioModel>> BuscarTodosOsComentariosDoPostAsync(string postId)
        {
            var comentarios = await _comentarioCollection.Find(x => x.PostId == postId).ToListAsync();

            // Se não houver comentários, retorna uma lista vazia
            if (!comentarios.Any()) return new List<ComentarioModel>();

            // Extrai os IDs dos usuários que fizeram os comentários
            var usuarioIds = comentarios.Select(c => c.UsuarioId).Distinct().ToList();

            // Busca todos os usuários de uma vez
            var usuarios = await _usuarioRepository.BuscarVariosUsuariosPorIdAsync(usuarioIds);

            // Cria um dicionário de usuários para associar aos comentários
            var usuarioDictionary = usuarios.ToDictionary(u => u.Id, u => u);

            // Associa os usuários aos comentários
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

        public async Task AddComentarioAsync(ComentarioModel comentario)
        {
            await _comentarioCollection.InsertOneAsync(comentario);
        }

        public async Task<bool> DeletarComentarioAsync(string id)
        {
            var deletarComentario = await _comentarioCollection.DeleteOneAsync(x => x.Id == id);
            return deletarComentario.DeletedCount > 0;
        }
    }
}