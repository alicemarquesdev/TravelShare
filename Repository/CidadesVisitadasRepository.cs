using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Classe responsável pela manipulação das cidades visitadas de um usuário.
    // Métodos disponíveis:
    // - VerificarCidadeVisitadaPeloUsuarioAsync(string usuarioId, string cidade): Verifica se uma cidade foi visitada pelo usuário.
    // - AddCidadeAsync(string usuarioId, string cidade): Adiciona uma cidade à lista de cidades visitadas do usuário.
    // - RemoveCidadeAsync(string usuarioId, string cidade): Remove uma cidade da lista de cidades visitadas do usuário.
    public class CidadesVisitadasRepository : ICidadesVisitadasRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;
        private readonly ILogger<CidadesVisitadasRepository> _logger;

        // Construtor que recebe o contexto do MongoDB e inicializa a coleção de usuários
        public CidadesVisitadasRepository(MongoContext mongoContext, ILogger<CidadesVisitadasRepository> logger)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");
            _logger = logger;
        }

        // Método para verificar se uma cidade foi visitada por um usuário
        public async Task<bool> VerificarCidadeVisitadaPeloUsuarioAsync(string usuarioId, string cidade)
        {
            try
            {
                var usuario = await _usuarioCollection.FindAsync(x => x.Id == usuarioId);

                // Validação usuário
                if (usuario == null)
                {
                    throw new Exception("Usuário não encontrado no banco de dados");
                }

                // Busca o usuário no banco de dados verificando se a cidade já está na lista de cidades visitadas
                var cidadeVisitada = await _usuarioCollection
                    .Find(x => x.Id == usuarioId && x.CidadesVisitadas.Contains(cidade))
                    .FirstOrDefaultAsync();

                // Retorna true se a cidade foi encontrada na lista de cidades visitadas
                return cidadeVisitada != null;
            }
            catch (Exception ex)
            {
                // Captura qualquer exceção e loga o erro (pode-se adicionar um logger aqui)
                _logger.LogError(ex, "Erro ao verificar cidade visitada.");
                throw new Exception("Erro ao verificar cidade visitada.");
            }
        }

        // Método para adicionar uma cidade à lista de cidades visitadas por um usuário
        public async Task<bool> AddCidadeAsync(string usuarioId, string cidade)
        {
            try
            {
                var usuario = await _usuarioCollection.FindAsync(x => x.Id == usuarioId);

                // Validação usuário
                if (usuario == null)
                {
                    throw new Exception("Usuário não encontrado no banco de dados");
                }

                // Cria o filtro para encontrar o usuário no banco de dados pelo ID
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);

                // Cria o comando de atualização para adicionar a cidade à lista de cidades visitadas
                var update = Builders<UsuarioModel>.Update.AddToSet(x => x.CidadesVisitadas, cidade);

                // Executa a atualização no banco de dados
                var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

                // Retorna true se a cidade foi adicionada com sucesso (modified count maior que 0)
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Captura qualquer exceção e loga o erro (pode-se adicionar um logger aqui)
                _logger.LogError(ex, "Erro ao adicionar cidade.");
                throw new Exception("Erro ao adicionar cidade.");
            }
        }

        // Método para remover uma cidade da lista de cidades visitadas por um usuário
        public async Task<bool> RemoveCidadeAsync(string usuarioId, string cidade)
        {
            try
            {
                var usuario = await _usuarioCollection.FindAsync(x => x.Id == usuarioId);

                // Validação usuário
                if (usuario == null)
                {
                    throw new Exception("Usuário não encontrado no banco de dados");
                }

                // Cria o filtro para encontrar o usuário no banco de dados pelo ID
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);

                // Cria o comando de atualização para remover a cidade da lista de cidades visitadas
                var update = Builders<UsuarioModel>.Update.Pull(x => x.CidadesVisitadas, cidade);

                // Executa a atualização no banco de dados
                var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

                // Retorna true se a cidade foi removida com sucesso (modified count maior que 0)
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Captura qualquer exceção e loga o erro 
                _logger.LogError(ex, "Erro ao remover cidade.");
                throw new Exception("Erro ao remover cidade.");
            }
        }
    }
}
