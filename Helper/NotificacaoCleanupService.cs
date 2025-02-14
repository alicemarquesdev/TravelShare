using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using TravelShare.Data;
using TravelShare.Models;

public class NotificacaoCleanupService : BackgroundService
{
    private readonly IMongoCollection<NotificacaoModel> _notificacaoCollection;
    private readonly TimeSpan _intervalo = TimeSpan.FromHours(24); // Roda uma vez por dia

    public NotificacaoCleanupService(MongoContext context)
    {
        _notificacaoCollection = context.GetCollection<NotificacaoModel>("Notificacoes");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RemoverNotificacoesAntigas();
            await Task.Delay(_intervalo, stoppingToken);
        }
    }

    private async Task RemoverNotificacoesAntigas()
    {
        var limiteData = DateTime.UtcNow.AddDays(-7); // Notificações com mais de 7 dias
        var filtro = Builders<NotificacaoModel>.Filter.Lt(n => n.DataCriacao, limiteData);

        var resultado = await _notificacaoCollection.DeleteManyAsync(filtro);
        Console.WriteLine($"🔄 {resultado.DeletedCount} notificações antigas removidas.");
    }
}
