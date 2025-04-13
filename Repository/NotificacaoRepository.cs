using MongoDB.Driver;
using TravelShare.Data;
using TravelShare.Enums;
using TravelShare.Models;
using TravelShare.Repository.Interfaces;

namespace TravelShare.Repository
{
    // Classe responsável pela manipulação das notificações no sistema.
    // Métodos disponíveis:
    // - BuscarNotificacaoPorIdAsync(string id): Busca uma notificação pelo seu ID.
    // - BuscarNotificacaoPorComentarioId(string comentarioId): Busca uma notificação pelo ID do comentário.
    // - BuscarTodasAsNotificacoesDoUsuarioAsync(string usuarioId): Busca todas as notificações de um usuário.
    // - AddNotificacaoAsync(NotificacaoModel notificacao): Adiciona uma nova notificação ao sistema.
    // - RemoverNotificacaoAsync(string usuarioDestino, string usuarioOrigem, NotificacaoEnum notificacao, string? postId): Remove uma notificação com base nos parâmetros fornecidos.
    // - RemoverNotificacaoPorComentarioAsync(string comentarioId): Remove uma notificação associada a um comentário.

    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly IMongoCollection<NotificacaoModel> _notificacaoCollection;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<NotificacaoRepository> _logger;

        // Construtor que recebe o contexto do MongoDB, repositórios de usuários e posts.
        public NotificacaoRepository(MongoContext context, IUsuarioRepository usuarioRepository, IPostRepository postRepository, ILogger<NotificacaoRepository> logger)
        {
            _notificacaoCollection = context.GetCollection<NotificacaoModel>("Notificacoes");
            _usuarioRepository = usuarioRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        // Busca uma notificação pelo ID.
        public async Task<NotificacaoModel?> BuscarNotificacaoPorIdAsync(string id)
        {
            try
            {
                var notificacao = await _notificacaoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (notificacao == null) return null;
                return notificacao;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar notificação por ID.");
                throw new Exception("Erro ao buscar notificação por ID.");
            }
        }

        // Busca uma notificação pelo comentarioId
        public async Task<NotificacaoModel?> BuscarNotificacaoPorComentarioId(string comentarioId)
        {
            try
            {
               return await _notificacaoCollection.Find(x => x.ComentarioId == comentarioId).FirstOrDefaultAsync();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar notificação por ID.");
                throw new Exception("Erro ao buscar notificação por ID.");
            }
        }

        // Busca todas as notificações de um usuário específico.
        public async Task<List<NotificacaoModel>> BuscarTodasAsNotificacoesDoUsuarioAsync(string usuarioId)
        {
            try
            {
                var notificacoes = await _notificacaoCollection
                    .Find(x => x.UsuarioDestinoId == usuarioId)
                    .SortByDescending(x => x.DataCriacao)
                    .ToListAsync();

                // Carrega informações adicionais para cada notificação (usuário de origem e post relacionado)
                foreach (var notificacao in notificacoes)
                {
                    notificacao.UsuarioOrigemModel = await _usuarioRepository.BuscarUsuarioPorIdAsync(notificacao.UsuarioOrigemId);

                    if (notificacao.PostId != null)
                    {
                        notificacao.Post = await _postRepository.BuscarPostPorIdAsync(notificacao.PostId);
                    }
                }

                return notificacoes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar notificações do usuário.");
                throw new Exception("Erro ao buscar notificações do usuario.");
            }
        }

        // Adiciona uma nova notificação ao banco de dados.
        public async Task AddNotificacaoAsync(NotificacaoModel notificacao)
        {
            try
            {
                if (notificacao == null) throw new ArgumentNullException(nameof(notificacao), "A notificação não pode ser nula.");

                if (string.IsNullOrEmpty(notificacao.UsuarioDestinoId) || string.IsNullOrEmpty(notificacao.UsuarioOrigemId))
                    throw new ArgumentException("Usuário de origem e destino devem ser informados.");

                await _notificacaoCollection.InsertOneAsync(notificacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar notificação.");
                throw new Exception("Erro ao adicionar notificação.");
            }
        }

        // Remove uma notificação com base nos parâmetros fornecidos.
        public async Task<bool> RemoverNotificacaoAsync(string usuarioDestinoId, string usuarioOrigemId, NotificacaoEnum notificacao, string? postId)
        {
            try
            {
                var usuarioDestinoDB = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioDestinoId);
                var usuarioOrigemDB = await _usuarioRepository.BuscarUsuarioPorIdAsync(usuarioOrigemId);

                if (usuarioDestinoDB == null || usuarioOrigemDB == null)
                    throw new Exception("Usuário(s) não encontrado(s) no banco de dados");

                var filtros = new List<FilterDefinition<NotificacaoModel>>
                {
                    Builders<NotificacaoModel>.Filter.Eq(n => n.UsuarioOrigemId, usuarioOrigemId),
                    Builders<NotificacaoModel>.Filter.Eq(n => n.UsuarioDestinoId, usuarioDestinoId),
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover notificação.");
                throw new Exception("Erro ao remover notificação");
            }
        }

        // Remove uma notificação associada a um comentário.
        public async Task<bool> RemoverNotificacaoPorComentarioAsync(string comentarioId)
        {
            try
            {
                var notificacao = await _notificacaoCollection.Find(x => x.ComentarioId == comentarioId).FirstOrDefaultAsync();

                if (notificacao == null)
                    throw new Exception("Notificação não encontrada");

                var filtro = Builders<NotificacaoModel>.Filter.Eq(n => n.ComentarioId, comentarioId);
                var resultado = await _notificacaoCollection.DeleteOneAsync(filtro);
                return resultado.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover notificação por comentário.");
                throw new Exception("Erro ao remover notificação por comentário.");
            }
        }
    }
}
