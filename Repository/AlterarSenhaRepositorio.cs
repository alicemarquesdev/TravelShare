using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class AlterarSenhaRepositorio : IAlterarSenhaRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;

        public AlterarSenhaRepositorio(MongoContext mongoContext)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");

        }
        public async Task<bool> AlterarSenhaAsync(AlterarSenhaModel alterarSenha)
        {
            var usuarioDb = await _usuarioCollection.Find(x => x.Id == alterarSenha.Id).FirstOrDefaultAsync();

            if (usuarioDb == null) return false;

            if (!usuarioDb.SenhaValida(alterarSenha.SenhaAtual))
            {
                throw new Exception("A senha atual informada não está correta.");
            }

            // Se a nova senha é igual a senha atual
            if (usuarioDb.SenhaValida(alterarSenha.NovaSenha))
            { 
                throw new Exception("A nova senha não pode ser igual à senha atual.");

            }

            usuarioDb.SetNovaSenha(alterarSenha.NovaSenha);


            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, alterarSenha.Id);
            var update = Builders<UsuarioModel>.Update.Set(x => x.Senha, alterarSenha.NovaSenha.GerarHash());

            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }



    }
}
