using EF.Estoques.Application.UseCases;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Estoques.Domain.Repository;
using EF.Estoques.Infra;
using EF.Estoques.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.Estoques.Config;

public static class DependencyInjectionConfig
{
    public static void RegisterServicesEstoques(this IServiceCollection services, IConfiguration configuration)
    {
        // Application - Use Cases
        services.AddScoped<IAtualizarEstoqueUseCase, AtualizarEstoqueUseCase>();
        services.AddScoped<IConsultaEstoqueUseCase, ConsultaEstoqueUseCase>();

        // Infra - Data 
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();
        services.AddDbContext<EstoqueDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }
}