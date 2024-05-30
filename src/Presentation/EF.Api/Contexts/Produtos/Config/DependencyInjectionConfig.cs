using EF.Produtos.Application.UseCases;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Repository;
using EF.Produtos.Infra.Data;
using EF.Produtos.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.Produtos.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServicesProdutos(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application - Use Cases
        services.AddScoped<IAtualizarProdutoUseCase, AtualizarProdutoUseCase>();
        services.AddScoped<IConsultarProdutoUseCase, ConsultarProdutoUseCase>();
        services.AddScoped<ICriarProdutoUseCase, CriarProdutoUseCase>();
        services.AddScoped<IRemoverProdutoUseCase, RemoverProdutoUseCase>();

        // Infra - Data
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddDbContext<ProdutoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}