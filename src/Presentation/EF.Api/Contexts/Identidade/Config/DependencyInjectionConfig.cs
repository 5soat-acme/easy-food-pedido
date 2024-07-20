using EF.Identidade.Application.UseCases;
using EF.Identidade.Application.UseCases.Interfaces;
using EF.Identidade.Domain.Repository;
using EF.Identidade.Infra;
using EF.Identidade.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.Identidade.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServicesIdentidade(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application - UseCases
        services.AddScoped<IIdentidadeUseCase, IdentidadeUseCase>();
        services.AddScoped<ICriarSolicitacaoExclusaoUseCase, CriarSolicitacaoExclusaoUseCase>();

        // Infra - Data
        services.AddScoped<ISolicitacaoExclusaoRepository, SolicitacaoExclusaoRepository>();
        services.AddDbContext<IdentidadeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}