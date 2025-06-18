using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelShare.Repository
{
    // Classe responsável pela alteração e redefinição da senha do usuário.
    // Métodos disponíveis:
    // - AlterarSenhaAsync(AlterarSenhaModel alterarSenha): Altera a senha atual do usuário, verificando a senha atual e a nova senha.
    // - RedefinirSenha(string id, string novaSenha): Redefine a senha de um usuário utilizando o seu ID e a nova senha fornecida.
    public class AlterarSenhaRepositorio : IAlteracaoSenhaRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;
        private readonly ILogger<AlterarSenhaRepositorio> _logger;

        // Construtor que recebe o contexto do MongoDB e inicializa a coleção de usuários
        public AlterarSenhaRepositorio(MongoContext mongoContext, ILogger<AlterarSenhaRepositorio> logger)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");
            _logger = logger;
        }

        // Método para alterar a senha de um usuário
        public async Task<bool> AlterarSenhaAsync(AlterarSenhaModel alterarSenha)
        {
            try
            {
                // Busca o usuário no banco de dados com o ID fornecido
                var usuarioDb = await _usuarioCollection.Find(x => x.Id == alterarSenha.Id).FirstOrDefaultAsync();

                // Se o usuário não for encontrado, retorna false
                if (usuarioDb == null) return false;

                // Verifica se a senha atual informada está correta
                if (!usuarioDb.SenhaValida(alterarSenha.SenhaAtual))
                {
                    throw new InvalidOperationException("A senha atual informada não está correta.");
                }

                // Se a nova senha for igual à senha atual, lança uma exceção
                if (usuarioDb.SenhaValida(alterarSenha.NovaSenha))
                {
                    throw new InvalidOperationException("A nova senha não pode ser igual à senha atual.");
                }

                // Define a nova senha para o usuário
                usuarioDb.SetNovaSenha(alterarSenha.NovaSenha);

                // Cria o filtro para encontrar o usuário no banco de dados pelo ID
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, alterarSenha.Id);

                // Cria o comando de atualização para definir a nova senha (já gerada com hash)
                var update = Builders<UsuarioModel>.Update.Set(x => x.Senha, alterarSenha.NovaSenha.GerarHash());

                // Executa a atualização no banco de dados
                var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

                // Retorna true se a senha foi atualizada com sucesso (modified count maior que 0)
                return resultado.ModifiedCount > 0;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao alterar a senha");
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                // Captura qualquer exceção e loga o erro (pode-se adicionar um logger aqui)
                _logger.LogError(ex, "Ocorreu um erro ao alterar a senha");
                throw new Exception("Ocorreu um erro ao alterar a senha.");
            }
        }

        // Método para redefinir a senha de um usuário
        public async Task<bool> RedefinirSenha(string id, string novaSenha)
        {
            try
            {
                // Busca o usuário no banco de dados pelo ID fornecido
                var usuarioDb = await _usuarioCollection.FindAsync(x => x.Id == id);

                // Se o usuário não for encontrado, retorna false
                if (usuarioDb == null) return false;

                // Gera o hash para a nova senha
                novaSenha = novaSenha.GerarHash();

                // Cria o filtro para encontrar o usuário no banco de dados pelo ID
                var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, id);

                // Cria o comando de atualização para definir a nova senha (já gerada com hash)
                var update = Builders<UsuarioModel>.Update.Set(x => x.Senha, novaSenha);

                // Executa a atualização no banco de dados
                var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

                // Retorna true se a senha foi atualizada com sucesso (modified count maior que 0)
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao redefinir  senha");
                // Captura qualquer exceção e loga o erro 
                throw new Exception("Ocorreu um erro ao redefinir a senha.");
            }
        }
    }
}
