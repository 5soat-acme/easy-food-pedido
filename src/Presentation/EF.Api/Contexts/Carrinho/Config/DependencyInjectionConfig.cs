using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Application.UseCases;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Repository;
using EF.Carrinho.Infra.Adapters.Estoque;
using EF.Carrinho.Infra.Adapters.Produtos;
using EF.Carrinho.Infra.Data;
using EF.Carrinho.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Api.Contexts.Carrinho.Config;

public static class DependencyInjectionConfig
{
    public static IServiceCollection RegisterServicesCarrinho(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application - Use Cases
        services.AddScoped<IAdicionarItemCarrinhoUseCase, AdicionarItemCarrinhoUseCase>();
        services.AddScoped<IAtualizarItemCarrinhoUseCase, AtualizarItemCarrinhoUseCase>();
        services.AddScoped<IConsultarCarrinhoUseCase, ConsultarCarrinhoUseCase>();
        services.AddScoped<IRemoverCarrinhoUseCase, RemoverCarrinhoUseCase>();
        services.AddScoped<IRemoverItemCarrinhoUseCase, RemoverItemCarrinhoUseCase>();

        // Application - Gateways & Gateways
        services.AddScoped<IEstoqueService, EstoqueAdapter>();
        services.AddScoped<IProdutoService, ProdutoAdapter>();

        // Infra - Data
        services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
        services.AddDbContext<CarrinhoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}