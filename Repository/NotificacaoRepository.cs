using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Enums;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    public class NotificacaoRepository : INotificacaoRepository
    {

        private readonly IMongoCollection<NotificacaoModel> _notificacaoCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPostRepository _postRepository;

        public NotificacaoRepository(MongoContext context, IUsuarioRepository usuarioRepository, IPostRepository postRepository)
        {
            _notificacaoCollection = context.GetCollection<NotificacaoModel>("Notificacoes");
            _usuarioRepository = usuarioRepository;
            _postRepository = postRepository;
        }

        public async Task<NotificacaoModel> BuscarNotificacaoPorIdAsync(string id)
        {
            return await _notificacaoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<NotificacaoModel>> BuscarTodasAsNotificacoesDoUsuarioAsync(string usuarioId)
        {
            var notificacoes = await _notificacaoCollection.Find(x => x.UsuarioDestino == usuarioId).SortByDescending(x => x.DataCriacao).ToListAsync();

            foreach (var notificacao in notificacoes)
            {
                notificacao.UsuarioOrigemModel = await _usuarioRepository.BuscarUsuarioPorIdAsync(notificacao.UsuarioOrigem);

                if (notificacao.PostId != null)
                {
                    notificacao.Post = await _postRepository.BuscarPostPorIdAsync(notificacao.PostId);
                }
            }

            return notificacoes;
        }

        public async Task AddNotificacaoAsync(NotificacaoModel notificacao)
        {
            await _notificacaoCollection.InsertOneAsync(notificacao);
        }


        public async Task<bool> RemoverNotificacaoAsync(string usuarioDestino, string usuarioOrigem, NotificacaoEnum notificacao, string? postId)
        {
            var filtros = new List<FilterDefinition<NotificacaoModel>>
    {
        Builders<NotificacaoModel>.Filter.Eq(n => n.UsuarioOrigem, usuarioOrigem),
        Builders<NotificacaoModel>.Filter.Eq(n => n.UsuarioDestino, usuarioDestino),
        Builders<NotificacaoModel>.Filter.Eq(n => n.Notificacao, notificacao)
    };

            if (postId is not null)
            {
                filtros.Add(Builders<NotificacaoModel>.Filter.Eq(n => n.PostId, postId));
            }
            else
            {
                filtros.Add(Builders<NotificacaoModel>.Filter.Or(
                    Builders<NotificacaoModel>.Filter.Eq(n => n.PostId, null),
                    Builders<NotificacaoModel>.Filter.Exists(n => n.PostId, false)
                ));
            }

            var filtro = Builders<NotificacaoModel>.Filter.And(filtros);

            var resultado = await _notificacaoCollection.DeleteOneAsync(filtro);

            return resultado.DeletedCount > 0;
        }


        public async Task<bool> RemoverNotificacaoPorComentarioAsync(string comentarioId)
        {
            var filtro = Builders<NotificacaoModel>.Filter.Eq(n => n.ComentarioId, comentarioId);
            var resultado = await _notificacaoCollection.DeleteOneAsync(filtro);

            return resultado.DeletedCount > 0;
        }


    }
}
