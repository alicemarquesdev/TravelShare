using TravelShare.Data;
using TravelShare.Helper;
using TravelShare.Helper.Interfaces;
using TravelShare.Repository;
using TravelShare.Repository.Interfaces;
using static TravelShare.Data.MongoContext;
using Serilog;

namespace TravelShare
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console() // Logs no Console
             .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // Logs em arquivo
             .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Adicionar Serilog como provedor de logs
            builder.Host.UseSerilog();

            // Adicionar servi�os � aplica��o
            ConfigureServices(builder);

            var app = builder.Build();

            // Configurar o pipeline de requisi��o HTTP
            ConfigureMiddleware(app);

            // Rodar a aplica��o
            app.Run();
        }

        // M�todo para configurar os servi�os
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Servi�os de Controllers e Views
            builder.Services.AddControllersWithViews();

            builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

            // Adicionar suporte � compacta��o de resposta
            builder.Services.AddResponseCompression();

            // Configura��o MongoDB
            builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<MongoContext>();

            // Configura��o de reposit�rios
            RegisterRepositories(builder);

        
            // Configura��o de servi�os adicionais
            builder.Services.AddScoped<ISessao, Sessao>();
            builder.Services.AddScoped<ICaminhoImagem, CaminhoImagem>();
            builder.Services.AddScoped<IEmail, Email>();

            // Configurar se��es de configura��o (GoogleAPI)
            builder.Services.Configure<GoogleAPISettings>(builder.Configuration.GetSection("GoogleAPISettings"));

            // Configurar se��es de configura��o (EmailSettings)
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Adicionar sess�o com configura��es
            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(30);
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Adicionar o IHttpContextAccessor
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Configura��o de servi�os adicionais, como Notifica��o Cleanup Service
            builder.Services.AddHostedService<NotificacaoCleanupService>();
        }

        // M�todo para registrar reposit�rios de forma mais limpa
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

        // M�todo para configurar o middleware
        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Captura o erro 400 (Bad Request) devido � falha no token
            app.UseStatusCodePagesWithRedirects("/Home/Error");

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            // Adicionar compress�o de resposta
            app.UseResponseCompression();

            // Mapeamento de rotas da aplica��o
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");
        }
    }
}
