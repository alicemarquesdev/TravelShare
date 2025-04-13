using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Classe responsável pela manipulação dos seguidores e seguindo de um usuário.
    // Métodos disponíveis:
    // - BuscarTodosSeguidoresAsync(string usuarioId): Retorna todos os seguidores de um usuário.
    // - BuscarSeguidorAsync(string usuarioId, string seguidorId): Verifica se um usuário específico é seguidor de outro.
    // - RemoverSeguidorAsync(string usuarioId, string seguidorId): Remove um seguidor da lista de seguidores do usuário.
    // - BuscarTodosSeguindoAsync(string usuarioId): Retorna todas as pessoas que um usuário está seguindo.
    // - BuscarSeguindoAsync(string usuarioId, string seguindoId): Verifica se um usuário específico está sendo seguido por outro.
    // - DeseguirUsuarioAsync(string usuarioId, string seguindoId): Remove alguém da lista de pessoas que um usuário está seguindo.
    // - SeguirUsuarioAsync(string usuarioId, string seguindoId): Adiciona um seguidor à lista de pessoas que um usuário está seguindo.
    public class SeguidorRepository : ISeguidorRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<SeguidorRepository> _logger;

        // Construtor que recebe o contexto do MongoDB e o repositório de usuários
        public SeguidorRepository(MongoContext mongoContext, IUsuarioRepository usuarioRepository, ILogger<SeguidorRepository> logger)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // Método para buscar todos os seguidores de um usuário
        public async Task<List<UsuarioModel>> BuscarTodosSeguidoresAsync(string usuarioId)
        {
            try
            {
                // Busca o usuário no banco de dados
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);

                // Validação para verificar se o usuário existe
                if (usuario == null) return new List<UsuarioModel>();

                List<UsuarioModel> seguidores = new List<UsuarioModel>();

                // Itera sobre os seguidores e busca o modelo completo de cada um
                foreach (var seguidor in usuario.Seguidores)
                {
                    var usuarioSeguidor = await _usuarioRepository.BuscarUsuarioPorIdAsync(seguidor);
                    seguidores.Add(usuarioSeguidor);
                }

                // Retorna a lista de seguidores encontrados
                return seguidores;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar seguidores");
                throw new Exception("Erro ao buscar seguidores.");
            }
        }

        // Método para buscar um seguidor específico
        public async Task<UsuarioModel?> BuscarSeguidorAsync(string usuarioId, string seguidorId)
        {
            try
            {
                // Busca no banco de dados se o seguidor está na lista de seguidores do usuário
                return await _usuarioCollection.Find(x => x.Id == usuarioId && x.Seguidores.Contains(seguidorId)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar seguidor");
                throw new Exception("Erro ao buscar seguidor.");
            }
        }

        // Método para remover um seguidor da lista de seguidores de um usuário
        public async Task<bool> RemoverSeguidorAsync(string usuarioId, string seguidorId)
        {
            try
            {
                var relacaoUsuarioSeguidor = BuscarSeguidorAsync(usuarioId, seguidorId);

                if (relacaoUsuarioSeguidor == null)
                {
                    throw new Exception("Relação entre usuário e o seguidor não encontrada no banco de dados");
                }

                // Filtro para encontrar o usuário
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
                var update = Builders<UsuarioModel>.Update.Pull(x => x.Seguidores, seguidorId);

                // Filtro para o seguidor que será removido
                var seguindoFilter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, seguidorId);
                var removerDosSeguindo = Builders<UsuarioModel>.Update.Pull(x => x.Seguindo, usuarioId);

                // Executa as atualizações no banco de dados para remover o seguidor
                var resultadoUsuario = await _usuarioCollection.UpdateOneAsync(filter, update);
                var resultadoSeguidor = await _usuarioCollection.UpdateOneAsync(seguindoFilter, removerDosSeguindo);

                // Retorna verdadeiro se as remoções foram bem-sucedidas
                return resultadoUsuario.ModifiedCount > 0 && resultadoSeguidor.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover seguidor");
                throw new Exception("Erro ao remover seguidor.");
            }
        }

        // Método para buscar todos os usuários que um usuário está seguindo
        public async Task<List<UsuarioModel>> BuscarTodosSeguindoAsync(string usuarioId)
        {
            try
            {
                // Busca o usuário no banco de dados
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);

                // Validação para verificar se o usuário existe
                if (usuario == null) return new List<UsuarioModel>();

                List<UsuarioModel> seguindo = new List<UsuarioModel>();

                // Itera sobre os usuários seguidos e busca o modelo completo de cada um
                foreach (var seguidor in usuario.Seguindo)
                {
                    var usuarioSeguindo = await _usuarioRepository.BuscarUsuarioPorIdAsync(seguidor);
                    seguindo.Add(usuarioSeguindo);
                }

                // Retorna a lista de usuários seguidos
                return seguindo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar seguidores");
                throw new Exception("Erro ao buscar seguidor.");
            }
        }

        // Método para verificar se um usuário está seguindo outro
        public async Task<UsuarioModel?> BuscarSeguindoAsync(string usuarioId, string seguindoId)
        {
            try
            {
                // Verifica se o usuário está seguindo o usuário alvo
                return await _usuarioCollection.Find(x => x.Id == usuarioId && x.Seguindo.Contains(seguindoId)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Caso haja erro, captura a exceção e retorna null
                _logger.LogError(ex, "Erro ao buscar seguidor");
                throw new Exception("Erro ao buscar seguidor.");
            }
        }

        // Método para remover alguém da lista de pessoas que o usuário está seguindo
        public async Task DeseguirUsuarioAsync(string usuarioId, string seguindoId)
        {
            try
            {
                var relacaoUsuarioSeguidor = BuscarSeguindoAsync(usuarioId, seguindoId);

                if (relacaoUsuarioSeguidor == null)
                {
                    throw new Exception("Relação entre usuário e o usuário seguindo não encontrada no banco de dados");
                }

                // Filtro para o usuário que deseja deseguir
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
                var update = Builders<UsuarioModel>.Update.Pull(x => x.Seguindo, seguindoId);

                // Filtro para remover o usuário da lista de seguidores do usuário seguido
                var usuarioSeguidoFilter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, seguindoId);
                var updateSeguido = Builders<UsuarioModel>.Update.Pull(x => x.Seguidores, usuarioId);

                // Executa as atualizações no banco de dados para deseguir
                var resultadoUsuario = await _usuarioCollection.UpdateOneAsync(filter, update);
                var resultadoSeguidor = await _usuarioCollection.UpdateOneAsync(usuarioSeguidoFilter, updateSeguido);

                // Verifica se ambas as remoções foram bem-sucedidas
                if (resultadoUsuario.ModifiedCount == 0 || resultadoSeguidor.ModifiedCount == 0)
                {
                    throw new Exception("Falha ao deseguir usuário.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deseguir usuario");
                throw new Exception("Erro ao deseguir usuario.");

            }
        }

        // Método para adicionar um seguidor à lista de pessoas que o usuário está seguindo
        public async Task SeguirUsuarioAsync(string usuarioId, string seguindoId)
        {
            try
            {
                var usuario = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioId);

                if (usuario == null)
                {
                    throw new Exception("Nenhum usuario encontrado no banco de dados");
                }

                var seguindo = await _usuarioRepository.BuscarUsuarioPorIdAsync(seguindoId);

                if (seguindo == null)
                {
                    throw new Exception("Usuario para seguir não encontrado no banco de dados.");
                }

                // Filtro para o usuário que deseja seguir outra pessoa
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, usuarioId);
                var update = Builders<UsuarioModel>.Update.AddToSet(x => x.Seguindo, seguindoId);

                // Filtro para adicionar o usuário à lista de seguidores do usuário seguido
                var usuarioSeguidoFilter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, seguindoId);
                var updateSeguido = Builders<UsuarioModel>.Update.AddToSet(x => x.Seguidores, usuarioId);

                // Executa as atualizações no banco de dados para seguir o usuário
                var resultadoUsuario = await _usuarioCollection.UpdateOneAsync(filter, update);
                var resultadoSeguidor = await _usuarioCollection.UpdateOneAsync(usuarioSeguidoFilter, updateSeguido);

                // Verifica se ambas as adições foram bem-sucedidas
                if (resultadoUsuario.ModifiedCount == 0 || resultadoSeguidor.ModifiedCount == 0)
                {
                    throw new Exception("Nada alterado no banco de dados.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao seguir usuario");
                throw new Exception("Erro ao seguir usuario.");
            }
        }
    }
}
