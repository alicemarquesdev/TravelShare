using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class CidadesVisitadasRepository : ICidadesVisitadasRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;

        public CidadesVisitadasRepository(MongoContext mongoContext)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");
        }

        public async Task<bool> VerificarCidadeVisitadaPeloUsuarioAsync(string usuarioId, string cidade)
        {
            var cidadeVisitada = await _usuarioCollection.Find(x => x.Id == usuarioId && x.CidadesVisitadas.Contains(cidade)).FirstOrDefaultAsync();

            return cidadeVisitada != null;
        }

        public async Task<bool> AddCidadeAsync(string usuarioId, string cidade)
        {
            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
            var update = Builders<UsuarioModel>.Update.AddToSet(X => X.CidadesVisitadas, cidade);

            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }

        public async Task<bool> RemoveCidadeAsync(string usuarioId, string cidade)
        {
            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
            var update = Builders<UsuarioModel>.Update.Pull(X => X.CidadesVisitadas, cidade);

            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }
    }
}