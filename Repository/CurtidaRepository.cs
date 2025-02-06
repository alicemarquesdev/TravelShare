using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class CurtidaRepository : ICurtidaRepository
    {
        private readonly IMongoCollection<PostsModel> _postsCollection;

        public CurtidaRepository(MongoContext mongoContext)
        {
            _postsCollection = mongoContext.GetCollection<PostsModel>("Posts");
        }

        public async Task<bool> BuscarCurtidaExistenteAsync(string postId, string usuarioId)
        {
            var curtidaExistente = await _postsCollection.Find(x => x.Id == postId && x.Curtidas.Contains(usuarioId)).FirstOrDefaultAsync();
            return curtidaExistente != null;
        }

        public async Task<bool> AddCurtidaAsync(string postId, string usuarioId)
        {
            var filter = Builders<PostsModel>.Filter.Eq(x => x.Id, postId);
            var update = Builders<PostsModel>.Update.AddToSet(x => x.Curtidas, usuarioId);

            var resultado = await _postsCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }

        public async Task<bool> RemoveCurtidaAsync(string postId, string usuarioId)
        {
            var filter = Builders<PostsModel>.Filter.Eq(x => x.Id, postId);
            var update = Builders<PostsModel>.Update.Pull(x => x.Curtidas, usuarioId);

            var resultado = await _postsCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }
    }
}