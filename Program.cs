using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Helper.Interfaces;
using TravelShare.Repository;
using TravelShare.Repository.Interfaces;
using static TravelShare.Data.MongoContext;

namespace TravelShare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicionar serviços à aplicação
            ConfigureServices(builder);

            var app = builder.Build();

            // Configurar o pipeline de requisição HTTP
            ConfigureMiddleware(app);

            // Rodar a aplicação
            app.Run();
        }

        // Método para configurar os serviços
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Serviços de Controllers e Views
            builder.Services.AddControllersWithViews();

            // Configuração MongoDB
            builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<MongoContext>();

            // Configuração de repositórios
            RegisterRepositories(builder);

            // Adicionar o IHttpContextAccessor
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Configuração de serviços adicionais
            builder.Services.AddScoped<ISessao, Sessao>();
            builder.Services.AddScoped<ICaminhoImagem, CaminhoImagem>();
            builder.Services.AddScoped<IEmail, Email>();

            // Configurar seções de configuração (GoogleAPI)
            builder.Services.Configure<GoogleAPISettings>(builder.Configuration.GetSection("GoogleAPISettings"));

            // Configurar seções de configuração (EmailSettings)
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Adicionar sessão com configurações
            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(30);
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Configuração de serviços adicionais, como Notificação Cleanup Service
            builder.Services.AddHostedService<NotificacaoCleanupService>();
        }

        // Método para registrar repositórios de forma mais limpa
        private static void RegisterRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ISeguidorRepository, SeguidorRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<ICurtidaRepository, CurtidaRepository>();
            builder.Services.AddScoped<ICidadesVisitadasRepository, CidadesVisitadasRepository>();
            builder.Services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
            builder.Services.AddScoped<IAlteracaoSenhaRepository, AlterarSenhaRepositorio>();
        }

        // Método para configurar o middleware
        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            // Mapeamento de rotas da aplicação
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");
        }
    }
}
