using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<UsuarioModel> _usuarioCollection;
        private readonly IMongoCollection<PostModel> _postCollection;
        private readonly IMongoCollection<ComentarioModel> _comentarioCollection;
        private readonly ISessao _sessao;

        public UsuarioRepository(MongoContext mongoContext, ISessao sessao)
        {
            _usuarioCollection = mongoContext.GetCollection<UsuarioModel>("Usuarios");
            _postCollection = mongoContext.GetCollection<PostModel>("Posts");
            _comentarioCollection = mongoContext.GetCollection<ComentarioModel>("Comentarios");
            _sessao = sessao;
        }

        public async Task<UsuarioModel> BuscarUsuarioPorIdAsync(string id)
        {
            return await _usuarioCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<UsuarioModel>> BuscarVariosUsuariosPorIdAsync(List<string> ids)
        {
            return await _usuarioCollection.Find(usuario => ids.Contains(usuario.Id)).ToListAsync();
        }

        public async Task<List<UsuarioModel>> BuscarSugestoesParaSeguirAsync(string id)
        {
            var usuario = await BuscarUsuarioPorIdAsync(id);

            if (usuario == null) throw new NullReferenceException("Nenhum usuário encontrado");

            var idsSeguindo = usuario.Seguindo ?? new List<string>();

            var sugestoesUsuarios = await _usuarioCollection.Find(x => x.Id != id && !idsSeguindo.Contains(x.Id)).SortByDescending(x => x.DataRegistro).Limit(12).ToListAsync();

            return sugestoesUsuarios;
        }

        public async Task<UsuarioModel> BuscarUsuarioPorEmailOuUsernameAsync(string emailOuUsername)
        {
            return await _usuarioCollection
                .Find(x => x.Email.Equals(emailOuUsername, StringComparison.CurrentCultureIgnoreCase) ||
                           x.Username.Equals(emailOuUsername, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync();
        }

        public async Task<List<UsuarioModel>> PesquisarUsuariosAsync(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                {
                    return new List<UsuarioModel>(); // Retorna uma lista vazia se o termo for inválido
                }

                var filtro = Builders<UsuarioModel>.Filter.Or(
                    Builders<UsuarioModel>.Filter.Regex(x => x.Nome, new BsonRegularExpression(termo, "i")),
                    Builders<UsuarioModel>.Filter.Regex(x => x.Username, new BsonRegularExpression(termo, "i")),
                    Builders<UsuarioModel>.Filter.Regex(x => x.CidadeDeNascimento, new BsonRegularExpression(termo, "i"))
                );

                return await _usuarioCollection.Find(filtro).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pesquisar usuários: {ex.Message}");
                return new List<UsuarioModel>(); // Retorna uma lista vazia em caso de erro
            }
        }

        public async Task AddUsuarioAsync(UsuarioModel usuario)
        {
            usuario.Nome.Trim();
            usuario.Email.ToLower().Trim();
            usuario.Username.ToLower().Trim();
            usuario.SetSenhaHash();
            await _usuarioCollection.InsertOneAsync(usuario);
        }

        public async Task<bool> AtualizarUsuarioAsync(UsuarioSemSenhaModel usuario)
        {
            var usuarioDb = await BuscarUsuarioPorIdAsync(usuario.Id);

            if (usuarioDb == null) throw new NullReferenceException("Usuário não encontrado!");

            string caminhoImagemAntiga = null;

            if (usuarioDb.ImagemPerfil != usuario.ImagemPerfil)
            {
                caminhoImagemAntiga = usuarioDb.ImagemPerfil;
            }

            // Criar filtro para localizar o documento pelo ID
            var filter = Builders<UsuarioModel>.Filter.Eq(u => u.Id, usuario.Id);

            // Criar as atualizações apenas com os campos relevantes
            var update = Builders<UsuarioModel>.Update
                .Set(u => u.ImagemPerfil, usuario.ImagemPerfil)
                .Set(u => u.Nome, usuario.Nome)
                .Set(u => u.Username, usuario.Username.ToLower().Trim())
                .Set(u => u.Email, usuario.Email.ToLower().Trim())
                .Set(u => u.DataNascimento, usuario.DataNascimento)
                .Set(u => u.CidadeDeNascimento, usuario.CidadeDeNascimento)
                .Set(u => u.Bio, usuario.Bio ?? string.Empty);

            // Atualizar o documento na coleção
            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            // Verificar se algum documento foi modificado
            if (resultado.ModifiedCount == 0) return false;

            if (!string.IsNullOrEmpty(caminhoImagemAntiga))
            {
                try
                {
                    // Aqui você define o caminho onde as fotos são armazenadas
                    var caminhoFoto = Path.Combine(caminhoImagemAntiga);

                    if (File.Exists(caminhoFoto))
                    {
                        File.Delete(caminhoFoto); // Deleta o arquivo do servidor
                        Console.WriteLine($"Foto do perfil antiga excluída: {caminhoImagemAntiga}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao tentar excluir a foto antiga: {ex.Message}");
                }
            }

            return true;
        }

        public async Task<bool> DeletarUsuarioAsync(string id)
        {
            var posts = await _postCollection.DeleteManyAsync(x => x.UsuarioId == id);

            var comentario = await _comentarioCollection.DeleteManyAsync(x => x.UsuarioId == id);

            var deleteUsuario = await _usuarioCollection.DeleteOneAsync(x => x.Id == id);
            _sessao.RemoverSessaoUsuario();

            return deleteUsuario.DeletedCount > 0;
        }

        public async Task<bool> RedefinirSenha(string id, string novaSenha)
        {
            var usuarioDb = await BuscarUsuarioPorIdAsync(id);

            if (usuarioDb == null) return false;

            novaSenha = novaSenha.GerarHash();

            var filter = Builders<UsuarioModel>.Filter.Eq(x => x.Id, id);
            var update = Builders<UsuarioModel>.Update.Set(x => x.Senha, novaSenha);

            var resultado = await _usuarioCollection.UpdateOneAsync(filter, update);

            return resultado.ModifiedCount > 0;
        }

        public async Task<List<UsuarioModel>> BuscarTodosOsUsuariosAsync()
        {
            return await _usuarioCollection.Find(Builders<UsuarioModel>.Filter.Empty).ToListAsync(); 
        }
    }
}