using EF.Cupons.Application.UseCases;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Repository;
using EF.Cupons.Infra;
using EF.Cupons.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.Cupons.Config;

public static class DependencyInjectionConfig
{
    public static void RegisterServicesCupons(this IServiceCollection services, IConfiguration configuration)
    {
        // Application - Use Cases
        services.AddScoped<IAtualizarCupomUseCase, AtualizarCupomUseCase>();
        services.AddScoped<IConsultarCupomUseCase, ConsultarCupomUseCase>();
        services.AddScoped<ICriarCupomUseCase, CriarCupomUseCase>();
        services.AddScoped<IInativarCupomUseCase, InativarCupomUseCase>();
        services.AddScoped<IInserirProdutoUseCase, InserirProdutoUseCase>();
        services.AddScoped<IRemoverProdutoUseCase, RemoverProdutoUseCase>();

        // Infra - Data 
        services.AddScoped<ICupomRepository, CupomRepository>();
        services.AddDbContext<CupomDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }
}