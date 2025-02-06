using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class SeguidorRepository : ISeguidorRepository
    {
        private IMongoCollection<UsuarioModel> _usuarioCollection;
        private IUsuarioRepository _usuarioRepository;

        public SeguidorRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuario");
            _usuarioRepository = usuarioRepository;
        }

        // CRUD Seguidor

        public async Task<List<UsuarioModel>> BuscarTodosSeguidoresAsync(string usuarioId)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
                return new List<UsuarioModel>();

            List<UsuarioModel> seguidores = new List<UsuarioModel>();

            foreach (var seguidor in usuario.Seguidores)
            {
                var usuarioSeguidor = await _usuarioRepository.BuscarUsuarioPorIdAsync(seguidor);
                seguidores.Add(usuarioSeguidor);
            }

            return seguidores;
        }

        public async Task<UsuarioModel> BuscarSeguidorAsync(string usuarioId, string seguidorId)
        {
            return await _usuarioCollection.Find(x => x.Id == usuarioId && x.Seguidores.Contains(seguidorId)).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoverSeguidorAsync(string usuarioId, string seguidorId)
        {
            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
            var update = Builders<UsuarioModel>.Update.Pull(x => x.Seguidores, seguidorId);

            var seguindo = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
            var removerDosSeguindo = Builders<UsuarioModel>.Update.Pull(x => x.Seguindo, usuarioId);

            await _usuarioCollection.UpdateOneAsync(seguindo, removerDosSeguindo);

            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }

        // CRUD Seguindo

        public async Task<List<UsuarioModel>> BuscarTodosSeguindoAsync(string usuarioId)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
                return new List<UsuarioModel>();

            List<UsuarioModel> seguindo = new List<UsuarioModel>();

            foreach (var seguidor in usuario.Seguindo)
            {
                var usuarioSeguindo = await _usuarioRepository.BuscarUsuarioPorIdAsync(seguidor);
                seguindo.Add(usuarioSeguindo);
            }

            return seguindo;
        }

        public async Task<UsuarioModel> BuscarSeguindoAsync(string usuarioId, string seguindoId)
        {
            return await _usuarioCollection.Find(x => x.Id == usuarioId && x.Seguindo.Contains(seguindoId)).FirstOrDefaultAsync();
        }

        public async Task DeseguirUsuarioAsync(string usuarioId, string seguindoId)
        {
            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
            var update = Builders<UsuarioModel>.Update.Pull(x => x.Seguindo, seguindoId);

            var usuarioSeguido = Builders<UsuarioModel>.Filter.Eq(x => x.Id, seguindoId);
            var updateSeguido = Builders<UsuarioModel>.Update.Pull(x => x.Seguidores, usuarioId);

            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);
            var removerUsuarioDosSeguidores = await _usuarioCollection.UpdateOneAsync(usuarioSeguido, updateSeguido);
        }

        public async Task SeguirUsuarioAsync(string usuarioId, string seguindoId)
        {
            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
            var update = Builders<UsuarioModel>.Update.AddToSet(x => x.Seguindo, seguindoId);

            var usuarioSeguido = Builders<UsuarioModel>.Filter.Eq(x => x.Id, seguindoId);
            var updateSeguido = Builders<UsuarioModel>.Update.AddToSet(x => x.Seguidores, usuarioId);

            var seguir = await _usuarioCollection.UpdateOneAsync(filter, update);
            var addUsuarioNosSeguidores = await _usuarioCollection.UpdateOneAsync(usuarioSeguido, updateSeguido);
        }
    }
}