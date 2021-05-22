using Microsoft.Extensions.DependencyInjection;
using SCAP.Data;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services;
using SCAP.Services.Interfaces;
using KissLog;
using Microsoft.AspNetCore.Http;

namespace SCAP.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAfastamentoRepository, AfastamentoRepository>()
                    .AddScoped<IDocumentoRepository, DocumentoRepository>()
                    .AddScoped<IMandatoRepository, MandatoRepository>()
                    .AddScoped<IParecerRepository, ParecerRepository>()
                    .AddScoped<IParentescoRepository, ParentescoRepository>()
                    .AddScoped<IProfessorRepository, ProfessorRepository>()
                    .AddScoped<ISecretarioRepository, SecretarioRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAfastamentoService, AfastamentoService>()
                    .AddScoped<IDocumentoService, DocumentoService>()
                    .AddScoped<IMandatoService, MandatoService>()
                    .AddScoped<IParecerService, ParecerService>()
                    .AddScoped<IParentescoService, ParentescoService>()
                    .AddScoped<IProfessorService, ProfessorService>()
                    .AddScoped<ISecretarioService, SecretarioService>()
                    .AddScoped<IEmailService, EmailService>()
                    .AddScoped<IUserService<Pessoa>, UserService<Pessoa>>()
                    .AddScoped<IUserService<Professor>, ProfessorService>()
                    .AddScoped<IUserService<Secretario>, SecretarioService>();

            services.AddScoped<INotificator, Notificator>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogger>((context) =>
            {
                return Logger.Factory.Get();
            });

            return services;
        }
    }
}
