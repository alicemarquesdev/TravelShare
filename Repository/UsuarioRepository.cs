using MongoDB.Bson;
using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Helper.Interfaces;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Repositório responsável por gerenciar as operações relacionadas aos usuários.
    // Métodos disponíveis:
    // - BuscarUsuarioPorIdAsync(string id): Busca um usuário pelo seu ID.
    // - BuscarUsuarioPorEmailOuUsernameAsync(string emailOuUsername): Busca um usuário por email ou username.
    // - PesquisarUsuariosAsync(string termo): Pesquisa usuários com base em um termo.
    // - BuscarVariosUsuariosPorIdAsync(List<string> ids): Busca vários usuários por seus IDs.
    // - BuscarTodosOsUsuariosAsync(): Busca todos os usuários cadastrados.
    // - BuscarSugestoesParaSeguirAsync(string id): Busca sugestões de usuários para seguir.
    // - AddUsuarioAsync(UsuarioModel usuario): Adiciona um novo usuário.
    // - AtualizarUsuarioAsync(UsuarioSemSenhaModel usuario): Atualiza as informações de um usuário.
    // - DeletarUsuarioAsync(string id): Deleta um usuário.
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;
        private readonly IMongoCollection<PostModel> _postCollection;
        private readonly IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly ISessao _sessao;

        // Construtor que recebe o contexto do MongoDB e o serviço de sessão
        public UsuarioRepository(MongoContext mongoContext, ISessao sessao)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");
            _postCollection = mongoContext.GetCollection<PostModel>("Posts");
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentarios");
            _sessao = sessao;
        }

        // Método que busca um usuário pelo seu ID
        public async Task<UsuarioModel?> BuscarUsuarioPorIdAsync(string id)
        {
            try
            {
                return await _usuarioCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuario" + ex.Message);
            }
        }

        // Método que busca um usuário por email ou username
        public async Task<UsuarioModel?> BuscarUsuarioPorEmailOuUsernameAsync(string emailOuUsername)
        {
            try
            {                return await _usuarioCollection
                    .Find(x => x.Email.Equals(emailOuUsername, StringComparison.CurrentCultureIgnoreCase) ||
                               x.Username.Equals(emailOuUsername, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuario" + ex.Message);
            }
        }

        // Método que pesquisa usuários por um termo de pesquisa (nome, username ou cidade)
        public async Task<List<UsuarioModel>> PesquisarUsuariosAsync(string termo)
        {
            try
            {
                var filtro = Builders<UsuarioModel>.Filter.Or(
                    Builders<UsuarioModel>.Filter.Regex(x => x.Nome, new BsonRegularExpression(termo, "i")),
                    Builders<UsuarioModel>.Filter.Regex(x => x.Username, new BsonRegularExpression(termo, "i")),
                    Builders<UsuarioModel>.Filter.Regex(x => x.CidadeDeNascimento, new BsonRegularExpression(termo, "i"))
                );

                return await _usuarioCollection.Find(filtro).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuarios" + ex.Message);
            }
        }

        // Método que busca vários usuários pelos seus IDs
        public async Task<List<UsuarioModel>> BuscarVariosUsuariosPorIdAsync(List<string> ids)
        {
            try
            {
                return await _usuarioCollection.Find(usuario => ids.Contains(usuario.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuarios" + ex.Message);
            }
        }

        // Método que busca todos os usuários cadastrados
        public async Task<List<UsuarioModel>> BuscarTodosOsUsuariosAsync()
        {
            try
            {
                return await _usuarioCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuarios" + ex.Message);
            }
        }

        // Método que sugere usuários para seguir (exceto o próprio usuário e os já seguidos)
        public async Task<List<UsuarioModel>> BuscarSugestoesParaSeguirAsync(string id)
        {
            try
            {
                var usuario = await BuscarUsuarioPorIdAsync(id);
                if (usuario == null) throw new NullReferenceException("Nenhum usuário encontrado");

                var idsSeguindo = usuario.Seguindo ?? new List<string>();

                return await _usuarioCollection
                    .Find(x => x.Id != id && !idsSeguindo.Contains(x.Id))
                    .SortByDescending(x => x.DataRegistro)
                    .Limit(12)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuarios" + ex.Message);
            }
        }

        // Método que adiciona um novo usuário
        public async Task AddUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                usuario.Nome = usuario.Nome.Trim();
                usuario.Email = usuario.Email.ToLower().Trim();
                usuario.Username = usuario.Username.ToLower().Trim();
                usuario.SetSenhaHash();
                await _usuarioCollection.InsertOneAsync(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar usuario" + ex.Message);
            }
        }

        // Método que atualiza as informações de um usuário (sem senha)
        public async Task<bool> AtualizarUsuarioAsync(UsuarioSemSenhaModel usuario)
        {
            try
            {
                var usuarioDb = await BuscarUsuarioPorIdAsync(usuario.Id);
                if (usuarioDb == null) throw new NullReferenceException("Usuário não encontrado no banco de dados!");

                var filter = Builders<UsuarioModel>.Filter.Eq(u => u.Id, usuario.Id);
                var update = Builders<UsuarioModel>.Update
                    .Set(u => u.ImagemPerfil, usuario.ImagemPerfil)
                    .Set(u => u.Nome, usuario.Nome)
                    .Set(u => u.Username, usuario.Username.ToLower().Trim())
                    .Set(u => u.Email, usuario.Email.ToLower().Trim())
                    .Set(u => u.DataNascimento, usuario.DataNascimento)
                    .Set(u => u.CidadeDeNascimento, usuario.CidadeDeNascimento)
                    .Set(u => u.Bio, usuario.Bio ?? string.Empty);

                var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar usuario" + ex.Message);

            }
        }

        // Método que deleta um usuário, seus posts e comentários associados
        public async Task<bool> DeletarUsuarioAsync(string id)
        {
            try
            {
                var usuarioDb = await BuscarUsuarioPorIdAsync(id);
                if (usuarioDb == null) throw new NullReferenceException("Usuário não encontrado no banco de dados!");

                await _postCollection.DeleteManyAsync(x => x.UsuarioId == id);
                await _comentarioCollection.DeleteManyAsync(x => x.UsuarioId == id);

                var deleteUsuario = await _usuarioCollection.DeleteOneAsync(x => x.Id == id);

                _sessao.RemoverSessaoDoUsuario();

                return deleteUsuario.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar usuario" + ex.Message);

            }
        }
    }
}
