using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IMongoCollection<UsuarioModel> _usuarioCollection;
        private IMongoCollection<PostsModel> _postCollection;
        private IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly ISessao _sessao;

        public UsuarioRepository(MongoContext mongoContext, ISessao sessao)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuario");
            _postCollection = mongoContext.GetCollection<PostsModel>("Posts");
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentario");
            _sessao = sessao;
        }

        public async Task<UsuarioModel> BuscarUsuarioPorIdAsync(string id)
        {
            return await _usuarioCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<UsuarioModel>> BuscarVariosUsuariosPorIdAsync(List<string> ids)
        {
            return await _usuarioCollection
                   .Find(usuario => ids.Contains(usuario.Id))
                   .ToListAsync();
        }

        public async Task<List<UsuarioModel>> BuscarSugestoesParaSeguir(string id)
        {
            var usuario = await BuscarUsuarioPorIdAsync(id);

            if (usuario == null) throw new Exception("Usuario Nulo");

            var idsSeguindo = usuario.Seguindo ?? new List<string>();

            var sugestoesUsuarios = await _usuarioCollection.Find(x => x.Id != id && !idsSeguindo.Contains(x.Id)).Limit(12).ToListAsync();

            return sugestoesUsuarios;
        }

        public async Task<UsuarioModel> BuscarUsuarioPorEmailOuUsername(string emailOuUsername)
        {
            return await _usuarioCollection
                .Find(x => x.Email.Equals(emailOuUsername, StringComparison.CurrentCultureIgnoreCase) ||
                           x.Username.Equals(emailOuUsername, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync();
        }

        public async Task AddUsuarioAsync(UsuarioModel usuario)
        {
            usuario.SetSenhaHash();
            await _usuarioCollection.InsertOneAsync(usuario);
        }

        public async Task AtualizarUsuarioAsync(UsuarioModel usuario)
        {
            var usuarioDb = await BuscarUsuarioPorIdAsync(usuario.Id);

            if (usuarioDb == null) throw new Exception("Houve um erro na atualização do usuário!");

            // Criar filtro para localizar o documento pelo ID
            var filter = Builders<UsuarioModel>.Filter.Eq(u => u.Id, usuario.Id);

            // Criar as atualizações apenas com os campos relevantes
            var update = Builders<UsuarioModel>.Update
                .Set(u => u.FotoPerfil, usuario.FotoPerfil)
                .Set(u => u.Nome, usuario.Nome)
                .Set(u => u.Username, usuario.Username)
                .Set(u => u.Email, usuario.Email)
                .Set(u => u.DataNascimento, usuario.DataNascimento)
                .Set(u => u.PaisNascimento, usuario.PaisNascimento)
                .Set(u => u.Bio, usuario.Bio ?? string.Empty)
                .Set(u => u.LocalizacaoAtual, usuario.LocalizacaoAtual ?? string.Empty);

            // Atualizar o documento na coleção
            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            // Verificar se algum documento foi modificado
            if (resultado.ModifiedCount == 0)
            {
                // Caso não haja alterações, lança uma exceção
                throw new Exception("Nenhuma alteração feita.");
            }
        }

        public async Task<bool> DeletarUsuarioAsync(string id)
        {
            var posts = await _postCollection.DeleteManyAsync(x => x.UsuarioId == id);

            var comentario = await _comentarioCollection.DeleteManyAsync(x => x.UsuarioId == id);

            var deleteUsuario = await _usuarioCollection.DeleteOneAsync(x => x.Id == id);
            _sessao.RemoverSessaoUsuario();

            return deleteUsuario.DeletedCount > 0;
        }
    }
}